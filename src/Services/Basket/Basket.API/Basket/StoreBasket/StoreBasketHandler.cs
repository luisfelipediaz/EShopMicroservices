namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBarketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBarketCommandValidator()
    {
        RuleFor(basket => basket.Cart).NotNull().WithMessage("Basket cannot be null");
        RuleFor(basket => basket.Cart.UserName).NotEmpty().WithMessage("Username is required");
    }
}

public class StoreBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        var cart = command.Cart;

        // TODO: Store the shopping cart in the database (use Marten upsert - if existe = update, if not = insert)
        // TODO: Update cache in redis
        await repository.StoreBasketAsync(cart, cancellationToken);

        return new StoreBasketResult(cart.UserName);
    }
}