namespace Nvents.Tests
{
    public class DummyFooSubscriptions
    {
        public bool FirstFooWasHandled { get; private set; }

        public void FirstFooSubscription(FooEvent @event)
        {
            FirstFooWasHandled = true;
        }

        public bool SecondFooWasHandled { get; private set; }

        public void SecondFooSubscription(FooEvent @event)
        {
            SecondFooWasHandled = true;
        }
    }
}