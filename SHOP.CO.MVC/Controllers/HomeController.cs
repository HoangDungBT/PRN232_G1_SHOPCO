using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SHOP.CO.MVC.Models;
using SHOP.CO.Application.Helpers;
using SHOP.CO.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SHOP.CO.MVC.Services;

namespace SHOP.CO.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string apiUrl;
        private readonly string odataApiUrl;
        private readonly IProductApiClient _productApiClient;

        public HomeController(IConfiguration configuration, IProductApiClient productApiClient)
        {
            var baseUrl = (configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7196").TrimEnd('/');
            apiUrl = $"{baseUrl}/api/products";
            odataApiUrl = $"{baseUrl}/odata/Products";
            _productApiClient = productApiClient;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productApiClient.GetProductsAsync();
                return View(products);
            }
            catch (Exception)
            {
                return View(new List<ProductVM>());
            }
        }

        // CATEGORY PAGE WITH ODATA FILTER & PAGING
        public async Task<IActionResult> Category(
            int? categoryId,
            string? brand,
            decimal? minPrice,
            decimal? maxPrice,
            string? size,
            string? color,
            string? sortOrder,
            int page = 1)
        {
            List<ProductVM> products = new();
            int totalCount = 0;
            const int pageSize = 9;

            // Fetch dynamic categories
            List<CategoryDto> categories = new();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var catResponse = await client.GetAsync($"{apiUrl.Replace("/products", "/categories")}");
                    if (catResponse.IsSuccessStatusCode)
                    {
                        var catJson = await catResponse.Content.ReadAsStringAsync();
                        categories = JsonConvert.DeserializeObject<List<CategoryDto>>(catJson) ?? new();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching categories: " + ex.Message);
            }
            ViewBag.Categories = categories;

            var queryString = ODataQueryBuilder.Build(
                categoryId,
                brand,
                minPrice,
                maxPrice,
                size,
                color,
                sortOrder,
                page,
                pageSize
            );

            string fullUrl = odataApiUrl + queryString;
            string rawJson = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Thiết lập Accept header chuẩn để yêu cầu JSON
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(fullUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        rawJson = await response.Content.ReadAsStringAsync();
                        rawJson = rawJson.Trim();

                        if (rawJson.StartsWith("["))
                        {
                            // API trả về mảng phẳng [...] do Content Negotiation hoặc custom routing
                            var flatList = JsonConvert.DeserializeObject<List<ProductDto>>(rawJson);
                            if (flatList != null)
                            {
                                totalCount = flatList.Count;
                                products = flatList.Select(p => new ProductVM
                                {
                                    Id = p.ProductId,
                                    Name = p.ProductName,
                                    Price = p.SalePrice ?? p.BasePrice,
                                    BasePrice = p.BasePrice,
                                    SalePrice = p.SalePrice,
                                    Image = p.ThumbnailUrl ?? "/images/heroimg.png",
                                    Description = p.Description ?? "",
                                    Category = p.CategoryName ?? ""
                                }).ToList();
                            }
                        }
                        else
                        {
                            // API trả về định dạng OData chuẩn {"value": [...], "Count": X}
                            var odataResult = JsonConvert.DeserializeObject<ODataResponse<ProductDto>>(rawJson);
                            if (odataResult != null)
                            {
                                totalCount = odataResult.Count;
                                products = odataResult.Value.Select(p => new ProductVM
                                {
                                    Id = p.ProductId,
                                    Name = p.ProductName,
                                    Price = p.SalePrice ?? p.BasePrice,
                                    BasePrice = p.BasePrice,
                                    SalePrice = p.SalePrice,
                                    Image = p.ThumbnailUrl ?? "/images/heroimg.png",
                                    Description = p.Description ?? "",
                                    Category = p.CategoryName ?? ""
                                }).ToList();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"OData API returned non-success status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi chi tiết phục vụ debug
                Console.WriteLine($"Error calling OData API. URL: {fullUrl}");
                Console.WriteLine($"Raw JSON response: {rawJson}");
                Console.WriteLine("Error Details: " + ex.ToString());
            }

            // Truyền các thông tin phân trang & bộ lọc xuống view
            ViewBag.CurrentPage = page;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.CategoryId = categoryId;
            ViewBag.Brand = brand;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Size = size;
            ViewBag.Color = color;
            ViewBag.SortOrder = sortOrder;

            return View(products);
        }

        // PRODUCT DETAIL
        public async Task<IActionResult> Detail(int id)
        {
            ProductDto? product = null;
            List<ProductDto> relatedProducts = new();
            List<ReviewDto> reviews = new();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync($"{apiUrl}/{id}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        product = JsonConvert.DeserializeObject<ProductDto>(json);
                    }

                    var relatedResponse = await client.GetAsync($"{apiUrl}/{id}/related?limit=4");
                    if (relatedResponse.IsSuccessStatusCode)
                    {
                        var jsonRelated = await relatedResponse.Content.ReadAsStringAsync();
                        relatedProducts = JsonConvert.DeserializeObject<List<ProductDto>>(jsonRelated) ?? new();
                    }

                    var reviewsResponse = await client.GetAsync($"{apiUrl}/{id}/reviews");
                    if (reviewsResponse.IsSuccessStatusCode)
                    {
                        var jsonReviews = await reviewsResponse.Content.ReadAsStringAsync();
                        reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(jsonReviews) ?? new();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching product details: " + ex.Message);
            }

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.RelatedProducts = relatedProducts;
            ViewBag.Reviews = reviews;
            return View("Details", product);
        }

        // SHOPPING CART
        public IActionResult Cart()
        {
            return View();
        }
    }
}