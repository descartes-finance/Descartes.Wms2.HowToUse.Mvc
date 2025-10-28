using Descartes.Wms2.HowToUse.Mvc.Helpers;
using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs.Errors;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Newtonsoft.Json;

using System.Net.Http.Headers;

using System.Net.Http.Json;
using System.Text;

public class Program
{
	public static async Task Main(string[] args)
	{
		var clientId = default(long);

		var host = Program.CreateHostBuilder(args).Build();
		var serviceProvider = host.Services;

		var configuration = (IConfiguration)serviceProvider.GetRequiredService(typeof(IConfiguration));

		using var httpClient = new HttpClient { BaseAddress = new Uri(configuration["WmsBaseUrl"]) };
		httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
		httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuration["SuperAdminToken"]);

		var legalAcceptance = httpClient.GetFromJsonAsync<LegalAcceptanceOutputModel>("/api/v1/legal-acceptances/active").Result;
		var languages = httpClient.GetFromJsonAsync<IList<LanguageOutputModel>>("/api/v1/languages").Result;
		var nationalities = httpClient.GetFromJsonAsync<IList<NationalityOutputModel>>("api/v1/nationalities").Result;
		var countries = httpClient.GetFromJsonAsync<IList<CountryOutputModel>>("api/v1/countries").Result;
		var pensionSituations = httpClient.GetFromJsonAsync<IList<PensionSituationOutputModel>>("api/v1/pension-situations").Result;
		var taxLiabilities = httpClient.GetFromJsonAsync<IList<TaxLiabilityOutputModel>>("api/v1/tax-liabilities").Result;
		var civilStatuses = httpClient.GetFromJsonAsync<IList<CivilStatusOutputModel>>("api/v1/civil-statuses").Result;
		var genders = httpClient.GetFromJsonAsync<IList<GenderOutputModel>>("/api/v1/genders").Result;

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Client Creation
		////////////////////////////////////////////////////////////////////////////////////////////////////

		var payload = JsonConvert.SerializeObject(new
		{
			ManagementPersonalDataConsent = DateTime.Now,
			Name = Guid.NewGuid().ToString().Replace("-", ""),
			Surname = Guid.NewGuid().ToString().Replace("-", ""),
			Email = Guid.NewGuid().ToString().Replace("-", "") + "@gmail.com",
			PhoneNumberPrefix = "0041",
			PhoneNumberNumber = "767965365",
			PensionSituationId = pensionSituations.First().Id,
			LanguageId = languages.First().Id,
			LegalAcceptanceId = legalAcceptance.Id,
			Address = new
			{
				City = "Zürich",
				Zip = "8300",
				Street = "Nostrasse",
				StreetNr = "1",
				CountryId = countries.First(x => x.Code == "CH").Id,
			},
			GenderId = genders.First().Id,
			Nationality1Id = nationalities.First(x => x.Code == "IT").Id,
			BirthDate = DateTime.Now.AddYears(-24)
		});

