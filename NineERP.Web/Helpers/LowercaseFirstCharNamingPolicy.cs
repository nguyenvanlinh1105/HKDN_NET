﻿using System.Text.Json;

namespace NineERP.Web.Helpers
{
    public class LowercaseFirstCharNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}
