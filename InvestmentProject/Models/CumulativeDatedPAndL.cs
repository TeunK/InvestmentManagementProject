using System;

namespace InvestmentProject.Models
{
    public class CumulativeDatedPAndL
    {
        public string Region { get; set; }
        public DateTime Date { get; set; }
        public long Value { get; set; }

        protected bool Equals(CumulativeDatedPAndL other)
        {
            return string.Equals(Region, other.Region) && Date.Equals(other.Date) && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CumulativeDatedPAndL) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Region != null ? Region.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                return hashCode;
            }
        }
    }
}
