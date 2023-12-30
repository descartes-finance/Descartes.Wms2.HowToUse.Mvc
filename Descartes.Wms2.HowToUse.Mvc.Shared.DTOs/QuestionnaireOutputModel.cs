namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class QuestionnaireResponseOutputModel
	{
		public long Id { get; set; }
		public string Description { get; set; }
		public decimal Score { get; set; }
		public int Order { get; set; }
		public string Code { get; set; }
		public decimal? MinValue { get; set; }
		public decimal? MaxValue { get; set; }
	}

	public class QuestionnaireQuestionOutputModel
	{
		public long Id { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public string Code { get; set; }

		public IList<QuestionnaireResponseOutputModel> Responses { get; set; }
	}

	public class QuestionnaireSectionOutputModel
	{
		public long Id { get; set; }
		public string Description { get; set; }
		public int Order { get; set; }
		public string Code { get; set; }

		public IList<QuestionnaireQuestionOutputModel> Questions { get; set; }
	}

	public class QuestionnaireOutputModel
	{
		public long Id { get; set; }
		public IList<long> BusinessLineIds { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public bool IsDeleted { get; set; }

		public IList<QuestionnaireSectionOutputModel> Sections { get; set; }
	}
}