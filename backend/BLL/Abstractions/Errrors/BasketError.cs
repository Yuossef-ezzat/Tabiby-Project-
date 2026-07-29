using BLL.Abstractions.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Errrors
{
    public static class BasketError
    {
        public static Errror NotFound(int Id)
           => new("NotFound",$"Basket for patient with Id {Id} Not found");
        public static Errror InsufficientStock(string medicationName,int availableStock)
           => new("InsufficientStock", $"'{medicationName}' has insufficient stock. Available quantity: {availableStock}.");
        public static Errror BasketAlreadyCheckedOut(int basketId)
           => new("BasketAlreadyCheckedOut", $"Basket '{basketId}' is already checked out. An order was already created from it.");
    }
}
