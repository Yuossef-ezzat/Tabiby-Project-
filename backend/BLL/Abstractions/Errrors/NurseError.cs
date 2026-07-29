using Azure.Core;
using BLL.Abstractions.Errors;
using DAL.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class NurseError
    {
        public static Errror NotFound
            (int nurseId) => new Errror( "NurseNotFound" , $"Nurse with ID {nurseId} was not found.");
        public static Errror RequestNotFound
            (int Id) => new Errror( "NurseRequestNotFound" , $"Nurse Request with ID {Id} was not found.");
        public static Errror InActive
            (int nurseId) => new Errror("NurseInactive", $"Nurse with ID {nurseId} is currently inactive.");
        public static Errror ActiveRequestExists
            () => new Errror("ActiveRequestExists", "You already have an active request with this nurse.");
        public static Errror UnauthorizedNursingAccess
            (int userId, int requestId) => new Errror("UnauthorizedNursingAccess", $"User '{userId}' is not authorized to modify nursing request '{requestId}'.");
        public static Errror InvalidStatus(string status)
            => new("Request.InvalidStatus", $"Request cannot be modified because its status is '{status}'.");
        public static Errror NotCancelable(int RequestId)
            => new("NotCancelable", $"Request '{RequestId}' cannot be canceled because it is no longer in Pending status.");
        public static Errror NotReviewable(int RequestId)
            => new("NotReviewable", $"Request '{RequestId}' cannot be reviewed because you must complete it first.");
        public static Errror ReviewExist(int RequestId)
            => new("ReviewExist", $"Review for request '{RequestId}' already exists.");
    }
}
