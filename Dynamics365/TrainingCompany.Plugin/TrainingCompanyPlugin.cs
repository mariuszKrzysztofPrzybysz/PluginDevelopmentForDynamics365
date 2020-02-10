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
            IOrganizationService service = null;
            IOrganizationServiceFactory serviceFactory = null;

            var entity = (Entity)context.InputParameters["Target"];
            bool creditRatingCheck = entity.Contains("ita_creditrating") && entity["ita_creditrating"] != null;
            bool priorityUpdate = context.MessageName.Equals("Update") && entity.Contains("ita_priority");

            if (creditRatingCheck || priorityUpdate)
            {
                serviceFactory = (IOrganizationServiceFactory)
                    serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                service = serviceFactory.CreateOrganizationService(context.UserId);
            }

            if (creditRatingCheck)
            {
                ExecuteCreditLimitValidation(context, service, entity);
            }
            if (priorityUpdate)
            {
                ExecutePriorityUpdate(context, service, entity);
            }
        }

        private EntityCollection RetrieveContacts(IOrganizationService organizationService, Guid companyId, int page, string pagingCookie)
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

        private void ExecuteCreditLimitValidation(IPluginExecutionContext context, IOrganizationService service, Entity entity)
        {

        }

        private void ExecutePriorityUpdate(IPluginExecutionContext context, IOrganizationService service, Entity entity)
        {
            int pageNumber = 1;
            string pagingCookie = string.Empty;
            EntityCollection result;
            do
            {
                result = RetrieveContacts(service, entity.Id, pageNumber, pagingCookie);
                foreach (var e in result.Entities)
                {
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
}
