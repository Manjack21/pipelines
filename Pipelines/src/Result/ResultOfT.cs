
namespace Pipelines;

public abstract class Result<T> : Result
{
}


public class Success<T> : Result<T>
{
    public Success(T value)
    {
        this.Value = value;
    }
    
    public T Value { get; }
}


