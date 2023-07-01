
namespace Pipelines;

public class Pipeline
{

    private List<Func<Result, Task<Result>>> steps = new List<Func<Result, Task<Result>>>();
    
    public Pipeline First(Func<Task<Result>> workload)
    {
        steps.Add(async (_) => await workload());
        return this;
    }
    
    public Pipeline First(Func<Result> workload)
    {
        steps.Add(async (_) => await Task.Run<Result>(workload));
        return this;
    }
    
    
    public Pipeline Then<TInput>(Func<TInput, Task<Result>> workload)
    {
        steps.Add(UnwrapResult<TInput>(workload));
        return this;
    }
    
    public Pipeline Then<TInput>(Func<TInput, Result> workload)
    {
        steps.Add(UnwrapResult<TInput>(async (prev) => await Task.Run<Result>(() => workload(prev))));
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
            
            if(current is Error) break;
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
