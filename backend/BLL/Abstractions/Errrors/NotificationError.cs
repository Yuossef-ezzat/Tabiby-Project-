using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class NotificationError
    {
        public static Errror NotificationNotFound(int notificationId) 
            => new  ("NotFound",$"Notification with ID {notificationId} not found.");
    }
}
