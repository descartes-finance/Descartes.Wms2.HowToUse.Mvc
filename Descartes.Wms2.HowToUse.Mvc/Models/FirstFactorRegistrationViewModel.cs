using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class FirstFactorRegistrationViewModel
	{
		public string ClientName { get; set; }
		public string ClientSurname { get; set; }
		public string ClientEmail { get; set; }
		public DateTime ClientBirthDate { get; set; }

		public IList<GenderOutputModel> Genders { get; set; }
	}
}
