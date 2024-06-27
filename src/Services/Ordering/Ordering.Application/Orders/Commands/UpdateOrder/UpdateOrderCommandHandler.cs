namespace Ordering.Application.Orders.Commands.UpdateOrder;

public class UpdateOrderCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderCommandResult>
{
    public async Task<UpdateOrderCommandResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        //update order entity from command object
        var orderId = OrderId.Of(command.Order.Id);

        var order = await dbContext.Orders.FindAsync([orderId], cancellationToken) ?? throw new OrderNotFoundException(command.Order.Id.ToString());

        UpdateOrder(order, command.Order);
        //save to db
        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        //return result
        return new UpdateOrderCommandResult(true);
    }

    private void UpdateOrder(Order order, OrderDto orderDto)
    {
        var updatedShippingAddress = Address.Of(orderDto.ShippingAddress.FirstName, orderDto.ShippingAddress.LastName, orderDto.ShippingAddress.EmailAddress, orderDto.ShippingAddress.AddressLine, orderDto.ShippingAddress.Country, orderDto.ShippingAddress.State, orderDto.ShippingAddress.ZipCode);
        var updatedBillingAddress = Address.Of(orderDto.BillingAddress.FirstName, orderDto.BillingAddress.LastName, orderDto.BillingAddress.EmailAddress, orderDto.BillingAddress.AddressLine, orderDto.BillingAddress.Country, orderDto.BillingAddress.State, orderDto.BillingAddress.ZipCode);
        var updatedPayment = Payment.Of(orderDto.Payment.CardName, orderDto.Payment.CardNumber, orderDto.Payment.Expiration, orderDto.Payment.Cvv, orderDto.Payment.PaymentMethod);

        order.Update(
            orderName: OrderName.Of(orderDto.OrderName),
            shippingAddress: updatedShippingAddress,
            billingAddress: updatedBillingAddress,
            payment: updatedPayment,
            orderStatus: orderDto.Status);
    }
}

