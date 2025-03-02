﻿using System;
using System.Runtime.Serialization;

namespace Aidan.Common.DependencyInjection.Exceptions
{
    [ Serializable ]
    internal class NoValidCtorFoundException : Exception
    {
        public NoValidCtorFoundException( ) { }

        public NoValidCtorFoundException( string message ) : base( message ) { }

        public NoValidCtorFoundException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected NoValidCtorFoundException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }
}