using HRVault.Api.Authorization;
using HRVault.Application.Attendance.DTOs;
using HRVault.Application.Attendance.Queries.GetEmployeeAttendanceDay;
using HRVault.Application.Attendance.Queries.GetEmployeeAttendanceWeek;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceController : BaseApiController
{
    [HttpGet("employee/{employeeId:guid}/day/{date}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<AttendanceDayDto>> GetEmployeeDay(
        Guid employeeId,
        DateOnly date)
    {
        var result =
            await Mediator.Send(
                new GetEmployeeAttendanceDayQuery(
                    employeeId,
                    date));

        return Ok(result);
    }

    [HttpGet("employee/{employeeId:guid}/week/{date}")]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<AttendanceWeekDto>> GetEmployeeWeek(
        Guid employeeId,
        DateOnly date)
    {
        var result =
            await Mediator.Send(
                new GetEmployeeAttendanceWeekQuery(
                    employeeId,
                    date));

        return Ok(result);
    }
}
