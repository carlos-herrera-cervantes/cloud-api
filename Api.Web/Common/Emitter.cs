using Api.Domain.Models;
using Api.Web.Handlers;
using Api.Web.Models;

namespace Api.Web.Common
{
    public static class Emitter
    {
        public static void EmitMessage(IOperationHandler operationHandler, CollectionEventReceived collectionEvent) 
            => operationHandler.Publish(collectionEvent);
    }
}