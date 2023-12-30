namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ClientAddressOutputModel
	{
		public long Id { get; set; }
		public string Street { get; set; }
		public string StreetNr { get; set; }
		public string Zip { get; set; }
		public string City { get; set; }
		public long CountryId { get; set; }

		public string CoName { get; set; }
		public string CoSurname { get; set; }
	}
}
