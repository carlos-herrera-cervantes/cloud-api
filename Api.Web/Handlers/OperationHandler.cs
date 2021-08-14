using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Api.Web.Models;

namespace Api.Web.Handlers
{
    public class OperationHandler : IOperationHandler, IDisposable
    {
        private readonly Subject<CollectionEventReceived> _subject;
        private readonly Dictionary<string, IDisposable> _subscribers;

        public OperationHandler()
        {
            _subject = new Subject<CollectionEventReceived>();
            _subscribers = new Dictionary<string, IDisposable>();
        }

        #region PublishMessage

        public void Publish(CollectionEventReceived eventType) => _subject.OnNext(eventType);

        #endregion

        #region ToSubscribePublisher

        public void Subscribe(string subscriberName, Action<CollectionEventReceived> action)
        {
            if (!_subscribers.ContainsKey(subscriberName))
            {
                _subscribers.Add(subscriberName, _subject.Subscribe(action));
            }
        }

        #endregion

        #region UnsubscribeObserver

        public void Dispose()
        {
            var isSubjectNotNull = !(_subject is null);

            if (isSubjectNotNull) _subject.Dispose();

            foreach (var subscriber in _subscribers)
            {
                subscriber.Value.Dispose();
            }
        }

        #endregion
    }
}