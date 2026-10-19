namespace IFP.Assignment5;

public static class Checkout
{
    public static decimal ApplyDiscount(decimal rate, decimal amount)
        => amount - amount * rate;

    public static decimal AddFee(decimal fee, decimal amount)
        => amount + fee;

    public static decimal RoundToCents(decimal amount)
        => Math.Round(amount, 2, MidpointRounding.AwayFromZero);
}
