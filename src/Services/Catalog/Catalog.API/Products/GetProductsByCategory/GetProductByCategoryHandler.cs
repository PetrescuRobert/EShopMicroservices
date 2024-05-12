
namespace Catalog.API.Products.GetProductsByCategory;

public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
public record GetProductsByCategoryResult(IEnumerable<Product> Products);
internal class GetProductsByCategoryQueryHandler(IDocumentSession documentSession, ILogger<GetProductsByCategoryQueryHandler> logger)
    : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProductsByCategoryQueryHandler.Handle called with {@Query}", query);

        var products = await documentSession.Query<Product>()
            .Where(product => product.Category.Contains(query.Category))
            .ToListAsync();

        return new GetProductsByCategoryResult(products);

    }
}




