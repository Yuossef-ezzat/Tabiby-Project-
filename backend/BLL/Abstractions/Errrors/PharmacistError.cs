using BLL.Abstractions.Errors;

namespace BLL.Abstractions.Errrors
{
    public static class PharmacistError
    {
        public static Errror NotFound(int Id)
            =>new ("NotFound", $"Pharmacist with id {Id} Not Found");
        public static Errror UnAuthorizedAccess()
            => new("UnAuthorizedAccess", $"You Don't Authorize");
    }
}
