using System.Globalization;
using static IFP.Assignment5.Checkout;
using static IFP.Assignment5.Functional;

namespace IFP.Assignment5;

internal static class SelfTests
{
    public static int Run()
    {
        var passed = 0;
        var failed = 0;

        void Equal<T>(string name, T expected, T actual)
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual))
            {
                passed++;
                Console.WriteLine($"PASS {name}: {actual}");
            }
            else
            {
                failed++;
                Console.Error.WriteLine($"FAIL {name}: expected {expected}, actual {actual}");
            }
        }

        Func<decimal, decimal> discount10 = Curry<decimal, decimal, decimal>(ApplyDiscount)(0.10m);
        Func<decimal, decimal> fee5 = Curry<decimal, decimal, decimal>(AddFee)(5m);
        var checkout = Compose<decimal, decimal, decimal>(Compose(discount10, fee5), RoundToCents);

        Equal("Curry supports different generic argument types", "item:7",
            Curry<string, int, string>((name, count) => $"{name}:{count}")("item")(7));
        Equal("Partial discount application", 1111.104m, discount10(1234.56m));
        Equal("Partial fee application", 1239.56m, fee5(1234.56m));
        Equal("Configured discount can be reused", 90m, discount10(100m));
        Equal("Compose executes f before g", 9,
            Compose<int, int, int>(x => x + 1, x => x * 3)(2));
        Equal("Compose supports different intermediate and result types", "length=4",
            Compose<string, int, string>(text => text.Length, length => $"length={length}")("shop"));
        Equal("Reusable checkout matches the required example", 1116.10m, checkout(1234.56m));
        Equal("Reusable checkout accepts another amount", 95m, checkout(100m));
        Equal("Pipe supports a changed result type", 4, "shop".Pipe(text => text.Length));
        Equal("Pipe chain matches checkout", 1116.10m,
            1234.56m.Pipe(discount10).Pipe(fee5).Pipe(RoundToCents));
        Equal("Standard pipeline", 1116.10m, Pipeline(discount10, fee5, RoundToCents)(1234.56m));
        Equal("Fee before discount", 1115.60m, Pipeline(fee5, discount10, RoundToCents)(1234.56m));
        Equal("Rounding first preserves later fractional cents", 1116.104m,
            Pipeline(RoundToCents, discount10, fee5)(1234.56m));
        Equal("Default output does not conceal fractional cents", "1116.1040",
            Pipeline(RoundToCents, discount10, fee5)(1234.56m).ToString(CultureInfo.InvariantCulture));
        Equal("Single-step pipeline", 15m, Pipeline(fee5)(10m));
        Equal("Pipeline() is identity for the required amount", 1234.56m, Pipeline()(1234.56m));
        Equal("Pipeline() is identity for zero", 0m, Pipeline()(0m));
        Equal("Pipeline() preserves negative high-precision input", -12.3456m, Pipeline()(-12.3456m));
        Equal("Positive midpoint rounds away from zero", 1.01m, RoundToCents(1.005m));
        Equal("Negative midpoint rounds away from zero", -1.01m, RoundToCents(-1.005m));

        Console.WriteLine($"Tests: {passed} passed, {failed} failed.");
        return failed == 0 ? 0 : 1;
    }
}
