﻿using Newtonsoft.Json;

namespace Aidan.Common.Utils.Utils
{
    public abstract class BaseJsonSerializer
    {
        protected JsonSerializerSettings Settings;

        protected BaseJsonSerializer()
        {
            Settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "dd-MM-yyyy"
            };
        }
    }
}