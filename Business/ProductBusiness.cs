using DataAccess;

namespace Business
{
    public class ProductBusiness
    {
        private readonly ProductDataAccess _productDataAccess;

        public ProductBusiness(ProductDataAccess productDataAccess)
        {
            _productDataAccess = productDataAccess;
        }

        public async Task<List<Product>> GetProducts(int pageNumber, int pageSize)
        {
            return await _productDataAccess.GetProductsAsync(pageNumber, pageSize);
        }

        public async Task<bool> UpdateProduct(int id, string newName, decimal newPrice, byte[] rowVersion)
        {
            var product = await _productDataAccess.GetByIdAsync(id);
            if (product is null)
                return false;

            product.Name = newName;
            product.Price = newPrice;

            await _productDataAccess.UpdateProductAsync(product, rowVersion);
            return true;
        }
    }
}