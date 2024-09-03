using Godot;
using GodotUtilities.DataStructures;
using GodotUtilities.Serialization;
using GodotUtilities.Serialization.Depot;

namespace GodotUtilities.GameData;

public class Data(IdDispenser idDispenser, Entities entities, 
    Models models, Serializer serializer,
    Dictionary<Type, object> singletons,
    RandomNumberGenerator random)
{
    public IdDispenser IdDispenser { get; private set; } = idDispenser;
    public Entities Entities { get; private set; } = entities;
    public Models Models { get; private set; } = models;
    public ModelIdRegister ModelIdRegister => (ModelIdRegister)Singletons[typeof(ModelIdRegister)];
    public Dictionary<Type, object> Singletons { get; private set; } = singletons;
    public Serializer Serializer { get; private set; } = serializer;
    public RandomNumberGenerator Random { get; private set; } = random;

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
    public static void SetupForLoad(Data d, string path, ModelImporter modelImporter)
    {
        var saveFile = Loader<SaveFile>.Load(path, d.Serializer);
        d.IdDispenser = saveFile.IdDispenser;
        d.Entities = saveFile.Entities;
        modelImporter.Import(d.Models);
    }

    public void SetEntitySingleton<T>() where T : Entity
    {
        var singleton = Entities.GetAll<T>().Single();
        SetSingleton<T>(singleton);
    }
    public void SetSingleton<T>(T t)
    {
        var type = typeof(T);
        if (t is null) throw new Exception();
        if (Singletons.TryGetValue(type, out var singleton)) throw new Exception();
        Singletons.Add(type, t);
    }
    
}