namespace GodotUtilities.GameData;

public static class EntityExt
{
    public static ERef<T> MakeRef<T>(this T entity)
        where T : Entity
    {
        return new ERef<T>(entity.Id);
    }
}