using System.Net.Http.Headers;
using System.Text;

using Descartes.Wms2.HowToUse.Mvc.Extensions;
using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class ClientController : Controller
	{
		private IConfiguration _configuration;

		public ClientController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public IActionResult GetClientId()
		{
			var viewModel = new GetClientIdViewModel();
#if DEBUG

			viewModel.ClientId = 226;
			viewModel.Token = "XXXXXXXXXXXXXXXX";
#endif

			return this.View("/Views/Client/GetClientId.cshtml", viewModel);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]

		[HttpPost]
		public IActionResult UpdateClientAddress(
			string street,
			string streetNr,
			string city,
			string zip,
			long countryId)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var userAddressInputModel = new
			{
				Street = street,
				StreetNr = streetNr,
				City = city,
				CountryId = countryId,
				Zip = zip
			};

			var userId = this.HttpContext.Session.Get<long>("ClientId");
			var userAddressInputModelAsString = JsonConvert.SerializeObject(userAddressInputModel);
			_ = httpClient.PutAsync($"api/v1/users/update-address/user-id/{userId}", new StringContent(userAddressInputModelAsString, Encoding.UTF8, "application/json")).Result;

			var clientData = httpClient.GetFromJsonAsync<ClientOutputModel>($"api/v1/users/{userId}").Result;

			return this.View("/Views/Client/UpdateAddress.cshtml", clientData);
		}

		[HttpPost]
		public IActionResult UpdateClient(
			long genderId,
			string name,
			string surname,
			string phoneNumberPrefix,
			string phoneNumberNumber,
			string email,
			DateTime birthDate,
			long civilStatusId,
			DateTime? civilStatusDate,
			long languageId,
			long nationalityId,
			bool deliverTaxStatement,
			long taxLiaabilityId,
			long workSituationId,
			long pensionSituationId)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var clientUpdateInputModel = new
			{
				Name = name,
				Surname = surname,
				Email = email,
				PhoneNumberPrefix = phoneNumberPrefix,
				PhoneNumberNumber = phoneNumberNumber,
				Url = "https://webhook.site/630f822b-53b5-4de2-afc6-85876c8d4abf",
				BirthDate = birthDate,
				CivilStatusId = civilStatusId,
				CivilStatusDate = civilStatusDate,
				GenderId = genderId,
				LanguageId = languageId,
				Nationality1Id = nationalityId,
				DeliverTaxStatement = deliverTaxStatement,
				TaxLiabilityId = taxLiaabilityId,
				WorkSituationId = workSituationId,
				PensionSituationId = pensionSituationId
			};

			var userId = this.HttpContext.Session.Get<long>("ClientId");
			var clientUpdateInputModelAsString = JsonConvert.SerializeObject(clientUpdateInputModel);
			_ = httpClient.PutAsync($"api/v1/users/user-id/{userId}", new StringContent(clientUpdateInputModelAsString, Encoding.UTF8, "application/json")).Result;

			var clientData = httpClient.GetFromJsonAsync<ClientOutputModel>($"api/v1/users/{userId}").Result;

			return this.View("/Views/Client/Update.cshtml", clientData);
		}

		[HttpPost]
		public IActionResult SetClientId(GetClientIdViewModel viewModel)
		{
			if (!this.ModelState.IsValid)
			{
				return this.View("/Views/Client/GetClientId.cshtml", viewModel);
			}

			this.HttpContext.Session.Set<long>("ClientId", viewModel.ClientId.Value);
			this.HttpContext.Session.Set<string>("Token", viewModel.Token);

			return this.LocalRedirect("/ClientPosition/PortfoliosList");
		}

		[HttpGet]
		public async Task<IActionResult> FirstFactorRegistration()
		{
			var clientEmail = $"{Guid.NewGuid().ToString().Replace("-", string.Empty)}@gmail.com";
			var clientName = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(4, 6);
			var clientSurname = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(1, 7);
			var day = new Random().Next(1, 28);
			var month = new Random().Next(1, 12);
			var year = new Random().Next(1965, 2005);
			var clientBirthDate = new DateTime(year, month, day);

			var fistFactorRegistrationViewModel = new FirstFactorRegistrationViewModel
			{
				ClientBirthDate = clientBirthDate,
				ClientEmail = clientEmail,
				ClientName = clientName,
				ClientSurname = clientSurname
			};

			return this.View("/Views/Client/FirstFactorRegistration.cshtml", fistFactorRegistrationViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> FirstFactorRegistration(FirstFactorRegistrationViewModel firstFactorRegistrationViewModel)
		{


			return this.View("/Views/Client/FirstFactorRegistration.cshtml");
		}
	}
}
