using System;

namespace TalentManagement.Domain.Entities
{
    public partial class EmailMsgRef
    {
        public decimal EmrSeqNum { get; set; }
        public string EmrType { get; set; }
        public string EmrMsgText { get; set; }
        public string EmrModule { get; set; }
        public string EmrActiveFlag { get; set; }
        public string EmrSubject { get; set; }
        public decimal? EmrCreateId { get; set; }
        public DateTime EmrCreateDate { get; set; }
        public decimal? EmrUpdateId { get; set; }
        public DateTime? EmrUpdateDate { get; set; }
    }
}
