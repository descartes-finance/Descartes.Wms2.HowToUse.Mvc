using Descartes.Wms2.HowToUse.Mvc.Shared.DTOs.Errors.Base;

namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs.Errors
{
    public class ValidationResultEntryModel
	{
		public string ValidationErrorCode { get; set; } = "INVALID-FIELD";
		public string ValidationErrorMessage { get; set; }
	}

	public class ValidationErrorModel : ErrorModel
	{
		public IDictionary<string, IList<ValidationResultEntryModel>> ValidationErrors { get; set; }
	}
}
