using System.Net.Http.Headers;

using Descartes.Wms2.HowToUse.Mvc.Extensions;
using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class ClientPositionController : Controller
	{
		private IConfiguration _configuration;

		public ClientPositionController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfoliosList()
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var clientData = await httpClient.GetFromJsonAsync<ClientMinimalDataOutputModel>($"api/v1/users/{this.HttpContext.Session.Get<long>("ClientId")}");
			var clientPortfolios = await httpClient.GetFromJsonAsync<IList<PortfolioOutputModel>>($"api/v1/user-portfolios/user-id/{this.HttpContext.Session.Get<long>("ClientId")}");

			return this.View("/Views/Client/PortfoliosList.cshtml", new PortfoliosListViewModel
			{
				ClientName = clientData.Name,
				ClientSurname = clientData.Surname,
				Portfolios = clientPortfolios
			});
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfolioDetail(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var clientData = await httpClient.GetFromJsonAsync<ClientMinimalDataOutputModel>($"api/v1/users/{this.HttpContext.Session.Get<long>("ClientId")}");
			var clientPortfolio = await httpClient.GetFromJsonAsync<PortfolioOutputModel>($"api/v1/user-portfolios/portfolio-id/{portfolioId}?portfolio-view-as=Snapshot");

			var clientCockpitViewModels = new PortfolioDetailViewModel
			{
				ClientSurname = clientData.Surname,
				ClientName = clientData.Name,
				Portfolio = clientPortfolio
			};

			return this.View("/Views/Client/PortfolioDetail.cshtml", clientCockpitViewModels);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfolioPerformances(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var portfolioPerformances = await httpClient.GetFromJsonAsync<PortfolioValuesOutputModel>($"api/v1/user-portfolios/portfolio-id/{portfolioId}/performances");

			return this.View("/Views/Client/PortfolioPerformances.cshtml", portfolioPerformances);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfolioMarketValues(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var portfolioPerformances = await httpClient.GetFromJsonAsync<PortfolioValuesOutputModel>($"api/v1/user-portfolios/portfolio-id/{portfolioId}/performances");

			return this.View("/Views/Client/PortfolioValues.cshtml", portfolioPerformances);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfolioCashAmount(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			// Get bank account types: Cash account/Custody account
			var bankAccountTypes = await httpClient.GetFromJsonAsync<List<BankAccountTypeOutputModel>>($"api/v1/cash-account-types");
			var casAccountType = bankAccountTypes.First(x => x.Code == "CASH-ACCOUNT");

			// Get portfolio's bank accounts
			var bankAccounts = await httpClient.GetFromJsonAsync<List<CashAccountOutputModel>>($"api/v1/user-cash-accounts/portfolio-id/{portfolioId}");
			var cashAccount = bankAccounts.First(x => x.AccountTypeId == casAccountType.Id);

			return this.View("/Views/Client/CashAccountBalance.cshtml", cashAccount);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> PortfolioCashAmountMovements(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			// Get bank account types: Cash account/Custody account
			var bankAccountTypes = await httpClient.GetFromJsonAsync<List<BankAccountTypeOutputModel>>($"api/v1/cash-account-types");
			var casAccountType = bankAccountTypes.First(x => x.Code == "CASH-ACCOUNT");

			// Get portfolio's bank accounts
			var bankAccounts = await httpClient.GetFromJsonAsync<List<CashAccountOutputModel>>($"api/v1/user-cash-accounts/portfolio-id/{portfolioId}");
			var cashAccount = bankAccounts.First(x => x.AccountTypeId == casAccountType.Id);

			// Get ALL movements
			var bankAccountMovements = await httpClient.GetFromJsonAsync<List<CashAccountMovementOutputModel>>($"api/v1/user-cash-accounts/cash-account-id/{cashAccount.Id}/movements");

			var allMovements = new List<CashAccountMovementViewModel>();

			foreach (var bankAccountMovement in bankAccountMovements)
			{
				var bookingCode = await this.GetBookingCodeById(bankAccountMovement.BookingCodeId);

				allMovements.Add(new CashAccountMovementViewModel
				{
					BookingCode = bookingCode.Code,
					BookingCodeDescription = bookingCode.Description,
					BookingDate = bankAccountMovement.BookingDate,
					MovementAmount = bankAccountMovement.Amount,
					MovementDescription = bankAccountMovement.BookingDescription,
					Saldo = bankAccountMovement.Saldo,
					TransactionId = bankAccountMovement.TransactionId,
					UsetText = bankAccountMovement.UsetText
				});
			}

			return this.View("/Views/Client/CashAccountMovements.cshtml", allMovements);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> BankDocumentList(long? portfolioId)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false)
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var url = $"api/v1/user-bank-documents/user-id/{this.HttpContext.Session.Get<long>("ClientId")}/list?starting-from=20000101";
			var bankDocuments = await httpClient.GetFromJsonAsync<IList<UserBankDocumentInfoOutputModel>>(url);
			var bankDocumentsPerPortfolio = bankDocuments.Where(x => x.UserPortfolioId == portfolioId).ToList();
			var bankDocumentTypes = await httpClient.GetFromJsonAsync<IList<BankDocumentTypeOutputModel>>("api/v1/bank-document-types");

			var bankDocumentListViewModel = new BankDocumentListViewModel
			{
				PortfolioId = portfolioId.GetValueOrDefault(),
				BankDocuments = bankDocumentsPerPortfolio.Select(x => new BankDocumentViewModel
				{
					Reference = x.Reference,
					Created = x.Created,
					Name = x.Description,
					DocumentType = bankDocumentTypes.First(d => d.Id == x.DocumentTypeId).Description
				}).ToList()
			};

			return this.View("/Views/Client/BankDocumentList.cshtml", bankDocumentListViewModel);
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> DownloadDocument(long? portfolioId, string reference)
		{
			var clientId = this.HttpContext.Session.Get<long?>("ClientId");
			if (clientId.HasValue == false || portfolioId.HasValue == false || string.IsNullOrWhiteSpace(reference))
			{
				return this.Redirect("/Client/GetClientId");
			}

			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			var url = $"api/v1/user-bank-documents/user-id/{clientId}/portfolio-id/{portfolioId}/download-bank-document/reference/{reference}";
			var byteArray = await httpClient.GetByteArrayAsync(url);

			return base.File(byteArray, "application/pdf");
		}

		#region Private Methods

		private async Task<BookingCodeOutputModel> GetBookingCodeById(long bookingCodeId)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			return await httpClient.GetFromJsonAsync<BookingCodeOutputModel>($"api/v1/booking-codes/id/{bookingCodeId}");
		}

		#endregion
	}
}
