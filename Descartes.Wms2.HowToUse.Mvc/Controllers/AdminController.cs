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
	public class AdminController : Controller
	{
		private IConfiguration _configuration;

		public AdminController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> CreateClient(long? userId)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["GuestToken"]);

			var legalAcceptance = await httpClient.GetFromJsonAsync<LegalAcceptanceOutputModel>("/api/v1/legal-acceptances/active");


			var viewModel = new ClientViewModel() { LegalAcceptanceId = legalAcceptance.Id };
#if DEBUG
			var birthDay = new Random().Next(1, 28);
			var monthDay = new Random().Next(1, 12);
			var yearDay = new Random().Next(1965, 2005);

			viewModel.BirthDate = new DateTime(yearDay, monthDay, birthDay);
			viewModel.Name = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(4, 6);
			viewModel.Surname = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(1, 7);
			viewModel.BirthDate = new DateTime(yearDay, monthDay, birthDay);
			viewModel.GenderId = 1;
			viewModel.LanguageId = 1;
			viewModel.NationalityId = 44;
			viewModel.CountryId = 44;
			viewModel.Street = "Nostreet";
			viewModel.StreetNr = "1";
			viewModel.Zip = "6340";
			viewModel.City = "Nowhere";
			viewModel.CivilStatusId = 1;
			viewModel.Email = $"{Guid.NewGuid().ToString().Replace("-", string.Empty)}@libero.com";
			viewModel.LegalAcceptanceId = 1;
			viewModel.PensionSituationId = 1;
			viewModel.PhonePrefix = "0041";
			viewModel.PhoneNumber = "767965365";

			viewModel.LegalAcceptanceId = legalAcceptance.Id;
			viewModel.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmltYXJ5Z3JvdXBzaWQiOiJERVNDQVJURVMiLCJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiIxIiwiZ2l2ZW5fbmFtZSI6IkN1bG8iLCJmYW1pbHlfbmFtZSI6IlJvdHRvIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNjY0OTY1ODM3LCJleHAiOjIxMzgzNTE0MzcsImlhdCI6MTY2NDk2NTgzN30.MTyQ77tS5Y1c5Kw_7QmNohLSqMgD1mjj1M1BvaHcvEQ";
#endif

			return this.View("/Views/Admin/CreateClient.cshtml", viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> CreateClient(ClientViewModel viewModel)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View("/Views/Admin/CreateClient.cshtml", viewModel);
			}

			var baseUrl = _configuration["WmsBaseUrl"];

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", viewModel.Token);

			var payload = JsonConvert.SerializeObject(new
			{
				ManagementPersonalDataConsent = DateTime.Now,
				viewModel.Name,
				viewModel.Surname,
				viewModel.Email,
				PhoneNumberPrefix = viewModel.PhonePrefix,
				PhoneNumberNumber = viewModel.PhoneNumber,
				PensionSituationId = viewModel.PensionSituationId.GetValueOrDefault(),
				LanguageId = viewModel.LanguageId.GetValueOrDefault(),
				LegalAcceptanceId = viewModel.LegalAcceptanceId.GetValueOrDefault(),
				Address = new
				{
					viewModel.City,
					viewModel.Zip,
					viewModel.Street,
					viewModel.StreetNr,
					CountryId = viewModel.CountryId.GetValueOrDefault()
				},
				GenderId = viewModel.GenderId.GetValueOrDefault(),
				Nationality1Id = viewModel.NationalityId.GetValueOrDefault(),
				viewModel.BirthDate
			});

			var httpResponseMessage = await httpClient.PostAsync("api/v1/users", new StringContent(payload, Encoding.UTF8, "application/json"));
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				viewModel.ClientId = UriHelper.GetIdFromLocationUri(httpResponseMessage.Headers.Location);

				this.HttpContext.Session.Set<long>("ClientId", viewModel.ClientId.Value);
				this.HttpContext.Session.Set<string>("Token", viewModel.Token);
			}
			else
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
			}

			return this.View("/Views/Admin/CreateClient.cshtml", viewModel);
		}
	}
}