using FluentValidation;
using Medgrupo.Business.Dtos;

namespace Medgrupo.Business.Validators;

public class ContactInputValidator : AbstractValidator<IContactInput>
{
    public ContactInputValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do contato é obrigatório.");

        RuleFor(x => x.BirthDate)
            .Must(d => d.Date <= DateTime.Today)
                .WithMessage("A data de nascimento não pode ser maior que hoje.")
            .Must(d => CalculateAge(d) != 0)
                .WithMessage("A idade não pode ser igual a 0.")
            .Must(d => CalculateAge(d) >= 18)
                .WithMessage("O contato deve ser maior de idade.");
    }

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}
