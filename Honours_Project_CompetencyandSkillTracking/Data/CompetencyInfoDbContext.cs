using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class CompetencyInfoDbContext : DbContext
    {
        #region Constructor
        public CompetencyInfoDbContext(DbContextOptions<CompetencyInfoDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        #endregion

        #region Public properties
        public DbSet<CompetencyData> CompetencyData { get; set; }
        #endregion

        #region Overriden methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompetencyData>().HasData(GetCompetencyData());
            base.OnModelCreating(modelBuilder);
        }
        #endregion


        #region Private methods
        private List<CompetencyData> GetCompetencyData()
        {
            return new List<CompetencyData>
            {
            };

        }
        #endregion
    }
}
