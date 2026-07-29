namespace BLL.Abstractions.Errors
{
    public sealed record Errror(string Code , string Message = "")
    {
        public static readonly Errror None = new Errror(string.Empty, string.Empty);

    }
}
