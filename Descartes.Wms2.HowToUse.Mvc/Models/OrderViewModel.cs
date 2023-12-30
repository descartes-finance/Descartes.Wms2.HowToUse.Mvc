using System.ComponentModel.DataAnnotations;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class OrderViewModel
	{
		public string PortfolioName { get; set; }

		[Required]
		public string Document { get; set; }
	}
}
