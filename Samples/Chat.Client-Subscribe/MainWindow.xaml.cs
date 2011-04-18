using System;
using System.Windows;
using Chat.Library;
using Nvents;

namespace Chat.Client_Subscribe
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
			// Subscribe to MessageSent events using the AddMessage method
			Events.Subscribe<MessageSent>(e => AddMessage(e));

			// Subscribe to UserKicked event using lambda and filter to only the current user
			Events.Subscribe<UserKicked>(e =>
				{
					// Inform that user has been kicked and exit the application when UserKicked event is published
					ExecuteOnUIThread(() =>
					{
						MessageBox.Show("You have been kicked");
						this.Close();
					});
				},
				e => e.UserId == currentUser.Id);
		}

		private void AddMessage(MessageSent message)
		{
			// Add message to listbox when MessageSent events are published
			ExecuteOnUIThread(() =>
				Messages.Items.Add(message));
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
