using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ValidationErrorToReturn
    {
        public int StutsCode { get; set; } = (int)HttpStatusCode.BadRequest;
        public string msg { get; set; } = "One or more validation errors occurred.";
        public IEnumerable<ValidationErrors> Errors { get; set; } = null!;

    }
}
