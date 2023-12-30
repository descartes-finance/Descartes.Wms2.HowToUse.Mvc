using Descartes.Wms2.HowToUse.Mvc.Extensions;
using Descartes.Wms2.HowToUse.Mvc.Models;

using Microsoft.AspNetCore.Mvc;

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
			viewModel.Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwcmltYXJ5Z3JvdXBzaWQiOiJERVNDQVJURVMiLCJuYW1laWQiOiIxIiwidW5pcXVlX25hbWUiOiIxIiwiZ2l2ZW5fbmFtZSI6IkN1bG8iLCJmYW1pbHlfbmFtZSI6IlJvdHRvIiwicm9sZSI6IkFkbWluIiwibmJmIjoxNjY0OTY1ODM3LCJleHAiOjIxMzgzNTE0MzcsImlhdCI6MTY2NDk2NTgzN30.MTyQ77tS5Y1c5Kw_7QmNohLSqMgD1mjj1M1BvaHcvEQ";
#endif

			return this.View("/Views/Client/GetClientId.cshtml", viewModel);
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
