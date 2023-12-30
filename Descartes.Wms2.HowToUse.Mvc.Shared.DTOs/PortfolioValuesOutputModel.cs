namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class PortfolioValueOutputModel
	{
		public DateTime Date { get; set; }
		public decimal? PerformanceSinceInception { get; set; }
		public decimal? PerformanceYearToDate { get; set; }
		public decimal? ProfitOrLost { get; set; }
		public decimal? PortfolioMarketValue { get; set; }
		public decimal? CashAmount { get; set; }
		public decimal? PortfolioTotalAssets { get; set; }
	}

	public class PortfolioValuesOutputModel
	{
        public long PortfolioId { get; set; }
		public string Name { get; set; }
		public string Color { get; set; }

		public IList<PortfolioValueOutputModel> Values { get; set; } = new List<PortfolioValueOutputModel>();
	}
}