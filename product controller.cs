using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using InventoryApp.Models;

namespace InventoryApp.Controllers
{
    public class ProductController : Controller
    {
        string cs = @"Data Source=.;Initial Catalog=InventoryDB;Integrated Security=True;";

        //READ
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Products", con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ProductName = dr["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(dr["Quantity"]),
                        Price = Convert.ToDecimal(dr["Price"])
                    });
                }
            }

            return View(products);
        }

        //CREATE-SHOW FORM
        public ActionResult Create()
        {
            return View();
        }

        //CREATE-SAVE DATA
        [HttpPost]
        public ActionResult Create(Product p)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Products
                    (ProductName, Quantity, Price)
                    VALUES (@name, @qty, @price)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 100).Value =
                    p.ProductName;
                cmd.Parameters.Add("@qty", SqlDbType.Int).Value = p.Quantity;
                cmd.Parameters.Add("@price", SqlDbType.Decimal).Value = p.Price;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // UPDATE - SHOW FORM
        public ActionResult Edit(int id)
        {
            Product p = null;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Products WHERE ProductId=@id", con);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    p = new Product
                    {
                        ProductId = Convert.ToInt32(dr["ProductId"]),
                        ProductName = dr["ProductName"].ToString(),
                        Quantity = Convert.ToInt32(dr["Quantity"]),
                        Price = Convert.ToDecimal(dr["Price"])
                    };
                }
            }

            if (p == null)
                return HttpNotFound();

            return View(p);
        }

        // UPDATE - SAVE CHANGES
        [HttpPost]
        public ActionResult Edit(Product p)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Products SET
                    ProductName=@name,
                    Quantity=@qty,
                    Price=@price
                    WHERE ProductId=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@name", SqlDbType.VarChar, 100).Value =
                    p.ProductName;
                cmd.Parameters.Add("@qty", SqlDbType.Int).Value = p.Quantity;

                SqlParameter price = cmd.Parameters.Add("@price",
                    SqlDbType.Decimal);

                price.Precision = 10;
                price.Scale = 2;
                price.Value = p.Price;

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = p.ProductId;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // DELETE
        public ActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM Products WHERE ProductId=@id", con);

                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}