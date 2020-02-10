using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace TrainingCompany.Plugin
{
    public class TrainingCompanyPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            var tracingService = (ITracingService)
                serviceProvider.GetService(typeof(ITracingService));

            var context = (IPluginExecutionContext)
            serviceProvider.GetService(typeof(IPluginExecutionContext));

            var entity = (Entity)context.InputParameters["Target"];
            if (entity.Contains("ita_priority"))
            {
                var serviceFactory = (IOrganizationServiceFactory)
                    serviceProvider.GetService(typeof(IOrganizationServiceFactory));

                var service = serviceFactory.CreateOrganizationService(context.UserId);

                int pageNumber = 1;
                string pagingCookie = string.Empty;
                EntityCollection result;
                do
                {
                    result = RetrieveContacts(tracingService, service, entity.Id, pageNumber, pagingCookie);
                    foreach (var e in result.Entities)
                    {
                        tracingService.Trace(e.Id.ToString());
                        var updatedContact = new Entity(e.LogicalName)
                        {
                            Id = e.Id
                        };
                        updatedContact["ita_priority"] = entity["ita_priority"];
                        service.Update(updatedContact);

                        /*
                        So why creating a new in-memory etity object?
                        The reason is that, if you call update on the entity retrieved from the query, there might be some other
                        attributes, and, even though you won’t be changing them, they will still be sent back to the server
                        */
                    }
                    pagingCookie = result.PagingCookie;
                    pageNumber++;
                }
                while (result.MoreRecords);
            }
        }

        private EntityCollection RetrieveContacts(ITracingService tracingService, IOrganizationService organizationService, Guid companyId, int page, string pagingCookie)
        {
            if (!string.IsNullOrWhiteSpace(pagingCookie))
            {
                pagingCookie = pagingCookie.Replace("\"", "'").Replace(">", "&gt;").Replace("<", "&lt;");
            }
            var fetchXml =
                @"<fetch version=""1.0""
                    count=""5""
                    page=""{1}""
                    paging-cookie=""{2}""
                    returntotalrecordcount=""true""
                    output-format=""xml-platform""
                    mapping=""logical""
                    distinct=""false"">
                    <entity name=""ita_trainingcontact"">
                        <attribute name=""ita_trainingcontactid"" />
                        <filter type=""and"">
                        <condition attribute=""ita_company"" operator=""eq"" value=""{0}"" />
                        </filter>
                    </entity>
                </fetch>";

            fetchXml = string.Format(fetchXml, companyId, page, pagingCookie);
            tracingService.Trace(fetchXml);
            var qe = new FetchExpression(fetchXml);
            var result = organizationService.RetrieveMultiple(qe);
            return result;
        }
    }
}
