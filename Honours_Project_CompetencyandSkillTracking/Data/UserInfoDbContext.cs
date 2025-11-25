using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class UserInfoDbContext : DbContext
    {
        #region Constructor
        public UserInfoDbContext(DbContextOptions<UserInfoDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        #endregion

        #region Public properties
        public DbSet<UserData> UserData { get; set; }
        #endregion

        #region Overriden methods
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserData>().HasData(GetUserData());
            base.OnModelCreating(modelBuilder);
        }
        #endregion


        #region Private methods
        private List<UserData> GetUserData()
        {
            return new List<UserData>
            {
            };

        }
        #endregion
    }
}
