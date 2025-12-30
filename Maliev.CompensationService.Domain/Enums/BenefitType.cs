namespace Maliev.CompensationService.Domain.Enums;

/// <summary>
/// Represents the category of benefit offered to employees
/// </summary>
public enum BenefitType
{
    /// <summary>
    /// Health insurance coverage
    /// </summary>
    HealthInsurance = 0,

    /// <summary>
    /// Dental insurance coverage
    /// </summary>
    DentalInsurance = 1,

    /// <summary>
    /// Vision insurance coverage
    /// </summary>
    VisionInsurance = 2,

    /// <summary>
    /// Life insurance coverage
    /// </summary>
    LifeInsurance = 3,

    /// <summary>
    /// 401(k) retirement plan
    /// </summary>
    Retirement401k = 4,

    /// <summary>
    /// Stock options or equity compensation
    /// </summary>
    StockOptions = 5,

    /// <summary>
    /// Paid time off (vacation, sick leave)
    /// </summary>
    PaidTimeOff = 6,

    /// <summary>
    /// Wellness programs and gym memberships
    /// </summary>
    WellnessProgram = 7,

    /// <summary>
    /// Education assistance and tuition reimbursement
    /// </summary>
    EducationAssistance = 8
}
