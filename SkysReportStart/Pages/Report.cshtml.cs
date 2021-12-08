using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkysReportStart.Model;
using System.Collections.Generic;
using System.Linq;

namespace SkysReportStart.Pages
{
    public class ReportModel : PageModel
    {
        private readonly ILogger<ReportModel> _logger;
        private readonly NorthwindContext _db;

        // Added this property to be able to read the filter from the razor page.
        public string SearchTerm { get; set; }
        public class Item
        {
            public string ProductName { get; set; }
            public string CategoryName { get; set; }
            public decimal? Price { get; set; }
            public string SupplierName { get; set; }
        }

        public List<Item> Items { get; set; }

        public ReportModel(ILogger<ReportModel> logger, NorthwindContext db)
        {
            _logger = logger;
            _db = db;
        }

        public void OnGet(string q)
        {
            //q är det man sökt efter
            SearchTerm = q;
            Items = new List<Item>();
            Items.AddRange(GetItems(q));
        }

        private IEnumerable<Item> GetItems(string q)
        {
            return _db.Products
                        .Include(p => p.Supplier)
                        .Include(p => p.Category)
                        .Where(p => q == null || p.ProductName.Contains(q) || p.Category.CategoryName.Contains(q) || p.Supplier.CompanyName.Contains(q))
                        .OrderBy(p => p.ProductName)
                        .Select(p => new Item
                        {
                            CategoryName = p.Category.CategoryName,
                            SupplierName = p.Supplier.CompanyName,
                            Price = p.UnitPrice,
                            ProductName = p.ProductName
                        });

            //Another way to do it with query syntax linq ... i feel like this approche is kinda more readble?? 
            //var ItemsQuery = from p in _db.Products
            //                 join s in _db.Suppliers on p.SupplierId equals s.SupplierId
            //                 join c in _db.Categories on p.CategoryId equals c.CategoryId
            //                 where (q == null || p.ProductName.Contains(q) || c.CategoryName.Contains(q) || s.CompanyName.Contains(q))
            //                 select new Item { CategoryName = c.CategoryName, SupplierName = s.CompanyName, Price = p.UnitPrice, ProductName = p.ProductName };
            // return ItemsQuery;
        }
    }
}
