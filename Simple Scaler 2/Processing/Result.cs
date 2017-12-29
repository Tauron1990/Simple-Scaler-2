using JetBrains.Annotations;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public abstract class Result
    {
        protected Result(bool success) => Success = success;

        public bool Success { get; }

        public static TType Get<TType>(Result result) => result is GenericResult<TType> genericResult ? genericResult.Result : default(TType);

        public static GenericResult<TType> Create<TType>(TType obj, bool success) => new GenericResult<TType>(obj, success);
    }
}