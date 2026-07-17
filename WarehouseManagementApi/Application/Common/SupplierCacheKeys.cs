namespace Application.Common;

public class SupplierCacheKeys
{
    public static string ById(string id)  => $"supplier:{id}";

    public const string SuppliersList = "suppliers:list";
}