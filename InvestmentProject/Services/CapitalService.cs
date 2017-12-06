using System;
using System.Collections.Generic;
using System.Linq;
using InvestmentProject.Data;
using InvestmentProject.Models;
using InvestmentProject.Models.RequestModels;

namespace InvestmentProject.Services
{
    public interface ICapitalService
    {
        List<Capital> GetCapitalsByStrategy(StrategiesRequestModel strategiesModel);
        List<CumulativeDatedPAndL> GetCumulativePAndLByDate(PAndLRequestModel pAndLModel);
        List<CompoundDailyReturn> GetCompoundDailyReturnsForStrategy(string strategy);
    }

    public class CapitalService : ICapitalService
    {
        private readonly IInvestmentContext _context;

        public CapitalService(IInvestmentContext context)
        {
            _context = context;
        }

        public List<Capital> GetCapitalsByStrategy(StrategiesRequestModel strategiesModel)
        {
            if (string.IsNullOrWhiteSpace(strategiesModel.Strategies)) return _context.Capitals.ToList();

            IEnumerable<string> requestedStrategies = strategiesModel.Strategies.Split(",");
            return _context.Capitals.Where(capital => requestedStrategies.Contains(capital.Strategy))
                .OrderBy(capital => capital.Date)
                .ToList();
        }

        public List<CumulativeDatedPAndL> GetCumulativePAndLByDate(PAndLRequestModel pAndLModel)
        {
            var startDate = !string.IsNullOrWhiteSpace(pAndLModel.StartDate) ? (DateTime?) Convert.ToDateTime(pAndLModel.StartDate) : null;
            var queryRegions = (string.IsNullOrWhiteSpace(pAndLModel.Region))
                ? new List<string> {"AP", "EU", "US"}
                : new List<string> {pAndLModel.Region};

            var datedPAndLsForRegionAfterStartDateOrderedByDate =
                QueryDatedPAndLsForRegionAfterStartDateOrderedByDate(queryRegions, startDate);

            var cumulatedPandLs = new List<CumulativeDatedPAndL>();
            var previouslyRecordedCumulatedPAndLForRegion = new Dictionary<string, long>();
            foreach (var queryRegion in queryRegions) previouslyRecordedCumulatedPAndLForRegion.Add(queryRegion, 0);

            foreach (var datedPAndL in datedPAndLsForRegionAfterStartDateOrderedByDate)
            {
                previouslyRecordedCumulatedPAndLForRegion[datedPAndL.Region] += datedPAndL.Value;
                cumulatedPandLs.Add(new CumulativeDatedPAndL()
                {
                    Date = datedPAndL.Date,
                    Region = datedPAndL.Region,
                    Value = previouslyRecordedCumulatedPAndLForRegion[datedPAndL.Region]
                });
            }

            return cumulatedPandLs;
        }

        public List<CompoundDailyReturn> GetCompoundDailyReturnsForStrategy(string strategy)
        {
            var orderedPAndLs = _context.PAndL.Where(pAndL => pAndL.Strategy == strategy).OrderBy(pAndL => pAndL.Date).ToList();
            var orderedCapitals = _context.Capitals.Where(capital => capital.Strategy == strategy).OrderBy(capital => capital.Date).ToList();

            var dailyReturns = ComputeDailyReturns(orderedPAndLs, orderedCapitals);

            decimal currentCompoundDailyReturn = 1;
            foreach (var dailyReturn in dailyReturns)
            {
                currentCompoundDailyReturn *= (dailyReturn.CompoundReturn + 1);
                dailyReturn.CompoundReturn = currentCompoundDailyReturn - 1;
            }

            return dailyReturns.Select(unroundedCompoundDailyReturn => new CompoundDailyReturn
                {
                    Strategy = unroundedCompoundDailyReturn.Strategy,
                    Date = unroundedCompoundDailyReturn.Date,
                    CompoundReturn = Math.Round(unroundedCompoundDailyReturn.CompoundReturn, 5)
                }).ToList();
        }

        private static List<CompoundDailyReturn> ComputeDailyReturns(IReadOnlyList<PAndL> orderedPAndLs, IReadOnlyList<Capital> orderedCapitals)
        {
            var dailyReturns = new List<CompoundDailyReturn>();

            var pAndLIterator = 0;
            var capitalsIterator = -1;
            long? currentCapital = null;
            while (pAndLIterator < orderedPAndLs.Count)
            {
                var currentPandL = orderedPAndLs[pAndLIterator];

                if (currentCapital == null ||
                    (orderedCapitals.Count > capitalsIterator + 1 && orderedCapitals[capitalsIterator + 1].Date <= currentPandL.Date))
                {
                    /* New monthly capital rapport provided, reset currentCapital to this amount.
                       NOTE: If instead the assumption was that the monthly capitals are ADDED to the 
                       already-existing capital, turn the line below into addition (+=)
                       ie. currentCapital += orderedCapitals[capitalsIterator + 1].Value;
                    */
                    currentCapital = orderedCapitals[capitalsIterator + 1].Value;
                    capitalsIterator++;
                }

                dailyReturns.Add(new CompoundDailyReturn
                {
                    Date = currentPandL.Date,
                    Strategy = currentPandL.Strategy,
                    CompoundReturn = (currentCapital.Value) == 0 ? 0 : (decimal)currentPandL.Value / currentCapital.Value
                });

                currentCapital += currentPandL.Value;

                pAndLIterator++;
            }

            return dailyReturns;
        }

        private IEnumerable<DatedPAndL> QueryDatedPAndLsForRegionAfterStartDateOrderedByDate(ICollection<string> queryRegions, DateTime? startDate)
        {
            return _context.PAndL
                .Join(
                    _context.StrategyRegion,
                    pAndL => pAndL.Strategy,
                    strategyRegion => strategyRegion.Strategy,
                    (pAndL, strategyRegion) => new DatedPAndL
                    {
                        Region = strategyRegion.Region,
                        Date = pAndL.Date,
                        Value = pAndL.Value
                    })
                .Where(datedCumulativePAndL => queryRegions.Contains(datedCumulativePAndL.Region) &&
                                               (startDate == null || datedCumulativePAndL.Date >= startDate.Value))
                .GroupBy(datedCumulativePAndL => new { datedCumulativePAndL.Date, datedCumulativePAndL.Region })
                .Select(groupedDatedCumulativePAndL => new DatedPAndL
                {
                    Region = groupedDatedCumulativePAndL.First().Region,
                    Date = groupedDatedCumulativePAndL.First().Date,
                    Value = groupedDatedCumulativePAndL.Sum(datedCumulativePAndL => datedCumulativePAndL.Value)
                })
                .OrderBy(datedCumulativePAndL => datedCumulativePAndL.Date)
                .ToList();
        }
    }
}
