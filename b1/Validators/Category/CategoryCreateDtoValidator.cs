using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace b1.Validators.Category
{
    public class CategoryCreateDtoValidator: AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator()
        {
            RuleFor(x => x.NameCategory)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }
}
