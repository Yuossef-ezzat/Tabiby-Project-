using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class ConsultationError
    {
        public static Errror ConsultationNotActive()
            => new ("Bad Request","Consultation is not active.");
        public static Errror ConsultationNotFound(int id)
            => new ("ConsultationNotFound", $"Consultation with Id {id} Not Found");
        public static Errror ConsultationReviewNotFound(int id)
            => new ("ConsultationReviewNotFound", $"Consultation Review with Id {id} Not Found");
        public static Errror UnauthorizedAccess()
            => new ("UnauthorizedAccess", $"You do not have access to this consultation.");
        public static Errror ConsultationNotCompleted()
            => new ("ConsultationNotCompleted", $"You Must have Complete the consultation before add review.");
        public static Errror ConsultationReviewAlreadyExists()
            => new ("ConsultationReviewAlreadyExists", $"You can't add more than one review.");
        public static Errror ConsultationAlreadyExists(int doctorId)
            => new ("ConsultationAlreadyExists", $"You have already request with doctor ID {doctorId} .");
        public static Errror InvalidStatus(string status)
            => new ("InvalidStatus", $"Invalid Status: {status}.");
    }
}
