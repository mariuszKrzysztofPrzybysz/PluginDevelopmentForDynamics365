using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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
            if (entity.Contains("ita_priority"))
            {
                var serviceFactory = (IOrganizationServiceFactory)
                    serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var service = serviceFactory.CreateOrganizationService(context.UserId);

                var fetchXml =
                    @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                        < entity name = ""ita_trainingcompany"" >
                            < attribute name = ""ita_trainingcompanyid"" />
                            < attribute name = ""ita_name"" />
                            < attribute name = ""createdon"" />
                            < order attribute = ""ita_name"" descending = ""false"" />
                            < filter type = ""and"" >
                                < condition attribute = ""ita_company"" operator= ""eq"" value = ""{0}"" />
                            </ filter >
                        </ entity >
                    </ fetch > ";
                fetchXml = string.Format(fetchXml, entity.Id);
                var qe = new FetchExpression(fetchXml);
                var result = service.RetrieveMultiple(qe);
                foreach (var e in result.Entities)
                {
                    var updatedContact = new Entity(e.LogicalName);
                    updatedContact.Id = e.Id;
                    updatedContact["ita_priority"] = entity["ita_priority"];
                    service.Update(updatedContact);

                    /*
                    So why creating a new in-memory etity object?
                    The reason is that, if you call update on the entity retrieved from the query, there might be some other
                    attributes, and, even though you won’t be changing them, they will still be sent back to the server
                    */

                    service.Update(e);
                }
            }
        }
    }
}
