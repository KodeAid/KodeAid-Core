// Copyright © Kris Penner. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using KodeAid.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace KodeAid.AspNetCore.SwaggerGen
{
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = new[] {
            new Tag { Name = "Products", Description = "Browse/manage the product catalog" },
            new Tag { Name = "Orders", Description = "Submit orders" }
        };
        }
    }

    public class SwaggerVersionedPropertyFilter : ISchemaFilter
    {
        public SwaggerVersionedPropertyFilter()
        {

        }

        public void Apply(Schema schema, SchemaFilterContext context)
        {
            if (schema?.Properties == null)
            {
                return;
            }

            var versionedProperties = context.SystemType.GetProperties()
                .Where(t => t.GetCustomAttribute<VersionedPropertyAttribute>() != null);

            foreach (var versionedProperty in versionedProperties)
            {
                if (schema.Properties.ContainsKey(versionedProperty.Name))
                {
                    schema.Properties.Remove(versionedProperty.Name);
                }
            }
        }
    }
}
