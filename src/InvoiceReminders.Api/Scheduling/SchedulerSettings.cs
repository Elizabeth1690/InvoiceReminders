using System.ComponentModel.DataAnnotations;

namespace InvoiceReminders.Api.Scheduling;

public sealed class SchedulerSettings
{
    public const string SectionName = "Scheduler";

    public bool Enabled { get; set; }

    [Range(1, 1440)]
    public int IntervalMinutes { get; set; } = 60;
}