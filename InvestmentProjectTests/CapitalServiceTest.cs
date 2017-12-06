using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InvestmentProject.Data;
using InvestmentProject.Models.RequestModels;
using InvestmentProject.Models;
using InvestmentProject.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace InvestmentProjectTests
{
    [TestClass]
    public class CapitalServiceTest
    {
        private ICapitalService _capitalService;

        [TestInitialize]
        public void Init()
        {
            Mock<DbSet<Capital>> mockCapitalSet = GenerateMockCapitalSet();
            Mock<DbSet<PAndL>> mockPAndLSet = GenerateMockPAndLSet();
            Mock<DbSet<StrategyRegion>> mockStrategyRegionSet = GenerateMockStrategyRegionSet();

            var mockContext = new Mock<IInvestmentContext>();
            mockContext.Setup(c => c.Capitals).Returns(mockCapitalSet.Object);
            mockContext.Setup(c => c.PAndL).Returns(mockPAndLSet.Object);
            mockContext.Setup(c => c.StrategyRegion).Returns(mockStrategyRegionSet.Object);

            _capitalService = new CapitalService(mockContext.Object);
        }

        [TestMethod]
        public void CapitalServiceReturnsMonthlyCapitalsForNoStrategy()
        {
            List<Capital> capitalsForGivenStrategy = _capitalService.GetCapitalsByStrategy(new StrategiesRequestModel {Strategies = ""});

            List<Capital> expectedCapitalsForGivenStrategy = new List<Capital> {
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), Value = 1000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-01"), Value = 2000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-01"), Value = 4000},
                new Capital {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-01"), Value = 3000},
                new Capital {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-01"), Value = 5000}
            };

            CollectionAssert.AreEquivalent(expectedCapitalsForGivenStrategy, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CapitalServiceReturnsMonthlyCapitalsForStrategy()
        {
            List<Capital> capitalsForGivenStrategy = _capitalService.GetCapitalsByStrategy(new StrategiesRequestModel { Strategies = "Strategy1" });

            List<Capital> expectedCapitalsForGivenStrategy = new List<Capital> {
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), Value = 1000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-01"), Value = 2000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-01"), Value = 4000}
            };

            CollectionAssert.AreEquivalent(expectedCapitalsForGivenStrategy, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CapitalServiceReturnsMonthlyCapitalsForMultipleStrategies()
        {
            List<Capital> capitalsForGivenStrategy = _capitalService.GetCapitalsByStrategy(new StrategiesRequestModel { Strategies = "Strategy1,Strategy2" });

            List<Capital> expectedCapitalsForGivenStrategy = new List<Capital> {
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), Value = 1000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-01"), Value = 2000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-01"), Value = 4000},
                new Capital {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-01"), Value = 3000}
            };

            CollectionAssert.AreEquivalent(expectedCapitalsForGivenStrategy, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CumulativePAndLWithoutParameters()
        {
            List<CumulativeDatedPAndL> capitalsForGivenStrategy = _capitalService.GetCumulativePAndLByDate(new PAndLRequestModel { Region = "", StartDate = ""});

            List<CumulativeDatedPAndL> expectedCumulativePandL = new List<CumulativeDatedPAndL> {
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-01"), Value = 50},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-02"), Value = 115},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-03"), Value = 115},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-04"), Value = 215},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-07"), Value = 230},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-08"), Value = 240},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-02-02"), Value = 295},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-03-10"), Value = 375},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-01"), Value = 120},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-02"), Value = 130},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-04"), Value = 290},
            };
            
            CollectionAssert.AreEquivalent(expectedCumulativePandL, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CumulativePAndLWithStartDate()
        {
            List<CumulativeDatedPAndL> capitalsForGivenStrategy = _capitalService.GetCumulativePAndLByDate(new PAndLRequestModel { Region = "", StartDate = "2017-01-07" });

            List<CumulativeDatedPAndL> expectedCumulativePandL = new List<CumulativeDatedPAndL> {
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-07"), Value = 15},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-01-08"), Value = 25},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-02-02"), Value = 80},
                new CumulativeDatedPAndL {Region = "EU", Date = DateTime.Parse("2017-03-10"), Value = 160}
            };

            CollectionAssert.AreEquivalent(expectedCumulativePandL, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CumulativePAndLWithRegionFilter()
        {
            List<CumulativeDatedPAndL> capitalsForGivenStrategy = _capitalService.GetCumulativePAndLByDate(new PAndLRequestModel { Region = "US", StartDate = "" });

            List<CumulativeDatedPAndL> expectedCumulativePandL = new List<CumulativeDatedPAndL> {
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-01"), Value = 120},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-02"), Value = 130},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-04"), Value = 290}
            };

            CollectionAssert.AreEquivalent(expectedCumulativePandL, capitalsForGivenStrategy);
        }

        [TestMethod]
        public void CumulativePAndLWithRegionAndDateFilter()
        {
            List<CumulativeDatedPAndL> capitalsForGivenStrategy = _capitalService.GetCumulativePAndLByDate(new PAndLRequestModel { Region = "US", StartDate = "2017-01-02" });

            List<CumulativeDatedPAndL> expectedCumulativePandL = new List<CumulativeDatedPAndL> {
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-02"), Value = 10},
                new CumulativeDatedPAndL {Region = "US", Date = DateTime.Parse("2017-01-04"), Value = 170}
            };

            CollectionAssert.AreEquivalent(expectedCumulativePandL, capitalsForGivenStrategy);
        }


        [TestMethod]
        public void CompoundDailyReturnsForStrategy1()
        {
            List<CompoundDailyReturn> compoundDailyReturns = _capitalService.GetCompoundDailyReturnsForStrategy("Strategy1");

            List<CompoundDailyReturn> expectedCompoundDailyReturns = new List<CompoundDailyReturn> {
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), CompoundReturn = 0.05M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-02"), CompoundReturn = 0.11M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-03"), CompoundReturn = 0.09M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-04"), CompoundReturn = 0.19M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-07"), CompoundReturn = 0.215M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-08"), CompoundReturn = 0.225M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-02"), CompoundReturn = 0.25869M},
                new CompoundDailyReturn {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-10"), CompoundReturn = 0.28386M}
            };

            CollectionAssert.AreEquivalent(expectedCompoundDailyReturns, compoundDailyReturns);
        }


        private static Mock<DbSet<Capital>> GenerateMockCapitalSet()
        {
            var data = new List<Capital>
            {
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), Value = 1000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-01"), Value = 2000},
                new Capital {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-01"), Value = 4000},
                new Capital {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-01"), Value = 3000},
                new Capital {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-01"), Value = 5000}
            }.AsQueryable();

            var mockCapitalSet = new Mock<DbSet<Capital>>();
            mockCapitalSet.As<IQueryable<Capital>>().Setup(m => m.Provider).Returns(data.Provider);
            mockCapitalSet.As<IQueryable<Capital>>().Setup(m => m.Expression).Returns(data.Expression);
            mockCapitalSet.As<IQueryable<Capital>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockCapitalSet.As<IQueryable<Capital>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mockCapitalSet;
        }

        private static Mock<DbSet<PAndL>> GenerateMockPAndLSet()
        {
            var data = new List<PAndL>
            {
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-01"), Value = 50},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-02"), Value = 60},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-03"), Value = -20},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-04"), Value = 100},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-07"), Value = 25},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-01-08"), Value = 10},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-02-02"), Value = 55},
                new PAndL {Strategy = "Strategy1", Date = DateTime.Parse("2017-03-10"), Value = 80},
                new PAndL {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-01"), Value = 120},
                new PAndL {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-02"), Value = 10},
                new PAndL {Strategy = "Strategy2", Date = DateTime.Parse("2017-01-04"), Value = 160},
                new PAndL {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-01"), Value = 0},
                new PAndL {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-02"), Value = 5},
                new PAndL {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-03"), Value = 20},
                new PAndL {Strategy = "Strategy3", Date = DateTime.Parse("2017-01-07"), Value = -10}
            }.AsQueryable();

            var mockPAndLSet = new Mock<DbSet<PAndL>>();

            mockPAndLSet.As<IQueryable<PAndL>>().Setup(m => m.Provider).Returns(data.Provider);
            mockPAndLSet.As<IQueryable<PAndL>>().Setup(m => m.Expression).Returns(data.Expression);
            mockPAndLSet.As<IQueryable<PAndL>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockPAndLSet.As<IQueryable<PAndL>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mockPAndLSet;
        }

        private static Mock<DbSet<StrategyRegion>> GenerateMockStrategyRegionSet()
        {
            var data = new List<StrategyRegion>
            {
                new StrategyRegion {Region = "EU", Strategy = "Strategy1"},
                new StrategyRegion {Region = "US", Strategy = "Strategy2"},
                new StrategyRegion {Region = "EU", Strategy = "Strategy3"},
            }.AsQueryable();

            var mockStrategyRegionSet = new Mock<DbSet<StrategyRegion>>();

            mockStrategyRegionSet.As<IQueryable<StrategyRegion>>().Setup(m => m.Provider).Returns(data.Provider);
            mockStrategyRegionSet.As<IQueryable<StrategyRegion>>().Setup(m => m.Expression).Returns(data.Expression);
            mockStrategyRegionSet.As<IQueryable<StrategyRegion>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockStrategyRegionSet.As<IQueryable<StrategyRegion>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

            return mockStrategyRegionSet;
        }
    }
}
