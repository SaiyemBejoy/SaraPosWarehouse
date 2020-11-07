using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class DataExchangeDal
    {
        OracleTransaction _trans;

        #region "Oracle Connection Check"
        private OracleConnection GetConnection()
        {
            var conString = ConfigurationManager.ConnectionStrings["OracleDbContext"];
            string strConnString = conString.ConnectionString;
            return new OracleConnection(strConnString);
        }
        #endregion

        #region "Cusrtomer Sale"
        public async Task<string> SaveAndUpdateCustomerSale(CustomerSaleModel objCustomerSaleModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CUSTOMER_SALE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_CUSTOMER_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerSaleModel.CustomerAutoId;
            objOracleCommand.Parameters.Add("P_CUSTOMER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerSaleModel.CustomerId;
            objOracleCommand.Parameters.Add("P_CUSTOMER_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.CustomerCode) ? objCustomerSaleModel.CustomerCode : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.CustomerName) ? objCustomerSaleModel.CustomerName : null;
            objOracleCommand.Parameters.Add("P_CONTACT_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.ContactNo) ? objCustomerSaleModel.ContactNo : null;
            objOracleCommand.Parameters.Add("P_EMAIL", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.Email) ? objCustomerSaleModel.Email : null;
            objOracleCommand.Parameters.Add("P_ADDRESS", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.Address) ? objCustomerSaleModel.Address : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_MEDIUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.CustomerMedium) ? objCustomerSaleModel.CustomerMedium : null;
            objOracleCommand.Parameters.Add("P_DISCOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.Discount) ? objCustomerSaleModel.Discount : null;
            objOracleCommand.Parameters.Add("P_ENROLMENT_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.EnrollmentDate) ? objCustomerSaleModel.EnrollmentDate : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerSaleModel.ShopId) ? objCustomerSaleModel.ShopId : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_INFO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerSaleModel.CustomerInfoId;

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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }
        #endregion

        #region "Requisition"
        public async Task<string> SaveRequisitionMain(RequisitionMainModel objRequisitionMainModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_REQUISITION_MAIN_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_REQUISITION_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRequisitionMainModel.RequisitionAutoId;
            objOracleCommand.Parameters.Add("P_REQUISITION_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRequisitionMainModel.RequisitionId;
            objOracleCommand.Parameters.Add("P_REQUISITION_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainModel.RequisitionNo) ? objRequisitionMainModel.RequisitionNo : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainModel.RequisitionDate) ? objRequisitionMainModel.RequisitionDate : null;
            objOracleCommand.Parameters.Add("P_CREATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainModel.CreatedBy) ? objRequisitionMainModel.CreatedBy : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainModel.ShopId) ? objRequisitionMainModel.ShopId : null;

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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        public async Task<string> SaveRequisitionMainItem(RequisitionMainItemModel objRequisitionMainItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_REQUISITION_MAIN_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_REQUI_MAIN_ITEM_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRequisitionMainItemModel.RequisitionMainItemAutoId;
            objOracleCommand.Parameters.Add("P_REQUISITION_MAIN_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRequisitionMainItemModel.RequisitionMainItemId;
            objOracleCommand.Parameters.Add("P_REQUISITION_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRequisitionMainItemModel.RequisitionAutoId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainItemModel.Barcode) ? objRequisitionMainItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainItemModel.ItemName) ? objRequisitionMainItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainItemModel.Price) ? objRequisitionMainItemModel.Price : null;
            objOracleCommand.Parameters.Add("P_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRequisitionMainItemModel.RqsnQuantity) ? objRequisitionMainItemModel.RqsnQuantity : null;
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }
        #endregion
        

        #region "SaleInfo"
        public async Task<string> SaveSaleInfo(SaleInfoModel objSaleInfoModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SALE_INFO_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_SALE_INFO_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.SaleInfoAutoId;
            objOracleCommand.Parameters.Add("P_SALE_INFO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.SaleInfoId;
            objOracleCommand.Parameters.Add("P_INVOICE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.InvoiceNumber) ? objSaleInfoModel.InvoiceNumber : null;
            objOracleCommand.Parameters.Add("P_INVOICE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.InvoiceDate) ? objSaleInfoModel.InvoiceDate : null;
            objOracleCommand.Parameters.Add("P_SALESMAN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.SalesManId) ? objSaleInfoModel.SalesManId : null;
            objOracleCommand.Parameters.Add("P_TOTAL_ITEM", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.TotalItem;
            objOracleCommand.Parameters.Add("P_TOTAL_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.TotalAmount;
            objOracleCommand.Parameters.Add("P_VAT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.Vat;
            objOracleCommand.Parameters.Add("P_DISCOUNT_P", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.DiscountPercent;
            objOracleCommand.Parameters.Add("P_DISCOUNT_A", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.DiscountAmount;
            objOracleCommand.Parameters.Add("P_BAG_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.BagPrice;
            objOracleCommand.Parameters.Add("P_SUB_TOTAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.SubTotal;
            objOracleCommand.Parameters.Add("P_NET_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.NetAmount;
            objOracleCommand.Parameters.Add("P_CUSTOMER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.CustomerId;
            objOracleCommand.Parameters.Add("P_EXCHANGE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.ExchangeYN;
            objOracleCommand.Parameters.Add("P_EXCHANGE_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleInfoModel.ExchangeShopId;
            objOracleCommand.Parameters.Add("P_HOLD_INVOICE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.HoldInvoiceYN) ? objSaleInfoModel.HoldInvoiceYN : null;
            objOracleCommand.Parameters.Add("P_CREATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.CreatedBy) ? objSaleInfoModel.CreatedBy : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.ShopId) ? objSaleInfoModel.ShopId : null;
            objOracleCommand.Parameters.Add("P_WAREHOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSaleInfoModel.WareHouseId) ? objSaleInfoModel.WareHouseId : null;
          
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }
        public async Task<string> SaveSaleInfoItem(SaleItemModel objSaleItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SALE_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_SALE_ITEM_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.SaleItemAutoId;
            objOracleCommand.Parameters.Add("P_SALE_INFO_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.SaleInfoAutoId;
            objOracleCommand.Parameters.Add("P_SALE_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.SaleItemId;
            objOracleCommand.Parameters.Add("P_SALE_INFO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.SaleInfoId;
            objOracleCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.Barcode;
            objOracleCommand.Parameters.Add("P_STYLE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.StyleName;
            objOracleCommand.Parameters.Add("P_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.Price;
            objOracleCommand.Parameters.Add("P_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.Quantity;
            objOracleCommand.Parameters.Add("P_VAT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.Vat;
            objOracleCommand.Parameters.Add("P_TOTAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleItemModel.Total;
            
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }
        public async Task<string> SaveSalePaymentInfo(SalePaymentInfoModel objSalePaymentInfoModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SALE_PAYMENT_INFO_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PAYMENT_TYPE_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.PaymentTypeAutoId;
            objOracleCommand.Parameters.Add("P_SALE_INFO_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.SaleInfoAutoId;
            objOracleCommand.Parameters.Add("P_PAYMENT_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.PaymentTypeId;
            objOracleCommand.Parameters.Add("P_SALE_INFO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.SaleInfoId;
            objOracleCommand.Parameters.Add("P_DISCOUNT_P", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.DiscountPercent;
            objOracleCommand.Parameters.Add("P_DISCOUNT_A", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.DiscountAmount;
            objOracleCommand.Parameters.Add("P_PAYMENT_TYPE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.PaymentType;
            objOracleCommand.Parameters.Add("P_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.Amount;
            objOracleCommand.Parameters.Add("P_SUB_TOTAL", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalePaymentInfoModel.SubTotal;
           
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        #endregion

        #region ShopURL
        public async Task<List<string>> GetAllShopUrl()
        {
            var sql = "SELECT " +
                      "SHOP_URL " +
                         "FROM VEW_SHOP";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) )
                {
                    
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<string> objShopModels = new List<string>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                string ShopUrl = objDataReader["SHOP_URL"].ToString();
                                objShopModels.Add(ShopUrl);
                            }

                            return objShopModels;
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

        public async Task<List<ShopModel>> GetAllShopUrlWithId()
        {
            var sql = "SELECT " +
                      "SHOP_ID, " +
                      "SHOP_URL " +
                      "FROM VEW_SHOP";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopModel> objShopModels = new List<ShopModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopModel model = new ShopModel
                                {
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopUrl = objDataReader["SHOP_URL"].ToString()
                                   
                                };
                                objShopModels.Add(model);
                            }

                            return objShopModels;
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

        public async Task<List<ShopModel>> GetAllShopActiveShopInfo()
        {
            var sql = "SELECT " +
                      "SHOP_ID, " +
                      "SHOP_URL " +
                      "FROM SHOP WHERE ACTIVE_YN = 'Y' ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopModel> objShopModels = new List<ShopModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopModel model = new ShopModel
                                {
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopUrl = objDataReader["SHOP_URL"].ToString()
                                   
                                };
                                objShopModels.Add(model);
                            }

                            return objShopModels;
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

        public async Task<ShopModel> GetAShopUrlWithShopId(int shopId)
        {
            var sql = "SELECT " +
                      "SHOP_URL " +
                      "FROM VEW_SHOP where SHOP_ID = :SHOP_ID ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ShopModel objShopModel = new ShopModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objShopModel.ShopUrl = objDataReader["SHOP_URL"].ToString();
                            }

                            return objShopModel;
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
        #endregion

        #region WarehouseStock

        public async Task<List<WarehouseStockModel>> GetAllWareHouseProductStockList()
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "VAT," +
                        "PRODUCT_CATEGORY," +
                        "PRODUCT_SUB_CATEGORY " +
                      "FROM VEW_STOCK_PRODUCT where QUANTITY > 0";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) )
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<WarehouseStockModel> objWarehouseStockModel = new List<WarehouseStockModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                WarehouseStockModel model = new WarehouseStockModel
                                {
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    BarCode = objDataReader["BARCODE"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString()),
                                    Category = objDataReader["PRODUCT_CATEGORY"].ToString(),
                                    SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString()
                                };
                                objWarehouseStockModel.Add(model);
                            }
                            return objWarehouseStockModel;
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

        public async Task<List<WarehouseStockModel>> GetAllWareHouseProductStockListByStyleName(string styleName)
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "VAT," +
                        "PRODUCT_CATEGORY," +
                        "PRODUCT_SUB_CATEGORY " +
                        "FROM VEW_STOCK_PRODUCT where  ((lower(ITEM_NAME) like lower('" + styleName.Trim() + " %'))  or upper(ITEM_NAME)like upper('" + styleName.Trim() + " %')) ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<WarehouseStockModel> objWarehouseStockModel = new List<WarehouseStockModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                WarehouseStockModel model = new WarehouseStockModel
                                {
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    BarCode = objDataReader["BARCODE"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString()),
                                    Category = objDataReader["PRODUCT_CATEGORY"].ToString(),
                                    SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString()
                                };
                                objWarehouseStockModel.Add(model);
                            }
                            return objWarehouseStockModel;
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

        #endregion

        #region Void InvoiceNumber

        public async Task<string> VoidInvoiceNumber(string invoiceNumber,string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_VOID_INVOICE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_INVOICE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(invoiceNumber) ? invoiceNumber : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;
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

                    string message = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();

                    return message;
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

        #endregion

        #region ExDirectReceiveData

        public async Task<string> SaveShopToShopExchangeMain(ShopToShopExchangeMain objShopToShopExchangeMain)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_TO_SHOP_EX_MAIN_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExchangeMain.StoreReceiveId;
            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_CHALLAN_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.StoreReceiveChallanNo) ? objShopToShopExchangeMain.StoreReceiveChallanNo : null;
            objOracleCommand.Parameters.Add("P_RECEIVE_FROM_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.ReceiveFrom) ? objShopToShopExchangeMain.ReceiveFrom : null;
            objOracleCommand.Parameters.Add("P_RECEIVED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.ReceivedBy) ? objShopToShopExchangeMain.ReceivedBy : null;
            objOracleCommand.Parameters.Add("P_RECEIVE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.ReceivedDate) ? objShopToShopExchangeMain.ReceivedDate : null;
            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.StoreReceive_YN) ? objShopToShopExchangeMain.StoreReceive_YN : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.WareHouseId) ? objShopToShopExchangeMain.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_RECEIVED_BY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.ShopId) ? objShopToShopExchangeMain.ShopId : null;
            objOracleCommand.Parameters.Add("P_SEASON_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExchangeMain.SeasonName) ? objShopToShopExchangeMain.SeasonName : null;
            objOracleCommand.Parameters.Add("P_SHOP_EX_CHALLAN_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExchangeMain.ShopExChallan_YN;

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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        public async Task<string> SaveShopToShopExchangeMainItem(ShopToShopExItem objShopToShopExItem)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_EXCG_MAIN_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_RECEIVE_ITEM_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
            objOracleCommand.Parameters.Add("P_RECEIVE_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.StoreReceiveAutoId;
            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.StoreReceiveItemId;
            objOracleCommand.Parameters.Add("P_STORE_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.StoreReceiveId;
            objOracleCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.ProductId; 
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExItem.ItemName) ? objShopToShopExItem.ItemName : null;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExItem.Barcode) ? objShopToShopExItem.Barcode : null;
            objOracleCommand.Parameters.Add("P_RECEIVE_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.ReceiveQuantity;
            objOracleCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopExItem.SalePrice;
            objOracleCommand.Parameters.Add("P_BRAND_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopExItem.BrandName) ? objShopToShopExItem.BrandName : null;
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        #endregion

        #region giftVoucher

        public async Task<List<GiftVoucherDeliveryApiModel>> GetAllGiftVoucher()
        {
            var sql = "SELECT  * FROM  GIFT_VOUCHER_DELIVERY ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<GiftVoucherDeliveryApiModel> objGiftVoucherDeliveryApiModel = new List<GiftVoucherDeliveryApiModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                GiftVoucherDeliveryApiModel model = new GiftVoucherDeliveryApiModel
                                {
                                    GiftVoucherDeliveryId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_DELIVERY_ID"].ToString()),
                                    GiftVoucherId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString()),
                                    DeliveryItemNum = Convert.ToInt32(objDataReader["GIFT_VOUCHER_ITEM_NUM"].ToString()),
                                    GiftVoucherCode = objDataReader["GIFT_VOUCHER_CODE"].ToString(),
                                    GiftVoucherValue = objDataReader["GIFT_VOUCHER_VALUE"].ToString(),
                                    RemainingValue = Convert.ToInt32(objDataReader["GIFT_VOUCHER_REMAINING_VALUE"].ToString()),
                                    DeliveryDate = objDataReader["DELIVERY_DATE"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    DepositYN = objDataReader["DEPOSIT_YN"].ToString(),
                                    DepositShopId = objDataReader["DEPOSIT_SHOP_ID"].ToString(),

                                };
                                objGiftVoucherDeliveryApiModel.Add(model);
                            }
                            return objGiftVoucherDeliveryApiModel;
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

        public async Task<string> SaveGiftVoucherDepositData(GiftVoucherDeliveryApiModel objGiftVoucherDeliveryApiModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFTVOUCHER_DEPOSIT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };


            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.GiftVoucherCode) ? objGiftVoucherDeliveryApiModel.GiftVoucherCode : null;
            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_VALUE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.GiftVoucherValue) ? objGiftVoucherDeliveryApiModel.GiftVoucherValue : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.GiftVoucherCustomerName) ? objGiftVoucherDeliveryApiModel.GiftVoucherCustomerName : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_PHONE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.GiftVoucherCustomerPhone) ? objGiftVoucherDeliveryApiModel.GiftVoucherCustomerPhone : null;
          
            objOracleCommand.Parameters.Add("P_DEPOSIT_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.DepositShopId) ? objGiftVoucherDeliveryApiModel.DepositShopId : null;
            objOracleCommand.Parameters.Add("P_UPDATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryApiModel.UpdateBy) ? objGiftVoucherDeliveryApiModel.UpdateBy : null;
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        public async Task<string> GiftVoucherDataUpdate(GiftVoucherDeliveryApiModel objGiftVoucherDeliveryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFTVOUCHER_DATA_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.GiftVoucherCode) ? objGiftVoucherDeliveryModel.GiftVoucherCode : null;
            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_REMAIN_VALUE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherDeliveryModel.RemainingValue > 0 ? objGiftVoucherDeliveryModel.RemainingValue.ToString(CultureInfo.InvariantCulture) : "0";
            objOracleCommand.Parameters.Add("P_UPDATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.UpdateBy) ? objGiftVoucherDeliveryModel.UpdateBy : null;
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                    strConn.Dispose();
                    objOracleCommand.Dispose();
                }
            }
            return strMessage;
        }

        #endregion

        #region Erp Api Execute

        public async Task<string> ErpPurchaseMaterialReportApi(PurchaseReportModelApi reportModelApi)
        {
            string message = "";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ApiConfiguration.ERPApi);

                //HTTP POST
                var postTask = client.PostAsJsonAsync("MaterialPurchaseReport", reportModelApi);
                postTask.Wait();
                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    message = result.ReasonPhrase;
                }
                
            }
            return message;
        }

        #endregion


    }
}