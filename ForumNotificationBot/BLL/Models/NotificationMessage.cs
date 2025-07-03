using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumNotificationBot.BLL.Models
{
    public class NotificationMessage
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RecipientEmail { get; set; }
        public long? RecipientTelegramId { get; set; }
        public int Priority { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
