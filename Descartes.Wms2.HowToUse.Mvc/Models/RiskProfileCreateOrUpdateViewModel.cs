using System.ComponentModel.DataAnnotations;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class QuestionnaireResponse
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public decimal Score { get; set; }
	}

	public class QuestionnaireQuestion
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }

		public IList<QuestionnaireResponse> Responses { get; set; }
	}

	public class QuestionnaireSection
	{
		public long Id { get; set; }
		public string Code { get; set; }
		public int Order { get; set; }
		public string Description { get; set; }

		public IList<QuestionnaireQuestion> Questions { get; set; }
	}

	public class Questionnaire
	{
		public long Id { get; set; }

		public IList<QuestionnaireSection> Sections { get; set; }

		public IList<QuestionnaireResponse> AllResponses
		{
			get
			{
				if (this.Sections == null)
				{
					return default;
				}

				var responses = new List<QuestionnaireResponse>();

				foreach (var question in this.Sections.SelectMany(s => s.Questions))
				{
					responses.AddRange(question.Responses);
				}

				return responses;
			}
		}
	}

	public class CustomerQuestionnaireResponse
	{
		public long QuestionnaireResponseId { get; set; }
	}

	public class RiskProfileCreateOrUpdateViewModel
	{
		[Required]
		public long? ClientId { get; set; }

		[Required]
		public long? BusinessLineId { get; set; }

		[Required]
		public long? InvestmentCategoryId { get; set; }

		[Required]
		public long? QuestionnaireId { get; set; }

		[Required]
		public ICollection<string> Responses { get; set; }

		public Questionnaire Questionnaire { get; set; }
		public IList<CustomerQuestionnaireResponse> CustomerQuestionnaireResponses { get; set; }
	}
}