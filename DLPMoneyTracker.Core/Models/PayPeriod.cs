using System;
using Microsoft.IdentityModel.Tokens;

namespace DLPMoneyTracker.Core.Models;

public class PayPeriod
{
    public required DateTime StartDate { get; set; }
    public required int NumberOfDays { get; set; } 

    public DateTime CurrentPayPeriod => GetCurrentStartDate();
    public DateTime NextPayPeriod => CurrentPayPeriod.AddDays(NumberOfDays); 

    private DateTime GetCurrentStartDate()
    {
        int countPayPeriods = (int)Math.Floor((DateTime.Today - StartDate).TotalDays / NumberOfDays);
        return StartDate.AddDays(countPayPeriods * NumberOfDays).Date;
    }
}