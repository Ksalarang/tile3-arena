using Utils;

namespace Events {
public class EventDeliverer<E> where E : Event {
    event EventSubscriber<E> subscribers;

    public void addSubscriber(EventSubscriber<E> subscriber) {
        subscribers += subscriber;
    }

    public void removeSubscriber(EventSubscriber<E> subscriber) {
        subscribers -= subscriber;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void handleEvent(E e) {
        if (subscribers != null) {
            subscribers.Invoke(e);
        } else {
            Log.warn($"{typeof(E)} deliverer", "subscribers is null");
        }
    }
}

public delegate void EventSubscriber<in E>(E e) where E : Event;
}