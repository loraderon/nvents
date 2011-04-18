using System;
using System.Windows;
using Chat.Library;
using Nvents;

namespace Chat.Client_EventSubscription
{
	public partial class MainWindow : Window
	{
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Publish a MessageSent event containing message text and current user
			Events.Publish(new MessageSent { Message = MessageToSend.Text, Sender = currentUser });
			MessageToSend.Text = "";
		}

		private void SubscribeToEvents()
		{
			// Workaround for issue #2 'EventSubscription<TEvent> default constructor does not start the service'
			// This issue is fixed in nvents 0.7
			Events.Service.Start();


			// Create a EventSubscription for the MessageSent event
			var messageSent = new EventSubscription<MessageSent>();
			// Register a handler for when Published is raised for MessageSent event
			messageSent.Published += messageSent_Published;


			// Create a EventSubscription for the UserKicked event and filter to only the current user
			var userKicked = new EventSubscription<UserKicked>(user => user.UserId == currentUser.Id);
			// Register a handler for when Published is raised for UserKicked event (will only be raised for events that passes the subscription filter)
			userKicked.Published += userKicked_Published;
		}

		void userKicked_Published(object sender, EventSubscription<UserKicked>.PublishedEventArgs e)
		{
			// Inform that user has been kicked and exit the application when UserKicked event is published
			ExecuteOnUIThread(() =>
			{
				MessageBox.Show("You have been kicked");
				this.Close();
			});
		}

		void messageSent_Published(object sender, EventSubscription<MessageSent>.PublishedEventArgs e)
		{
			// Add message to listbox when MessageSent events are published
			ExecuteOnUIThread(() =>
				Messages.Items.Add(e.Event));
		}

		#region not related to nvents

		private User currentUser;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string name = GetUserName();
			currentUser = new User { Id = Guid.NewGuid(), Name = name };

			SubscribeToEvents();
		}

		private string GetUserName()
		{
			string name;
			do
			{
				name = Microsoft.VisualBasic.Interaction.InputBox("Enter your name", this.Title + " - Enter name");
			} while (string.IsNullOrEmpty(name));
			return name;
		}

		private void ExecuteOnUIThread(Action action)
		{
			Dispatcher.Invoke(action);
		}
		#endregion
	}
}
