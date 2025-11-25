using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class UserDataServices
    {
        #region Private members
        private UserInfoDbContext dbContext;
        #endregion

        #region Constructor
        public UserDataServices(UserInfoDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        #endregion

        #region Public methods
        public async Task<List<UserData>> GetUserDataAsync()
        {
            return await dbContext.UserData.ToListAsync();
        }

        public async Task<UserData> AddUserDataAsync(UserData userData)
        {
            try
            {
                dbContext.UserData.Add(userData);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return userData;
        }

        public async Task<UserData> UpdateUserDataAsync(UserData userData)
        {
            try
            {
                var userDataExist = dbContext.UserData.FirstOrDefault(p => p.UserId == userData.UserId);
                if (userDataExist != null)
                {
                    dbContext.Update(userData);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return userData;
        }

        public async Task DeleteUserDataAsync(UserData userData)
        {
            try
            {
                dbContext.UserData.Remove(userData);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion
    }
}