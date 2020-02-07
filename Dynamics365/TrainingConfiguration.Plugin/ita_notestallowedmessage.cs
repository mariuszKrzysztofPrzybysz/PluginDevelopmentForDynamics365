using Microsoft.Xrm.Sdk;
using System;

namespace TrainingConfiguration.Plugin
{
    public class ita_notestallowedmessage : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            var entity = (Entity)context.InputParameters["Target"];
            if (entity.Contains("ita_notestallowedmessage"))
            {
                if (entity["ita_notestallowedmessage"] is null)
                {
                    throw new InvalidPluginExecutionException("Cannot be null");
                }
            }
        }
    }
}
