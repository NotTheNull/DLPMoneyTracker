using DLPMoneyTracker.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyTrackerBlaze.Models
{
    public class NotificationHelper
    {
        public const string MSG_PAGETITLE = "PageTitleChanged";
        private readonly NotificationSystem notifyThis;

        public NotificationHelper(NotificationSystem notifyThis)
        {
            this.notifyThis = notifyThis;
        }


        public void SetPageTitle(string title)
        {
            notifyThis.SendMessage(MSG_PAGETITLE, title);
        }

        public void Subscribe_SetPageTitle(SendMessageHandler eventHandler)
        {
            ArgumentNullException.ThrowIfNull(eventHandler);
            notifyThis.MessageEvents[MSG_PAGETITLE] += eventHandler;
        }

        public void Unsubscribe_SetPageTItle(SendMessageHandler eventHandler)
        {
            ArgumentNullException.ThrowIfNull(eventHandler);
            notifyThis.MessageEvents[MSG_PAGETITLE] -= eventHandler;
        }

    }
}

