// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Microsoft.IIS.Administration
{
    using AspNetCore.Mvc.Formatters;
    using System;
    using Newtonsoft.Json;
    using System.Buffers;
    using Microsoft.AspNetCore.Mvc;

    class JsonOutputFormatter : NewtonsoftJsonOutputFormatter
    {
        public JsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, MvcOptions mvcOptions) : base(serializerSettings, charPool, mvcOptions)
        {
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return context.ContentType.StartsWith("application/", StringComparison.OrdinalIgnoreCase)
                && context.ContentType.ToString().IndexOf("json", StringComparison.OrdinalIgnoreCase) != -1;
        }
    }
}
