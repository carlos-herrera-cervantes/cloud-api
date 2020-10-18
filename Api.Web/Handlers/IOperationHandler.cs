using System;
using Api.Web.Models;

namespace Api.Web.Handlers
{
    public interface IOperationHandler
    {
         void Publish(CollectionEventReceived eventType);
         void Subscribe(string subscriberName, Action<CollectionEventReceived> action);
    }
}