namespace HRVault.Application.Attendance.Models;

public class AttendanceWeekDayInput
{
    public AttendanceDayResult Day { get; set; } = null!;

    // Indica se houve trabalho real nesse dia.
    // Para semana variável isto é o que conta para os
    // RequiredWorkingDaysPerWeek.
    public bool HasWorked { get; set; }
}
