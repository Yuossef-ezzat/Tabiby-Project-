using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class DoctorError
    {
        public static Errror NotFound(int id) => new ("Doctor not found", $"Doctor with ID {id} does not exist.");
        public static Errror InActive(int id) => new ("Doctor inactive", $"Doctor with ID {id} is currently inactive.");
        public static Errror InvalidSpecialization(string specialization) => new ("Invalid specialization", $"The specialization '{specialization}' is not recognized.");
        public static Errror DoctorInActive(int id) => new ("Doctor InActive", $"The Doctor with id '{id}' is not Active.");

    }
}
