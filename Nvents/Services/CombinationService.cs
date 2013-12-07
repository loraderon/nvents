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
        private int lastEventNumber;

        public List<IService> Services { get; private set; }

	    readonly System.Timers.Timer timer = new System.Timers.Timer(60000) { AutoReset = false };
	    private TimeSpan timeLimit = TimeSpan.FromMinutes(1);
	    public TimeSpan UniqueCheckTimeLimit
	    {
	        get { return timeLimit; }
	        set
	        {
	            timeLimit = value;
	            timer.Interval = value.TotalMilliseconds;
	        }
	    }

        /// <summary>
        /// Setting to All causes all duplicate events to be filtered. 
        /// Setting to None will cause duplicate events to be handled again. 
        /// Setting to UseEquals will suppress events that already been processed based on the Equals method. 
        /// </summary>
        public DuplicateSuppressionOption DuplicateSuppression { get; set; }

        readonly ConcurrentDictionary<object, Tuple<int, DateTime>> alreadyProcessedEvents = new ConcurrentDictionary<object, Tuple<int, DateTime>>();

	    public CombinationService(params IService[] services)
	    {
	        DuplicateSuppression = DuplicateSuppressionOption.All;
	        Services = services.ToList();
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

        public override void Publish<TEvent>(TEvent e)
        {
            object nvent = DuplicateSuppression == DuplicateSuppressionOption.All
                            ? new UniqueEventWrapper(e)
                            : (object)e;

            foreach (IService service in Services)
                service.Publish(nvent);
        }

	    private void HandleEvent(object nvent)
	    {
            // Check if event has already been processed
	        if (DuplicateSuppression != DuplicateSuppressionOption.None)
	        {
                // Make sure Unique event is only handled once
                // Assign a number each time an event is processed, only process the event
                // if the same number is in the dictionary (indicated it was just added this time around)
                
                int localNumber = Interlocked.Increment(ref lastEventNumber);
                var eventData = alreadyProcessedEvents.GetOrAdd(nvent, new Tuple<int, DateTime>(localNumber, DateTime.UtcNow));
                if (eventData.Item1 != localNumber)
                    return;
            }

            // Unwrap if wrapped for DuplicateSuppression == All
            //  so correct event is passed to handler
            object baseNvent = UnwrapEvent(nvent);

	        foreach (var registration in registrations.Where(x => ShouldEventBeHandled(x, baseNvent)))
                registration.Action.Invoke(baseNvent);
	    }

	    private static object UnwrapEvent(object nvent)
	    {
	        UniqueEventWrapper wrapper = nvent as UniqueEventWrapper;
	        if (wrapper != null)
	            nvent = wrapper.Event;
	        return nvent;
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
	}

    public enum DuplicateSuppressionOption
    {
        /// <summary>
        /// All will wrap all events and filter duplicates received for the same event
        ///  based on a Guid in the wrapper
        /// </summary>
        All,
        /// <summary>
        /// UseEquals will filter events that have already been received 
        ///  if a matching object has been received based on the events Equals method
        /// </summary>
        UseEquals,
        /// <summary>
        /// None will process all received events, regardless of whether or not they 
        /// have previously been received and processed
        /// </summary>
        None
    }
}