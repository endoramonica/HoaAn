using VietCommerce.Core.Entities.Products;

public static class ProductExtensions
{
    public static decimal GetActivePrice(this Product product)
    {
        var now = DateTime.UtcNow;
        return product.Prices?
            .Where(p => p.IsActive
                && p.EffectiveFrom <= now
                && (p.EffectiveTo == null || p.EffectiveTo >= now))
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => p.Price)
            .FirstOrDefault() ?? 0;
    }

    public static string GetMainImageUrl(this Product product)
    {
        return product.Images?
            .OrderByDescending(i => i.IsMain)
            .ThenBy(i => i.CreatedAt)
            .Select(i => i.Url)
            .FirstOrDefault() ?? string.Empty;
    }
}
