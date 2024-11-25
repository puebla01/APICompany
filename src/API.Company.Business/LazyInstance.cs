using Microsoft.Extensions.DependencyInjection;
using System;

namespace API.Company.Business
{
    public class LazyInstance<T> : Lazy<T>
    {
        public LazyInstance(IServiceProvider serviceProvider) : base(() => serviceProvider.GetRequiredService<T>())
        {

        }
    }
}
