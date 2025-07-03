using ForumNotificationBot.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumNotificationBot.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByTelegramIdAsync(long telegramId);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddUserAsync(User user);
    }
}
