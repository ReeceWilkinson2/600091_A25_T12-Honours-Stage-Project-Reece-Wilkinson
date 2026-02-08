using Honours_Project_CompetencyandSkillTracking.Canvas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Data
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Student?> AuthenticateAsync(string email, string password)
        {
            return await _db.Students
                .FirstOrDefaultAsync(u =>
                    u.StEmail == email &&
                    u.Password == password);
        }

        public async Task<Student> AddUserAsync(Student user)
        {
            try
            {
                _db.Students.Add(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Student> UpdateUserAsync(Student user)
        {
            try
            {
                _db.Students.Update(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteUserAsync(Student user)
        {
            try
            {
                _db.Students.Remove(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Student?> GetUserByEmailAsync(string email)
        {
            return await _db.Students.FirstOrDefaultAsync(u => u.StEmail == email);
        }

        public async Task<Student?> GetUserByIdAsync(string userId)
        {
            return await _db.Students.FindAsync(userId);
        }
    }
}