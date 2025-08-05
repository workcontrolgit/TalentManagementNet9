using System;

namespace TalentManagement.Domain.Entities
{
    public partial class Help
    {
        public decimal HelpSeqNum { get; set; }
        public string HelpPageNameTxt { get; set; }
        public string HelpAttributeNameTxt { get; set; }
        public string HelpPageNameDisplayTxt { get; set; }
        public string HelpAttribNameDisplayTxt { get; set; }
        public string HelpTooltipTxt { get; set; }
        public string HelpMessageTxt { get; set; }
        public string HelpActiveInd { get; set; }
        public string HelpCreateId { get; set; }
        public DateTime HelpCreateDate { get; set; }
        public string HelpUpdateId { get; set; }
        public DateTime HelpUpdateDate { get; set; }
    }
}
