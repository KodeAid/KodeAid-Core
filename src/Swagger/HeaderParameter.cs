// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Microsoft.OpenApi.Models;

namespace KodeAid.AspNetCore.Swagger
{
    public class HeaderParameter : OpenApiParameter
    {
        public HeaderParameter()
        {
            In = ParameterLocation.Header;
            Schema = new OpenApiSchema()
            {
                Type = "string",
            };
            Required = false;
        }
    }
}
