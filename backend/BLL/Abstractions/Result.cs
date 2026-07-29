using BLL.Abstractions.Errors;


namespace BLL.Abstractions
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Errror Error { get; }

        protected Result(bool isSuccess, Errror error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
            => new(true, Errror.None);

        public static Result Failure(Errror error)
            => new(false, error);
    }
    public class Result<T> : Result
    {
        
        public T Value { get; set; }
        private Result(bool isSuccess, T value , Errror error) : base(isSuccess, error) 
        {
            Value = value;
        }
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, Errror.None);
        }
        public static Result<T> Failure(Errror error)
        {
            return new Result<T>(false, default(T)!, error);
        }
    }

}
