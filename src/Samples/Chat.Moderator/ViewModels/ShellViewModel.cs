using System;
using Caliburn.Micro;
using Nvents.Services;
using Chat.Library;

namespace Chat.Moderator.ViewModels
{
	public class ShellViewModel : PropertyChangedBase
	{
		IPublisher publisher;

		public ShellViewModel(IPublisher publisher)
		{
			this.publisher = publisher;
		}

		public void KickUser(MessageSent message)
		{
			publisher.Publish(new UserKicked { UserId = message.Sender.Id });
		}

		private BindableCollection<MessageSent> messages = new BindableCollection<MessageSent>();
		public BindableCollection<MessageSent> Messages
		{
			get { return messages; }
			set { messages = value; NotifyOfPropertyChange(() => this.Messages); }
		}
	}
}
