namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class ClientOutputModel
	{
		public long Id { get; set; }
		public string AccountNumber { get; set; }
		public long? TitleId { get; set; }
		public string Name { get; set; }
		public string Surname { get; set; }
		public long GenderId { get; set; }
		public long LanguageId { get; set; }
		public long? TaxLiabilityId { get; set; }
		public long? CivilStatusId { get; set; }
		public DateTime? CivilStatusDate { get; set; }
		public long? WorkSituationId { get; set; }
		public long? PensionSituationId { get; set; }
		public string Email { get; set; }
		public string PhoneNumberPrefix { get; set; }
		public string PhoneNumberNumber { get; set; }
		public string PhoneNumber => this.PhoneNumberPrefix + this.PhoneNumberNumber;
		public DateTime? BirthDate { get; set; }
		public bool IsDeleted { get; set; }
		public bool DeliverTaxStatement { get; set; }
		public DateTime Created { get; set; }
		public DateTime? Modified { get; set; }
		public bool IsLocked { get; set; }

		public ClientAddressOutputModel Address { get; set; }
		public IList<ClientNationalityOutputModel> Nationalities { get; set; }
	}
}
