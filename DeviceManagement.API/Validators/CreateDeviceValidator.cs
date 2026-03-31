using DeviceManagement.Core.DTOs.Device;
using FluentValidation;

namespace DeviceManagement.API.Validators;

public class CreateDeviceValidator : AbstractValidator<DeviceRequestDto>
{
    private static readonly string[] AllowedTypes = ["Phone", "Tablet"];

    public CreateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(150).WithMessage("Name cannot exceed 150 characters.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.")
            .MaximumLength(100).WithMessage("Manufacturer cannot exceed 100 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(t => AllowedTypes.Contains(t))
            .WithMessage("Type must be either 'Phone' or 'Tablet'.");

        RuleFor(x => x.OperatingSystem)
            .NotEmpty().WithMessage("Operating System is required.");

        RuleFor(x => x.OsVersion)
            .NotEmpty().WithMessage("OS Version is required.");

        RuleFor(x => x.Processor)
            .NotEmpty().WithMessage("Processor is required.");

        RuleFor(x => x.RamAmount)
            .GreaterThan(0).WithMessage("RAM must be greater than 0 GB.")
            .LessThanOrEqualTo(512).WithMessage("RAM value seems unrealistic.");
    }
}