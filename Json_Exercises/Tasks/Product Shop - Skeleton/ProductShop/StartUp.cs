using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.Models.ModelDto;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            using (var db = new ProductShopContext())
            {
                //var inputJson = File.ReadAllText("./../../../Datasets/categories-products.json");
                var result = GetUsersWithProducts(db);
                Console.WriteLine(result);


            }
        }
        //Problem 1.
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Length}";
        }
        //Problem 2.
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Length}";
        }
        //Problem 3.
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<Category[]>(inputJson)
                .Where(c => c.Name != null);

            context.AddRange(categories);
            var count = context.SaveChanges();

            return $"Successfully imported {count}";
        }
        //Problem 4.
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);
            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesProducts.Length}";
        }
        //Problem 5.
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToList();
            var exportedJson = JsonConvert.SerializeObject(products, Formatting.Indented);
            return exportedJson;
        }
        //Problem 6.
        public static string GetSoldProducts(ProductShopContext context)
        {
            //Get all users who have at least 1 sold item with a buyer. Order them by last name, then by first name. 
            //Select the person's first and last name. For each of the sold products (products with buyers), select the product's name, 
            //price and the buyer's first and last name.
            var users = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                    .Where(p => p.Buyer != null)
                    .Select(p => new SoldProductDto
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToList()
                })
                .ToList();

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;
        }
        //Problem 7.
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //Get all categories. Order them in descending order by the category’s products count. For each category select its name, the number of
            //products, the average price of those products (rounded to second digit after the decimal separator) and the total revenue
            //(total price sum and rounded to second digit after the decimal separator) of those products (regardless if they have a buyer or not).

            var categories =
                context
                .Categories
                .OrderByDescending(c => c.CategoryProducts.Count())
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count(),
                    averagePrice = $@"{c.CategoryProducts.Sum(p => p.Product.Price) / c.CategoryProducts.Count:F2}",
                    totalRevenue = $"{c.CategoryProducts.Sum(p => p.Product.Price):F2}"
                })
                .ToList();

            var jsonOutput = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return jsonOutput;
        }
        //Problem 7.
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //Get all users who have at least 1 sold product with a buyer. Order them in descending order by the number of sold products with a buyer. Select only their first and last name, age and for each product - name and price. Ignore all null values.
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,

                    soldProducts = new
                    {
                        count = u.ProductsSold
                                .Where(p => p.Buyer != null)
                                .Count(),

                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                       .Select(ps => new
                       {
                           name = ps.Name,
                           price = ps.Price
                       })
                       .ToList()
                    }
                })
                .OrderByDescending(u => u.soldProducts.count)
                .ToList();

            var userOutput = new
            {
                usersCount = users.Count,
                users = users
            };

            var jsonExport = JsonConvert.SerializeObject(userOutput, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return jsonExport;
        }
    }


}