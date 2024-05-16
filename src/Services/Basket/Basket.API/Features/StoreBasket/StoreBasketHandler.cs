namespace Basket.API.Features.StoreBasket;

public record StoreBasketCommand(ShoppingCart ShoppingCart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(command => command.ShoppingCart).NotNull().WithMessage("Cart can not be null");
        RuleFor(command => command.ShoppingCart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketCommandHandler : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart shoppingCart = command.ShoppingCart;

        //TODO: store basket in database (use Marten upsert - if exist = update, if not = insert)
        //TODO: update cache

        return new StoreBasketResult("swn");
    }
}
