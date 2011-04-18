using System;
using System.Windows;
using Chat.Library;
using Nvents;

namespace Chat.Client_Handler.Handlers
{
	/// <summary>
	/// Handles UserKicked events
	/// </summary>
	public class UserKickedHandler : IHandler<UserKicked>
	{
		User currentUser;

		public UserKickedHandler(User currentUser)
		{
			this.currentUser = currentUser;
		}

		public void Handle(UserKicked @event)
		{
			// In nvents 0.7 this could be a filter on Events.RegisterHandler
			if (currentUser.Id != @event.UserId)
				return;

			// Inform that user has been kicked and exit the application when UserKicked event is handled
			Application.Current.Dispatcher.Invoke((Action)(() =>
			{
				MessageBox.Show("You have been kicked");
				Application.Current.MainWindow.Close();
			}));			
		}
	}
}
