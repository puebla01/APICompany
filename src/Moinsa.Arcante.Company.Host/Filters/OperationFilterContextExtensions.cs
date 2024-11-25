using Microsoft.AspNetCore.Mvc.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Moinsa.Arcante.Company.Host.Filters
{
    public static class OperationFilterContextExtensions
    {
        public static CustomAttribute RequieredAttribute<T> (this OperationFilterContext context)
            where T : ICustomAttribute
        {

            IEnumerable<IFilterMetadata> globalAttributes = context //OperationFilterContext
                .ApiDescription//ApiDescripor
                .ActionDescriptor//ActionDescriptor
                .FilterDescriptors//IList<FilterDescription
                .Select(p=> p.Filter);

            object[] controllerAttributes = context
                .MethodInfo?
                .DeclaringType?
                .GetCustomAttributes(true) ?? Array.Empty<object>();
            object[] methodAttributes = context
                    .MethodInfo?
                    .GetCustomAttributes(true) ?? Array.Empty<object>();

            List<T> constainsAttribute = globalAttributes
                .Union(controllerAttributes)
                .Union(methodAttributes)
                .OfType<T>()
                .ToList();

            return constainsAttribute.Count == 0
                ? new CustomAttribute(false, false)
                : new CustomAttribute(true, constainsAttribute.First().IsMandatory);
        }
    }
}
