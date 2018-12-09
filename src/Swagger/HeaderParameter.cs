// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using Swashbuckle.AspNetCore.Swagger;

namespace KodeAid.AspNetCore.Swagger
{
    public class HeaderParameter : NonBodyParameter
    {
        public HeaderParameter()
        {
            In = "header";
            Type = "string";
            Required = false;
        }
    }
}
