using System;
using Microsoft.AspNet.SignalR.Hubs;
using Unity;

namespace IOM
{
    public class UnityHubActivator : IHubActivator
    {
        private readonly IUnityContainer _container;
 
        public UnityHubActivator(IUnityContainer container)
        {
            _container = container;
        }
 
        public IHub Create(HubDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException("descriptor");
            }
 
            if (descriptor.HubType == null)
            {
                return null;
            }
 
            var hub = _container.Resolve(descriptor.HubType) ?? Activator.CreateInstance(descriptor.HubType);
            return hub as IHub;
        }
    }
}