using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class InvoiceInfoDAL
    {
        #region "Oracle Connection Check"
        private OracleConnection GetConnection()
        {
            var conString = ConfigurationManager.ConnectionStrings["OracleDbContext"];
            string strConnString = conString.ConnectionString;
            return new OracleConnection(strConnString);
        }
        #endregion

        public async Task<InvoiceInfoModel> GetInvoiceInfo(string invoiceNumber, int shopId)
        {
            var sql = @"SELECT SI.SALE_INFO_ID,
            SI.SALE_INFO_AUTO_ID,
            SI.INVOICE_NUMBER,
            TO_CHAR(SI.INVOICE_DATE, 'dd/mm/yyyy') INVOICE_DATE,
            SI.TOTAL_ITEM,
            SI.TOTAL_AMOUNT,
            SI.DISCOUNT_P,
            SI.DISCOUNT_A,
            SI.SUB_TOTAL,
            SI.NET_AMOUNT,
            SI.SHOP_ID,
            S.SHOP_NAME,
            SI.WAREHOUSE_ID,
            CS.CUSTOMER_ID,
            CS.CUSTOMER_NAME,
            CS.CONTACT_NO
                FROM    SALE_INFO SI
            LEFT JOIN
            CUSTOMER_SALE CS
            ON(Si.CUSTOMER_ID = CS.CUSTOMER_ID) LEFT JOIN SHOP S ON (SI.SHOP_ID = S.SHOP_ID) WHERE SI.INVOICE_NUMBER = :INVOICE_NUMBER AND SI.SHOP_ID = :SHOP_ID ";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":INVOICE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = invoiceNumber;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId.ToString();
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        InvoiceInfoModel invoiceInfoModel = new InvoiceInfoModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                invoiceInfoModel.SaleInfoAutoId = Convert.ToInt32(objDataReader["SALE_INFO_AUTO_ID"].ToString());
                                invoiceInfoModel.SaleInfoId = Convert.ToInt32(objDataReader["SALE_INFO_ID"].ToString());
                                invoiceInfoModel.InvoiceNumber = objDataReader["INVOICE_NUMBER"].ToString();
                                invoiceInfoModel.InvoiceDate = objDataReader["INVOICE_DATE"].ToString();
                                invoiceInfoModel.TotalItem = Convert.ToInt32(objDataReader["TOTAL_ITEM"].ToString());
                                invoiceInfoModel.TotalAmount = Convert.ToDouble(objDataReader["TOTAL_AMOUNT"].ToString());
                                invoiceInfoModel.DiscountPercent = Convert.ToInt32(objDataReader["DISCOUNT_P"].ToString());
                                invoiceInfoModel.DiscountAmount = Convert.ToDouble(objDataReader["DISCOUNT_A"].ToString());
                                invoiceInfoModel.SubTotal = Convert.ToDouble(objDataReader["SUB_TOTAL"].ToString());
                                invoiceInfoModel.NetAmount = Convert.ToDouble(objDataReader["NET_AMOUNT"].ToString());
                                invoiceInfoModel.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                invoiceInfoModel.ShopName = objDataReader["SHOP_NAME"].ToString();
                                invoiceInfoModel.WareHouseId = Convert.ToInt32(objDataReader["WAREHOUSE_ID"].ToString());
                                string customerId = objDataReader["CUSTOMER_ID"].ToString();
                                invoiceInfoModel.CustomerId = !string.IsNullOrWhiteSpace(customerId) ? Convert.ToInt32(objDataReader["CUSTOMER_ID"].ToString()) : 0;
                                invoiceInfoModel.CustomerName = objDataReader["CUSTOMER_NAME"].ToString();
                                invoiceInfoModel.CustomerContactNO = objDataReader["CONTACT_NO"].ToString();
                            }

                            return invoiceInfoModel;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error : " + ex.Message);
                        }
                        finally
                        {
                            objDataReader.Dispose();
                            objCommand.Dispose();
                            objConnection.Dispose();
                        }
                    }

                }
            }
        }

        public async Task<ItemInfoShopReceiveModel> iteminfoForDirectShopReceiveInfo(int productId, int itemId)
        {
            var sql = @"SELECT ITEM_ID,
                       PRODUCT_ID,
                       CATEGORY_ID,
                       CATEGORY_NAME,
                       SUB_CATEGORY_ID,
                       SUB_CATEGORY_NAME,
                       SALES_MEASURING_UNIT_ID,
                       SALES_MEASURING_UNIT_NAME,
                       BRAND_ID,
                       BRAND_NAME,
                        SALE_PRICE,
                       WARE_HOUSE_ID,
                       SHOP_ID,
                       VAT_PERCENT FROM VEW_PRODUCT_ITEM_INFO
                        WHERE PRODUCT_ID = :PRODUCT_ID AND ITEM_ID = :ITEM_ID ";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = itemId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ItemInfoShopReceiveModel itemInfoShopReceiveModel = new ItemInfoShopReceiveModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                itemInfoShopReceiveModel.SalePrice =
                                    Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                                itemInfoShopReceiveModel.Brand = objDataReader["BRAND_NAME"].ToString();
                                itemInfoShopReceiveModel.Category = objDataReader["CATEGORY_NAME"].ToString();
                                itemInfoShopReceiveModel.SubCategory = objDataReader["SUB_CATEGORY_NAME"].ToString();
                                itemInfoShopReceiveModel.Umo = objDataReader["SALES_MEASURING_UNIT_NAME"].ToString();
                            }

                            return itemInfoShopReceiveModel;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error : " + ex.Message);
                        }
                        finally
                        {
                            objDataReader.Dispose();
                            objCommand.Dispose();
                            objConnection.Dispose();
                        }
                    }

                }
            }
        }

        public async Task<List<SaleItemForInvoiceModel>> GetInvoiceItemInfo(int saleInfoAutoId, int saleInfoId)
        {
            var sql = @"SELECT SI.SALE_ITEM_ID,
       SI.SALE_INFO_ID,
       SI.BARCODE,
       SI.STYLE_NAME,
       SI.PRICE,
       SI.QUANTITY,
       SI.TOTAL,
       SI.VAT,
       S.DISCOUNT_P,
       ((S.DISCOUNT_P / 100) * (SI.PRICE * SI.QUANTITY)) DISCOUNT_AMOUNT,
       (SI.PRICE - (S.DISCOUNT_P / 100) * SI.PRICE) DISCOUNT_PRICE,
       (SI.TOTAL - ((S.DISCOUNT_P / 100) * (SI.PRICE * SI.QUANTITY)))
          NET_AMOUNT,
       SI.ITEM_ID,
       SI.PRODUCT_ID
        FROM    SALE_ITEM SI
       JOIN
          SALE_INFO S
       ON (SI.SALE_INFO_AUTO_ID = S.SALE_INFO_AUTO_ID)
        WHERE SI.SALE_INFO_ID = :SALE_INFO_ID AND SI.SALE_INFO_AUTO_ID = :SALE_INFO_AUTO_ID";

            //Hide Sec Shop To Shop Ex A Qty bashi hole Problem Hossilo
            //((S.DISCOUNT_P / 100) * (SI.PRICE * SI.QUANTITY)) DISCOUNT_AMOUNT,
            //(SI.TOTAL - ((S.DISCOUNT_P / 100) * (SI.PRICE * SI.QUANTITY)))
            //End
            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SALE_INFO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = saleInfoId;
                    objCommand.Parameters.Add(":SALE_INFO_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = saleInfoAutoId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SaleItemForInvoiceModel> model = new List<SaleItemForInvoiceModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SaleItemForInvoiceModel invoiceInfoModel = new SaleItemForInvoiceModel();
                                invoiceInfoModel.SaleItemId = Convert.ToInt32(objDataReader["SALE_ITEM_ID"].ToString());
                                invoiceInfoModel.SaleInfoId = Convert.ToInt32(objDataReader["SALE_INFO_ID"].ToString());
                                invoiceInfoModel.Barcode = objDataReader["BARCODE"].ToString();
                                invoiceInfoModel.StyleName = objDataReader["STYLE_NAME"].ToString();
                                invoiceInfoModel.Price = Convert.ToDouble(objDataReader["PRICE"].ToString());
                                invoiceInfoModel.Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString());
                                invoiceInfoModel.Vat = Convert.ToDouble(objDataReader["VAT"].ToString());
                                invoiceInfoModel.Total = Convert.ToDouble(objDataReader["TOTAL"].ToString());
                                invoiceInfoModel.DiscountPrice = Convert.ToDouble(objDataReader["DISCOUNT_PRICE"].ToString());
                                invoiceInfoModel.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                invoiceInfoModel.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());

                                model.Add(invoiceInfoModel);
                            }

                            return model;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error : " + ex.Message);
                        }
                        finally
                        {
                            objDataReader.Dispose();
                            objCommand.Dispose();
                            objConnection.Dispose();
                        }
                    }

                }
            }
        }
    }
}