using DAL.Models;

namespace DAL.Specifications.NotificationSpecs
{
    public class UnreadNotificationsForUserSpecification : BaseSpecification<Notification>
    {
        public UnreadNotificationsForUserSpecification(int userId)
            : base(n => n.UserId == userId && !n.IsRead)
        {
        }
    }
}
