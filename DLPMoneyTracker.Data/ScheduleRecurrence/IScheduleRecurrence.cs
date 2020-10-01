using System;
using System.Collections.Generic;
using System.Text;

namespace DLPMoneyTracker.Data.ScheduleRecurrence
{
    
    public enum RecurrenceFrequency
    {
        Monthly,
        SemiAnnual,
        Annual
    }


    public interface IScheduleRecurrence
    {
        public const int NOTIFICATION_DAYS_PRIOR = -7;
        public const string FREQUENCY_MONTHLY = "MONTHLY";
        public const string FREQUENCY_SEMI_ANNUAL = "SEMIANNUAL";
        public const string FREQUENCY_ANNUAL = "ANNUAL";

        DateTime NextOccurence { get; }
        DateTime NotificationDate { get; }
        RecurrenceFrequency Frequency { get; }


        string GetFileData();
        void LoadFileData(string data);

    }

}
