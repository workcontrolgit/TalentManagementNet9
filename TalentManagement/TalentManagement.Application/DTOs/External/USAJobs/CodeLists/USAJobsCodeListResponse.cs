using System.Text.Json.Serialization;

namespace TalentManagement.Application.DTOs.External.USAJobs.CodeLists
{
    /// <summary>
    /// Base response structure for all USAJobs code list endpoints
    /// </summary>
    /// <typeparam name="T">The specific code list item type</typeparam>
    public class USAJobsCodeListResponse<T> where T : BaseCodeListItem
    {
        [JsonPropertyName("CodeList")]
        public List<USAJobsCodeListItem<T>>? CodeList { get; set; }
        
        [JsonPropertyName("DateGenerated")]
        public DateTime? DateGenerated { get; set; }
    }

    /// <summary>
    /// Individual code list item wrapper
    /// </summary>
    /// <typeparam name="T">The specific code list data type</typeparam>
    public class USAJobsCodeListItem<T> where T : BaseCodeListItem
    {
        [JsonPropertyName("ValidValue")]
        public List<T>? ValidValue { get; set; }
        
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    /// <summary>
    /// Base structure for all code list items
    /// </summary>
    public abstract class BaseCodeListItem
    {
        [JsonPropertyName("Code")]
        public string? Code { get; set; }

        [JsonPropertyName("Value")]
        public string? Value { get; set; }

        [JsonPropertyName("LastModified")]
        public DateTime? LastModified { get; set; }

        [JsonPropertyName("IsDisabled")]
        public string? IsDisabled { get; set; }

        /// <summary>
        /// Helper property to check if the item is disabled
        /// </summary>
        public bool IsActive => !string.Equals(IsDisabled, "Yes", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Occupational Series code list item
    /// </summary>
    public class OccupationalSeriesItem : BaseCodeListItem
    {
        [JsonPropertyName("JobFamily")]
        public string? JobFamily { get; set; }
    }

    /// <summary>
    /// Pay Plan code list item
    /// </summary>
    public class PayPlanItem : BaseCodeListItem
    {
        // Pay plans typically just use the base properties
    }

    /// <summary>
    /// Hiring Path code list item
    /// </summary>
    public class HiringPathItem : BaseCodeListItem
    {
        // Hiring paths typically just use the base properties
    }

    /// <summary>
    /// Position Schedule Type code list item
    /// </summary>
    public class PositionScheduleTypeItem : BaseCodeListItem
    {
        // Position schedule types typically just use the base properties
    }

    /// <summary>
    /// Work Schedule code list item
    /// </summary>
    public class WorkScheduleItem : BaseCodeListItem
    {
        // Work schedules typically just use the base properties
    }

    /// <summary>
    /// Security Clearance code list item
    /// </summary>
    public class SecurityClearanceItem : BaseCodeListItem
    {
        // Security clearances typically just use the base properties
    }

    /// <summary>
    /// Country code list item
    /// </summary>
    public class CountryItem : BaseCodeListItem
    {
        // Countries typically just use the base properties
    }

    /// <summary>
    /// Postal Code (State) code list item
    /// </summary>
    public class PostalCodeItem : BaseCodeListItem
    {
        // Postal codes typically just use the base properties
    }

    /// <summary>
    /// Geographic Location code list item
    /// </summary>
    public class GeoLocationItem : BaseCodeListItem
    {
        // Geographic locations typically just use the base properties
    }

    /// <summary>
    /// Travel Requirement code list item
    /// </summary>
    public class TravelRequirementItem : BaseCodeListItem
    {
        // Travel requirements typically just use the base properties
    }

    /// <summary>
    /// Remote Work code list item
    /// </summary>
    public class RemoteWorkItem : BaseCodeListItem
    {
        // Remote work options typically just use the base properties
    }
}