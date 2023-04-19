using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System_Shopper.Models;

namespace System_Shopper.Pages
{
    public class EditProductModel : PageModel
    {
        [BindProperty]
        public Product ExistingProduct { get; set; } = new Product();

        [BindProperty]
        public Product NewProduct { get; set; } = new Product();

        [BindProperty]
        public List<SelectListItem> Manufacturers { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public List<SelectListItem> ProductTypes { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public List<SelectListItem> Discounts { get; set; } = new List<SelectListItem>();

        public void OnGet(int id)
        {
            ExistingProduct.ProductId = id;
            PopulateExistingProduct();
            PopulateManufacturers();
            PopulateDiscounts();
        }

        private void PopulateExistingProduct()
        {
            using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string sql = "SELECT * FROM Product WHERE ProductId=@productId";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@productId", ExistingProduct.ProductId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ExistingProduct.ProductName = reader["ProductName"].ToString();
                        ExistingProduct.ProductDescription = reader["ProductDescription"].ToString();
                        ExistingProduct.ManufacturerId = int.Parse(reader["ManufacturerId"].ToString());
                        ExistingProduct.Price = decimal.Parse(reader["Price"].ToString());
                        ExistingProduct.DiscountId = int.Parse(reader["DiscountId"].ToString());
                        ExistingProduct.ProductImage = reader["ProductImage"].ToString();
                    }
                }
            }
        }

        private void PopulateManufacturers()
        {
            using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string sql = "SELECT * FROM Manufacturer ORDER BY ManufacturerName";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SelectListItem manufacturer = new SelectListItem();
                        manufacturer.Value = reader["ManufacturerId"].ToString();
                        manufacturer.Text = reader["ManufacturerName"].ToString();
                        Manufacturers.Add(manufacturer);
                    }
                }
            }
        }

        private void PopulateProductTypes()
        {
            using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string sql = "SELECT * FROM ProductType ORDER BY ProductType";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SelectListItem productType = new SelectListItem();
                        productType.Value = reader["ProductTypeId"].ToString();
                        productType.Text = reader["ProductType"].ToString();
                        ProductTypes.Add(productType);
                    }
                }
            }
        }
        private void PopulateDiscounts()
        {
            using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
            {
                string sql = "SELECT * FROM Discount ORDER BY DiscountPercent";
                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        SelectListItem discount = new SelectListItem();
                        discount.Value = reader["DiscountId"].ToString();
                        discount.Text = reader["DiscountName"].ToString() + ": " + reader["DiscountPercent"].ToString();
                        Discounts.Add(discount);
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection conn = new SqlConnection(DBHelper.GetConnectionString()))
                {
                    string sql = "UPDATE Product " +
                        "SET ProductName = @productName, ProductDescription = @productDescription, ManufacturerId = @manufacturerId, Price = @price, DiscountId = @discountId, ProductImage = @productImage " +
                        "WHERE ProductId = @productId;";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@productName", NewProduct.ProductName);
                    cmd.Parameters.AddWithValue("@productDescription", NewProduct.ProductDescription);
                    cmd.Parameters.AddWithValue("@manufacturerId", NewProduct.ManufacturerId);
                    cmd.Parameters.AddWithValue("@price", NewProduct.Price);
                    cmd.Parameters.AddWithValue("@discountId", NewProduct.DiscountId);
                    cmd.Parameters.AddWithValue("@productImage", NewProduct.ProductImage);
                    cmd.Parameters.AddWithValue("@productId", NewProduct.ProductId);
                    conn.Open();

                    cmd.ExecuteNonQuery();
                }
                return RedirectToPage("Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
