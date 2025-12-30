namespace Maliev.CompensationService.Domain.Events;

/// <summary>
/// Event received when a new employee is created in the Employee Service
/// </summary>
/// <param name="EmployeeId">Unique identifier of the employee</param>
/// <param name="EmployeeNumber">Unique employee number</param>
/// <param name="StartDate">The employee's start date</param>
/// <param name="DepartmentId">Identifier of the department the employee belongs to</param>
/// <param name="PositionId">Optional identifier of the position</param>
/// <param name="ManagerId">Optional identifier of the manager</param>
/// <param name="Timestamp">When the event occurred</param>
public record EmployeeCreatedEvent(
    Guid EmployeeId,
    string EmployeeNumber,
    DateTime StartDate,
    Guid DepartmentId,
    Guid? PositionId,
    Guid? ManagerId,
    DateTime Timestamp);
