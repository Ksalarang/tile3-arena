using System;
using System.Collections.Generic;

namespace Events {
    public class EventBus<E> where E : Event {
        readonly Dictionary<Type, EventDeliverer<E>> deliverers = new();

        //todo: rename to signal
        public void sendEvent(E e) {
            var eventType = e.GetType();
            // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
            if (!deliverers.ContainsKey(eventType)) {
                deliverers.Add(eventType, new EventDeliverer<E>());
            }
            deliverers[eventType].handleEvent(e);
        }

        public void subscribe<TEvent>(EventSubscriber<E> subscriber) where TEvent : E {
            var eventType = typeof(TEvent);
            // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
            if (!deliverers.ContainsKey(eventType)) {
                deliverers.Add(eventType, new EventDeliverer<E>());
            }
            deliverers[eventType].addSubscriber(subscriber);
        }

        public void unsubscribe<TEvent>(EventSubscriber<E> subscriber) where TEvent : E {
            if (deliverers.TryGetValue(typeof(TEvent), out var deliverer)) {
                deliverer.removeSubscriber(subscriber);
            }
        }
    }
}