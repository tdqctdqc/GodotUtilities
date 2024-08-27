using MessagePack;
using MessagePack.Resolvers;

namespace GodotUtilities.Serialization;

public class Serializer
{
    private MessagePackSerializerOptions _options;
    public Serializer()
    {
        var resolver = MessagePack.Resolvers.CompositeResolver.Create(
            // enable extension packages first
            GodotCustomResolver.Instance, 
            MessagePack.Resolvers.ContractlessStandardResolver.Instance,
            MessagePack.Resolvers.StandardResolver.Instance,
            // finally use standard (default) resolver
            StandardResolver.Instance,
            MessagePack.Resolvers.TypelessObjectResolver.Instance
        );
        _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
        
        // Pass options every time or set as default
        // MessagePackSerializer.DefaultOptions = _options;
    }
    public byte[] Serialize<T>(T t)
    {
        return MessagePackSerializer.Serialize(t.GetType(), t, _options);
    }
    public byte[] Serialize(object t, Type type)
    {
        return MessagePackSerializer.Serialize(type, t, _options);
    }
    public T Deserialize<T>(byte[] bytes)
    {
        return MessagePackSerializer.Deserialize<T>(bytes, _options);
    }
    public object Deserialize(byte[] bytes, Type type)
    {
        return MessagePackSerializer.Deserialize(type, bytes, _options);
    }
    
    
}

//supported types
