using Descartes.Wms2.HowToUse.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class HomeController : Controller
	{
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
