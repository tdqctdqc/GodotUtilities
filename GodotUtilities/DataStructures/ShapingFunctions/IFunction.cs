
namespace GodotUtilities.DataStructures.ShapingFunctions;

public interface IFunction<TArg, TResult>
{
    TResult Calc(TArg t);
}