namespace Library.Messaging
{
    public interface IRpcClient<T>
    {
        T CallProcedure(object payload);
    }
}
