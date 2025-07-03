using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForumNotificationBot.DAL.Data
{
    public class User
    {
        public int Id { get; set; }             
        public int UserId { get; set; }          
        public long? TelegramId { get; set; }    
        public string Email { get; set; }        
    }
}
