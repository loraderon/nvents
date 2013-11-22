using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;

namespace Nvents.Services
{
	/// <summary>
	/// In memory service for publishing/subscribing to events in the same process
	/// </summary>
	public class CombinationService : ServiceBase
	{
        public List<IService> Services { get; private set; }

	    readonly System.Timers.Timer timer = new System.Timers.Timer(60000) { AutoReset = false };
	    private TimeSpan timeLimit = TimeSpan.FromMinutes(1);
	    public TimeSpan UniqueCheckTimeLimit
	    {
	        get { return timeLimit; }
	        set
	        {
	            timeLimit = value;
	            timer.Interval = value.Milliseconds;
	        }
	    }

        /// <summary>
        /// Setting to All causes all duplicate events to be filtered. 
        /// Setting to None will cause duplicate events to be handled again. 
        /// Setting to UniqueOnly will suppress events that implement IUniqueNvent. 
        /// </summary>
        public DuplicateSuppressionOption DuplicateSuppression { get; set; }

        readonly ConcurrentDictionary<Guid, Tuple<int, DateTime>> alreadyProcessedEvents = new ConcurrentDictionary<Guid, Tuple<int, DateTime>>();

	    public CombinationService(params IService[] services)
	    {
	        DuplicateSuppression = DuplicateSuppressionOption.All;
	        Services = services.ToList();
	    }

	    private void CleanupProcessedEvents(object sender, ElapsedEventArgs e)
	    {
	        try
	        {
                Tuple<int, DateTime> dummy;
                DateTime earliestAllowedTime = DateTime.UtcNow.Subtract(UniqueCheckTimeLimit);
                var expiredKeys = alreadyProcessedEvents
                                    .Where(pair => pair.Value.Item2 < earliestAllowedTime)
                                    .Select(pair => pair.Key).ToList();
                foreach (var key in expiredKeys)
                    alreadyProcessedEvents.TryRemove(key, out dummy);
	        }
	        catch
	        {
	        }

            timer.Start();
	    }

	    protected override void OnStart()
	    {
	        foreach (IService service in Services)
	        {
	            service.Start();
                service.Subscribe<object>(HandleEvent);
	        }

            timer.Elapsed += CleanupProcessedEvents;
            timer.Start();
	    }

        protected override void OnStop()
        {
            timer.Stop();

            foreach (IService service in Services)
            {
                service.Stop();
                service.Unsubscribe<object>();
            }
        }

	    private void HandleEvent(object nvent)
	    {
            foreach (var registration in registrations
                .Where(x => ShouldEventBeHandled(x, nvent)))
            {
                // Unwrap if wrapped for DuplicateSuppression == All
                //  so correct event is passed to handler
                object nvent2 = UnwrapEvent(nvent);
                ThreadPool.QueueUserWorkItem(s =>
                    ((EventRegistration)s).Action(nvent2), registration);
            }
	    }

	    private int lastEventNumber;
	    protected override bool ShouldEventBeHandled(EventRegistration registration, object e)
	    {
            if (DuplicateSuppression != DuplicateSuppressionOption.None)
	        {
                // Make sure Unique event is only handled once
                // Assign a number each time an event is processed, only process the event
                // if the same number is in the dictionary (indicated it was just added this time around)
	            IUniqueNvent unique = e as IUniqueNvent;
	            if (unique != null)
	            {
                    int localNumber = Interlocked.Increment(ref lastEventNumber);
	                var eventData = alreadyProcessedEvents.GetOrAdd(unique.ID, new Tuple<int, DateTime>(localNumber, DateTime.UtcNow));
	                if (eventData.Item1 != localNumber)
                        return false;
	            }

                // Unwrap if wrapped for DuplicateSuppression == All
                //  so base.ShouldEventBeHandled checks the correct type
                e = UnwrapEvent(e);
	        }

	        return base.ShouldEventBeHandled(registration, e);
	    }

	    private static object UnwrapEvent(object nvent)
	    {
	        UniqueEventWrapper wrapper = nvent as UniqueEventWrapper;
	        if (wrapper != null)
	            nvent = wrapper.Event;
	        return nvent;
	    }

	    public override void Publish<TEvent>(TEvent e)
		{
            object nvent = DuplicateSuppression==DuplicateSuppressionOption.All 
                            && !(e is IUniqueNvent)
                            ? new UniqueEventWrapper(e)
                            : (object)e;

			foreach(IService service in Services)
                service.Publish(nvent);
		}
	}

    public enum DuplicateSuppressionOption
    {
        All,
        UniqueOnly,
        None
    }
}