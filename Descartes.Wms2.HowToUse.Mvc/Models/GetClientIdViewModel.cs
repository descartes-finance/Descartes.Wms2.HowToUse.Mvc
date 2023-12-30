using System.ComponentModel.DataAnnotations;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class GetClientIdViewModel
	{
		[Required]
		public string Token { get; set; }

		[Required]
		public long? ClientId { get; set; }
	}
}
