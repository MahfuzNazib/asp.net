using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceSystem.Models;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace InvoiceSystem.Controllers
{
    public class POSController : Controller
    {
        string Str = @"Data Source=DESKTOP-8MIOL09\MSSQLSERVER2008;Initial Catalog=InvoiceDB;Integrated Security=True";

        // GET: POSController
        public ActionResult Index()
        {

            DataTable dtblProducts = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(Str))
            {
                sqlCon.Open();
                string query = "SELECT * FROM Products";

                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.Fill(dtblProducts);
                sqlCon.Close();
                
            }
            return View(dtblProducts);
        }

        [HttpGet]
        public ActionResult InvoiceList()
        {
            DataTable dtblInvoiceList = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(Str)) 
            {
                sqlCon.Open();
                string query = "SELECT * FROM Invoice Order By InvoiceId DESC";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.Fill(dtblInvoiceList);
                sqlCon.Close();
            }
            return View(dtblInvoiceList);
        }

        // GET : Product Details 
        public JsonResult GetProductInfo(int ProductId)
        {
            ProductModel productModel = new ProductModel();
            DataTable dtblProduct = new DataTable();
            using(SqlConnection sqlCon = new SqlConnection(Str))
            {
                sqlCon.Open();
                string query = "SELECT * FROM Products WHERE ProductId = @ProductId";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductId", ProductId);
                sqlDa.Fill(dtblProduct);
            }

            if(dtblProduct.Rows.Count == 1)
            {
                productModel.Price = Convert.ToInt32(dtblProduct.Rows[0][2]) ;
                return new JsonResult(Ok(productModel));
            } 
            else
            {
                return new JsonResult(Ok("error"));
            }
        }

        // GET: POSController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateNewInvoice()
        {
            try
            {
                // Generate New Invoice Number. Its Random.
                Random RndNumber = new Random();
                int NewInvoiceCode = RndNumber.Next();

                string InvoiceDate = Request.Form["InvoiceDate"];
                string InvoiceCode = Convert.ToString(NewInvoiceCode);
                double SubTotal = Convert.ToDouble(Request.Form["SubTotal"]);
                double Discount = Convert.ToDouble(Request.Form["Discount"]);
                double Vat = Convert.ToDouble(Request.Form["Vat"]);
                double GrandTotal = Convert.ToDouble(Request.Form["GrandTotal"]);


                using (SqlConnection sqlCon = new SqlConnection(Str))
                {
                    sqlCon.Open();
                    string query = "INSERT INTO Invoice OUTPUT Inserted.InvoiceId VALUES(@InvoiceDate, @InvoiceCode, @SubTotal, @Discount, @Vat, @GrandTotal)";

                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@InvoiceDate", InvoiceDate);
                    sqlCmd.Parameters.AddWithValue("@InvoiceCode", InvoiceCode);
                    sqlCmd.Parameters.AddWithValue("@SubTotal", SubTotal);
                    sqlCmd.Parameters.AddWithValue("@Discount", Discount);
                    sqlCmd.Parameters.AddWithValue("@Vat", Vat);
                    sqlCmd.Parameters.AddWithValue("@GrandTotal", GrandTotal);
                    int thisInvoiceId = (int)sqlCmd.ExecuteScalar();


                    string[] pid = Request.Form["productid"];
                    string[] Quantity = Request.Form["Quantity"];
                    string[] UnitPrice = Request.Form["UnitPrice"];
                    string[] TotalPrice = Request.Form["TotalPrice"];

                    // Insert Data into InvoicePurchaseDetails DB Table
                    foreach (var ProductId in pid.Select((id, index) => (id, index)))
                    {
                        string invoicePurchaseDetailsQuery = "INSERT INTO InvoicePurchaseDetails VALUES(@InvoiceId, @ProductId, @Quantity, @Unit, @TotalPrice)";

                        SqlCommand sqlCmdq = new SqlCommand(invoicePurchaseDetailsQuery, sqlCon);
                        sqlCmdq.Parameters.AddWithValue("@InvoiceId", thisInvoiceId);
                        sqlCmdq.Parameters.AddWithValue("@ProductId", ProductId.id);
                        sqlCmdq.Parameters.AddWithValue("@Quantity", Quantity[ProductId.index]);
                        sqlCmdq.Parameters.AddWithValue("@Unit", UnitPrice[ProductId.index]);
                        sqlCmdq.Parameters.AddWithValue("@TotalPrice", TotalPrice[ProductId.index]);

                        sqlCmdq.ExecuteNonQuery();

                    }
                    sqlCon.Close();
                }
            } 
            catch (Exception ex)
            {
                Console.WriteLine("Error Occure" + ex);
            }

            return RedirectToAction("InvoiceList");
        }


        public ActionResult ViewInvoice(int id)
        {
            int InvoiceID = id;
            DataTable dtblInvoiceDetails = new DataTable();
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(Str))
                {
                    sqlCon.Open();

                    string query = "SELECT InvoicePurchaseDetails.Id, InvoicePurchaseDetails.InvoiceId, InvoicePurchaseDetails.ProductId, " +
                        "Products.ProductName,InvoicePurchaseDetails.Unit, InvoicePurchaseDetails.Quantity, InvoicePurchaseDetails.TotalPrice " +
                        "FROM InvoicePurchaseDetails LEFT JOIN Products ON Products.ProductId = InvoicePurchaseDetails.ProductId WHERE " +
                        "InvoicePurchaseDetails.InvoiceId = @InvoiceID ORDER BY InvoicePurchaseDetails.Id DESC";
                    SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                    sqlDa.SelectCommand.Parameters.AddWithValue("@InvoiceID", InvoiceID);
                    sqlDa.Fill(dtblInvoiceDetails);
                    sqlCon.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return View(dtblInvoiceDetails);

        }


    }
}
