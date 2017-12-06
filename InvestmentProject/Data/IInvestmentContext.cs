using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvestmentProject.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentProject.Data
{
    public interface IInvestmentContext
    {
        DbSet<Capital> Capitals { get; }
        DbSet<PAndL> PAndL { get; }
        DbSet<StrategyRegion> StrategyRegion { get; }
    }

    public class InvestmentContext : DbContext, IInvestmentContext
    {
        public InvestmentContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Capital> Capitals { get; set; }
        public DbSet<PAndL> PAndL { get; set; }
        public DbSet<StrategyRegion> StrategyRegion { get; set; }
    }
}
