namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class UserBankDocumentInfoOutputModel
	{
		public long UserPortfolioId { get; set; }
		public long DocumentTypeId { get; set; }
		public string DocumentTypeCode { get; set; }
		public string Description { get; set; }
		public string Reference { get; set; }
		public DateTime Created { get; set; }
		public DateTime Viewed { get; set; }
	}
}
