using ForumNotificationBot.BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForumNotificationBot.DAL.Repositories
{
    public interface INotificationRepository
    {
        Task AddAsync(NotificationEntity notification);
        Task<bool> ExistsByTelegramIdAsync(string telegramId);
        Task<List<NotificationEntity>> GetByTelegramIdAsync(string telegramId);
    }
}
