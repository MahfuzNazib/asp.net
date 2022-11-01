using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceSystem.Models
{
    public class ProductDBAccessLayer
    {
        string Str = @"Data Source=MAHFUZ-NAZIB;Initial Catalog=InvoiceDB;Integrated Security=True";

        public string AddProduct(ProductEntities productEntities)
        {
            try
            {
                using (SqlConnection sqlCon = new SqlConnection(Str))
                {
                    sqlCon.Open();
                    string query = "INSERT INTO Products VALUES (@ProductName, @Price)";

                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.Parameters.AddWithValue("@ProductName", productEntities.ProductName);
                    sqlCmd.Parameters.AddWithValue("@Price", productEntities.Price);
                    sqlCmd.ExecuteNonQuery();

                    sqlCon.Close();
                    return ("Product Save Successfully");
                }

            } catch(Exception ex)
            {
                return (ex.Message.ToString());
            }
        }


        //public string GetAllProducts()
        //{
        //    try
        //    {
        //        using (SqlConnection sqlCon = new SqlConnection(Str))
        //        {
        //            sqlCon.Open();
        //            string query = "SELECT * FROM Products";

        //            SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
        //            sqlCmd.ExecuteNonQuery();

        //            sqlCon.Close();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return (ex.Message.ToString());
        //    }
        //}
    }
}
