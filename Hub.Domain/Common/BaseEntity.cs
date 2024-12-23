using Hub.Domain.Common.Interfaces;

namespace Hub.Domain.Common
{
    [Serializable]
    public abstract class BaseEntity : IBaseEntity
    {
        public abstract long Id { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity);
        }

        private static bool IsTransient(IBaseEntity obj)
        {
            return obj != null && Equals(obj.Id, default(long));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(IBaseEntity other)
        {
            if (other == null)
                return false;

            if (!(other is BaseEntity))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = ((BaseEntity)other).GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(long)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(BaseEntity x, BaseEntity y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity x, BaseEntity y)
        {
            return !(x == y);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}
