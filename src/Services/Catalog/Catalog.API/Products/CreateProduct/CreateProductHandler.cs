using FluentValidation;

namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price) : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(_ => _.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(_ => _.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(_ => _.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(_ => _.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}

internal class CreateProductCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult> 
{
    public async  Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Business logic to create a product
        // 1. Create Product entity from command object
        var product = new Product
        {
            Name = command.Name,
            Category = command.Category,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price,
        };

        // 2. Save to database
        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        // 3. Return a CreateProductResult result
        return new CreateProductResult(product.Id);
    }
}

