using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForumNotificationBot.BLL.Models
{
    [Table("NotificationMessages")]
    public class NotificationEntity
    {
        [Key]
        public int Id { get; set; }
        public int User { get; set; }
        [Required]
        public string NotificationType { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        [Required]
        public string RecipientEmail { get; set; }
        [Required]
        public string RecipientTelegramId { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
