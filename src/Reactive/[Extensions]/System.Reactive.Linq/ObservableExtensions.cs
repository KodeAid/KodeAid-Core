// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Reactive.Subjects;

namespace System.Reactive.Linq
{
    public static class ObservableExtensions
    {
        public static IObservable<T> Throttle<T>(this IObservable<T> source, Func<T, TimeSpan> dueTimeSelector)
        {
            var timerSubscription = default(IDisposable);
            var timerSubscriptionLock = new object();
            var throttledPublisher = new Subject<T>();
            return source
                .Where(f =>
                {
                    var timeSpan = dueTimeSelector(f);
                    if (timeSpan == TimeSpan.Zero)
                    {
                        return true;
                    }
                    lock (timerSubscriptionLock)
                    {
                        if (timerSubscription != null)
                        {
                            timerSubscription.Dispose();
                        }
                        timerSubscription = Observable.Timer(timeSpan).Subscribe(tick =>
                        {
                            timerSubscription = null;
                            throttledPublisher.OnNext(f);
                        });
                    }
                    return false;
                })
                .Merge(throttledPublisher);
        }
    }
}
