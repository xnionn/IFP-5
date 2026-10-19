namespace IFP.Assignment5;

public static class Functional
{
    public static Func<A, Func<B, C>> Curry<A, B, C>(Func<A, B, C> function)
        => first => second => function(first, second);

    public static Func<A, C> Compose<A, B, C>(Func<A, B> f, Func<B, C> g)
        => value => g(f(value));

    public static R Pipe<T, R>(this T value, Func<T, R> function)
        => function(value);

    public static Func<decimal, decimal> Pipeline(params Func<decimal, decimal>[] steps)
        => steps.Aggregate(
            (Func<decimal, decimal>)(amount => amount),
            (pipeline, step) => Compose(pipeline, step));
}
