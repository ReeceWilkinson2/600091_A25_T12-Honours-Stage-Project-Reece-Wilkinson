using Honours_Project_CompetencyandSkillTracking.Canvas.Classes;
using Honours_Project_CompetencyandSkillTracking.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Honours_Project_CompetencyandSkillTracking.Components.Services
{
    public class UserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            return await _db.Students
                .FirstOrDefaultAsync(u =>
                    u.StEmail == email &&
                    u.Password == password);
        }

        public async Task<User> AddUserAsync(User user)
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

        public async Task<User> UpdateUserAsync(User user)
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

        public async Task DeleteUserAsync(User user)
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Students.FirstOrDefaultAsync(u => u.StEmail == email);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _db.Students
                .Include(s => s.Submissions)
                    .ThenInclude(sub => sub.Assignment)
                .Include(s => s.Submissions)
                    .ThenInclude(sub => sub.Comments)
                .Include(s => s.Courses)
                    .ThenInclude(c => c.Assignments)
                .FirstOrDefaultAsync(s => s.StudentId == userId);
        }

        public async Task<List<ModuleData>> GetModuleDataAsync(string courseTitle, string level)
        {
            return await _db.ModulesCSV
                .Where(m => m.Title == courseTitle && m.Level == level)
                .ToListAsync();
        }

        public async Task<List<User>> GetStudentDataAsync(string role)
        {
            return await _db.Students
                .Where(m => m.Role == role)
                .ToListAsync();
        }
    }
}