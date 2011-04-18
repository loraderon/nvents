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
			Events.Publish(new MessageSent { Message = MessageToSend.Text, Sender = currentUser });
			MessageToSend.Text = "";
		}

		private void SubscribeToEvents()
		{
			Events.Subscribe<MessageSent>(e => AddMessage(e));
			Events.Subscribe<UserKicked>(e =>
				{
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
