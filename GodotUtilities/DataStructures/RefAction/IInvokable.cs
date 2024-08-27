namespace GodotUtilities.DataStructures.RefAction;

public interface IInvokable
{
    void Invoke();
}
public interface IInvokable<T> 
{
    void Invoke(T t);
}