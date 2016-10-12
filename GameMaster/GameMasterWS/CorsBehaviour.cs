using eacc.gamemaster.ws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace eacc.gamemaster.ws
{
    public class CorsMessageInspector : IDispatchMessageInspector
    {
        public string clientOrigin;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            HttpRequestMessageProperty requestProperty = request.Properties[HttpRequestMessageProperty.Name]
                          as HttpRequestMessageProperty;

            if (requestProperty != null && requestProperty.Headers.AllKeys.Contains("Origin") && !string.IsNullOrEmpty(requestProperty.Headers["Origin"]))
            {
                clientOrigin = requestProperty.Headers["Origin"];
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            HttpResponseMessageProperty httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;

            if (httpHeader != null)
            {
                httpHeader.ApplyCrossOriginSupport(clientOrigin);
            }
        }

      
    }

    public class CorsBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            ChannelDispatcher channelDispatcher = endpointDispatcher.ChannelDispatcher;
            if (channelDispatcher != null)
            {
                foreach (EndpointDispatcher ed in channelDispatcher.Endpoints)
                {
                    ed.DispatchRuntime.MessageInspectors.Add(new CorsMessageInspector());
                }
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion
    }

    public class CorsBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new CorsBehavior();
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(CorsBehavior);
            }
        }
    }

    public static class ExtentionsMethods
    {
        public static void ApplyCrossOriginSupport(this HttpResponseMessageProperty message, string origin)
        {
            message.Headers.Add("Access-Control-Allow-Origin", origin);
            message.Headers.Add("Access-Control-Allow-Method", "GET,POST,PUT,PATCH,DELETE,OPTIONS");
        }
    }
}