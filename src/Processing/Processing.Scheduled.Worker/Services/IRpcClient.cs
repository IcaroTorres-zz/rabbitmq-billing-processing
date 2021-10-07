namespace Processing.Scheduled.Worker.Services
{
    public interface IRpcClient<T>
    {
        T CallProcedure(object payload);
    }
}
