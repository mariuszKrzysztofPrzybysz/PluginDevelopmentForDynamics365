using Microsoft.Xrm.Sdk;
using System;

namespace TrainingCompany.Plugin
{
    public class TrainingCompanyPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            var entity = (Entity)context.InputParameters["Target"];
            if(entity.Contains("ita_priority"))
            {
                throw new InvalidPluginExecutionException("test");
            }
        }
    }
}
