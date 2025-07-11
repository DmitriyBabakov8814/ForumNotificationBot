﻿using System;

namespace ForumNotificationBot.BLL.Models
{
    public class NotificationMessage
    {
        public int User { get; set; }
        public string NotificationType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientTelegramId { get; set; }
        public int Priority { get; set; }
    }

}
