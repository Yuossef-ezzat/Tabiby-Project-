using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class AdminError
    {
        public static Errror NotFoundError(int id)
            =>new ("NotFound",$"user with id {id} Not Found") ;
        public static Errror AdminCantBeDeleted()
            => new ( "BadRequest", "Can't Delete Admins");
        public static Errror UserHasRelatedRecords ()
            => new ( "BadRequest", "This account cannot be deleted because it has linked records (appointments, schedules, etc.). Please deactivate the account instead. ");
        public static Errror AdminCantBeDisActive()
            => new ( "BadRequest", "Can't DisActive Admins");
        public static Errror UserAlreadyExists(string email)
            => new ( "Conflict", $"A user with the email '{email}' already exists.");
    }
}
