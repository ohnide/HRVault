using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using HRVault.Application.Employees.WorkSchedules.DTOs;
using HRVault.Domain.Entities;
using MediatR;

namespace HRVault.Application.Employees.WorkSchedules.Commands.AssignEmployeeWorkSchedule;

public class AssignEmployeeWorkScheduleCommandHandler
    : IRequestHandler<AssignEmployeeWorkScheduleCommand, EmployeeWorkScheduleDto>
{
    private readonly IEmployeeWorkScheduleRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICurrentUserService _currentUser;

    public AssignEmployeeWorkScheduleCommandHandler(
        IEmployeeWorkScheduleRepository repository,
        IEmployeeRepository employeeRepository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _currentUser = currentUser;
    }

    public async Task<EmployeeWorkScheduleDto> Handle(
        AssignEmployeeWorkScheduleCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var companyId = _currentUser.CompanyId.Value;

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                companyId,
                cancellationToken);

        if (employee is null)
            throw new NotFoundException("Funcionário não encontrado.");

        var workSchedule =
            await _repository.GetWorkScheduleAsync(
                request.WorkScheduleId,
                companyId,
                cancellationToken);

        if (workSchedule is null)
            throw new NotFoundException("Horário não encontrado.");

        /*
         * Uma atribuição que comece exatamente na mesma data de outra
         * é ambígua e não deve ser criada automaticamente.
         */
        var sameStart =
            await _repository.GetByStartDateAsync(
                request.EmployeeId,
                companyId,
                request.StartDate,
                cancellationToken);

        if (sameStart is not null)
        {
            throw new InvalidOperationException(
                "Já existe uma atribuição de horário com esta data de início.");
        }

        /*
         * Procuramos a atribuição que cobre a nova data.
         *
         * Exemplo:
         * A = 01/01 -> atual
         * Novo B = 01/09
         *
         * A passa para 01/01 -> 31/08.
         */
        var coveringAssignment =
            await _repository.GetAssignmentForDateAsync(
                request.EmployeeId,
                companyId,
                request.StartDate,
                cancellationToken);

        if (coveringAssignment is not null)
        {
            /*
             * Como sameStart já foi validado, StartDate é necessariamente
             * anterior à nova data.
             */
            coveringAssignment.EndDate =
                request.StartDate.AddDays(-1);

            coveringAssignment.UpdatedAt =
                DateTime.UtcNow;

            coveringAssignment.UpdatedBy =
                _currentUser.UserId;

            _repository.Update(coveringAssignment);
        }

        /*
         * Não permitimos criar uma atribuição que atravesse uma atribuição
         * futura já existente.
         *
         * Nesse cenário, a nova atribuição termina no dia anterior à próxima.
         *
         * Isto permite também inserir corretamente um horário no histórico:
         *
         * A 01/01 -> 31/03
         * C 01/06 -> atual
         *
         * inserir B em 01/04:
         * B 01/04 -> 31/05
         */
        var nextAssignment =
            await _repository.GetNextAssignmentAsync(
                request.EmployeeId,
                companyId,
                request.StartDate,
                cancellationToken);

        var newAssignment =
            new EmployeeWorkSchedule
            {
//                Id = Guid.NewGuid(),
                CompanyId = companyId,
                EmployeeId = request.EmployeeId,
                WorkScheduleId = request.WorkScheduleId,
                StartDate = request.StartDate,
                EndDate = nextAssignment is null
                    ? null
                    : nextAssignment.StartDate.AddDays(-1),

                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUser.UserId
            };

        await _repository.AddAsync(
            newAssignment,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new EmployeeWorkScheduleDto
        {
            Id = newAssignment.Id,
            EmployeeId = newAssignment.EmployeeId,

            WorkScheduleId = workSchedule.Id,
            WorkScheduleName = workSchedule.Name,
            WorkScheduleType = workSchedule.Type.ToString(),

            StartDate = newAssignment.StartDate,
            EndDate = newAssignment.EndDate,

            IsCurrent =
                newAssignment.StartDate <=
                    DateOnly.FromDateTime(DateTime.UtcNow) &&
                (!newAssignment.EndDate.HasValue ||
                 newAssignment.EndDate.Value >=
                    DateOnly.FromDateTime(DateTime.UtcNow))
        };
    }
}
