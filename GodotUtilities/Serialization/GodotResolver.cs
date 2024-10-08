using Godot;
using MessagePack;
using MessagePack.Formatters;

namespace GodotUtilities.Serialization;

public class GodotCustomResolver : IFormatterResolver
{
    // Resolver should be singleton.
    public static readonly IFormatterResolver Instance = new GodotCustomResolver();

    private GodotCustomResolver()
    {
    }

    // GetFormatter<T>'s get cost should be minimized so use type cache.
    public IMessagePackFormatter<T> GetFormatter<T>()
    {
        return FormatterCache<T>.Formatter;
    }

    private static class FormatterCache<T>
    {
        public static readonly IMessagePackFormatter<T> Formatter;

        // generic's static constructor should be minimized for reduce type generation size!
        // use outer helper method.
        static FormatterCache()
        {
            Formatter = (IMessagePackFormatter<T>)GodotResolverGetFormatterHelper.GetFormatter(typeof(T));
        }
    }
}

internal static class GodotResolverGetFormatterHelper
{
    // If type is concrete type, use type-formatter map
    static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>()
    {
        {typeof(Vector2), new Vector2Formatter()},
        {typeof(Color), new ColorFormatter()}
    };

    static Dictionary<Type, object> GetMap()
    {
        var map = new Dictionary<Type, object>()
        {
            { typeof(Vector2), new Vector2Formatter() },
            { typeof(Color), new ColorFormatter() }
        };
        // var polymorphTypes = Assembly.GetExecutingAssembly()
        //     .GetConcreteTypesOfType<Polymorph>();
        //
        // foreach (var polymorphType in polymorphTypes)
        // {
        //     var formatter = typeof(PolymorphFormatter<>)
        //         .MakeGenericType(polymorphType)
        //         .GetMethod(nameof(PolymorphFormatter<Polymorph>.Construct), BindingFlags.Static | BindingFlags.Public)
        //         .Invoke(null, new object[0]);
        //     map.Add(polymorphType, formatter);
        // }
        return map;
    }
    internal static object GetFormatter(Type t)
    {
        object formatter;
        if (formatterMap.TryGetValue(t, out formatter))
        {
            return formatter;
        }

        // If type can not get, must return null for fallback mechanism.
        return null;
    }
}