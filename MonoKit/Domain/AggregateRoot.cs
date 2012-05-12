// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregateRoot.cs" company="sgmunn">
//   (c) sgmunn 2012  
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace MonoKit.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Reflection;

    public abstract class AggregateRoot : IAggregateRoot
    {
        private static MethodExecutor Executor = new MethodExecutor();

        private readonly List<IDomainEvent> uncommittedEvents;

        public AggregateRoot()
        {
            this.uncommittedEvents = new List<IDomainEvent>();
        }

        public Guid AggregateId { get; protected set; }

        public int Version { get; protected set; }

        public IEnumerable<IDomainEvent> UncommittedEvents
        {
            get
            {
                return this.uncommittedEvents;
            }
        }

        public void Commit()
        {
            this.uncommittedEvents.Clear();
        }


        protected void ApplyEvents(IList<IDomainEvent> domainEvents)
        {
            foreach (var domainEvent in domainEvents)
            {
                this.ApplyEvent(domainEvent);
                this.Version = domainEvent.Version;
            }
        }

        protected void NewEvent(DomainEvent domainEvent)
        {
            this.Version++;

            domainEvent.AggregateId = this.AggregateId;
            domainEvent.Version = this.Version;
            this.ApplyEvent(domainEvent);
            this.uncommittedEvents.Add(domainEvent);
        }

        private void ApplyEvent(IDomainEvent domainEvent)
        {
            if (!Executor.ExecuteMethodForSingleParam(this, domainEvent))
            {
                throw new MissingMethodException(string.Format("Aggregate {0} does not support a method that can be called with {1}", this, domainEvent));
            }
        }
    }

    public abstract class AggregateRoot<TState> : AggregateRoot, ISnapshot where TState : class, new()
    {
        public AggregateRoot()
        {
            this.InternalState = new TState();
        }

        protected TState InternalState { get; private set; }

        public void LoadFromSnapshot(object snapshot, int snapshotVersion)
        {
            this.InternalState = snapshot as TState;
            this.Version = snapshotVersion;
        }

        public object GetSnapshot()
        {
            return this.InternalState;
        }
    }

    ////public class StoredDomainRepository: IDomainRepository
    ////{
    ////    private Func<IRepository<StoredDomainAggregate>> factory;
        
    ////    public StoredDomainRepository(Func<IRepository<StoredDomainAggregate>> factory)
    ////    {
    ////        this.factory = factory;
    ////    }
        
    ////    public void Load(IAggregateRoot aggregate)
    ////    {
    ////        var repository = this.factory();
    ////        var storedAggregate = repository.Get(aggregate.AggregateId);
            
    ////        var state = JsonConvert.DeserializeObject(storedAggregate.Aggregate, new JsonSerializerSettings
    ////                {
    ////                    TypeNameHandling = TypeNameHandling.All
    ////                });
           
    ////        ((ILoadFromState)aggregate).LoadFromState(state, storedAggregate.Version);
    ////    }
        
    ////    // todo: move to uow
    ////    public void Save(IAggregateRoot aggregate, int expectedVersion)
    ////    {
    ////        // todo: we need to lock here I think,
    ////        // get current version of the root (from db), concurrency version check
    ////        // then we need a unit of work in this repository to ...
            
            
    ////        // todo: do the save, check for version
    ////        var repository = this.factory();
   
    ////        var state = ((ILoadFromState)aggregate).GetInternalState();

    ////        var storedAggregate = new StoredDomainAggregate
    ////        {
    ////            AggregateId = aggregate.AggregateId,
    ////            Version = aggregate.Version,
    ////            Aggregate = JsonConvert.SerializeObject(state, Formatting.Indented, new JsonSerializerSettings
    ////                {
    ////                    TypeNameHandling = TypeNameHandling.All
    ////                }),
    ////        };
            
    ////        repository.Save(storedAggregate);    
    ////    }
    ////}
    
    //public class EventSourcedDomainRepository : IDomainRepository
    //{
    //    private Func<Guid, IRepository<IDomainEvent>> factory;
        
    //    public EventSourcedDomainRepository(Func<Guid, IRepository<IDomainEvent>> factory)
    //    {
    //        this.factory = factory;
    //    }
        
    //    public void Load(IAggregateRoot aggregate)
    //    {
    //        var repository = this.factory(aggregate.AggregateId);
    //        ((IEventSourced)aggregate).LoadFromHistory(repository.GetAll().ToList());
    //    }
        
    //    // todo: move to uow
    //    public void Save(IAggregateRoot aggregate, int expectedVersion)
    //    {
    //        var repository = this.factory(aggregate.AggregateId);
    //        //repository.Save();
    //    }
    //}

    
    
    
    //public class DomainRepositoryFactory : IDomainRepositoryFactory
    //{
    //    public IDomainRepository GetRepository(Guid id)
    //    {
    //        // 
    //        throw new NotImplementedException();
    //        //return new DomainRepository<T>((r) => new SqliteEventStore(r, null));
    //    }
    //}
        
    //public class StoredDomainAggregate
    //{
    //    //[MonoKit.Data.SQLite.PrimaryKey]
    //    public Guid AggregateId { get; set; }

    //    public int Version { get; set; }
        
    //    public string Aggregate { get; set; }
    //}

    
    //public abstract class EventStore : IRepository<IDomainEvent>
    //{
    //    public EventStore(Guid aggregateId)
    //    {
    //        this.AggregateId = aggregateId;
    //    }

    //    public Guid AggregateId { get; private set; }
        
    //    public IDomainEvent Get(Guid id)
    //    {
    //        // returns a single event, probably not that useful
    //        throw new NotSupportedException();
    //    }
        
    //    public abstract IEnumerable<IDomainEvent> GetAll();
        
    //    public void Save(IDomainEvent domainEvent)
    //    {
    //        // saves a new event for the aggregate
    //        if (this.AggregateId != domainEvent.AggregateId)
    //        {
    //            throw new InvalidOperationException("Cannot save an event for a different aggregate");
    //        }
            
    //        this.InternalSave(domainEvent);
    //    }
        
    //    protected abstract void InternalSave(IDomainEvent domainEvent);
    //}
    
    //public class StoredDomainEvent
    //{
    //    //[MonoKit.Data.SQLite.PrimaryKey]
    //    public Guid EventId { get; set; }
        
    //    //[MonoKit.Data.SQLite.Indexed]
    //    public Guid AggregateId { get; set; }

    //    public int Version { get; set; }
        
    //    public string Event { get; set; }
    //}
    
    //public class SqliteEventStore : EventStore
    //{
    //    private static bool NeedsInitialization = true;
        
    //    private MonoKit.Data.SQLite.SQLiteConnection connection;
        
    //    public SqliteEventStore(Guid aggregateId, MonoKit.Data.SQLite.SQLiteConnection connection) : base(aggregateId)
    //    {
    //        this.connection = connection;
    //    }
        
    //    public override IEnumerable<IDomainEvent> GetAll()
    //    {
    //        var storedEvents = new List<StoredDomainEvent>();
            
    //        var repo = new MonoKit.Data.SQLiteRepository<StoredDomainEvent>(this.connection, NeedsInitialization);
    //        using (repo)
    //        {
    //            NeedsInitialization = false;

    //            // get all events for the aggregate sorted by version
    //            storedEvents.AddRange(repo.GetAll().Where(x => x.AggregateId == this.AggregateId).OrderBy(x => x.Version).ToList());
    //        }
            
    //        var events = from ev in storedEvents
    //            select (JsonConvert.DeserializeObject(ev.Event, new JsonSerializerSettings
    //                {
    //                    TypeNameHandling = TypeNameHandling.All
    //                }) as IDomainEvent);
            
    //        return events;
    //    }
        
    //    protected override void InternalSave(IDomainEvent domainEvent)
    //    {
    //        var repo = new MonoKit.Data.SQLiteRepository<StoredDomainEvent>(this.connection, NeedsInitialization);
    //        using (repo)
    //        {
    //            NeedsInitialization = false;
                
    //            var storedEvent = new StoredDomainEvent
    //            {
    //                AggregateId = domainEvent.AggregateId,
    //                EventId = domainEvent.EventId,
    //                Version = domainEvent.Version,
    //                Event = JsonConvert.SerializeObject(domainEvent, Formatting.Indented, new JsonSerializerSettings
    //                {
    //                    TypeNameHandling = TypeNameHandling.All
    //                }),
    //            };
                
    //            repo.Insert(storedEvent);
    //        }
    //    }
    //}
}
