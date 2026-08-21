using HRVault.Api.Authorization;
using HRVault.Application.Employees.WorkSchedules.Commands.AssignEmployeeWorkSchedule;
using HRVault.Application.Employees.WorkSchedules.DTOs;
using HRVault.Application.Employees.WorkSchedules.Queries.GetEmployeeWorkSchedules;
using Microsoft.AspNetCore.Mvc;

namespace HRVault.Api.Controllers;

[ApiController]
[Route("api/Employees/{employeeId:guid}/work-schedules")]
public class EmployeeWorkSchedulesController : BaseApiController
{
    [HttpGet]
    [HasPermission("Employees.View")]
    public async Task<ActionResult<List<EmployeeWorkScheduleDto>>> GetHistory(
        Guid employeeId)
    {
        var result =
            await Mediator.Send(
                new GetEmployeeWorkSchedulesQuery(
                    employeeId));

        return Ok(result);
    }

    [HttpPost]
    [HasPermission("Employees.Edit")]
    public async Task<ActionResult<EmployeeWorkScheduleDto>> Assign(
        Guid employeeId,
        [FromBody] AssignEmployeeWorkScheduleRequest request)
    {
        var result =
            await Mediator.Send(
                new AssignEmployeeWorkScheduleCommand(
                    employeeId,
                    request.WorkScheduleId,
                    request.StartDate));

        return Ok(result);
    }
}

public class AssignEmployeeWorkScheduleRequest
{
    public Guid WorkScheduleId { get; set; }
    public DateOnly StartDate { get; set; }
}
