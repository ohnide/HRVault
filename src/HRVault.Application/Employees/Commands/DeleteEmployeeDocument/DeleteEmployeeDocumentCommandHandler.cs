using HRVault.Application.Common.Exceptions;
using HRVault.Application.Common.Interfaces;
using MediatR;

namespace HRVault.Application.Employees.Commands.DeleteEmployeeDocument;

public class DeleteEmployeeDocumentCommandHandler
    : IRequestHandler<DeleteEmployeeDocumentCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmployeeDocumentCommandHandler(
        IEmployeeRepository employeeRepository,
        IDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork)
    {
        _employeeRepository = employeeRepository;
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteEmployeeDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.CompanyId is null)
            throw new UnauthorizedAccessException();

        var employee =
            await _employeeRepository.GetByIdAndCompanyAsync(
                request.EmployeeId,
                _currentUser.CompanyId.Value,
                cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(
                "Employee not found.");
        }

        var document =
            await _documentRepository.GetByIdAndEmployeeIdAsync(
                request.DocumentId,
                request.EmployeeId,
                cancellationToken);

        if (document is null)
        {
            throw new NotFoundException(
                "Document not found.");
        }

        var storageName = document.StorageName;

        await _documentRepository.DeleteAsync(
            document,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        await _fileStorageService.DeleteAsync(
            storageName,
            cancellationToken);
    }
}