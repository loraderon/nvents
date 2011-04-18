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
			// publisher is injected by ninject
			this.publisher = publisher;
		}

		public void KickUser(MessageSent message)
		{
			// Publish a UserKicked event containing id of the user being kicked
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
