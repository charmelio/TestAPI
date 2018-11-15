using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApplicationCoreTest1.Extensions
{
    public class SwaggerEnumExt : IDocumentFilter
    {
        public void Apply(SwaggerDocument doc, DocumentFilterContext context)
        {
            foreach (var schema in doc.Definitions)
            {
                foreach (var property in schema.Value.Properties)
                {
                    var enums = property.Value.Enum;
                    if (enums != null && enums.Count > 0)
                    {
                        property.Value.Description += DescribeEnum(enums);
                    }
                }
            }

            if (doc.Paths.Count <= 0) return;

            foreach (var path in doc.Paths.Values)
            {
                DescribeEnumParameters(path.Parameters);
                var possibleParameterisedOperations = new List<Operation> { path.Get, path.Post, path.Put };
                possibleParameterisedOperations.FindAll(p => p != null).ForEach(e => DescribeEnumParameters(e.Parameters));
            }
        }

        private string DescribeEnum(IEnumerable<object> enums)
        {
            var enumDescriptions = new List<string>();
            Type type = null;
            foreach (var enumOption in enums)
            {
                if (type == null)
                    type = enumOption.GetType();

                enumDescriptions.Add($"{Convert.ChangeType(enumOption, type.GetEnumUnderlyingType())} = {Enum.GetName(type, enumOption)}");
            }

            return $"{Environment.NewLine}{string.Join(Environment.NewLine, enumDescriptions)}";
        }

        private void DescribeEnumParameters(IList<IParameter> parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                if (param.Extensions.ContainsKey("enum")
                    && param.Extensions["enum"] is IList<object> paramEnums
                    && paramEnums.Count > 0)
                {
                    param.Description += DescribeEnum(paramEnums);
                }
            }
        }
    }
}