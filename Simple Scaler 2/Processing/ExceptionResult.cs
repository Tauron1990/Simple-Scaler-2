using System;

namespace Simple_Scaler_2.Processing
{
    public sealed class ExceptionResult : Result
    {
        public ExceptionResult(Exception exception) : base(false) => Exception = exception;

        public Exception Exception { get; }
    }
}