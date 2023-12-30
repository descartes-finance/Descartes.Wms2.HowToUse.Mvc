using System.ComponentModel.DataAnnotations;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class ClientViewModel : IValidatableObject
	{
		[Required]
		public long? LegalAcceptanceId { get; set; }

		public long? ClientId { get; set; }

		[Required]
		public string Token { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Surname { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		public string PhonePrefix { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public long? GenderId { get; set; }

		[Required]
		public long? LanguageId { get; set; }

		[Required]
		public string Street { get; set; }

		[Required]
		public string StreetNr { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string Zip { get; set; }

		[Required]
		public long? CountryId { get; set; }

		[Required]
		public long? PensionSituationId { get; set; }

		[Required]
		public long? NationalityId { get; set; }

		[Required]
		public long? CivilStatusId { get; set; }

		public DateTime? CivilStatusDate { get; set; }

		[Required]
		public DateTime BirthDate { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var results = new List<ValidationResult>();

			if (this.CivilStatusId != 1 && this.CivilStatusDate == null)
			{
				results.Add(new ValidationResult("Civil status date is mandatory.", new string[] { nameof(this.CivilStatusId) }));
			}

			return results;
		}
	}
}