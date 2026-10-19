using System.Globalization;
using IFP.Assignment5;
using static IFP.Assignment5.Checkout;
using static IFP.Assignment5.Functional;

CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

if (args.Length == 1 && args[0] == "--test")
    return SelfTests.Run();

if (args.Length != 0)
{
    Console.Error.WriteLine("Usage: dotnet run -- [--test]");
    return 2;
}

const decimal amount = 1234.56m;

Func<decimal, decimal> discount10 = Curry<decimal, decimal, decimal>(ApplyDiscount)(0.10m);
Func<decimal, decimal> fee5 = Curry<decimal, decimal, decimal>(AddFee)(5m);

Func<decimal, decimal> checkout = Compose<decimal, decimal, decimal>(
    Compose(discount10, fee5), RoundToCents);

decimal pipedPrice = amount.Pipe(discount10).Pipe(fee5).Pipe(RoundToCents);

var standard = Pipeline(discount10, fee5, RoundToCents);
var feeBeforeDiscount = Pipeline(fee5, discount10, RoundToCents);
var roundingFirst = Pipeline(RoundToCents, discount10, fee5);
var empty = Pipeline();

Console.WriteLine("Assignment 5 - Composing Functions Using Pipelining");
Console.WriteLine("Alimzhan Almukhambetov | IT-2510");
Console.WriteLine($"Input amount: {amount}");
Console.WriteLine($"Nested original: {RoundToCents(AddFee(5m, ApplyDiscount(0.10m, amount)))}");
Console.WriteLine($"Compose checkout: {checkout(amount)}");
Console.WriteLine($"Pipe calculation: {pipedPrice}");
Console.WriteLine($"Pipeline discount -> fee -> round: {standard(amount)}");
Console.WriteLine($"Pipeline fee -> discount -> round: {feeBeforeDiscount(amount)}");
Console.WriteLine($"Pipeline round -> discount -> fee: {roundingFirst(amount)}");
Console.WriteLine($"Pipeline() identity: {empty(amount)}");
Console.WriteLine($"Reused checkout with 100: {checkout(100m)}");
return 0;
