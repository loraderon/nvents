using System;
using System.Windows;
using Chat.Client_Handler.Handlers;
using Chat.Library;
using Nvents;

namespace Chat.Client_Handler
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
			// Creating and registering the MessageSentHandler
			var messageSentHandler = new MessageSentHandler(Messages);
			Events.RegisterHandler(messageSentHandler);

			// Creating and registering the UserKickedHandler
			var userKickedHandler = new UserKickedHandler(currentUser);
			Events.RegisterHandler(userKickedHandler);

			// Filtering for handlers are added in nvents 0.7
			// Events.RegisterHandler(userKickedHandler, e => e.UserId == currentUser.Id);
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