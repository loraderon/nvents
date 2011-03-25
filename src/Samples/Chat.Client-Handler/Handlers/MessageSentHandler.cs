using System;
using System.Windows.Controls;
using Chat.Library;
using Nvents;

namespace Chat.Client_Handler.Handlers
{
	public class MessageSentHandler : IHandler<MessageSent>
	{
		ListBox list;

		public MessageSentHandler(ListBox list)
		{
			this.list = list;
		}

		public void Handle(MessageSent @event)
		{
			list.Dispatcher.Invoke((Action)(() 
				=> list.Items.Add(@event)));
		}
	}
}
