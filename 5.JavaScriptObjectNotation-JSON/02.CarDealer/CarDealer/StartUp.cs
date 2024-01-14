using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();



            string output = string.Empty;
            string inputJson = string.Empty;

            //Query 9. Import Suppliers
            //inputJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //output = ImportSuppliers(context, inputJson);

            //Query 10. Import Parts
            //inputJson = File.ReadAllText("../../../Datasets/parts.json");
            //output = ImportParts(context, inputJson);

            //Query 11.Import Cars
            //inputJson = File.ReadAllText("../../../Datasets/cars.json");
            //output = ImportCars(context, inputJson);

            //Query 12. Import Customers
            //inputJson = File.ReadAllText("../../../Datasets/customers.json");
            //output = ImportCustomers(context, inputJson);

            //Query 13. Import Sales
            //inputJson = File.ReadAllText("../../../Datasets/sales.json");
            //output = ImportSales(context, inputJson);

            //Query 14. Export Ordered Customers
            //output = GetOrderedCustomers(context);

            //Query 15. Export Cars from Make Toyota
            //output = GetCarsFromMakeToyota(context);

            //Query 16. Export Local Suppliers
            //output = GetLocalSuppliers(context);

            //Query 17. Export Cars with Their List of Parts
            //output = GetCarsWithTheirListOfParts(context);

            //Query 18. Export Total Sales by Customer
            output = GetTotalSalesByCustomer(context);

            //19.Export Sales With Applied Discount
            //output = GetSalesWithAppliedDiscount(context);


            Console.WriteLine(output);
        }
        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var supplisiers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);
            context.AddRangeAsync(supplisiers);
            context.SaveChanges();

            return $"Successfully imported {supplisiers.Count()}.";
        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                .Where(p =>
                        context.Suppliers
                        .Select(s => s.Id)
                            .Contains(p.SupplierId));


            context.AddRangeAsync(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var importCarsDto = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            ICollection<Car> carsToAdd = new HashSet<Car>();

            foreach (var carDto in importCarsDto)
            {
                Car currCar = new()
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };
                foreach (var partId in carDto.PartsIds.Distinct())
                {
                    currCar.PartsCars.Add(new PartCar
                    {
                        PartId = partId,

                    });
                }
                carsToAdd.Add(currCar);
            }

            context.AddRangeAsync(carsToAdd);
            context.SaveChanges();

            return $"Successfully imported {carsToAdd.Count()}.";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.AddRangeAsync(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.AddRangeAsync(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        //Query 14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(s => s.BirthDate)
                .ThenBy(s => s.IsYoungDriver)
                .Select(s => new
                {
                    Name = s.Name,
                    BirthDate = s.BirthDate.ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo),
                    IsYoungDriver = s.IsYoungDriver
                })
                .ToList();
            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //Query 15. Export Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsToyota = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();
            return JsonConvert.SerializeObject(carsToyota, Formatting.Indented);
        }

        //Query 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);

        }

        //Query 17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carInfo = context.Cars
                 .AsNoTracking()
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TraveledDistance
                    },
                    parts = c.PartsCars
                        .Select(p => new
                        {
                            Name = p.Part.Name,
                            Price = p.Part.Price.ToString("0.00")
                        })
                        .ToArray()
                }).ToArray();
            return JsonConvert.SerializeObject(carInfo, Formatting.Indented);
        }

        //Query 18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
          
            var customers = context.Customers
                    .Where(c => c.Sales.Count() > 0)
                    .Select(c => new
                    {
                        fullName = c.Name,
                        boughtCars = c.Sales.Count(),
                        spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(p => p.Part.Price))
                    })
                    .OrderByDescending(x => x.spentMoney)
                    .ThenByDescending(x => x.boughtCars)
                    .ToList();

            
            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }


        //19.Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance

                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = ((s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100))).ToString("f2")
                }).ToArray();
            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}