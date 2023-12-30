namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class RiskCategorizationOutputModel
	{
		public long Id { get; set; }

		public IList<long> BusinessLineIds { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public string ProfileName { get; set; }

		public string ProfileDescription { get; set; }

		public string StrategyName { get; set; }

		public string StrategyDescription { get; set; }

		public int TimeHorizon { get; set; }

		public string TimeHorizonDescription { get; set; }

		public int RiskyAssets { get; set; }

		public int LessRiskyAssets { get; set; }

		public string AlternativeNickName { get; set; }

		public string AlternativeName { get; set; }

		public string AlternativeDescription { get; set; }
	}
}