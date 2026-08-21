using HRVault.Application.Attendance.DTOs;
using MediatR;

namespace HRVault.Application.Attendance.Queries.GetEmployeeAttendanceWeek;

public record GetEmployeeAttendanceWeekQuery(
    Guid EmployeeId,
    DateOnly Date
) : IRequest<AttendanceWeekDto>;
