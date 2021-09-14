﻿using System;
using System.Net.Http;

namespace Aidan.Common.Core.Interfaces.Contract
{
    public interface IHttpClient
    {
        void SetBaseUrl( Uri uri );
        void SetBearerToken( string token );
        HttpResponseMessage Patch( string uri, string body );
        HttpResponseMessage Get( string uri );
        HttpResponseMessage Post( string uri, string body );
    }
}