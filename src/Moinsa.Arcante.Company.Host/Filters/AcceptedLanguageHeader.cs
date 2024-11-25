using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Moinsa.Arcante.Company.Host.Filters
{
    [AttributeUsage(validOn:AttributeTargets.Class | AttributeTargets.Method)]
    public class AcceptedLanguageHeader : Attribute, IOperationFilter, ICustomAttribute
    {
        public static string HeaderName = "accept-language";
        public bool IsMandatory { get; }

        public AcceptedLanguageHeader (bool isMandatory = false)
        {
            IsMandatory = isMandatory;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            CustomAttribute attribute = context.RequieredAttribute<AcceptedLanguageHeader>();

            //Si el endpoint utiliza nuestro acceptedlanguageheader nos pinta el parametro y si no, no lo pinta.
            if (!attribute.ConstainAttribute)
                return;

            operation.Parameters.Add(item: new OpenApiParameter()
            {
                Name = HeaderName,
                In = ParameterLocation.Header,
                Required = attribute.Mandatory,
                Example= new Microsoft.OpenApi.Any.OpenApiString("es-ES"),
                Schema = new OpenApiSchema() { Type= "string" }
            });
        }

    }
}
