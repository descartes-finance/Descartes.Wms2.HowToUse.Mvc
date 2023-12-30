using System.ComponentModel.DataAnnotations;

namespace Descartes.Wms2.HowToUse.Mvc.Models
{
	public class RiskProfileViewModel : IValidatableObject
	{
		[Required]
		public long? BusinessLineId { get; set; }

		[Required]
		public long? InvestmentCategoryId { get; set; }

		[Required]
		public ICollection<string> Responses { get; set; }		

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			var result = new List<ValidationResult>();

			if (this.Responses == null || this.Responses.Any(string.IsNullOrWhiteSpace))
			{
				result.Add(new ValidationResult("Please provide all responses"));
			}

			return result;
		}
	}
}
