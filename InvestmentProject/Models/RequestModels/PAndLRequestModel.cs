using System.ComponentModel.DataAnnotations;

namespace InvestmentProject.Models.RequestModels
{
    public class PAndLRequestModel
    {
        public long Id { get; set; }

        [RegularExpression(@"(AP|EU|US)$", ErrorMessage = "Invalid region provided, must be either AP, EU or US")]
        public string Region { get; set; }
        [RegularExpression(@"\d{4}-\d{2}-\d{2}$", ErrorMessage = "Incorrect date format provided, must be of format yyyy-MM-dd")]
        public string StartDate { get; set; }
    }
}
