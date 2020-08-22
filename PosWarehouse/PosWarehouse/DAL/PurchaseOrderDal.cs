using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Antlr.Runtime.Misc;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.Controllers;
using PosWarehouse.ViewModel;

namespace PosWarehouse.DAL
{
    public class PurchaseOrderDal
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

        public async Task<List<PurchaseOrderModel>> PurchaseOrderList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "PURCHASE_ORDER_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "VENDOR_ID," +
                        "to_char (ORDER_DATE, 'dd/mm/yyyy') ORDER_DATE," +
                         "to_char(DELIVERY_DATE, 'dd/mm/yyyy') DELIVERY_DATE," +
                        "DELIVERY_SHOP_ID," +
                        "DELIVERY_SHOP_NAME," +
                        "SEASON_ID," +
                        "SEASON_NAME," +
                        "WARE_HOUSE_ID," +
                        "SHOP_ID " +
                      "FROM VEW_PURCHASE_ORDER where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID  ORDER BY PURCHASE_ORDER_NUMBER DESC ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseOrderModel> objPurchaseOrderModels = new List<PurchaseOrderModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseOrderModel model = new PurchaseOrderModel
                                {
                                    PurchaseOrderId = Convert.ToInt32(objDataReader["PURCHASE_ORDER_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    OrderDate = objDataReader["ORDER_DATE"].ToString(),
                                    DeliveryDate = objDataReader["DELIVERY_DATE"].ToString(),
                                    DeliveryShopId = Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString()),
                                    DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    SeasonId = Convert.ToInt32(objDataReader["SEASON_ID"].ToString()),
                                    SeasonName = objDataReader["SEASON_NAME"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objPurchaseOrderModels.Add(model);
                            }
                            return objPurchaseOrderModels;
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

        public async Task<List<PurchaseItem>> GetPurchaseOrderItemList(string purchaseOrderNumber)
        {


            var sql = "SELECT " +
                      "PURCHASE_ORDER_ITEM_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "PRODUCT_ID," +
                        "ITEM_ID," +
                        "ITEM_NAME," +
                        "ITEM_CODE," +
                        "UOM," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "QUANTITY," +
                        "VAT " +
                      "FROM VEW_PURCHASE_ORDER_ITEM where PURCHASE_ORDER_NUMBER =:PURCHASE_ORDER_NUMBER ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseOrderNumber;
                  
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItem> objPurchaseItemModels = new List<PurchaseItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItem model = new PurchaseItem
                                {
                                    PurchaseOrderItemId = Convert.ToInt32(objDataReader["PURCHASE_ORDER_ITEM_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["ITEM_CODE"].ToString(),
                                    Uom = objDataReader["UOM"].ToString(),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    VatPercent = Convert.ToDouble(objDataReader["VAT"].ToString())

                                };
                                objPurchaseItemModels.Add(model);
                            }
                            return objPurchaseItemModels;
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

        public async Task<List<string>> GetProductGrids(string query)
        {
            query = "%" + query + "%";

            var sql = "SELECT SEARCH_NAME FROM VEW_PRODUCT_ITEM_SEARCH WHERE LOWER(SEARCH_NAME) LIKE :SEARCH_NAME ";
            sql += "AND ROWNUM <= 10 ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SEARCH_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = query.ToLower();

                    await objConnection.OpenAsync();

                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<string> grid = new List<string>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                string name = objDataReader["SEARCH_NAME"].ToString();
                                grid.Add(name);
                            }
                            return grid;
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

        public async Task<List<PurchaseItem>> GetItemList(string searchKey, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                      "PRODUCT_ID," +
                      "PRODUCT_NAME," +
                      "PRODUCT_STYLE," +
                      "ITEM_CODE," +
                      "ITEM_NAME," +
                      "ITEM_ATTR_NAME," +
                      "SALES_MEASURING_UNIT_ID," +
                      "SALES_MEASURING_UNIT_NAME," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE," +
                      "BRAND_ID," +
                      "BRAND_NAME," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID, " +
                      "VAT_PERCENT " +
                      "FROM VEW_PRODUCT_ITEM_INFO WHERE WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            sql += "AND ( lower(PRODUCT_NAME) = lower(:SearchKey) " +
                   "OR lower(PRODUCT_STYLE) = lower(:SearchKey) " +
                   "OR lower(ITEM_CODE) = lower(:SearchKey) " +
                   "OR lower(ITEM_NAME) = lower(:SearchKey) )";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    objCommand.Parameters.Add(":SearchKey", OracleDbType.Varchar2, ParameterDirection.Input).Value = searchKey.Trim();

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItem> objItemList = new List<PurchaseItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItem model = new PurchaseItem();

                                model.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                model.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                model.ItemName = objDataReader["ITEM_NAME"].ToString();
                                model.Barcode = objDataReader["ITEM_CODE"].ToString();
                                model.Uom = objDataReader["SALES_MEASURING_UNIT_NAME"].ToString();
                                model.LastPurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString());
                                model.PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString());
                                model.SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                                model.VatPercent = Convert.ToDouble(objDataReader["VAT_PERCENT"].ToString());
                                model.IsActiveItem = true;
                                //model.VatPercent = 0.0;
                                objItemList.Add(model);
                            }

                            return objItemList;
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

        public async Task<string> PurchaseOrderSave(PurchaseOrderModel objPurchaseOrderModel)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PURCHASE_ORDER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseOrderModel.PurchaseOrderId;
            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.PurchaseOrderNumber) ? objPurchaseOrderModel.PurchaseOrderNumber : null;
            objOracleCommand.Parameters.Add("P_VENDOR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseOrderModel.VendorId > 0 ? objPurchaseOrderModel.VendorId : (object)null;
            objOracleCommand.Parameters.Add("P_ORDER_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.OrderDate) ? objPurchaseOrderModel.OrderDate : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.DeliveryDate) ? objPurchaseOrderModel.DeliveryDate : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseOrderModel.DeliveryShopId > 0 ? objPurchaseOrderModel.DeliveryShopId : (object)null;
            objOracleCommand.Parameters.Add("P_SEASON_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseOrderModel.SeasonId > 0 ? objPurchaseOrderModel.SeasonId : (object)null;
            objOracleCommand.Parameters.Add("P_SHOP_DISPLAY_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.DisplayDate) ? objPurchaseOrderModel.DisplayDate : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.UpdateBy) ? objPurchaseOrderModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.WareHouseId) ? objPurchaseOrderModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseOrderModel.ShopId) ? objPurchaseOrderModel.ShopId : null;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_ORDER_NUMBER", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string orderNumber = objOracleCommand.Parameters["P_ORDER_NUMBER"].Value.ToString();

                    return orderNumber;
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

        public async Task<string> PurchaseOrderDelete(string orderNumber, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PURCHASE_ORDER_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(orderNumber) ? orderNumber : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();

                    return orderNumber;
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

        public async Task<string> PurchaseOrderItemSave(List<PurchaseItem> objPurchaseItem, string orderNumber, string employeeId, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PURCHASE_ORDER_ITEM_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objPurchaseItem)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(orderNumber) ? orderNumber : null;
                            objCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ItemId > 0 ? data.ItemId : (object)null;
                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductId > 0 ? data.ProductId : (object)null;
                            objCommand.Parameters.Add("P_ITEM_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.Barcode) ? data.Barcode : null;
                            objCommand.Parameters.Add("P_PURCHASE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.PurchasePrice > 0 ? data.PurchasePrice : (object)null;
                            objCommand.Parameters.Add("P_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.Quantity > 0 ? data.Quantity : null;
                            objCommand.Parameters.Add("P_VAT ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.VatPercent;

                            objCommand.Parameters.Add("P_UPDATE_BY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = employeeId;
                            objCommand.Parameters.Add("P_WARE_HOUSE_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                            objCommand.Parameters.Add("P_SHOP_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                            objCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                            using (_trans = objConnection.BeginTransaction())
                            {
                                try
                                {
                                    await objCommand.ExecuteNonQueryAsync();
                                    _trans.Commit();

                                    vMessage = objCommand.Parameters["p_message"].Value.ToString();
                                }
                                catch (Exception ex)
                                {
                                    _trans.Rollback();
                                    throw new Exception("Error : " + ex.Message);
                                }
                                finally
                                {
                                    _trans.Dispose();
                                }
                            }
                        }
                        return vMessage;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error : " + ex.Message);
                    }
                    finally
                    {
                        objConnection.Close();
                        objCommand.Dispose();
                        objConnection.Dispose();
                    }
                }
            }
        }

        public async Task<string> PurchaseOrderFabricSave(List<FabricModel> objFabricModel, string orderNumber, string employeeId, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PURCHASE_ORDER_FABRIC_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objFabricModel)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(orderNumber) ? orderNumber : null;
                            objCommand.Parameters.Add("P_FABRIC_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.FabricId > 0 ? data.FabricId : (object)null;
                            objCommand.Parameters.Add("P_GARMENTS_TYPE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.GarmentsType) ? data.GarmentsType : "";
                            objCommand.Parameters.Add("P_FABRIC_TYPE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.FabricType) ? data.FabricType : "";
                            objCommand.Parameters.Add("P_FABRIC_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.FabricCode) ? data.FabricCode : "";
                            objCommand.Parameters.Add("P_CONSUMPTION", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.Consumption) ? data.Consumption : "";
                            objCommand.Parameters.Add("P_FABRIC_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.FabricQuantity) ? data.FabricQuantity : "";
                            objCommand.Parameters.Add("P_WARE_HOUSE_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                            objCommand.Parameters.Add("P_SHOP_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                            objCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                            using (_trans = objConnection.BeginTransaction())
                            {
                                try
                                {
                                    await objCommand.ExecuteNonQueryAsync();
                                    _trans.Commit();

                                    vMessage = objCommand.Parameters["p_message"].Value.ToString();
                                }
                                catch (Exception ex)
                                {
                                    _trans.Rollback();
                                    throw new Exception("Error : " + ex.Message);
                                }
                                finally
                                {
                                    _trans.Dispose();
                                }
                            }
                        }
                        return vMessage;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error : " + ex.Message);
                    }
                    finally
                    {
                        objConnection.Close();
                        objCommand.Dispose();
                        objConnection.Dispose();
                    }
                }
            }
        }


        public async Task<string> PurchaseOrderItemDelete(string orderNumber, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PURCHASE_ORDER_ITEM_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = orderNumber;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();

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

        public async Task<string> PurchaseOrderAndItemDelete(string orderNumber, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PO_AND_ITEM_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = orderNumber;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();

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

        public async Task<PurchaserOrderModelForReport> GetPurchaseOrderInfoForHtmlRpt(string poNumber)
        {
            var sql = "SELECT " +
                      "PURCHASE_ORDER_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                      "to_char (ORDER_DATE, 'dd/mm/yyyy') ORDER_DATE," +
                      "to_char(DELIVERY_DATE, 'dd/mm/yyyy') DELIVERY_DATE," +
                      "to_char(SHOP_DISPLAY_DATE, 'dd/mm/yyyy') SHOP_DISPLAY_DATE," +
                        "VENDOR_ID," +
                        "DELIVERY_SHOP_ID," +
                        "DELIVERY_SHOP_NAME," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "SEASON_ID," +
                        "SEASON_NAME " +
                      "FROM VEW_RPT_HTML_PO_ORDER_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber; 
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        PurchaserOrderModelForReport objPurchaserOrderModelForReport = new PurchaserOrderModelForReport();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objPurchaserOrderModelForReport.PurchaseOrderId = Convert.ToInt32(objDataReader["PURCHASE_ORDER_ID"].ToString());
                                objPurchaserOrderModelForReport.PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString();
                                objPurchaserOrderModelForReport.OrderDate = objDataReader["ORDER_DATE"].ToString();
                                objPurchaserOrderModelForReport.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                objPurchaserOrderModelForReport.DisplayDate = objDataReader["SHOP_DISPLAY_DATE"].ToString();
                                objPurchaserOrderModelForReport.DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString();
                                objPurchaserOrderModelForReport.SeasonName = objDataReader["SEASON_NAME"].ToString();
                                objPurchaserOrderModelForReport.CreatedBy = objDataReader["CREATE_BY"].ToString();
                                objPurchaserOrderModelForReport.CreatedDate = objDataReader["CREATE_DATE"].ToString();
                            }
                            return objPurchaserOrderModelForReport;
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

        public async Task<List<PurchaseItemRpt>> PurchaseOrderItemListForHtmlRpt(string poNumber)
        {
            var sql = "SELECT " +
                      "PURCHASE_ORDER_NUMBER," +
                        "PRODUCT_ID," +
                        "PRODUCT_STYLE," +
                        "PRODUCT_NAME," +
                        "CATEGORY_NAME," +
                        "SUB_CATEGORY_NAME," +
                        "KARCUPI," +
                        "WASH," +
                        "PRINT," +
                        "EMBROIDERY," +
                        "DESIGNER_NAME," +
                        "MERCHANDISER_NAME," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE " +
                        "FROM VEW_RPT_HTML_PO_ITEM_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                  
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemRpt> objPurchaseItemRpt = new List<PurchaseItemRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemRpt model = new PurchaseItemRpt
                                {  
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ProductType = objDataReader["PRODUCT_NAME"].ToString(),
                                    ProductStyle = objDataReader["PRODUCT_STYLE"].ToString(),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    Category = objDataReader["CATEGORY_NAME"].ToString(),
                                    Subcategory = objDataReader["SUB_CATEGORY_NAME"].ToString(),
                                    Designer = objDataReader["DESIGNER_NAME"].ToString(),
                                    Merchandiser = objDataReader["MERCHANDISER_NAME"].ToString(),
                                    Wash = objDataReader["WASH"].ToString(),
                                    Print = objDataReader["PRINT"].ToString(),
                                    Karcupi = objDataReader["KARCUPI"].ToString(),
                                    Embroidery = objDataReader["EMBROIDERY"].ToString()
                                    
                                };
                                objPurchaseItemRpt.Add(model);
                            }
                            return objPurchaseItemRpt;
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

        public async Task<List<FabricModel>> PurchaseOrderFabricListForHtmlRpt(string poNumber)
        {
            var sql = "SELECT " +
                        "FABRIC_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "GARMENTS_TYPE," +
                        "FABRIC_TYPE," +
                        "FABRIC_CODE," +
                        "CONSUMPTION," +
                        "FABRIC_QUANTITY " +
                        "FROM VEW_PURCHASE_ORDER_FABRIC where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<FabricModel> objFabricModel = new List<FabricModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                FabricModel model = new FabricModel
                                {
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    GarmentsType = objDataReader["GARMENTS_TYPE"].ToString(),
                                    FabricType = objDataReader["FABRIC_TYPE"].ToString(),
                                    FabricCode = objDataReader["FABRIC_CODE"].ToString(),
                                    Consumption = objDataReader["CONSUMPTION"].ToString(),
                                    FabricQuantity = objDataReader["FABRIC_QUANTITY"].ToString()
                                   
                                };
                                objFabricModel.Add(model);
                            }
                            return objFabricModel;
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

        public async Task<List<PurchaseItemColorRpt>> PurchaseOrderColorListForHtmlRpt(string poNumber, int productId)
        {
            var sql = "SELECT DISTINCT " +
                        "ITEM_COLOR " +
                        "FROM VEW_RPT_HTML_PO_COLOR_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER AND PRODUCT_ID = :PRODUCT_ID ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemColorRpt> objPurchaseItemColorRpt = new List<PurchaseItemColorRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemColorRpt model = new PurchaseItemColorRpt
                                {
                                    ColorName = objDataReader["ITEM_COLOR"].ToString()
                                };
                                objPurchaseItemColorRpt.Add(model);
                            }
                            return objPurchaseItemColorRpt;
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

        public async Task<List<PurchaseItemSizeRpt>> PurchaseOrderSizeListForHtmlRpt(string poNumber, int productId)
        {
            var sql = "SELECT DISTINCT " +
                        "ITEM_SIZE, " +
                        "SUM(QUANTITY) QUANTITY " +
                        "FROM VEW_RPT_HTML_PO_SIZE_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER AND PRODUCT_ID = :PRODUCT_ID GROUP BY ITEM_SIZE ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;


                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemSizeRpt> objPurchaseItemSizeRpt = new List<PurchaseItemSizeRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemSizeRpt model = new PurchaseItemSizeRpt
                                {
                                    SizeName = objDataReader["ITEM_SIZE"].ToString(),
                                    Quantity= Convert.ToInt32(objDataReader["QUANTITY"].ToString())
                                };
                                objPurchaseItemSizeRpt.Add(model);
                            }
                            return objPurchaseItemSizeRpt;
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
        public async Task<List<PurchaseItemSizeRpt>> PoSizeListForHtmlRpt(string poNumber)
        {
            var sql = "SELECT " +
                      "ITEM_ID, " +
                        "ITEM_SIZE, " +
                        "QUANTITY " +
                        "FROM VEW_RPT_HTML_PO_SIZE_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                    //objCommand.Parameters.Add(":ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = itemId;


                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemSizeRpt> objPurchaseItemSizeRpt = new List<PurchaseItemSizeRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemSizeRpt model = new PurchaseItemSizeRpt
                                {
                                    SizeName = objDataReader["ITEM_SIZE"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString())
                                };
                                objPurchaseItemSizeRpt.Add(model);
                            }
                            return objPurchaseItemSizeRpt;
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

        public async Task<List<PurchaseItemSizeRpt>> PurchaseOrderSizeListForHtmlHeadingRpt(string poNumber)
        {
            var sql = "SELECT DISTINCT " +
                        "ITEM_SIZE " +
                        "FROM VEW_RPT_HTML_PO_SIZE_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER ORDER BY ITEM_SIZE ASC ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                 
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemSizeRpt> objPurchaseItemSizeRpt = new List<PurchaseItemSizeRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemSizeRpt model = new PurchaseItemSizeRpt
                                {
                                    SizeName = objDataReader["ITEM_SIZE"].ToString()
                                
                                };
                                objPurchaseItemSizeRpt.Add(model);
                            }
                            return objPurchaseItemSizeRpt;
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

        public async Task<List<PurchaseItemFitRpt>> PurchaseOrderFitListForHtmlRpt(string poNumber, int productId)
        {
            var sql = "SELECT " +
                        "PURCHASE_ORDER_NUMBER," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_FIT " +
                        "FROM VEW_RPT_HTML_PO_FIT_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER AND PRODUCT_ID = :PRODUCT_ID ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemFitRpt> objPurchaseItemFitRpt = new List<PurchaseItemFitRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemFitRpt model = new PurchaseItemFitRpt
                                {
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    Fit = objDataReader["ITEM_FIT"].ToString()
                                };
                                objPurchaseItemFitRpt.Add(model);
                            }
                            return objPurchaseItemFitRpt;
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


        public async Task<List<PurchaseItemColorRpt>> PurchaseOrderColorListForHtmlRpt(string poNumber)
        {
            var sql = "SELECT DISTINCT " +
                        "ITEM_COLOR " +
                        "FROM VEW_RPT_HTML_PO_COLOR_DETAILS where PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = poNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItemColorRpt> objPurchaseItemColorRpt = new List<PurchaseItemColorRpt>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItemColorRpt model = new PurchaseItemColorRpt
                                {
                                    ColorName = objDataReader["ITEM_COLOR"].ToString()
                                };
                                objPurchaseItemColorRpt.Add(model);
                            }
                            return objPurchaseItemColorRpt;
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

        #region Test GetColorAndSize

        public async Task<List<ProductColorSizeList>> GetAllProductColorSize(int productId)
        {
            var sql = @"SELECT DISTINCT P.PRODUCT_ID,
            P.PRODUCT_STYLE,
            0 ITEM_ID,
            PIA.ATTRIBUTE_VALUE_ID COLOR_ID,
                PIA.ATTRIBUTE_VALUE_NAME COLOR_NAME,
                PIA2.ATTRIBUTE_VALUE_ID SIZE_ID,
                PIA2.ATTRIBUTE_VALUE_NAME SIZE_NAME
            FROM PRODUCT P
                INNER JOIN PRODUCT_ITEM_ATTRIBUTE PIA
            ON(P.PRODUCT_ID = PIA.PRODUCT_ID AND PIA.ATTRIBUTE_ID = 5)
            INNER JOIN PRODUCT_ITEM_ATTRIBUTE PIA2
            ON(P.PRODUCT_ID = PIA2.PRODUCT_ID AND PIA2.ATTRIBUTE_ID = 4) WHERE P.PRODUCT_ID = :PRODUCT_ID
            ORDER BY PRODUCT_ID,
                PRODUCT_STYLE,
                ITEM_ID,
                COLOR_NAME,
                SIZE_NAME";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Int32, ParameterDirection.Input).Value = productId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductColorSizeList> productColorSizeLists = new List<ProductColorSizeList>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductColorSizeList model = new ProductColorSizeList();

                                model.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                model.StyleName = objDataReader["PRODUCT_STYLE"].ToString();
                                model.ColorId = Convert.ToInt32(objDataReader["COLOR_ID"].ToString());
                                model.ColorName = objDataReader["COLOR_NAME"].ToString();
                                model.SizeId = Convert.ToInt32(objDataReader["SIZE_ID"].ToString());
                                model.SizeName = objDataReader["SIZE_NAME"].ToString();

                                model.ItemId = GetProductItemId(model.ProductId, model.ColorId, model.SizeId);

                                productColorSizeLists.Add(model);
                            }
                            return productColorSizeLists;
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

        //public async Task<List<PurchaseItemColorRpt>> GetAllProductColor(int productId)
        //{
        //    var sql = "SELECT * VEW_PRODUCT_COLOR_SIZE WHERE PRODUCT_ID = :PRODUCT_ID";

        //    using (OracleConnection objConnection = GetConnection())
        //    {
        //        using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
        //        {
        //            objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Int32, ParameterDirection.Input).Value = productId;
        //            await objConnection.OpenAsync();
        //            using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
        //            {
        //                List<PurchaseItemColorRpt> productColorSizeLists = new List<PurchaseItemColorRpt>();
        //                try
        //                {
        //                    while (await objDataReader.ReadAsync())
        //                    {
        //                        PurchaseItemColorRpt model = new PurchaseItemColorRpt();

        //                        model.ColorId = Convert.ToInt32(objDataReader["COLOR_ID"].ToString());
        //                        model.ColorName = objDataReader["COLOR_NAME"].ToString();

        //                        model.SizeModelList = await GetProductSizeList(productId, model.ColorId);

        //                        productColorSizeLists.Add(model);
        //                    }
        //                    return productColorSizeLists;
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw new Exception("Error : " + ex.Message);
        //                }
        //                finally
        //                {
        //                    objDataReader.Dispose();
        //                    objCommand.Dispose();
        //                    objConnection.Dispose();
        //                }
        //            }


        //        }
        //    }
        //}

        public int GetItemQuantity(int itemId, string poNumber)
        {
            var sql = "SELECT QUANTITY FROM PURCHASE_ORDER_ITEM WHERE PURCHASE_ORDER_NUMBER = :PURCHASE_ORDER_NUMBER AND ITEM_ID = :ITEM_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Int32, ParameterDirection.Input).Value = poNumber;
                    objCommand.Parameters.Add(":ITEM_ID", OracleDbType.Int32, ParameterDirection.Input).Value = itemId;

                    objConnection.Open();
                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        try
                        {
                            int quantity = 0;
                            while (objDataReader.Read())
                            {
                                quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString());
                            }
                            return quantity;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error : " + e.Message);
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

        public int GetProductItemId(int productId, int colorId, int sizeId)
        {
            var sql = @"SELECT PRODUCT_ITEM_ID
            FROM PRODUCT_ITEM_ATTRIBUTE
            WHERE PRODUCT_ID = :PRODUCT_ID
            GROUP BY PRODUCT_ITEM_ID
                HAVING SUM(CASE ATTRIBUTE_VALUE_ID WHEN :COLOR_ID THEN 1 ELSE 0 END) > 0
            AND SUM(CASE ATTRIBUTE_VALUE_ID WHEN :SIZE_ID THEN 1 ELSE 0 END) > 0";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Int32, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":COLOR_ID", OracleDbType.Int32, ParameterDirection.Input).Value = colorId;
                    objCommand.Parameters.Add(":SIZE_ID", OracleDbType.Int32, ParameterDirection.Input).Value = sizeId;

                    objConnection.Open();
                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        try
                        {
                            int itemId = 0;
                            while (objDataReader.Read())
                            {
                                itemId = Convert.ToInt32(objDataReader["PRODUCT_ITEM_ID"].ToString());
                            }
                            return itemId;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error : " + e.Message);
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

    }
}