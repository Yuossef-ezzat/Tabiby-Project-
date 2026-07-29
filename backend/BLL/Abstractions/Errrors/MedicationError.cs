using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class MedicationError
    {
        public static Errror NotFound(int Id)
            => new ("NotFound",$"Medication with id {Id} Not found");
        public static Errror UnAuthorizedAccess()
            => new ("UnAuthorizedAccess", $"You Don't Authorize");

    }
}
