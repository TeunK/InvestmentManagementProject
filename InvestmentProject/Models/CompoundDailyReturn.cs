using System;

namespace InvestmentProject.Models
{
    public class CompoundDailyReturn
    {
        public string Strategy { get; set; }
        public DateTime Date { get; set; }
        public decimal CompoundReturn { get; set; }

        protected bool Equals(CompoundDailyReturn other)
        {
            return string.Equals(Strategy, other.Strategy) && Date.Equals(other.Date) && CompoundReturn == other.CompoundReturn;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CompoundDailyReturn) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Strategy != null ? Strategy.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ CompoundReturn.GetHashCode();
                return hashCode;
            }
        }
    }
}
