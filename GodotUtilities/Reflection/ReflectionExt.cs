using System.Reflection;

namespace GodotUtilities.Reflection;

public static class ReflectionExt 
{
    public static object? InvokeGeneric(this MethodInfo mi, 
        object ob, Type[] genericParams, object[] args)
    {
        var genericMi = mi.MakeGenericMethod(genericParams);
        return genericMi.Invoke(ob, args);
    }
    public static List<T> GetStaticPropertiesOfType<T>(this Type type)
    {
        return type.GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Where(p => typeof(T).IsAssignableFrom(p.PropertyType))
            .Select(p => (T)p.GetValue(null))
            .ToList();
    }
    public static Dictionary<string, T> GetPropertiesOfTypeByName<T>(this object instance)
    {
        return instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => typeof(T).IsAssignableFrom(p.PropertyType))
            .ToDictionary(p => p.Name, p => (T)p.GetValue(instance));
    }
    public static List<T> GetPropertiesOfType<T>(this object instance)
    {
        return instance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => typeof(T).IsAssignableFrom(p.PropertyType))
            .Select(p => (T)p.GetValue(instance))
            .ToList();
    }
    public static List<Type> GetTypesOfType<TAbstract>(this Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => typeof(TAbstract).IsAssignableFrom(t))
            .ToList();
    }
    public static List<Type> GetConcreteTypesOfType<TAbstract>(this Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(t => t.IsConcreteType()
                        && typeof(TAbstract).IsAssignableFrom(t))
            .ToList();
    }
    public static List<Type> GetConcreteTypesOfType(this Assembly assembly, Type abstractType)
    {
        return assembly.GetTypes()
            .Where(t => t.IsConcreteType() && abstractType.IsAssignableFrom(t)).ToList();
    }

    public static bool IsConcreteType(this Type t)
    {
        return t.IsInterface == false && t.IsAbstract == false;
    }
    public static List<Type> GetDirectlyDerivedTypes(this Type baseType, Type[] types)
    {
        return baseType.GetDerivedTypes(types).Where(t => t.BaseType == baseType).ToList();
    }
    public static List<Type> GetDerivedTypes(this Type baseType, 
        IEnumerable<Type> types)
    {
        // Get all types from the given assembly
        List<Type> derivedTypes = new List<Type>();
        foreach (var type in types)
        {
            if (IsSubclassOf(type, baseType))
            {
                // The current type is derived from the base type,
                // so add it to the list
                derivedTypes.Add(type);
            }
        }

        return derivedTypes;
    }

    public static bool IsSubclassOf(Type type, Type baseType)
    {
        if (type == null || baseType == null || type == baseType)
            return false;

        if (baseType.IsGenericType == false)
        {
            if (type.IsGenericType == false)
                return type.IsSubclassOf(baseType);
        }
        else
        {
            baseType = baseType.GetGenericTypeDefinition();
        }

        type = type.BaseType;
        Type objectType = typeof(object);

        while (type != objectType && type != null)
        {
            Type curentType = type.IsGenericType ?
                type.GetGenericTypeDefinition() : type;
            if (curentType == baseType)
                return true;

            type = type.BaseType;
        }

        return false;
    }
    public static bool HasAttribute<TAttribute>(this MemberInfo c) where TAttribute : Attribute
    {
        return c.GetCustomAttributesData().Any(d => d.AttributeType == typeof(TAttribute));
    }
    public static T MakeInstanceMethodDelegate<T>(this MethodInfo m) where T : Delegate
    {
        return (T)Delegate.CreateDelegate(typeof(T), null, m);
    }
}