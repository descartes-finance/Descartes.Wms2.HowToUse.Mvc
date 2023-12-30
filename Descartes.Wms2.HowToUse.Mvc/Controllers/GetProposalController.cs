using System.Net.Http.Headers;

using Descartes.Wms2.HowToUse.Mvc.Extensions;
using Descartes.Wms2.HowToUse.Mvc.Models;
using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

using Microsoft.AspNetCore.Mvc;

namespace Descartes.Wms2.HowToUse.Mvc.Controllers
{
	public class GetProposalController : Controller
	{
		private IConfiguration _configuration;

		public GetProposalController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpGet]
		public async Task<IActionResult> RiskProfileCalculation()
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			// Assuming the client is interested in 3A plan, we need to retrieve VORSORGE businee line and 3A investment category ID
			var businessLines = await httpClient.GetFromJsonAsync<IList<BusinessLineOutputModel>>("api/v1/business-lines");
			var businessLineVorsorge = businessLines.First(x => x.Code == "VORSORGE");

			var investmentCategories = await httpClient.GetFromJsonAsync<IList<InvestmentCategoryOutputModel>>("api/v1/investment-categories/business-line-id/" + businessLineVorsorge.Id);
			var investmentCategory3A = investmentCategories.First(x => x.Code == "3A");

			// Now get questionnaire
			var activeQuestionnaire = await httpClient.GetFromJsonAsync<QuestionnaireOutputModel>($"api/v1/questionnaires/business-line-id/{businessLineVorsorge.Id}/active");
			var activeQuestionnaireQuestions = activeQuestionnaire.Sections.SelectMany(x => x.Questions);

			// Get client responses, if present.
			var clientQuestionnaireOutputModel = new ClientQuestionnaireOutputModel();

			try
			{
				clientQuestionnaireOutputModel = await httpClient.GetFromJsonAsync<ClientQuestionnaireOutputModel>("api/v1/user-questionnaires/user-id/" + this.HttpContext.Session.Get<long>("ClientId") + "/business-line-id/" + businessLineVorsorge.Id);
			}
			catch (HttpRequestException ex)
			{
				if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					clientQuestionnaireOutputModel = null;
				}
			}

			var responses = clientQuestionnaireOutputModel != null 
				? clientQuestionnaireOutputModel.Responses.Select(x => new CustomerQuestionnaireResponse { QuestionnaireResponseId = x.QuestionnaireResponseId }).ToList()
				: null;

#if DEBUG
			if (responses == null || responses.Any())
			{
				responses = new List<CustomerQuestionnaireResponse>
				{
					new CustomerQuestionnaireResponse { QuestionnaireResponseId = 177 },
					new CustomerQuestionnaireResponse { QuestionnaireResponseId = 178 },
					new CustomerQuestionnaireResponse { QuestionnaireResponseId = 187 },
					new CustomerQuestionnaireResponse { QuestionnaireResponseId = 192 },
				};
			}
