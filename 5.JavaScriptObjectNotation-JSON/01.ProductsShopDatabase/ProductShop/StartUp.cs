using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            //Database
            ProductShopContext context = new ProductShopContext();

            string inputJson = string.Empty;
            string output = string.Empty;

            //01.Import Users
            inputJson = File.ReadAllText(@"../../../Datasets/users.json");
            output = ImportUsers(context, inputJson);

            //02.Import Products
            //inputJson = File.ReadAllText(@"../../../Datasets/products.json");
            //output = ImportProducts(context, inputJson);

            //03.Import Categories
            //inputJson = File.ReadAllText(@"../../../Datasets/categories.json");
            //output = ImportCategories(context, inputJson);

            //O4.Import Categories and Products
            //inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //output = ImportCategoryProducts(context, inputJson);

            //05.Export Products in Range
            //output = GetProductsInRange(context);

            //06.Export Sold Products
            //output = GetSoldProducts(context);

            //07.Export Categories By Products Count
            //output = GetCategoriesByProductsCount(context);

            //08.Export Users and Products
            //output = GetUsersWithProducts(context);


            Console.WriteLine(output);
        }

        // 1.Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";

        }
        // 2.Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            if (products != null)
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            return $"Successfully imported {products?.Length}";
        }

        // 3.Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categoriesAll = JsonConvert.DeserializeObject<Category[]>(inputJson);

            var validCategories = categoriesAll
                .Where(c => c.Name != null)
                .ToArray();

            if (validCategories != null)
            {
                context.Categories.AddRange(validCategories);
                context.SaveChanges();

            }

            return $"Successfully imported {validCategories?.Length}";
        }

        // 4.Import Categories and Product
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Length}";

        }

        //5. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .OrderBy(p => p.price)
                .ToArray();

            var jsonProducts = JsonConvert.SerializeObject(products, Formatting.Indented);

            return jsonProducts;
        }

        // 6. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var productSold = context.Users
                .Where(ps => ps.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(pc => pc.LastName)
                .ThenBy(pc => pc.FirstName)
                .AsNoTracking()
                .Select(ps => new
                {
                    firstName = ps.FirstName,
                    lastName = ps.LastName,
                    soldProducts = ps.ProductsSold
                        .Select(sp => new
                        {
                            name = sp.Name,
                            price = sp.Price,
                            buyerFirstName = sp.Buyer.FirstName,
                            buyerLastName = sp.Buyer.LastName
                        }).ToArray()
                })
                .ToArray();

            var jsonSoldProducts = JsonConvert.SerializeObject(productSold, Formatting.Indented);

            return jsonSoldProducts;
        }

        // 7.Export Categories by Products Count

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoeirsByProductsCount = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .AsNoTracking()
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = c.CategoriesProducts
                        .Average(c => c.Product.Price)
                            .ToString("f2"),
                    totalRevenue = c.CategoriesProducts
                        .Sum(c => c.Product.Price)
                            .ToString("f2")
                })
                .ToArray();

            return JsonConvert.SerializeObject(categoeirsByProductsCount, Formatting.Indented);
        }

        // 8.Exxprt Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(p => p.BuyerId != null),
                        products = u.ProductsSold
                            .Where(p => p.BuyerId != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price,

                            }).ToArray()
                    }
                })
                .AsNoTracking()
                .OrderByDescending(u => u.soldProducts.count)
                .ToArray();

            var objectResult = new
            {
                usersCount = users.Length,
                users = users
            };

            return JsonConvert.SerializeObject(objectResult, Formatting.Indented,new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
    }
}