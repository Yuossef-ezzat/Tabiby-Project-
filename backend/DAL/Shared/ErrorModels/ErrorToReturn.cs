using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ErrorToReturn
    {
        public int StutsCode { get; set; }
        public string ErrorMsg { get; set; } = null!;
        public List<string>? errors { get; set; }
    }
}
