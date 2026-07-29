using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Exceptions
{
    public abstract class NotFoundException(string msg) 
                                  : Exception(msg)
    {
    }
}
