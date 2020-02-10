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
            var qe = new QueryExpression("ita_trainingcontact");
            qe.Criteria.AddCondition(new ConditionExpression("ita_company", ConditionOperator.Equal, companyId));

            qe.PageInfo = new PagingInfo
            {
                Count = 5,
                PageNumber = page,
                PagingCookie = pagingCookie,
                ReturnTotalRecordCount = true
            };

            var result = organizationService.RetrieveMultiple(qe);
            return result;
        }
    }
}
