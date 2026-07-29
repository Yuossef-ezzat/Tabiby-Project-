using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Specifications.NotificationSpecs
{
    public class NotificationsByUserIdSpecs : BaseSpecification<Notification>
    {
        public NotificationsByUserIdSpecs(int userId) : base(N => N.UserId == userId)
        {
            ApplyOrderByDescending(N => N.CreatedAt);
        }
    }
}
