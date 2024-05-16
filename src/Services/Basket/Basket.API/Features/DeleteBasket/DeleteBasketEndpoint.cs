
namespace Basket.API.Features.DeleteBasket;

public record DeleteBasketRequest(string UserName);
public record DeleteBasketResponse(bool IsSuccesfull);

public class DeleteBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/", async (string request, ISender sender) =>
        {
            DeleteBasketCommand command = request.Adapt<DeleteBasketCommand>();

            DeleteBasketResult result = await sender.Send(command);

            DeleteBasketResponse response = result.Adapt<DeleteBasketResponse>();

            return Results.Ok(response);
        })
            .WithName("Delete Basket")
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Basket")
            .WithDescription("Delete Basket");
    }
}
