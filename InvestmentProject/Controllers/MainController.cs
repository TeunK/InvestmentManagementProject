using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestmentProject.Data;
using InvestmentProject.Models;
using InvestmentProject.Models.RequestModels;
using InvestmentProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace InvestmentProject.Controllers
{
    [Route("api/")]
    public class MainController : ControllerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICapitalService _capitalService;

        public MainController(ICapitalService capitalService)
        {
            _capitalService = capitalService;
        }
        
        [HttpGet("monthly-capital/")]
        [Produces("application/json")]
        public IActionResult GetCapitalsForStrategies([FromQuery] StrategiesRequestModel strategiesModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultingCapitals = _capitalService.GetCapitalsByStrategy(strategiesModel)
                .Select(capital => new
                {
                    strategy = capital.Strategy,
                    date = capital.Date.ToString("yyyy-MM-dd"),
                    capital = capital.Value
                });

            return Ok(resultingCapitals);
        }
        
        [HttpGet("cumulative-pnl/")]
        [Produces("application/json")]
        public IActionResult GetCumulativePAndL([FromQuery] PAndLRequestModel pAndLModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var resultingCumulatedPandLs = _capitalService.GetCumulativePAndLByDate(pAndLModel)
                .Select(cumulatedPandL => new
                {
                    region = cumulatedPandL.Region,
                    date = cumulatedPandL.Date.ToString("yyyy-MM-dd"),
                    cumulativePnl = cumulatedPandL.Value
                });

            return Ok(resultingCumulatedPandLs);
        }
        
        [HttpGet("compound-daily-returns/{strategy}/")]
        [Produces("application/json")]
        public IActionResult GetCompoundDailyReturns(string strategy)
        {
            if (string.IsNullOrWhiteSpace(strategy))
            {
                Logger.Error("Invalid request: Strategy was not provided");
                return BadRequest("Strategy was not provided");
            }

            var resultingCompoundDailyReturns = _capitalService.GetCompoundDailyReturnsForStrategy(strategy)
                .Select(compoundDailyReturns => new
                {
                    strategy = compoundDailyReturns.Strategy,
                    date = compoundDailyReturns.Date.ToString("yyyy-MM-dd"),
                    compoundReturn = compoundDailyReturns.CompoundReturn
                });

            return Ok(resultingCompoundDailyReturns);
        }
    }
}
