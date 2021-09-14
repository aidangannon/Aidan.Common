﻿namespace Aidan.Common.Core.Interfaces.Contract
{
    public interface ILoggerAdapter<T> where T : class
    {
        void LogInfo( string message );

        void LogError( string message );
    }
}