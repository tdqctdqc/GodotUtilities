namespace GodotUtilities.GameData;

public interface IInheritedModelComponent
    : IModelComponent
{
    void InheritTo(IComponentedEntity entity, Data data);
}