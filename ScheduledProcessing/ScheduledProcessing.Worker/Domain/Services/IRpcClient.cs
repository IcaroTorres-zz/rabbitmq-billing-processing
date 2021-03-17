namespace ScheduledProcessing.Worker.Domain.Services
{
    public interface IRpcClient<T>
    {
        T CallProcedure<P>(P payload);
    }
}
