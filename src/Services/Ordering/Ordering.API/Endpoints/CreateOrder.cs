
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.API.Endpoints;
public record CreateOrderRequest(OrderDto Order);
public record CreateOrderResponse(Guid Id);
public class CreateOrder : ICarterModule
{
    // Accepts a CreateOrderRequest object
    // Map the request to a CreateOrderCommand
    // Use MediatR to send the command to the corresponding handler.
    // Returns a response with the created order's ID.
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/orders", async (CreateOrderRequest request, ISender sender) =>
        {
            var comand = request.Adapt<CreateOrderCommand>();

            var result = await sender.Send(comand);

            var response = result.Adapt<CreateOrderResponse>();

            return Results.Created($"/orders/{response.Id}", response);
        })
            .WithName("CreateOrder")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Order")
            .WithDescription("Create Order");
    }
}
