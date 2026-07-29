using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class UserError
    {
        public static Errror UserNotFound(string email)
            => new ("UserNotFound",$"User With Email '{email}' Not Found");
        public static Errror UserNotFound(int id)
            => new ("UserNotFound",$"User With Id '{id}' Not Found");
        public static Errror InvalidCredential()
            => new ("InvalidCredential", $"Wrong Password or Email");
        public static Errror FailedToSendEmail()
            => new ("FailedToSendEmail", $"Failed to send email");
        public static Errror ProfileUpdateFailed(List<string> errors)
            => new ("ProfileUpdateFailed", $"Profile update failed: {string.Join(", ", errors)}");

    }
}
