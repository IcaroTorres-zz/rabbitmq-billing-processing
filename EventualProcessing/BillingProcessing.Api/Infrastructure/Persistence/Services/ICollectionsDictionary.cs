namespace BillingProcessing.Api.Infrastructure.Persistence.Services
{
    public interface ICollectionsDictionary
    {
        string GetCollectionName(string entityName);
    }
}
