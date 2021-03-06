﻿namespace Simple_Scaler_2.Processing
{
    public sealed class GenericResult<TType> : Result
    {
        public GenericResult(TType result, bool success) : base(success)
        {
            Result = result;
        }

        public TType Result { get; }
    }
}