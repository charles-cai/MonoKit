// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObservableDomainEventBus.cs" company="sgmunn">
//   (c) sgmunn 2012  
//
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//   the Software.
// 
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//   THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//   IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using MonoKit.Reactive.Subjects;
    using MonoKit.Data;
    using MonoKit.Reactive.Linq;

    public sealed class ObservableDomainEventBus : IDomainEventBus
    {
        private readonly Subject<IDomainEvent> eventPublisher;

        public ObservableDomainEventBus()
        {
            this.eventPublisher = new Subject<IDomainEvent>();
        }

        public void Publish(IDomainEvent evt)
        {
            this.eventPublisher.OnNext(evt);
        }

        public IDisposable Subscribe(IObserver<IDomainEvent> observer)
        {
            return this.eventPublisher.Subscribe(observer);
        }
    }

    public static class EventBusExtensions
    {
        /// <summary>
        /// Returns IDataModelEvents that are for the specified identity type.  These could be events or read model changes
        /// </summary>            
//        public static IObservable<IDataModelEvent> ForIdentity<T>(this IObservable<IDataModelEvent> source) where T : IUniqueIdentity
//        {
//            return source.Where (x => x != null && typeof(T).IsAssignableFrom(x.Identity.GetType()));
//        }
//
//        /// <summary>
//        /// Returns data model changes for the given identity, events or read models
//        /// </summary>            
//        public static IObservable<IDataModelChange> DataModelChangesForIdentity<T>(this IObservable<IDataModelEvent> source) where T : IUniqueIdentity
//        {
//            return source.ForIdentity<T>()
//                .OfType<IDataModelChange>();
//        }
//
//        /// <summary>
//        /// Returns events for the given identity type
//        /// </summary>            
//        public static IObservable<IAggregateEvent> EventsForIdentity<T>(this IObservable<IDataModelEvent> source) where T : IUniqueIdentity
//        {
//            return source.ForIdentity<T>()
//                .OfType<IAggregateEvent>();
//        }

        /// <summary>
        /// Returns data model changes for the specified read model type, excluding deleted read models
        /// </summary>            
        public static IObservable<T> DataModelChangesForType<T>(this IObservable<IDomainEvent> source)
        {
            return source.OfType<IDataModelChange>()
                .Where (x => x != null && x.Change != DataModelChangeKind.Deleted && typeof(T).IsAssignableFrom(x.DataModel.GetType()))
                    .Select(x => (T)x.DataModel);
        }

        /// <summary>
        /// Returns delete notifications for read models of the specified identity type
        /// </summary>            
//        public static IObservable<IUniqueIdentity> DataModelDeletionsForIdentity<T>(this IObservable<IDataModelEvent> source) where T : IUniqueIdentity
//        {
//            return source.ForIdentity<T>()
//                .OfType<IDataModelChange>()
//                    .Where(x => x.Deleted)
//                    .Select(x => x.Identity);
//        }
    }
}

