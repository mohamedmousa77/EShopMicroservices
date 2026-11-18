
namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, string Description, string ImageFile, decimal Price, List<string> Category)
    : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Product description is required.");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Product category is required.");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Product image file is required.");
    }
}

internal class CreateProductCommandHandler
    (IDocumentSession session, IValidator<CreateProductCommand> validator)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Validate the command
        var reuslt = await validator.ValidateAsync(command, cancellationToken);
        var errors = reuslt.Errors.Select(x => x.ErrorMessage).ToList();
        if(errors.Any())
        {
            throw new ValidationException(errors.FirstOrDefault());
        }


        var product = new Product
        {
            Name = command.Name,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price,
            Category = command.Category
        };

        // Save to Db
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        // Return the result
        return new CreateProductResult(product.Id);

    }
}
