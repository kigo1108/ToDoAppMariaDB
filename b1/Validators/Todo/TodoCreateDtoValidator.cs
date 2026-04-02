using FluentValidation;
namespace b1.Validators.Todo
{
    public class TodoCreateDtoValidator : AbstractValidator<TodoCreateDto>
    {
        public TodoCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId must be greater than 0.");
        }
    }
}
