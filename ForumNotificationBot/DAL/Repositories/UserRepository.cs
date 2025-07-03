using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;      
using ForumNotificationBot.DAL.Data;

namespace ForumNotificationBot.DAL.Repositories
{
    
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;

        public async Task<bool> ExistsByTelegramIdAsync(long telegramId)
        {
            return await _db.Users.AnyAsync(u => u.TelegramId == telegramId);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _db.Users.AnyAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
    }
}
