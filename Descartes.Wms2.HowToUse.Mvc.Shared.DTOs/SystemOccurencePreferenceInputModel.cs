namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
	public class SystemOccurencePreferenceInputModel
	{
		public long Id { get; set; }
		public bool IsEmailEnabled { get; set; }
		public bool IsSmsEnabled { get; set; }
		public bool IsHttpEnabled { get; set; }
	}
}