#endif

            // Prepare the view model with questionnaire questions and client responses if presents.
            var viewModel = new RiskProfileCreateOrUpdateViewModel
			{
				BusinessLineId = businessLineVorsorge.Id,
				InvestmentCategoryId = investmentCategory3A.Id,
				Questionnaire = ToQuestionnaire(activeQuestionnaire),
				ClientId = this.HttpContext.Session.Get<long>("ClientId"),
				CustomerQuestionnaireResponses = responses
			};

			return this.View("/Views/GetProposal/RiskProfileCalculationResult.cshtml", viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> RiskProfileCalculation(
			RiskProfileViewModel riskProfileViewModel)
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			if (!this.ModelState.IsValid)
			{
				var activeQuestionnaire = await httpClient.GetFromJsonAsync<QuestionnaireOutputModel>($"api/v1/questionnaires/business-line-id/{riskProfileViewModel.BusinessLineId}/active");
				var activeQuestionnaireQuestions = activeQuestionnaire.Sections.SelectMany(x => x.Questions);

				var viewModel = this.BuildRiskProfileCreateOrUpdateViewModelAsync(riskProfileViewModel, ToQuestionnaire(activeQuestionnaire));
				return this.View("/Views/GetProposal/RiskProfileCalculationResult.cshtml", viewModel);
			}

			// Calculate risk profile
			string arg = string.Join(",", riskProfileViewModel.Responses.Select(long.Parse).ToList());
			string uri = "api/v1/risk-categorizations/" + $"business-line-id/{riskProfileViewModel.BusinessLineId}/calculate-risk?csv-response-ids={arg}";

			var riskCategorizationOutputModel = default(RiskCategorizationOutputModel);
			try
			{
				riskCategorizationOutputModel = await httpClient.GetFromJsonAsync<RiskCategorizationOutputModel>(uri);
			}
			catch (HttpRequestException ex)
			{
				if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
				{
					this.ViewData.Add("Error", "Sorry, no risk profile for you.");

					var activeQuestionnaire = await httpClient.GetFromJsonAsync<QuestionnaireOutputModel>($"api/v1/questionnaires/business-line-id/{riskProfileViewModel.BusinessLineId}/active");
					var activeQuestionnaireQuestions = activeQuestionnaire.Sections.SelectMany(x => x.Questions);

					var viewModel = this.BuildRiskProfileCreateOrUpdateViewModelAsync(riskProfileViewModel, ToQuestionnaire(activeQuestionnaire));
					return this.View("/Views/GetProposal/RiskProfileCalculationResult.cshtml", viewModel);
				}

				throw ex;
			}

			var riskCategorizazionId = riskCategorizationOutputModel.Id;
			this.HttpContext.Session.Set<long>("ClientRiskId", riskCategorizazionId);
            this.HttpContext.Session.Set<long>("BusinessLineId", riskProfileViewModel.BusinessLineId.Value);
            this.HttpContext.Session.Set<long>("InvestmentCategoryId", riskProfileViewModel.InvestmentCategoryId.Value);
            this.HttpContext.Session.Set<long>("Response1Id", long.Parse(riskProfileViewModel.Responses.ElementAt(0)));
            this.HttpContext.Session.Set<long>("Response2Id", long.Parse(riskProfileViewModel.Responses.ElementAt(1)));
            this.HttpContext.Session.Set<long>("Response3Id", long.Parse(riskProfileViewModel.Responses.ElementAt(2)));
            this.HttpContext.Session.Set<long>("Response4Id", long.Parse(riskProfileViewModel.Responses.ElementAt(3)));

            return this.LocalRedirect("/GetProposal/ShowProposal");
		}

		[HttpGet]
		[ResponseCache(NoStore = true, Duration = 0)]
		public async Task<IActionResult> ShowProposal()
		{
			using var httpClient = new HttpClient { BaseAddress = new Uri(_configuration["WmsBaseUrl"]) };
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json", 1));
			httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("de-DE", 1));
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.HttpContext.Session.Get<string>("Token"));

			// Get suggested proposal
			var proposals = await httpClient.GetFromJsonAsync<List<ProposalOutputModel>>($"api/v1/proposals/investment-category-id/{this.HttpContext.Session.Get<long>("InvestmentCategoryId")}/risk-id/{this.HttpContext.Session.Get<long>("ClientRiskId")}");
			var proposalSelectedByUser = proposals.First();

			this.HttpContext.Session.Set<long>("ProposalId", proposalSelectedByUser.Id);

			return this.View("/Views/GetProposal/ShowProposal.cshtml", proposalSelectedByUser);
		}

		#region Private Methods

		private RiskProfileCreateOrUpdateViewModel BuildRiskProfileCreateOrUpdateViewModelAsync(
			RiskProfileViewModel payload,
			Questionnaire questionnaire)
		{
			var viewModel = new RiskProfileCreateOrUpdateViewModel
			{
				BusinessLineId = payload.BusinessLineId,
				InvestmentCategoryId = payload.InvestmentCategoryId,
				Questionnaire = questionnaire,
				CustomerQuestionnaireResponses = payload.Responses.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new CustomerQuestionnaireResponse { QuestionnaireResponseId = long.Parse(x) }).ToList()
			};

			if (payload.Responses != null && payload.Responses.Any())
			{
				var customerQuestionnaireResponse = new List<CustomerQuestionnaireResponse>();
				var allPossibleResponses = questionnaire.AllResponses;

				foreach (var responseIdFromUser in payload.Responses.Where(x => !string.IsNullOrWhiteSpace(x)))
				{
					var questionnaireResponse = allPossibleResponses.FirstOrDefault(x => x.Id == long.Parse(responseIdFromUser));
					if (questionnaireResponse != null)
					{
						customerQuestionnaireResponse.Add(new CustomerQuestionnaireResponse { QuestionnaireResponseId = questionnaireResponse.Id });
					}
				}

				viewModel.CustomerQuestionnaireResponses = customerQuestionnaireResponse;
			}

			return viewModel;
		}

		private static Questionnaire ToQuestionnaire(
			QuestionnaireOutputModel outputModel)
		{
			if (outputModel == null)
			{
				return default;
			}

			return new Questionnaire
			{
				Id = outputModel.Id,
				Sections = outputModel.Sections.Select(s => new QuestionnaireSection
				{
					Id = s.Id,
					Code = s.Code,
					Description = s.Description,
					Order = s.Order,
					Questions = s.Questions.Select(q => new QuestionnaireQuestion
					{
						Id = q.Id,
						Code = q.Code,
						Order = q.Order,
						Description = q.Description,
						Responses = q.Responses.Select(r => new QuestionnaireResponse
						{
							Id = r.Id,
							Code = r.Code,
							Description = r.Description,
							Score = r.Score
						}).ToList()
					}).ToList()
				}).ToList()
			};
		}

		#endregion
	}
}