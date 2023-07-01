
namespace Pipelines;

public abstract class Result<T> : Result
{
    public Result(T value)
    {
        this.Value = value;
    }
    public T Value { get; }
}


public class Success<T> : Result<T>
{
    public Success(T value) : base(value)
    {
    }
}


public class Abort<T> : Result<T>
{
    public Abort(T value, Exception ex) : base(value)
    {
        this.Exception = ex;
    }
    
    public Exception Exception { get; }
}