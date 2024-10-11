using Godot;
using GodotUtilities.DataStructures;
using GodotUtilities.Serialization;
using GodotUtilities.Serialization.Depot;
using MessagePack;

namespace GodotUtilities.GameData;

public class Data
{
    [SerializationConstructor] protected Data(IdDispenser idDispenser, Entities entities, 
        Models models, Serializer serializer,
        Dictionary<Type, object> singletons,
        RandomNumberGenerator random)
    {
        IdDispenser = idDispenser;
        Entities = entities;
        Models = models;
        Singletons = singletons;
        Serializer = serializer;
        Random = random;
    }

    public IdDispenser IdDispenser { get; private set; }
    public Entities Entities { get; private set; }
    public Models Models { get; private set; }
    public ModelIdRegister ModelIdRegister => (ModelIdRegister)Singletons[typeof(ModelIdRegister)];
    public Dictionary<Type, object> Singletons { get; private set; }
    public Serializer Serializer { get; private set; }
    public RandomNumberGenerator Random { get; private set; }
    public Logger.Logger Logger { get; protected set; }

    public static void SetupForHost(Data d, ModelImporter modelImporter)
    {
        modelImporter.Import(d.Models);
        var register = new ModelIdRegister(d.IdDispenser.TakeId(),
            new Dictionary<int, string>(),
            new Dictionary<string, int>());
        d.Entities.AddEntity(register, d);
        
        foreach (var (name, model) in d.Models.ModelsByName)
        {
            register.Register(model, d);
        }
    }
    public static void SetupForRemote(Data d, ModelImporter modelImporter)
    {
        modelImporter.Import(d.Models);
    }
    public static void SetupForLoad(Data d, ModelImporter modelImporter)
    {
        modelImporter.Import(d.Models);
    }

    
    public void SetSingleton(Entity t)
    {
        var type = t.GetType();
        if (t is null) throw new Exception();
        if (Singletons.TryGetValue(type, out var singleton)) throw new Exception();
        Singletons.Add(type, t);
    }

    public T GetSingleton<T>()
    {
        return (T)Singletons[typeof(T)];
    }
}