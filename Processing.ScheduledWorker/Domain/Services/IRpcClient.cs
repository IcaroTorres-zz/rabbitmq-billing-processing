namespace Processing.ScheduledWorker.Domain.Services
{
    public interface IRpcClient<T>
    {
        T CallProcedure<P>(P payload);
    }
}
