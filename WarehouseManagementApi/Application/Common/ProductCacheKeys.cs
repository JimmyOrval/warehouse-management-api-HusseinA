namespace Application.Common;

public static class ProductCacheKeys
{
    public static string ById(string id)  => $"product:{id}";
    
    public static string List(bool? onlyAvailable)  => $"products:list:{onlyAvailable}";

    // added this to simplify invalidations
    public static readonly string[] ListVariations =
        [
            List(true),
            List(false),
            List(null)
        ];
}