namespace TalentManagement.Domain.Entities
{
    public partial class UserGroup
    {
        public byte UgId { get; set; }
        public string UgDescrText { get; set; }
        public string UgActiveInd { get; set; }
        public string Email { get; set; }
    }
}
