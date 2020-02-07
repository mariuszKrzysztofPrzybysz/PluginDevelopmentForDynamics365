using Microsoft.Xrm.Sdk;
using System;

namespace Training.Plugins
{
    public class ValidationPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            var entity = (Entity)context.InputParameters["Target"];
            var name = (string)entity["name"];
            if ("Test".Equals(name))
            {
                throw new InvalidPluginExecutionException("Cannot use this name");
            }
        }
    }
}
