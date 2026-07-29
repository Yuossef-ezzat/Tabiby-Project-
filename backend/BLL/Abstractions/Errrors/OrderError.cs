using BLL.Abstractions.Errors;
using DAL.Models.Users;

namespace BLL.Abstractions.Errrors
{
    public static class OrderError
    {
        public static Errror NotFound(int OrderId)
            => new ("OrderNotFound", $"Order '{OrderId}' was not found.");
        public static Errror OrderCantBeDeleted(int OrderId)
            => new ("OrderCantBeDeleted", $"Order '{OrderId}' cannot be Deleted ");
        public static Errror NotCancelable(int OrderId)
            => new ("NotCancelable", $"Order '{OrderId}' cannot be canceled because it is no longer in Pending status.");
    }
}
