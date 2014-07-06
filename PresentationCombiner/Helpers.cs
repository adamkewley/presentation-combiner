using System;
using System.ComponentModel;

namespace PresentationCombiner
{
    public static class Helpers
    {
        // Events

        public static void Raise(this PropertyChangedEventHandler sourceEvent, object sender, string propertyName)
        {
            if (sourceEvent != null)
                sourceEvent(sender, new PropertyChangedEventArgs(propertyName));
        }

        public static void Raise(this EventHandler sourceEvent, object sender, EventArgs e)
        {
            if (sourceEvent != null)
                sourceEvent(sender, e);
        }

        // Observables

        private sealed class SimpleObserver<T> : IObserver<T>
        {
            private readonly Action<T> handler;

            public SimpleObserver(Action<T> handler) { this.handler = handler; }
            
            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(T value) { handler(value); }
        }

        public static IDisposable Subscribe<T>(this IObservable<T> observable, Action<T> handler)
        {
            return observable.Subscribe(new SimpleObserver<T>(handler));
        }

        // App Helpers

        public static bool ClientHasPowerpointInstalled()
        {
            Type pptType = Type.GetTypeFromProgID("Powerpoint.Application");

            return pptType != null;
        }
    }
}
