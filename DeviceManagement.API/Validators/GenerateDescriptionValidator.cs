using DeviceManagement.Core.DTOs.Device;
using FluentValidation;

namespace DeviceManagement.API.Validators;

public class GenerateDescriptionValidator : AbstractValidator<GenerateDescriptionDto>
{
    public GenerateDescriptionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.");

        RuleFor(x => x.OperatingSystem)
            .NotEmpty().WithMessage("Operating System is required.");

        RuleFor(x => x.Processor)
            .NotEmpty().WithMessage("Processor is required.");

        RuleFor(x => x.RamAmount)
            .GreaterThan(0).WithMessage("RAM must be greater than 0.");
    }
}