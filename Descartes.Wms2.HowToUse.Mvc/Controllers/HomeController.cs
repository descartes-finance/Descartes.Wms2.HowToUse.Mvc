using Descartes.Wms2.HowToUse.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class HomeController : Controller
	{
		//[HttpGet]
		//public IActionResult WebHookTest()
		//{
		//	using var httpClient = new HttpClient { BaseAddress = new Uri("https://webhook.site") };
		//	httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
		//	httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));

		//	var httpResponseMessage = httpClient.PostAsync("/token", null).Result;
		//	var responseAsString = httpResponseMessage.Content.ReadAsStringAsync().Result;
		//	var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseAsString);
		//	var token = response["uuid"];

		//	var payload = JsonConvert.SerializeObject(new { Name = "Ismano", Surname = "Del Bianco" });
		//	httpResponseMessage = httpClient.PostAsync("d14b2889-df1c-4c41-9b8f-6013df580761", new StringContent(payload, Encoding.UTF8, "application/json")).Result;

		//	var payloadSent = httpClient.GetFromJsonAsync<Dictionary<string, string>>("https://webhook.site/token/d14b2889-df1c-4c41-9b8f-6013df580761/request/latest/raw").Result;

		//	return this.Ok();
		//}

		//[HttpGet]
		//public IActionResult WebHookTest()
		//{
		//	using var httpClient = new HttpClient { BaseAddress = new Uri("https://webhook.site") };
		//	httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
		//	httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));

		//	// var payloadSent = httpClient.GetStringAsync("https://webhook.site/token/e070063f-b1a5-4201-8a9a-35e9e54c8ff6/requests?sorting=newest&page=1").Result;
		//	var payloadSent = httpClient.GetFromJsonAsync<object>("https://webhook.site/token/e070063f-b1a5-4201-8a9a-35e9e54c8ff6/requests?sorting=newest&page=1").Result;

		//	// "text_content"
		//	// "subject"

		//	return this.Ok();
		//}

		[HttpGet]
		public IActionResult Index()
		{
			return this.View("/Views/Home/Home.cshtml");
		}

		[HttpPost]
		public IActionResult CreateAsAdmin()
		{
			return this.LocalRedirect("/Admin/CreateClient");
		}

		[HttpPost]
		public IActionResult ClientCockpit(GetClientIdViewModel viewModel)
		{
			return this.LocalRedirect("/Client/GetClientId");
		}

		[HttpPost]
		public IActionResult CreateAsClient()
		{
			return this.LocalRedirect("/Client/FirstFactorRegistration");
		}
	}
}
