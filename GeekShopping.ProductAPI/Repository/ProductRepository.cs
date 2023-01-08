using AutoMapper;
using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Model;
using GeekShopping.ProductAPI.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {

        public readonly MySQLContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(
            MySQLContext dbContext,
            IMapper mapper
        )
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


        public async Task<IEnumerable<ProductVO>> FindAll()
        {
            var products = await _dbContext.Products.ToListAsync();
            var productsVO = _mapper.Map<List<ProductVO>>(products);
            return productsVO;
        }

        public async Task<ProductVO> FindById(long id)
        {
            var product = await _dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            var productVO = _mapper.Map<ProductVO>(product);
            return productVO;
        }

        public async Task<ProductVO> Create(ProductVO vo)
        {
            var product = _mapper.Map<Product>(vo);
            _dbContext.Add(product);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ProductVO>(product);
        }

        public async Task<ProductVO> Update(ProductVO vo)
        {
            var product = _mapper.Map<Product>(vo);
            _dbContext.Update(product);
            await _dbContext.SaveChangesAsync();
            return _mapper.Map<ProductVO>(product);
        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                var product = await _dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
                if (product == null) return false;
                _dbContext.Remove(product);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



    }
}
