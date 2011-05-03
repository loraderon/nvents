# Chat samples
This is a sample solution for showing different ways of using nvents.

## Chat.Client-Subscribe
Demonstrates the easiest way to subscribe to events using `Events.Subscribe<MyEvent>(e => ..)` see [Chat.Client-Subscribe/MainWindow.xaml.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Client-Subscribe/MainWindow.xaml.cs#L20).

## Chat.Client-Handler
Demonstrates the usage of event handlers by implementing `IHandler<TEvent>`. Two handlers are used [Chat.Client-Handler/Handlers/MessageSentHandler.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Client-Handler/Handlers/MessageSentHandler.cs) and [Chat.Client-Handler/Handlers/UserKickedHandler.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Client-Handler/Handlers/UserKickedHandler.cs) and they are registered by calling `Events.RegisterHandler(handler)` in [Chat.Client-Handler/MainWindow.xaml.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Client-Handler/MainWindow.xaml.cs#L22).

## Chat.Client-EventSubscription
Demonstrates the usage of standard .NET event registration by creating a new `EventSubscription<MyEvent>` and attaching to it's `Published` event see [Chat.Client-EventSubscription/MainWindow.xaml.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Client-EventSubscription/MainWindow.xaml.cs#L25).

## Chat.Moderator
A more complete example using event handlers and the `IPublisher` interface. Also demonstrating IoC ([Ninject](http://ninject.org/)) and MVVM ([Caliburn.Micro](http://caliburnmicro.codeplex.com/)) with nvents.
The event handler [Chat.Moderator/Handlers/MessageSentHandler.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator/Handlers/MessageSentHandler.cs) is registered in [Chat.Moderator/NventsModule.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator/NventsModule.cs#L27) which also binds the `IPublisher` interface to the default nvents service. `IPublisher` is used in [Chat.Moderator/ViewModels/ShellViewModel.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator/ViewModels/ShellViewModel.cs#L21) to publish `UserKicked` events.

## Chat.Moderator.Tests
Test that the event is published by stubbing, mocking or [faking](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator.Tests/Fakes/FakePublisher.cs) `IPublisher` and asserting that `Publish` was called as shown in [Chat.Moderator.Tests/ViewModels/ShellViewModelTests.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator.Tests/ViewModels/ShellViewModelTests.cs).
Test that the event is subscribed to by stubbing, mocking or [faking](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator.Tests/Fakes/FakeService.cs) `Events.Service` and asserting that `Subscribe` was called as shown in [Chat.Moderator.Tests/NventsModuleTests.cs](https://github.com/loraderon/nvents/blob/master/Samples/Chat.Moderator.Tests/NventsModuleTests.cs#17).

## Chat.Library
Library containing `UserKicked` and `MessageSent` events and the `User` class.

---
[nvents](http://nvents.org) - an open source library for strongly typed publishing/subscribing of events over the network.
