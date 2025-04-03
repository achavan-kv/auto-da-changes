
namespace Blue.Cosacs.Shared
{
    public abstract class Entity
    {
// ReSharper disable UnusedAutoPropertyAccessor.Local
        public virtual int Id { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Local
        public virtual int Version { get; set; }

        //public virtual bool? IsDeleted { get; set; }

        //public virtual string DeletedBy { get; set; }
        //public virtual DateTime? DeletedOn { get; set; }

        //public virtual string CreatedBy { get; set; }
        //public virtual DateTime? CreatedOn { get; set; }

        //public virtual string LastUpdatedBy { get; set; }
        //public virtual DateTime? LastUpdatedOn { get; set; }
    }
}
