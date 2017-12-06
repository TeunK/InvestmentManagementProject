using System;

namespace InvestmentProject.Models
{
    public class Capital
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Strategy { get; set; }
        public long Value { get; set; }

        protected bool Equals(Capital other)
        {
            return Id == other.Id && Date.Equals(other.Date) && string.Equals(Strategy, other.Strategy) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Capital) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (Strategy != null ? Strategy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }
    }
}
