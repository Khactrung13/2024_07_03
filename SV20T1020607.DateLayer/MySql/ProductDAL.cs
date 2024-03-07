using System;
using Dapper;
using System.Data;
using SV20T1020607.DomainModels;

namespace SV20T1020607.DataLayer.MySql
{
    public class ProductDAL : BaseDAL, IProductDAL
    {
        public ProductDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Product data)
        {
            throw new NotImplementedException();
        }

        public long AddAttribute(ProductAttribute data)
        {
            throw new NotImplementedException();
        }

        public long AddPhoto(ProductPhoto data)
        {
            throw new NotImplementedException();
        }

        public int Count(string searchValue = "", int CategoryID = 0, int SupplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            throw new NotImplementedException();
        }

        public bool Delele(int ProductID)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAttrbute(long AttributeID)
        {
            throw new NotImplementedException();
        }

        public bool DeletePhoTo(long PhotoID)
        {
            throw new NotImplementedException();
        }

        public Product Get(int ProductID)
        {
            throw new NotImplementedException();
        }

        public ProductAttribute? GetAttribute(long AttributeID)
        {
            throw new NotImplementedException();
        }

        public ProductPhoto? GetPhoto(long PhotoID)
        {
            throw new NotImplementedException();
        }

        public bool IsUsed(int ProductID)
        {
            throw new NotImplementedException();
        }

        public IList<Product> List(int page = 1, int pageSize = 0, string searchValue = " ", int CategoryID = 0,
            int SupplierID = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            List<Product> list = new List<Product>();

            if (!string.IsNullOrEmpty(searchValue))
            {
                searchValue = "%" + searchValue + "%";
            }
            using (var connection = OpenConnection())
            {
                var sql = @"with cte as(
                            select  *,
                                    row_number() over(order by ProductName) as RowNumber
                            from    Products
                            where   (@SearchValue = N'' or ProductName like @SearchValue)
                                and (@CategoryID = 0 or CategoryID = @CategoryID)
                                and (@SupplierID = 0 or SupplierId = @SupplierID)
                                and (Price >= @MinPrice)
                                and (@MaxPrice <= 0 or Price <= @MaxPrice)
                        )
                        select * from cte
                        where   (@PageSize = 0)
                            or (RowNumber between (@Page - 1)*@PageSize + 1 and @Page * @PageSize)";
                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = searchValue ?? "",
                    CategoryID = CategoryID,
                    SupplierID= SupplierID,
                    minPrice= minPrice,
                    maxPrice= maxPrice
                };
                list = connection.Query<Product>(sql, parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }

        public IList<ProductAttribute> ListAttributes(int ProductID)
        {
            throw new NotImplementedException();
        }

        public IList<ProductPhoto> ListPhotos(int ProductID)
        {
            throw new NotImplementedException();
        }

        public bool Update(Product data)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAttribute(ProductAttribute data)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePhoto(ProductPhoto data)
        {
            throw new NotImplementedException();
        }
    }
}

