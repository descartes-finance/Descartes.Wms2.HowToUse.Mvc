namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ClientQuestionnaireResponseOutputModel
	{
		public long QuestionnaireQuestionId { get; set; }
		public long QuestionnaireResponseId { get; set; }
		public decimal? ResponseValue { get; set; }
	}

	public class ClientQuestionnaireOutputModel
	{
		public long UserId { get; set; }
		public long BusinessLineId { get; set; }
		public long QuestionnaireId { get; set; }
		public DateTime Created { get; set; }
		public bool IsExpired { get; set; }

		public IEnumerable<ClientQuestionnaireResponseOutputModel> Responses { get; set; }
	}
}
