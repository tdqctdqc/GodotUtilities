namespace GodotUtilities.GameData;

public static class ModelExt
{
    public static ModelRef<T> MakeNameRef<T>(this T t) where T : Model
    {
        return new ModelRef<T>(t.Name);
    }
    public static ModelIdRef<T> MakeIdRef<T>(this T t, Data d) where T : Model
    {
        var id = d.ModelIdRegister.IdsByName[t.Name];
        return new ModelIdRef<T>(id);
    }
}