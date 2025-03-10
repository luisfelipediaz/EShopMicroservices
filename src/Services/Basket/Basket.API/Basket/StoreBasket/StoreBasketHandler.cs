using Discount.Grpc;

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

public class StoreBasketCommandHandler(
    IBasketRepository repository,
    DiscountProtoService.DiscountProtoServiceClient discountProto)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        await DeductDiscount(command, cancellationToken);

        var cart = command.Cart;
        await repository.StoreBasketAsync(cart, cancellationToken);

        return new StoreBasketResult(cart.UserName);
    }

    private async Task DeductDiscount(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        foreach (var basketItem in command.Cart.Items)
        {
            var coupon = await discountProto.GetDiscountAsync(
                new GetDiscountRequest { ProductName = basketItem.ProductName }, cancellationToken: cancellationToken);
            basketItem.Price -= coupon.Amount;
        }
    }
}