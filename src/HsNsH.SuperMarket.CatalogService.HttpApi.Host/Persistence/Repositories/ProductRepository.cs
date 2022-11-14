using System.Linq.Expressions;
using HsNsH.SuperMarket.CatalogService.Domain.Models;
using HsNsH.SuperMarket.CatalogService.Domain.Repositories;
using HsNsH.SuperMarket.CatalogService.Persistence.Contexts;

namespace HsNsH.SuperMarket.CatalogService.Persistence.Repositories;

public class ProductRepository : GenericRepository<CatalogServiceDbContext, Product>, IProductRepository
{
    public ProductRepository(CatalogServiceDbContext dbContext) : base(dbContext)
    {
        DefaultPropertySelector = new List<Expression<Func<Product, object>>> { x => x.Category };
    }
}