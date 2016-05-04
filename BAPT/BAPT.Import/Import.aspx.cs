using System;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Configuration;
using System.Data.SqlClient;
using BAPT.Database;
namespace BAPT
{
    public partial class Import : System.Web.UI.Page
    {
        OleDbConnection Econ;
        SqlConnection con;
        string constr, Query, sqlconn;
        protected void Page_Load(object sender, EventArgs e)
        {
          //string priceSource= Request.Url.OriginalString.Replace("Import.aspx","")+"PricingSheet/Pricing.xlsx";
            InsertExcelRecords(Server.MapPath("~/PricingSheet/Pricing_NoWiFi.xlsx"));
        }
        private void ExcelConn(string FilePath)
        {

            constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", FilePath);
            Econ = new OleDbConnection(constr);
            

        }
        private void connection()
        {
            sqlconn = ConfigurationManager.ConnectionStrings["SqlCom"].ConnectionString;
            con = new SqlConnection(sqlconn);

        }  
        private void InsertExcelRecords(string FilePath)
        {
            ExcelConn(FilePath);


            Query = string.Format("Select *	FROM [{0}]", "Sheet1$");
          
            OleDbCommand Ecom = new OleDbCommand(Query, Econ);
            Econ.Open();

            DataSet ds = new DataSet();
            OleDbDataAdapter oda = new OleDbDataAdapter(Query, Econ);
          
            oda.Fill(ds);
            DataTable Exceldt = ds.Tables[0]; 
             Econ.Close();
             using (var context = new BAPTEntities())
             {
                 //remove the old records
                 context.BAPTXOPricings.RemoveRange(context.BAPTXOPricings);
                 context.SaveChanges();
                 #region loop through the excel data
                 foreach (DataRow row in Exceldt.Rows)
               {
              
               
                     BAPTXOPricing price = new BAPTXOPricing();
                     string download = row["Download"].ToString();
                     if (download.Contains("Mbps"))
                     {
                         download = download.Replace("Mbps", "").Trim();
                         price.DownLoad = Convert.ToInt32(Convert.ToDouble(download) * 1000);
                     }
                     else if (download.Contains("Kbps"))
                         price.DownLoad = Convert.ToInt32(download.Replace("Kbps", "").Trim());
                     else
                         price.DownLoad = Convert.ToInt32(download.Trim());

                     string upload = row["Upload"].ToString();

                     if (upload.Contains("Mbps"))
                     {
                         upload = upload.Replace("Mbps", "").Trim();
                         price.Upload = Convert.ToInt32(Convert.ToDouble(upload) * 1000);
                     }
                     else if (upload.Contains("Kbps"))
                         price.Upload = Convert.ToInt32(upload.Replace("Kbps", "").Trim());
                     else
                         price.Upload = Convert.ToInt32(upload.Trim());

                     price.IPType = row["IP Type"].ToString();
                     price.LoopType = row["Loop Type"].ToString();

                     price.MRC = Math.Round(Convert.ToDecimal(row["MRC"].ToString().Trim()), 2);




                     price.NRC = Math.Round(Convert.ToDecimal(row["NRC"].ToString().Trim()),2);

                     price.Install = Math.Round(Convert.ToDecimal(row["Install"].ToString().Trim()), 2);
                    
                    
                     price.Term = Convert.ToInt32(row["TERM"].ToString().Trim());
                     price.Network = row["Network"].ToString();
                     price.PriceGroup = row["Price Group"].ToString();
                    



                     context.BAPTXOPricings.Add(price);

            }
                 #endregion
                 context.SaveChanges();

            } 
            
          
            //connection();
            ////creating object of SqlBulkCopy    
            //SqlBulkCopy objbulk = new SqlBulkCopy(con);
            ////assigning Destination table name    
            //objbulk.DestinationTableName = "Employee";
            ////Mapping Table column    
            //objbulk.ColumnMappings.Add("Name", "Name");
            //objbulk.ColumnMappings.Add("City", "City");
            //objbulk.ColumnMappings.Add("Address", "Address");
            //objbulk.ColumnMappings.Add("Designation", "Designation");
            ////inserting Datatable Records to DataBase    
            //con.Open();
            //objbulk.WriteToServer(Exceldt);
            //con.Close();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            string CurrentFilePath = Path.GetFullPath(FileUpload1.PostedFile.FileName);
            InsertExcelRecords(CurrentFilePath);
        }  
    }
}