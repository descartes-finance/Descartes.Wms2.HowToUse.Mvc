namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class PortfolioOrderOutputModel
	{
		public long PortfolioId { get; set; }
		public long Id { get; set; }
		public string ExternalId { get; set; }
		public long ProposalId { get; set; }
		public DateTime Created { get; set; }
		public DateTime? DeliveringDate { get; set; }
		public DateTime? ClosingDate { get; set; }

		public long OrderStatusId { get; set; }
		public DateTime? OrderStatusDate { get; set; }

		public long OrderTypeId { get; set; }
		public long? OrderIdToFixOrRevoke { get; set; }

		public string SubmissionErrorMessage { get; set; }
		public string ReasonForRejectionCode { get; set; }
		public string ReasonForRejection { get; set; }

		public string CorrelationId { get; set; }

		public bool ResendingPossible { get; set; }
		public long? UserPortfolioOrderIdResending { get; set; }

		public ICollection<PortfolioOrderDocumentOutputModel> Documents { get; set; }
	}
}
