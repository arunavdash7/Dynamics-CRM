using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Fakes;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.Sdk.Messages;
using SendingEmailToLead;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Entity lead = new Entity("lead");
            Guid userId = Guid.NewGuid();
            Guid leadId = Guid.NewGuid();
            // IPluginExecutionContext

            var pluginExecutionContext = new StubIPluginExecutionContext();
            pluginExecutionContext.UserIdGet = () =>
            {
                return userId;
            };
            pluginExecutionContext.InputParametersGet = () =>
            {
                ParameterCollection parameters = new ParameterCollection();
                parameters["Target"] = lead;
                //parameters["Assignee"] = teamRef;
                return parameters;
            };

            // IOrganizationService
            var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService();

            // IOrganizationServiceFactory
            var factory = new StubIOrganizationServiceFactory();
            factory.CreateOrganizationServiceNullableOfGuid = id =>
            {
                return service;
            };

            // IServiceProvider
            var serviceProvider = new System.Fakes.StubIServiceProvider();
            serviceProvider.GetServiceType = t =>
            {
                if (t == typeof(IPluginExecutionContext))
                {
                    return pluginExecutionContext;
                }
                else if (t == typeof(IOrganizationServiceFactory))
                {
                    return factory;
                }

                return null;
            };
            service.CreateEntity = (entity) =>
            {
                return lead.Id;
            };

            service.ExecuteOrganizationRequest = (executerequest) =>
             {
                 return new SendEmailResponse();
             };
            Class1 target = new Class1();
            target.Execute(serviceProvider);
        }
    }
}
