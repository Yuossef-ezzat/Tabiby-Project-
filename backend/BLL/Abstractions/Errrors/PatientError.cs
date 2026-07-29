using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class PatientError
    {
        public static Errror PatientNotFound(int patientId)
            => new Errror("PatientNotFound", $"Patient '{patientId}' was not found.");
    }
}
