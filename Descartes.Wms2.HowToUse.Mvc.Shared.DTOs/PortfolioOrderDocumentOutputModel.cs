namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class PortfolioOrderDocumentOutputModel
	{
		public long Id { get; set; }
		public long OrderId { get; set; }
		public string DocumentType { get; set; }
		public string Reference { get; set; }
		public long Size { get; set; }
	}
}