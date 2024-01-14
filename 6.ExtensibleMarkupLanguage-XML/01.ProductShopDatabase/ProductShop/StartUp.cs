using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop;

public class StartUp
{
    public static void Main()
    {
        //Database
        ProductShopContext context = new ProductShopContext();

        //Variables
        string output = string.Empty;
        string inputXml = string.Empty;

        //01.Import Users
        //inputXml = File.ReadAllText(@"../../../Datasets/users.xml");
        //output = ImportUsers(context, inputXml);

        //02.Import Products
        //inputXml = File.ReadAllText(@"../../../Datasets/products.xml");
        //output = ImportProducts(context, inputXml);

        //03.Import Categories
        inputXml = File.ReadAllText(@"../../../Datasets/categories.xml");
        output = ImportCategories(context, inputXml);

        //04.Import Categories and Products
        //inputXml = File.ReadAllText(@"../../../Datasets/categories-products.xml");
        //output = ImportCategoryProducts(context, inputXml);

        //05.Export Products In Range
        //output = GetProductsInRange(context);

        //06.Export Sold Products
        //output = GetSoldProducts(context);

        //07.Export Categories By Products Count
        //output = GetCategoriesByProductsCount(context);

        //08.Export Users and Products
        //output = GetUsersWithProducts(context);

        Console.WriteLine(output);
    }

    

    //01.Import Users
    public static string ImportUsers(ProductShopContext context, string inputXml)
    {
        
        var xmlParser = new XmlParser();

        
        ImportUserDto[] importUserDtos = xmlParser.Deserialize<ImportUserDto[]>(inputXml, "Users");


        var users = new List<User>();

        foreach (var userDto in importUserDtos)
        {
            User user = new User()
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Age = userDto.Age
            };
            users.Add(user);
        }
        
        context.Users.AddRange(users);
        context.SaveChanges();

        
        return $"Successfully imported {users.Count()}";
    }

    //02.Import Products
    public static string ImportProducts(ProductShopContext context, string inputXml)
    {
        
        var xmlParser = new XmlParser();

        
        ImportProductDto[] productDtos = xmlParser.Deserialize<ImportProductDto[]>(inputXml, "Products");

       
        var products = new List<Product>();
        foreach (var productDto in productDtos)
        {
            Product product = new Product()
            {
                Name = productDto.Name,
                Price = productDto.Price,
                SellerId = productDto.SellerId,
                BuyerId = productDto.BuyerId
            };
            products.Add(product);
        }

       
        context.Products.AddRange(products);
        context.SaveChanges();

        
        return $"Successfully imported {products.Count}";
    }

    //03.Import Categories
    public static string ImportCategories(ProductShopContext context, string inputXml)
    {
        
        var xmlParser = new XmlParser();

        
        ImportCategoryDto[] categoryDtos = xmlParser.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

        
        var categories = new List<Category>();

        foreach (var categoryDto in categoryDtos)
        {
            if (categoryDto.Name != null)
            {
                Category category = new Category()
                {
                    Name = categoryDto.Name

                };
                categories.Add(category);
            }
        }

        
        context.Categories.AddRange(categories);
        context.SaveChanges();

        
        return $"Successfully imported {categories.Count}";
    }

    
    public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
    {
        
        var xmlParser = new XmlParser();

        
        ImportCategoryProductDto[] categoryProductDtos = xmlParser.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");

        
        List<CategoryProduct> categoryProducts = new List<CategoryProduct>();
        HashSet<int> productIds = context.Products.Select(p => p.Id).ToHashSet<int>();
        HashSet<int> categoryIds = context.Categories.Select(c => c.Id).ToHashSet<int>();

        foreach (var dto in categoryProductDtos)
        {
            if (productIds.Contains(dto.ProductId) && categoryIds.Contains(dto.CategoryId))
            {
                var categoryProduct = new CategoryProduct()
                {
                    ProductId = dto.ProductId,
                    CategoryId = dto.CategoryId,
                };
                categoryProducts.Add(categoryProduct);
            }
        }

        
        context.CategoryProducts.AddRange(categoryProducts);
        context.SaveChanges();

        
        return $"Successfully imported {categoryProducts.Count}";
    }

    
    public static string GetProductsInRange(ProductShopContext context)
    {
        var xmlParser = new XmlParser();

        
        var productsInRange = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductDto()
                {
                    Price = p.Price,
                    Name = p.Name,
                    BuyerName = p.Buyer.FirstName + " " + p.Buyer.LastName
                })
                .ToArray();

        
        return xmlParser.Serialize<ExportProductDto[]>(productsInRange, "Products");
    }

    //06.Export Sold Products
    public static string GetSoldProducts(ProductShopContext context)
    {
        var xmlParser = new XmlParser();

        
        var usersSoldProducts = context.Users
            .Where(u => u.ProductsSold.Count > 0 && u.ProductsSold.Any(ps => ps.BuyerId != null))
            .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            .Take(5)
            .Select(u => new ExportSoldProductsDto
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                SoldProducts = u.ProductsSold.Select(p => new ProductDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                })
                .ToArray()
            })
            .ToArray();

        
        return xmlParser.Serialize<ExportSoldProductsDto[]>(usersSoldProducts, "Users");
    }

    //07.Export Categories By Products Count
    public static string GetCategoriesByProductsCount(ProductShopContext context)
    {
        var xmlParser = new XmlParser();

        
        var categories = context.Categories
            .AsNoTracking()
            .Select(c => new ExportCategoryDto
            {
                Name = c.Name,
                Count = c.CategoryProducts.Count,
                AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
            })
            .OrderByDescending(exd => exd.Count)
            .ThenBy(exd => exd.TotalRevenue)
            .ToArray();

        
        return xmlParser.Serialize<ExportCategoryDto[]>(categories, "Categories");
    }

    //08.Export Users and Products
    public static string GetUsersWithProducts(ProductShopContext context)
    {
        var xmlParser = new XmlParser();

       
        var usersInfo = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UserInfo()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsCount()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new SoldProduct()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

        ExportUserCountDto exportUserCountDto = new ExportUserCountDto()
        {
            Count = context.Users.Count(u => u.ProductsSold.Any()),
            Users = usersInfo
        };

        
        return xmlParser.Serialize<ExportUserCountDto>(exportUserCountDto, "Users");
    }
}