﻿using System.Collections.Generic;
using Aidan.Common.Core.Interfaces.Contract;
using Aidan.Common.Utils.Utils;
using Aidan.Common.Utils.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using NSubstitute;

namespace Aidan.Common.Utils.Test
{
    public abstract class AspActionFilterGivenWhenThen<TFilter> : GivenWhenThen<TFilter> where TFilter : IActionFilter
    {
        private HttpContext _mockHttpContext;
        private HttpRequest _mockHttpRequest;
        protected ActionExecutingContext MockActionExecutingContext;
        protected MvcAdapter MvcAdapter;
        protected ISerializer Serializer;

        protected virtual Dictionary<string, StringValues> SetupHeaders( )
        {
            return new Dictionary<string, StringValues>( );
        }

        protected virtual Dictionary<string, StringValues> SetupQueryParams( )
        {
            return new Dictionary<string, StringValues>( );
        }

        protected virtual Dictionary<string, object> SetupActionArguments( )
        {
            return new Dictionary<string, object>( );
        }

        protected TType GetResultObject<TType>( ) where TType : class
        {
            var objectResult = MockActionExecutingContext.Result as ContentResult;
            return Serializer.Deserialize<TType>( objectResult?.Content );
        }

        protected override void Given( )
        {
            Serializer = new JsonSnakeCaseSerialzier( new JsonSnakeCaseResolver( new JsonSnakeCaseFieldNameParser( ) ) );
            _mockHttpContext = Substitute.For<HttpContext>( );
            _mockHttpRequest = Substitute.For<HttpRequest>( );
            MvcAdapter = new MvcAdapter( Serializer );
        }

        protected override void When( )
        {
            _mockHttpRequest
                .Headers
                .Returns( new HeaderDictionary( SetupHeaders( ) ) );
            _mockHttpRequest
                .Query
                .Returns( new QueryCollection( SetupQueryParams( ) ) );
            _mockHttpContext
                .Request
                .Returns( _mockHttpRequest );
            MockActionExecutingContext = new ActionExecutingContext( new ActionContext( _mockHttpContext,
                    new RouteData( ), new ActionDescriptor( ) ), new List<IFilterMetadata>( ),
                SetupActionArguments( ), new object( ) );
        }
    }
}