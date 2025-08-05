using System;

namespace TalentManagement.Domain.Entities
{
    public partial class PdRequest
    {
        public decimal PdrqSeqNum { get; set; }
        public decimal PdrqRequesterHruId { get; set; }
        public decimal? PdrqHrspecialistHruId { get; set; }
        public string PdrqReqInfo { get; set; }
        public string PdrqReqCompletedFlag { get; set; }
        public decimal PdrqCreateId { get; set; }
        public DateTime PdrqCreateDate { get; set; }
        public decimal? PdrqUpdateId { get; set; }
        public DateTime? PdrqUpdateDate { get; set; }
        public string PdrqJobcode { get; set; }
        public string PdrqPosition { get; set; }
        public string PdrqType { get; set; }
        public DateTime? PdrqJobcodeEffdt { get; set; }
    }
}
