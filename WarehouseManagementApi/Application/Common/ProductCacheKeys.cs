namespace Application.Common;

public static class ProductCacheKeys
{
    public static string ById(string id)  => $"product:{id}";

    public static readonly string[] ListVariations =
        ["products:list:True", "products:list:False", "products:list:"];
}