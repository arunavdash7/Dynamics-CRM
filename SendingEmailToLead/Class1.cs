using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendingEmailToLead
{
    public class Class1:IPlugin
    {
        public void Execute(IServiceProvider serviceprovider)
        {
            
            IPluginExecutionContext Context = (IPluginExecutionContext)serviceprovider.GetService(typeof(IPluginExecutionContext));//getting the context from input parameters
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceprovider.GetService(typeof(IOrganizationServiceFactory));//getting service factory 
            IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(Context.UserId);//retrieving the service 
            ITracingService tracingService = (ITracingService)serviceprovider.GetService(typeof(ITracingService));// getting the tracing service
            if(Context.InputParameters.Contains("Target") && Context.InputParameters["Target"] is Entity)
            {
                Entity lead = (Entity)Context.InputParameters["Target"];//getting lead entity from inputparameters
                string EmaildId = lead.GetAttributeValue<string>("emailaddress1");//retrieving the Email Id from lead entity
                string Name = lead.GetAttributeValue<string>("lastname");//retrieving last name of lead
                // from current user
                Entity from = new Entity("activityparty");
                from["partyid"] = new EntityReference("systemuser", Context.UserId);

                // to newly created lead user
                Entity to = new Entity("activityparty");
                to["partyid"] = new EntityReference("lead" , lead.Id);

                //created a webresource named as "new_RealMadrid" which stores the image file
                string webresource = "<html> <img  src='~/WebResources/new_RealMadrid'  width='205' height='150'></html>";
                // Create Email
                Entity Email = new Entity("email");
                Email["from"] = new Entity[] { from };
                Email["to"] = new Entity[] { to };
                Email["subject"] = "Welcome Mr./ Mrs " + Name;
                Email["description"] = "<h3> Dear  " + Name + "</h3>" + "<br/>" + "Welcome to my blog. Hope it helped you." + "<br/>" + " " + "<br/>" + "visit my blog for any query" + "" + "<br/>" + webresource;

                // Send email request
                Guid _emailId = service.Create(Email);
                SendEmailRequest reqSendEmail = new SendEmailRequest();
                reqSendEmail.EmailId = _emailId;
                reqSendEmail.TrackingToken = "";
                reqSendEmail.IssueSend = true;

                SendEmailResponse res = (SendEmailResponse)service.Execute(reqSendEmail);
            }
        }
    }
}
