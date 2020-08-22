using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;

namespace PosWarehouse.DAL
{
    public class DeliveredProductDAL
    {
        private OracleTransaction _trans;
        #region "Oracle Connection Check"
        private OracleConnection GetConnection()
        {
            var conString = ConfigurationManager.ConnectionStrings["OracleDbContext"];
            string strConnString = conString.ConnectionString;
            return new OracleConnection(strConnString);
        }
        #endregion

        public async Task<List<DeliveredProduct>> GetAllDeliveredProduct()
        {
            const string sql = "SELECT " +
                               "STORE_DELIVERY_NUMBER," +
                               "REGISTER_ID," +
                               "DELIVERY_SHOP_ID," +
                               "DELIVERY_SHOP_NAME," +
                               "PURCHASE_RECEIVE_NUMBER," +
                               "TO_CHAR(DELIVERY_DATE,'dd/mm/yyyy')DELIVERY_DATE ," +
                               "REQUISTION_NO," +
                               "RECEIVE_CHALLAN_DELIVERY," +
                               "STORE_DELIVERY_ITEM_ID," +
                               "ITEM_ID," +
                               "PRODUCT_ID," +
                               "ITEM_NAME," +
                               "BARCODE," +
                               "DELIVERY_QUANTITY," +
                               "PURCHASE_PRICE," +
                               "SALE_PRICE," +
                               "VAT, " +
                               "UMO, " +
                                "PRODUCT_CATEGORY, " +
                                "PRODUCT_SUB_CATEGORY, " +
                                "PRODUCT_BRAND, " +
                                "RECEIVED_SHOP_ID " +
                               "FROM VEW_DELIVERED_PRODUCT ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DeliveredProduct> objDeliveredProduct = new List<DeliveredProduct>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DeliveredProduct model = new DeliveredProduct();
                                model.StoreDeliveryNumber = objDataReader["STORE_DELIVERY_NUMBER"].ToString();
                                model.RegisterId = objDataReader["REGISTER_ID"].ToString();
                                model.DeliveryShopId = !string.IsNullOrWhiteSpace(objDataReader["DELIVERY_SHOP_ID"].ToString()) ? Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString()) : 0;
                                model.DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString();
                                model.PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString();
                                model.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                model.RequisitionNo = objDataReader["REQUISTION_NO"].ToString();
                                model.ReceiveChallanDelivery = objDataReader["RECEIVE_CHALLAN_DELIVERY"].ToString();
                                model.DeliveryItemId = !string.IsNullOrWhiteSpace(objDataReader["STORE_DELIVERY_ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["STORE_DELIVERY_ITEM_ID"].ToString()) : 0;
                                model.ItemId = !string.IsNullOrWhiteSpace(objDataReader["ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["ITEM_ID"].ToString()) : 0;
                                model.ProductId = !string.IsNullOrWhiteSpace(objDataReader["PRODUCT_ID"].ToString()) ? Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()) : 0;
                                model.ItemName = objDataReader["ITEM_NAME"].ToString();
                                model.BarCode = objDataReader["BARCODE"].ToString();
                                model.DeliveryQuantity = !string.IsNullOrWhiteSpace(objDataReader["DELIVERY_QUANTITY"].ToString()) ? Convert.ToInt32(objDataReader["DELIVERY_QUANTITY"].ToString()) : 0;
                                model.PurchasePrice = !string.IsNullOrWhiteSpace(objDataReader["PURCHASE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()) : 0;
                                model.SalePrice = !string.IsNullOrWhiteSpace(objDataReader["SALE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()) : 0;
                                model.Vat = !string.IsNullOrWhiteSpace(objDataReader["VAT"].ToString()) ? Convert.ToDouble(objDataReader["VAT"].ToString()) : 0;
                                model.Umo = objDataReader["UMO"].ToString(); 
                                model.Category = objDataReader["PRODUCT_CATEGORY"].ToString();
                                model.SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString();
                                model.Brand = objDataReader["PRODUCT_BRAND"].ToString();
                                model.ReceivedShopId = !string.IsNullOrWhiteSpace(objDataReader["RECEIVED_SHOP_ID"].ToString()) ? Convert.ToInt32(objDataReader["RECEIVED_SHOP_ID"].ToString()) : 0;

                                objDeliveredProduct.Add(model);
                            }
                            return objDeliveredProduct;
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

        public async Task<List<DeliveredProduct>> GetAllDeliveredProduct(int shopId)
        {
            const string sql = "SELECT " +
                               "STORE_DELIVERY_NUMBER," +
                               "REGISTER_ID," +
                               "DELIVERY_SHOP_ID," +
                               "DELIVERY_SHOP_NAME," +
                               "SEASON_ID," +
                               "SEASON_NAME," +
                               "PURCHASE_RECEIVE_NUMBER," +
                               "TO_CHAR(DELIVERY_DATE,'dd/mm/yyyy')DELIVERY_DATE ," +
                               "REQUISTION_NO," +
                               "RECEIVE_CHALLAN_DELIVERY," +
                               "STORE_DELIVERY_ITEM_ID," +
                               "ITEM_ID," +
                               "PRODUCT_ID," +
                               "ITEM_NAME," +
                               "BARCODE," +
                               "DELIVERY_QUANTITY," +
                               "PURCHASE_PRICE," +
                               "SALE_PRICE," +
                               "VAT, " +
                               "UMO, " +
                               "PRODUCT_CATEGORY, " +
                               "PRODUCT_SUB_CATEGORY, " +
                               "PRODUCT_BRAND, " +
                               "RECEIVED_SHOP_ID " +
                               "FROM VEW_DELIVERED_PRODUCT WHERE DELIVERY_SHOP_ID = :DELIVERY_SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DELIVERY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DeliveredProduct> objDeliveredProduct = new List<DeliveredProduct>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DeliveredProduct model = new DeliveredProduct();
                                model.StoreDeliveryNumber = objDataReader["STORE_DELIVERY_NUMBER"].ToString();
                                model.RegisterId = objDataReader["REGISTER_ID"].ToString();
                                model.DeliveryShopId = !string.IsNullOrWhiteSpace(objDataReader["DELIVERY_SHOP_ID"].ToString()) ? Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString()) : 0;
                                model.DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString();
                                model.SeasonId = !string.IsNullOrWhiteSpace(objDataReader["SEASON_ID"].ToString()) ? Convert.ToInt32(objDataReader["SEASON_ID"].ToString()) : 0;
                                model.SeasonName = objDataReader["SEASON_NAME"].ToString();
                                model.PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString();
                                model.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                model.RequisitionNo = objDataReader["REQUISTION_NO"].ToString();
                                model.ReceiveChallanDelivery = objDataReader["RECEIVE_CHALLAN_DELIVERY"].ToString();
                                model.DeliveryItemId = !string.IsNullOrWhiteSpace(objDataReader["STORE_DELIVERY_ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["STORE_DELIVERY_ITEM_ID"].ToString()) : 0;
                                model.ItemId = !string.IsNullOrWhiteSpace(objDataReader["ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["ITEM_ID"].ToString()) : 0;
                                model.ProductId = !string.IsNullOrWhiteSpace(objDataReader["PRODUCT_ID"].ToString()) ? Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()) : 0;
                                model.ItemName = objDataReader["ITEM_NAME"].ToString();
                                model.BarCode = objDataReader["BARCODE"].ToString();
                                model.DeliveryQuantity = !string.IsNullOrWhiteSpace(objDataReader["DELIVERY_QUANTITY"].ToString()) ? Convert.ToInt32(objDataReader["DELIVERY_QUANTITY"].ToString()) : 0;
                                model.PurchasePrice = !string.IsNullOrWhiteSpace(objDataReader["PURCHASE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()) : 0;
                                model.SalePrice = !string.IsNullOrWhiteSpace(objDataReader["SALE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()) : 0;
                                model.Vat = !string.IsNullOrWhiteSpace(objDataReader["VAT"].ToString()) ? Convert.ToDouble(objDataReader["VAT"].ToString()) : 0;
                                model.Umo = objDataReader["UMO"].ToString();
                                model.Category = objDataReader["PRODUCT_CATEGORY"].ToString();
                                model.SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString();
                                model.Brand = objDataReader["PRODUCT_BRAND"].ToString();
                                model.ReceivedShopId = !string.IsNullOrWhiteSpace(objDataReader["RECEIVED_SHOP_ID"].ToString()) ? Convert.ToInt32(objDataReader["RECEIVED_SHOP_ID"].ToString()) : 0;

                                objDeliveredProduct.Add(model);
                            }
                            return objDeliveredProduct;
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

        public async Task<List<TransferReturnProduct>> GetAllReturnProduct(int shopId)
        {
            const string sql = "SELECT " +
                               "STOCK_TRANSFER_CHALLAN_NUM," +
                                "STOCK_TRANSFER_ID," +
                                "TRANSFER_SHOPID_TO," +
                                "TRANSFER_SHOPTO_NAME," +
                                "TRANSFER_SHOPID_FROM," +
                                "TRANSFER_SHOP_FROM_NAME," +
                                "TRANSFERED_BY," +
                                "TRANSFER_DATE," +
                                "RECEIVE_YN," +
                                "REQUISITION_NUM," +
                                "STOCK_TRANSFER_ITEM_ID," +
                                "ITEM_ID," +
                                "PRODUCT_ID," +
                                "ITEM_NAME," +
                                "BARCODE," +
                                "TRANSFER_QUANTITY," +
                                "PURCHASE_PRICE," +
                                "SALE_PRICE," +
                                "VAT," +
                                "UMO," +
                                "PRODUCT_CATEGORY," +
                                "PRODUCT_SUB_CATEGORY," +
                                "PRODUCT_BRAND," +
                                "WARE_HOUSE_ID " +
                               "FROM VEW_TRAN_TO_SHOP_PRODUCT WHERE TRANSFER_SHOPID_TO = :TRANSFER_SHOPID_TO AND RECEIVE_YN ='N' ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":TRANSFER_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<TransferReturnProduct> objTransferReturnProduct = new List<TransferReturnProduct>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                TransferReturnProduct model = new TransferReturnProduct();
                                model.StockTranferChallanNo = objDataReader["STOCK_TRANSFER_CHALLAN_NUM"].ToString();
                                model.StockTransferId = objDataReader["STOCK_TRANSFER_ID"].ToString();
                                model.TransferShopIdTo = !string.IsNullOrWhiteSpace(objDataReader["TRANSFER_SHOPID_TO"].ToString()) ? Convert.ToInt32(objDataReader["TRANSFER_SHOPID_TO"].ToString()) : 0;
                                model.TransferShopToName = objDataReader["TRANSFER_SHOPTO_NAME"].ToString();
                                model.TransferShopIdFrom =
                                    !string.IsNullOrWhiteSpace(objDataReader["TRANSFER_SHOPID_FROM"].ToString())
                                        ? Convert.ToInt32(objDataReader["TRANSFER_SHOPID_FROM"].ToString())
                                        : 0;
                                model.TransferShopIdFromName = objDataReader["TRANSFER_SHOP_FROM_NAME"].ToString();
                                model.TransferDate = objDataReader["TRANSFER_DATE"].ToString();
                                model.RequisitionNo = objDataReader["REQUISITION_NUM"].ToString();
                                model.StocktransferItemId = !string.IsNullOrWhiteSpace(objDataReader["STOCK_TRANSFER_ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["STOCK_TRANSFER_ITEM_ID"].ToString()) : 0;
                                model.ItemId = !string.IsNullOrWhiteSpace(objDataReader["ITEM_ID"].ToString()) ? Convert.ToInt32(objDataReader["ITEM_ID"].ToString()) : 0;
                                model.ProductId = !string.IsNullOrWhiteSpace(objDataReader["PRODUCT_ID"].ToString()) ? Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()) : 0;
                                model.ItemName = objDataReader["ITEM_NAME"].ToString();
                                model.BarCode = objDataReader["BARCODE"].ToString();
                                model.TransferQuantity = !string.IsNullOrWhiteSpace(objDataReader["TRANSFER_QUANTITY"].ToString()) ? Convert.ToInt32(objDataReader["TRANSFER_QUANTITY"].ToString()) : 0;
                                model.PurchasePrice = !string.IsNullOrWhiteSpace(objDataReader["PURCHASE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()) : 0;
                                model.SalePrice = !string.IsNullOrWhiteSpace(objDataReader["SALE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()) : 0;
                                model.Vat = !string.IsNullOrWhiteSpace(objDataReader["VAT"].ToString()) ? Convert.ToDouble(objDataReader["VAT"].ToString()) : 0;
                                model.Umo = objDataReader["UMO"].ToString();
                                model.Category = objDataReader["PRODUCT_CATEGORY"].ToString();
                                model.SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString();
                                model.Brand = objDataReader["PRODUCT_BRAND"].ToString();
                                model.TransferBy = objDataReader["TRANSFERED_BY"].ToString();
                                model.WarehouseId = !string.IsNullOrWhiteSpace(objDataReader["WARE_HOUSE_ID"].ToString()) ? Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString()) : 0;

                                objTransferReturnProduct.Add(model);
                            }
                            return objTransferReturnProduct;
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

        public async Task<string> DeliveryProductUpdateByShop(string deliveryNumber, int deliveryShop, string updateBy)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_SD_UPDATE_BY_SHOP")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = deliveryNumber;
            objOracleCommand.Parameters.Add("P_DELIVERY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = deliveryShop;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = updateBy;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objOracleCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    _trans = strConn.BeginTransaction();
                    await objOracleCommand.ExecuteNonQueryAsync();
                    _trans.Commit();
                    strConn.Close();

                    string strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();

                    return strMessage;
                }
                catch (Exception ex)
                {
                    _trans.Rollback();
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }

        }

        public async Task<string> ReturnProductUpdateByShop(string sttransferChallanNoNumber, int toShopShop, string updateBy)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_ST_UPDATE_STOCK_TR")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STOCK_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = sttransferChallanNoNumber;
            objOracleCommand.Parameters.Add("P_TO_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = toShopShop;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = updateBy;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objOracleCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    _trans = strConn.BeginTransaction();
                    await objOracleCommand.ExecuteNonQueryAsync();
                    _trans.Commit();
                    strConn.Close();

                    string strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();

                    return strMessage;
                    //return new Tuple<string, string>(strMessage, fromShopId);
                }
                catch (Exception ex)
                {
                    _trans.Rollback();
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }

        }
    }
}