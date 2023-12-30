namespace Descartes.Wms2.HowToUse.Mvc.Shared.DTOs
{
    public class LegalAcceptanceSectionOutputModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
    }

    public class LegalAcceptanceOutputModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }

        public IList<LegalAcceptanceSectionOutputModel> Sections { get; set; }
    }
}