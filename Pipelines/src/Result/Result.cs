
namespace Pipelines;

public abstract class Result
{
    public Result EnsureType<T>()
    {
        if( this is Result<T>) 
            return new Success();
        else
            return new Error(new ResultTypeMismatch(typeof(Result<T>), this.GetType()));
    }

    public T? Unwrap<T>()
    {
        if(this is Result<T> success)
            return success.Value;
        
        return default(T);
    }

    public bool IsSuccess {
        get
        {
            return this is Success || 
                   (this.GetType().IsGenericType && this.GetType().GetGenericTypeDefinition() == typeof(Success<>));
        }        
    }
}

public class Success : Result
{
}

public class Error : Result
{
    public Error(Exception ex)
    {        
        this.Exception = ex;
    }

    public Exception Exception { get; }
}


