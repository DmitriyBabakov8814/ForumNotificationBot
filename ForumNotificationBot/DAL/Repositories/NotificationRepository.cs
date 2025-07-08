using ForumNotificationBot.BLL.Models;
using ForumNotificationBot.DAL.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumNotificationBot.DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _db;
        public NotificationRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(NotificationEntity notification)
        {
            _db.NotificationMessages.Add(notification);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsByTelegramIdAsync(string telegramId)
        {
            return await _db.NotificationMessages
                            .AnyAsync(n => n.RecipientTelegramId == telegramId);
        }

        public async Task<List<NotificationEntity>> GetByTelegramIdAsync(string telegramId)
        {
            return await _db.NotificationMessages
                            .Where(n => n.RecipientTelegramId == telegramId)
                            .ToListAsync();
        }
    }
}
