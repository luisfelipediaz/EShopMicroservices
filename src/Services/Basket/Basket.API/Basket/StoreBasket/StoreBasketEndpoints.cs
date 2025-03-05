namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);

public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<StoreBasketCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/basket/{result.UserName}", response);
            })
            .WithName("StoreBasket")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
            .WithSummary("Store basket for user")
            .WithDescription("Store basket for user");
    }
}