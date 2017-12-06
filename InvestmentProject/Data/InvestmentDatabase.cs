using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InvestmentProject.Models;
using NLog;

namespace InvestmentProject.Data
{
    public class InvestmentDatabase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Initialize(InvestmentContext context)
        {
            Logger.Info("Deleting and recreating database...");
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            StoreCapitalData(context);
            StorePAndLData(context);
            StoreStrategyRegionData(context);
        }

        private static void StoreCapitalData(InvestmentContext context)
        {
            if (context.Capitals.Any()) return;

            var fileDir = Directory.GetCurrentDirectory() + "/csv/capital.csv";
            Logger.Info("reading csv file: " + fileDir);

            using (var sr = new StreamReader(fileDir))
            {
                var strategies = sr.ReadLine().Split(",").Skip(1).ToList();

                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    IEnumerable<string> columns = currentLine.Split(",");
                    var date = columns.First();

                    var capitalValuesList = columns.Skip(1).ToList();
                    for (var i = 0; i < capitalValuesList.Count(); i++)
                    {
                        context.Capitals.Add(new Capital
                        {
                            Date = DateTime.Parse(date),
                            Strategy = strategies[i],
                            Value = Convert.ToInt64(capitalValuesList[i])
                        });
                    }
                }

                context.SaveChanges();
            }

            Logger.Info("CAPITALS STORED: " + context.Capitals.Count());
        }

        private static void StorePAndLData(InvestmentContext context)
        {
            if (context.PAndL.Any()) return; // DB has been seeded

            var fileDir = Directory.GetCurrentDirectory() + "/csv/pnl.csv";
            Logger.Info("reading csv file: " + fileDir);

            using (var sr = new StreamReader(fileDir))
            {
                var profitsAndLosses = sr.ReadLine().Split(",").Skip(1).ToList();

                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    IEnumerable<string> columns = currentLine.Split(",");
                    var date = columns.First();

                    var pAndLValuesList = columns.Skip(1).ToList();
                    for (var i = 0; i < pAndLValuesList.Count(); i++)
                    {
                        context.PAndL.Add(new PAndL
                        {
                            Date = DateTime.Parse(date),
                            Strategy = profitsAndLosses[i],
                            Value = Convert.ToInt64(pAndLValuesList[i])
                        });
                    }
                }

                context.SaveChanges();
            }

            Logger.Info("PROFITS AND LOSSES STORED: " + context.PAndL.Count());
        }

        private static void StoreStrategyRegionData(InvestmentContext context)
        {
            if (context.StrategyRegion.Any()) return; // DB has been seeded

            var fileDir = Directory.GetCurrentDirectory() + "/csv/properties.csv";
            Logger.Info("reading csv file: " + fileDir);

            using (var sr = new StreamReader(fileDir))
            {
                sr.ReadLine(); //pop header line

                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    var data = currentLine.Split(",");
                    var strategy = data[0];
                    var region = data[1];

                    context.StrategyRegion.Add(new StrategyRegion
                    {
                        Region = region,
                        Strategy = strategy
                    });
                }

                context.SaveChanges();
            }

            Logger.Info("STRATEGIES AND REGIONS STORED: " + context.StrategyRegion.Count());
        }
    }
}