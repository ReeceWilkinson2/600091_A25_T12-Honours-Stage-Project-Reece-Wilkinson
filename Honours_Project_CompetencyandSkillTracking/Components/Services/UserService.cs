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

        private static readonly Random _random = new Random();

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.StEmail == email);

            if (user == null)
                return null;

            // Check if password is hashed
            if (user.Password.StartsWith("$2"))
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                    return user;
            }
            else
            {
                if (user.Password == password)
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(password);
                    await _db.SaveChangesAsync();

                    return user;
                }
            }

            return null;
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                _db.Users.Add(user);
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
                _db.Users.Update(user);
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
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.StEmail == email);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _db.Users
               .Include(s => s.Submissions)
                   .ThenInclude(sub => sub.Assignment)
                       .ThenInclude(a => a.Course)
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
            return await _db.Users
                .Where(m => m.Role == role)
                .ToListAsync();
        }

        public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.StudentId == userId && u.Password == oldPassword);

            if (user == null)
            {
                return false;
            }

            user.Password = newPassword;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateUniqueStudentIdAsync()
        {
            string id;

            do
            {
                id = _random.Next(100000, 999999).ToString();
            }
            while (await _db.Users.AnyAsync(u => u.StudentId == id));

            return id;
        }
    }
}