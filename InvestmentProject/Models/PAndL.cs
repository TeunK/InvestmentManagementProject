using System;

namespace InvestmentProject.Models
{
    public class PAndL
    {
        public long Id { get; set; }
        public long Value { get; set; }
        public DateTime Date { get; set; }
        public string Strategy { get; set; }
    }
}
