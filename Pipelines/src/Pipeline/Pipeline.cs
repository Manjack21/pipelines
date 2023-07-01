
namespace Pipelines;

public class Pipeline
{

    private List<Func<Result, Task<Result>>> steps = new List<Func<Result, Task<Result>>>();
    private Func<Error, Task<Result>>? errorHandler = null;


    public Pipeline First(Func<Task<Result>> workload)
    {
        steps.Add(async (_) => await workload());
        return this;
    }
    
    public Pipeline First(Func<Result> workload)
    {
        steps.Add((_) => Task.FromResult<Result>(workload()));
        return this;
    }
    
    
    public Pipeline Then<TInput>(Func<TInput, Task<Result>> workload)
    {
        steps.Add(UnwrapResult<TInput>(workload));
        return this;
    }
    
    public Pipeline Then<TInput>(Func<TInput, Result> workload)
    {
        steps.Add(UnwrapResult<TInput>((prev) => Task.FromResult<Result>(workload(prev))));
        return this;
    }

    public Pipeline HandleError(Func<Error, Result> errorHandler)
    {
        this.errorHandler = (error) => Task.FromResult(errorHandler(error));
        return this;
    }

    public Pipeline HandleError(Func<Error, Task<Result>> errorHandler)
    {
        this.errorHandler = errorHandler;
        return this;
    }

    public async Task<Result> InvokeAsync(CancellationToken cToken = default)
    {
        Result current = new Success();
        foreach(var step in this.steps)
        {
            try{
                current = await step(current);
            }
            catch(Exception ex)
            {
                current = new Error(ex);
            }

            if(current is Error error) {
                if(errorHandler != null) current = await this.errorHandler(error);

                if(!current.IsSuccess)
                    break;
            }
        }

        return current;
    }
    
    private Func<Result, Task<Result>> UnwrapResult<T>(Func<T, Task<Result>> workload)
    {
        return async (Result previous) => {
            if(previous.EnsureType<T>() is Error error) return error; 
            T prevValue = previous.Unwrap<T>()!; 

            return await workload(prevValue);
        };
    }

    
}
