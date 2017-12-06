using System.ComponentModel.DataAnnotations;

namespace InvestmentProject.Models.RequestModels
{
    public class StrategiesRequestModel
    {
        public long Id { get; set; }

        [MaxLength(10000, ErrorMessage = "Unusual length of strategy names provided, must be kept below 10000 characters")]
        [RegularExpression(@"^([,]?Strategy[0-9]{1,4})*$", ErrorMessage = "Provided strategies should be of format: StrategyX for X being maximum of 4 digits number. Chaining is optional when separated by a comma ',' eg: Strategy1,Strategy2")]
        public string Strategies { get; set; }
    }
}
