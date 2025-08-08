namespace TalentManagement.Domain.Common
{
    /// <summary>
    /// Base class for all entities that have an ID property.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Initializes a new instance of BaseEntity with a new GUID.
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}