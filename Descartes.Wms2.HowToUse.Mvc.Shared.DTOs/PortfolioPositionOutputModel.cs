namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class PortfolioPositionOutputModel
	{
		public long Id { get; set; }
		public decimal Allocation { get; set; }
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal PriceInChf { get; set; }
		public decimal Performance { get; set; }
		public DateTime Date { get; set; }
		public long CurrencyId { get; set; }
		public long SecurityId { get; set; }
	}
}