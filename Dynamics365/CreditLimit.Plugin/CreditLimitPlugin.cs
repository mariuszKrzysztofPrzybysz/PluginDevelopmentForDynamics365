using System;
using Microsoft.Xrm.Sdk;

namespace CreditLimit.Plugin
{
    public class CreditLimitPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = null;

            var entity = (Entity)context.InputParameters["Target"];


        }
    }
}
