using System.Net.Http.Headers;
using System.Text;

using Descartes.Wms2.HowToUse.Mvc.Extensions;
using Descartes.Wms2.HowToUse.Mvc.Helpers;
using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs.Errors;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class OrderController : Controller
	{
		private IConfiguration _configuration;

		public OrderController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> PreparePdf()
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var client = await httpClient.GetFromJsonAsync<ClientOutputModel>($"api/v1/users/{this.HttpContext.Session.Get<long>("ClientId")}");

			// Collect some (hardcoded) fields...
			var fields = new List<PdfPlaceHolder>();
			fields.Add(new PdfPlaceHolder("Name", client.Name));
			fields.Add(new PdfPlaceHolder("Surname", client.Surname));
			fields.Add(new PdfPlaceHolder("Language", "DE"));
			fields.Add(new PdfPlaceHolder("Gender", "MALE"));
			fields.Add(new PdfPlaceHolder("Street", "Nowhere 0"));
			fields.Add(new PdfPlaceHolder("ZipAndCity", "6000 Elsewhere"));
			fields.Add(new PdfPlaceHolder("Country", "Zambia"));
			fields.Add(new PdfPlaceHolder("Nationality", "Mars"));
			fields.Add(new PdfPlaceHolder("Birthdate", "01.01.1990"));
			fields.Add(new PdfPlaceHolder("Email", client.Email));
			fields.Add(new PdfPlaceHolder("Phone", client.PhoneNumber));

			var byteArray = await this.GetPdf(fields);

			return this.View("/Views/Order/ShowPdf.cshtml", new ShowPdfViewModel { PdfContent = byteArray });
		}

		[HttpPost]
		public async Task<IActionResult> Submit(OrderViewModel orderViewModel)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

            // As the client agrees with the investments proposed, save his questionnaire: this will also set a risk profile.
            var payload = JsonConvert.SerializeObject(new
            {
                BusinessLineId = this.HttpContext.Session.Get<long>("BusinessLineId"),
                UserId = this.HttpContext.Session.Get<long>("ClientId"),
                Responses = new[]
                {
					new { ResponseId = this.HttpContext.Session.Get<long>("Response1Id") },
					new { ResponseId = this.HttpContext.Session.Get<long>("Response2Id") },
					new { ResponseId = this.HttpContext.Session.Get<long>("Response3Id") },
					new { ResponseId = this.HttpContext.Session.Get<long>("Response4Id") }
				}
            });

            _ = httpClient.PostAsync("api/v1/user-risk-categorizations", new StringContent(payload, Encoding.UTF8, "application/json")).Result;

            // Order reservation and order submission
            var reservationPayload = JsonConvert.SerializeObject(new { UserId = this.HttpContext.Session.Get<long>("ClientId"), InvestmentCategoryId = this.HttpContext.Session.Get<long>("InvestmentCategoryId") });
			var httpResponseMessage = httpClient.PostAsync("api/v1/user-portfolio-orders/reservation", new StringContent(reservationPayload, Encoding.UTF8, "application/json")).Result;
			var reservationAsString = httpResponseMessage.Content.ReadAsStringAsync().Result;
			var reservation = JsonConvert.DeserializeObject<ReservationOutputModel>(reservationAsString);

            var accountHoldingInstitution = httpClient.GetFromJsonAsync<AccountHoldingInstitutionOutputModel>("api/v1/account-holding-institutions/code/LIENHARDT").Result;

            var orderPortfolioCreationInputModel = new
			{
				ReservationId = reservation.Id,
				AccountHoldingInstitutionCode = accountHoldingInstitution.Code,
				UserId = this.HttpContext.Session.Get<long>("ClientId"),
				InvestmentCategoryId = this.HttpContext.Session.Get<long>("InvestmentCategoryId"),
				ProposalId = this.HttpContext.Session.Get<long>("ProposalId"),
				Name = orderViewModel.PortfolioName,
				ReasonToChangeProposalResponsesIds = default(List<long>) // In case user agreed with suggested proposal, this list can be left empty
			};

			var httpContent = HttpHelper.CreateMultipartFormDataHttpContent(orderPortfolioCreationInputModel, Convert.FromBase64String(orderViewModel.Document));
			httpResponseMessage = httpClient.PostAsync("api/v1/user-portfolio-orders/creation", httpContent).Result;
			if (httpResponseMessage.IsSuccessStatusCode == false)
			{

				if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
				{
					var errors = new Dictionary<string, string>();
					var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
					var apiErrorModel = JsonConvert.DeserializeObject<ValidationErrorModel>(content);

					foreach (var validationError in apiErrorModel.ValidationErrors)
					{
						foreach (var validationErrorDescription in validationError.Value)
						{
							errors.Add(validationError.Key, validationErrorDescription.ValidationErrorMessage);
						}
					}

					this.ViewData.Add("ApiError", errors);
				}

				return this.View("/Views/Order/Submit.cshtml");
			}
			else
			{
				this.ViewData.Add("PortfolioName", orderViewModel.PortfolioName);
				this.ViewData.Add("OrderId", UriHelper.GetIdFromLocationUri(httpResponseMessage.Headers.Location));
				this.ViewData.Add("IBAN", reservation.Iban);

				return this.View("/Views/Order/Submit.cshtml");
			}			
		}

		#region Private Methods

		private async Task<byte[]> GetPdf(IEnumerable<PdfPlaceHolder> placeHolderValues)
		{
			var payload = new PdfDataInputModel
			{
				PlaceHolderValues = placeHolderValues.Select(x => new FormField(x.Name, x.Value)).ToList()
			};

			var byteArray = FileContentHelper.GetFileContent(this.GetType(), "Contract-3A.pdf");

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["PdfFillerBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var httpContent = HttpHelper.CreateMultipartFormDataHttpContent(payload, byteArray);
			var httpResponseMessage = await httpClient.PostAsync("api/v1/pdf-service/fill-pdf-with-data", httpContent);
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				return await httpResponseMessage.Content.ReadAsByteArrayAsync();
			}

			return default;
		}

		#endregion
	}
}
