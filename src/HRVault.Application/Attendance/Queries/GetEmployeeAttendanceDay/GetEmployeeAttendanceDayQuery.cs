using HRVault.Application.Attendance.DTOs;
using MediatR;

namespace HRVault.Application.Attendance.Queries.GetEmployeeAttendanceDay;

public record GetEmployeeAttendanceDayQuery(
    Guid EmployeeId,
    DateOnly Date
) : IRequest<AttendanceDayDto>;
