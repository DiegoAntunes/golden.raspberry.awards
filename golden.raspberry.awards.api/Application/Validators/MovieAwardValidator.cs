using FluentValidation;
using golden.raspberry.awards.api.Domain.Entities;

namespace golden.raspberry.awards.api.Application.Validators
{
    public class MovieAwardValidator : AbstractValidator<MovieAwardNomination>
    {
        public MovieAwardValidator()
        {
            RuleFor(c => c.Year)
                .NotEmpty().WithMessage("Please enter the year.")
                .NotNull().WithMessage("Please enter the year.");

            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Please enter the title.")
                .NotNull().WithMessage("Please enter the title.");

            RuleFor(c => c.Studio)
                .NotEmpty().WithMessage("Please enter the studio.")
                .NotNull().WithMessage("Please enter the studio.");

            RuleFor(c => c.Producer)
                .NotEmpty().WithMessage("Please enter the producer.")
                .NotNull().WithMessage("Please enter the producer.");
        }
    }
}
