using System.Net.Http.Headers;
using System.Text;

using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class SuperAdminController : Controller
	{
		private IConfiguration _configuration;

		public SuperAdminController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public IActionResult CreateAdmin()
		{
			return base.View("/Views/SuperAdmin/AdminCreating.cshtml");
		}

		[HttpPost]
		public IActionResult CreateAdmin(SuperAdminViewModel viewModel)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["SuperAdminToken"]);

			var clientEmail = $"{Guid.NewGuid().ToString().Replace("-", string.Empty)}@gmail.com";
			var name = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(4, 6);
			var surname = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(1, 7);
			var day = new Random().Next(1, 28);
			var month = new Random().Next(1, 12);
			var year = new Random().Next(1965, 2005);
			var clientBirthDate = new DateTime(year, month, day);

			var gender = httpClient.GetFromJsonAsync<GenderOutputModel>("/api/v1/genders/code/MALE").Result;
			var language = httpClient.GetFromJsonAsync<LanguageOutputModel>("/api/v1/languages/code/DE").Result;

			var createAdminPayload = new
			{
				Name = name,
				Surname = surname,
				Email = (name + "." + surname + "@gmail.com").ToLowerInvariant(),
				PhoneNumberPrefix = "0041",
				PhoneNumberNumber = "767965365",
				Url = viewModel.Url, // Please splecify YOUR url
				GenderId = gender.Id,
				LanguageId = language.Id
			};

			var payLoadAsString = JsonConvert.SerializeObject(createAdminPayload);

			var response = httpClient.PostAsync($"api/v1/users/admins", new StringContent(payLoadAsString, Encoding.UTF8, "application/json")).Result;

			viewModel.Id = this.GetIdFromLocationUri(response.Headers.Location);
			viewModel.Name = createAdminPayload.Name;
			viewModel.Surname = createAdminPayload.Surname;

			return base.View("/Views/SuperAdmin/AdminCreated.cshtml", viewModel);
		}

		private long GetIdFromLocationUri(Uri uri)
		{
			var theUriAsString = uri.GetLeftPart(UriPartial.Path);
			return long.Parse(theUriAsString.Split('/').ToList().Last());
		}
	}
}
