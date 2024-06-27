using FluentValidation;

namespace Ordering.Application.Orders.Commands.UpdateOrder;
public record UpdateOrderCommand(OrderDto Order) : ICommand<UpdateOrderCommandResult>;
public record UpdateOrderCommandResult(bool IsSusscess);

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Order.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("Name is required|");
        RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerIs is required");
    }
}