		var httpResponseMessage = await httpClient.PostAsync("api/v1/users", new StringContent(payload, Encoding.UTF8, "application/json"));
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			clientId = UriHelper.GetIdFromLocationUri(httpResponseMessage.Headers.Location);
		}
		else
		{
			// Get error messages
			if (httpResponseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				var errors = new Dictionary<string, string>();
				var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
				var apiErrorModel = JsonConvert.DeserializeObject<ValidationErrorModel>(content);

				foreach (var validationError in apiErrorModel.ValidationErrors)
				{
					foreach (var validationErrorDescription in validationError.Value)
					{
						Console.WriteLine(validationError.Key + " = " + validationErrorDescription.ValidationErrorMessage);
					}
				}

				return;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////
		// Client risk profile calculation, needed for portfolio creation
		////////////////////////////////////////////////////////////////////////////////////////////////////

		// Assuming the client is interested in 3A plan, we need to retrieve VORSORGE businee line and 3A investment category ID
		var businessLines = await httpClient.GetFromJsonAsync<IList<BusinessLineOutputModel>>("api/v1/business-lines");
		var businessLineVorsorge = businessLines.First(x => x.Code == "VORSORGE");

		var investmentCategories = await httpClient.GetFromJsonAsync<IList<InvestmentCategoryOutputModel>>("api/v1/investment-categories/business-line-id/" + businessLineVorsorge.Id);
		var investmentCategory3A = investmentCategories.First(x => x.Code == "3A");

		// Now get questionnaire
		var activeQuestionnaire = await httpClient.GetFromJsonAsync<QuestionnaireOutputModel>($"api/v1/questionnaires/business-line-id/{businessLineVorsorge.Id}/active");
		var activeQuestionnaireQuestions = activeQuestionnaire.Sections.SelectMany(x => x.Questions);

		// Assuming these are the client responses, now we will calculate the client risk profile
		string arg = "177,178,187,192";
		string uri = "api/v1/risk-categorizations/" + $"business-line-id/{businessLineVorsorge.Id}/calculate-risk?csv-response-ids={arg}";

		var riskCategorizationOutputModel = default(RiskCategorizationOutputModel);
		try
		{
			riskCategorizationOutputModel = await httpClient.GetFromJsonAsync<RiskCategorizationOutputModel>(uri);
		}
		catch (HttpRequestException ex)
		{
			if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				Console.WriteLine("Sorry, no risk profile for you.");

				return;
			}
		}

		// Retrieve SUGGESTED investment proposal based on client risk profile
		var proposals = await httpClient.GetFromJsonAsync<List<ProposalOutputModel>>($"api/v1/proposals/investment-category-id/{investmentCategory3A.Id}/risk-id/{riskCategorizationOutputModel.Id}");

		// Assume client selected the first one
		var proposal = proposals.First();

		////////////////////////////////////////////////////////////////////////////////////////////////////
		/// PDF Contract preparationusing some some hardcoded fields...

		var clientData = httpClient.GetFromJsonAsync<ClientOutputModel>($"api/v1/users/{clientId}").Result;

		var fields = new List<PdfPlaceHolder>();
		fields.Add(new PdfPlaceHolder("Name", clientData.Name));
		fields.Add(new PdfPlaceHolder("Surname", clientData.Surname));
		fields.Add(new PdfPlaceHolder("Language", "DE"));
		fields.Add(new PdfPlaceHolder("Gender", "MALE"));
		fields.Add(new PdfPlaceHolder("Street", "Nowhere 0"));
		fields.Add(new PdfPlaceHolder("ZipAndCity", "6000 Elsewhere"));
		fields.Add(new PdfPlaceHolder("Country", "Zambia"));
		fields.Add(new PdfPlaceHolder("Nationality", "Mars"));
		fields.Add(new PdfPlaceHolder("Birthdate", clientData.BirthDate.Value.ToString("dd.MM.yyyy")));
		fields.Add(new PdfPlaceHolder("Email", clientData.Email));
		fields.Add(new PdfPlaceHolder("Phone", clientData.PhoneNumber));

		var contractByteArray = GetPdf(configuration, fields, configuration["SuperAdminToken"]);

		// As the client agrees with the investments proposed, save his questionnaire responses: this will also set his risk profile.
		payload = JsonConvert.SerializeObject(new
		{
			BusinessLineId = businessLineVorsorge.Id,
			UserId = clientId,
			Responses = new[]
			{
				new { ResponseId = 177 },
				new { ResponseId = 178 },
				new { ResponseId = 187 },
				new { ResponseId = 192 }
			}
		});

		_ = httpClient.PostAsync("api/v1/user-risk-categorizations", new StringContent(payload, Encoding.UTF8, "application/json")).Result;

		// Now submit the order
		var accountHoldingInstitution = httpClient.GetFromJsonAsync<AccountHoldingInstitutionOutputModel>("api/v1/account-holding-institutions/code/LIENHARDT").Result;

		var orderPortfolioCreationInputModel = new
		{
			AccountHoldingInstitutionCode = accountHoldingInstitution.Code,
			UserId = clientId,
			InvestmentCategoryId = investmentCategory3A.Id,
			ProposalId = proposal.Id,
			Name = "it can be empty",
			ReasonToChangeProposalResponsesIds = default(List<long>) // In case user agreed with suggested proposal, this list can be left empty
		};

		var httpContent = HttpHelper.CreateMultipartFormDataHttpContent(orderPortfolioCreationInputModel, contractByteArray, "UserContracts");
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
						Console.WriteLine(validationError.Key + " = " + validationErrorDescription.ValidationErrorMessage);
					}
				}

				return;
			}
		}

		Console.WriteLine("Order URL = " + UriHelper.GetIdFromLocationUri(httpResponseMessage.Headers.Location));
	}

	#region Private Methods

	private static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((_, config) =>
			{
				config.AddJsonFile("appsettings.json", optional: false);
			})
			.ConfigureServices((hostContext, services) =>
			{
				var config = hostContext.Configuration;
			});

	private static byte[] GetPdf(IConfiguration configuration, IEnumerable<PdfPlaceHolder> placeHolderValues, string accessToken)
	{
		var payload = new PdfDataInputModel
		{
			PlaceHolderValues = placeHolderValues.Select(x => new FormField(x.Name, x.Value)).ToList()
		};

		var byteArray = FileContentHelper.GetFileContent(typeof(Program), "Contract-3A.pdf");

		using var httpClient = new HttpClient { BaseAddress = new Uri(configuration["PdfFillerBaseUrl"]) };
		httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
		httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
		httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

		var httpContent = HttpHelper.CreateMultipartFormDataHttpContent(payload, byteArray, "Attachments");
		var httpResponseMessage = httpClient.PostAsync("api/v1/pdf-service/fill-pdf-with-data", httpContent).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return httpResponseMessage.Content.ReadAsByteArrayAsync().Result;
		}

		return default;
	}

	#endregion
}
