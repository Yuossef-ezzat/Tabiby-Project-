using DAL.Models.Users;
using DAL.Shared;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Notification : BaseEntity
    {
        public int UserId { get; set; }
        public string Message { get; set; }

        public NotificationType Type { get; set; }

        public bool IsRead { get; set; } = false;


        
        public virtual ApplicationUser User { get; set; }
    }
}
