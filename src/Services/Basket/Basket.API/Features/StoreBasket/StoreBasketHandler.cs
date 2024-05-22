
using Discount.gRPC;

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

public class StoreBasketCommandHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart shoppingCart = command.ShoppingCart;

        await DeductDiscount(shoppingCart, cancellationToken);

        //TODO: store basket in database (use Marten upsert - if exist = update, if not = insert)
        //TODO: update cache
        await repository.StoreBasket(shoppingCart, cancellationToken);

        return new StoreBasketResult(shoppingCart.UserName);
    }

    private async Task DeductDiscount(ShoppingCart shoppingCart, CancellationToken cancellationToken)
    {
        // TODO: communicate with Discount.gRpc and calculate latest prices of products in the cart.
        foreach (var item in shoppingCart.Items)
        {
            var coupon = await discountProtoServiceClient.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);

            item.Price -= coupon.Amount;
        }
    }
}
