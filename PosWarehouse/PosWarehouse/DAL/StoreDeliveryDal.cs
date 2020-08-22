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
    public class StoreDeliveryDal
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

        public async Task<string> MaxRequisitionNumber()
        {
            string reqNumber = "";

            var sql =
                "SELECT LPAD((NVL(MAX(TO_NUMBER(REQUISTION_NO)), 0) + 1), 8, 0)REQUISTION_NO FROM STORE_DELIVERY";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    objConnection.Open();
                    using (OracleDataReader objDataReader = (OracleDataReader) await objCommand.ExecuteReaderAsync())
                    {
                        try
                        {
                            while (objDataReader.Read())
                            {
                                reqNumber = objDataReader["REQUISTION_NO"].ToString();
                            }
                            return reqNumber;
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
        public async Task<string> GetMaxChallanNumber()
        {
            string challanNo = "";

            var sql = "SELECT    TO_CHAR (SYSDATE, 'yy')|| LPAD (TO_CHAR (SYSDATE, 'MM'), 2, 0)|| LPAD (TO_CHAR (SYSDATE, 'dd'), 2, 0)||" +
                      " (SELECT LPAD (NVL (MAX (SUBSTR ( (STORE_DELIVERY_NUMBER), 9, 8)), 0)+ 1,8,0) " +
                      " FROM STORE_DELIVERY) STORE_DELIVERY_NUMBER  FROM DUAL ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    objConnection.Open();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        try
                        {
                            while (objDataReader.Read())
                            {
                                challanNo = objDataReader["STORE_DELIVERY_NUMBER"].ToString();
                            }
                            return challanNo;
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
        public async Task<ProductInventoryModel> GetProductItemDetailsByBarcode(string barcode)
        {

            var sql = "SELECT " +
                        "PRODUCT_ID," +
                        "ITEM_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "ORDER_QUANTITY," +
                        "RECEIVE_QUANTITY," +
                        "CURRENT_STOCK," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE, " +
                        "VAT " +
                      "FROM VEW_PRODUCT_INVENTORY where BARCODE = :BARCODE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = barcode;                 
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ProductInventoryModel objProductInventoryModels = new ProductInventoryModel();
                        try
                        {
                            //DataTable dt = new DataTable();
                            //dt.Load(objDataReader);
                            
                            while (await objDataReader.ReadAsync())
                            {
                                objProductInventoryModels.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                objProductInventoryModels.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                objProductInventoryModels.ItemName = objDataReader["ITEM_NAME"].ToString();
                                objProductInventoryModels.Barcode = objDataReader["BARCODE"].ToString();
                                objProductInventoryModels.OrderQuantity =Convert.ToInt32(objDataReader["ORDER_QUANTITY"].ToString());
                                objProductInventoryModels.DeliveryQuantity =Convert.ToInt32(objDataReader["RECEIVE_QUANTITY"].ToString());
                                objProductInventoryModels.CurrentStock = Convert.ToInt32(objDataReader["CURRENT_STOCK"].ToString());
                                objProductInventoryModels.PurchasePrice =Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString());
                                objProductInventoryModels.SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                                objProductInventoryModels.Vat = Convert.ToDouble(objDataReader["VAT"].ToString());
                            }

                            return objProductInventoryModels;
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

        public async Task<List<ReceiveItem>> ReceiveChallanWiseProductItem(string purchaseReceiveNumber, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "PURCHASE_RECEIVE_ITEM_ID," +
                      "PURCHASE_RECEIVE_NUMBER," +
                      "PRODUCT_ID," +
                      "ITEM_ID," +
                      "ITEM_NAME," +
                      "BARCODE," +
                      "ORDER_QUANTITY," +
                      "RECEIVE_QUANTITY," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE, " +
                      "VAT " +
                      "FROM VEW_PURCHASE_RECEIVE_ITEM where PURCHASE_RECEIVE_NUMBER = :PURCHASE_RECEIVE_NUMBER ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_RECEIVE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseReceiveNumber;
                    
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ReceiveItem> objReceiveItemModel = new List<ReceiveItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ReceiveItem model = new ReceiveItem
                                {
                                    PurchaseReceiveItemId = Convert.ToInt32(objDataReader["PURCHASE_RECEIVE_ITEM_ID"].ToString()),
                                    PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString(),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    OrderQuantity = Convert.ToInt32(objDataReader["ORDER_QUANTITY"].ToString()),
                                    ReceiveQuantity = Convert.ToInt32(objDataReader["RECEIVE_QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),                                   
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString())                                   
                                };
                                objReceiveItemModel.Add(model);
                            }
                            return objReceiveItemModel;
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

        public async Task<Tuple<string, string>> StoreDeliverySave(StoreDeliveryModel objStoreDeliveryModel)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_STORE_DELIVERY_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STORE_DELIVERY_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStoreDeliveryModel.StoreDeliveryId;
            objOracleCommand.Parameters.Add("P_STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.DeliveryNumber) ? objStoreDeliveryModel.DeliveryNumber : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStoreDeliveryModel.DeliveryShopId > 0 ? objStoreDeliveryModel.DeliveryShopId : (object)null;
            objOracleCommand.Parameters.Add("P_REGISTER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStoreDeliveryModel.RegisterId > 0 ? objStoreDeliveryModel.RegisterId : (object)null;
            objOracleCommand.Parameters.Add("P_SEASON_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStoreDeliveryModel.SeasonId > 0 ? objStoreDeliveryModel.SeasonId : (object)null;
            objOracleCommand.Parameters.Add("P_PURCHASE_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStoreDeliveryModel.PurchaseReceiveId > 0 ? objStoreDeliveryModel.PurchaseReceiveId : (object) null;
            objOracleCommand.Parameters.Add("P_PURCHASE_RECEIVE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.PurchaseReceiveNumber) ? objStoreDeliveryModel.PurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.DeliveryDate) ? objStoreDeliveryModel.DeliveryDate : null;
            objOracleCommand.Parameters.Add("P_REQUISTION_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.RequistionNo) ? objStoreDeliveryModel.RequistionNo : null;
            objOracleCommand.Parameters.Add("P_RECEIVE_CHALLAN_DELIVERY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.ReceiveChallanDelivery) ? objStoreDeliveryModel.ReceiveChallanDelivery : null;

            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.UpdateBy) ? objStoreDeliveryModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.WareHouseId) ? objStoreDeliveryModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStoreDeliveryModel.ShopId) ? objStoreDeliveryModel.ShopId : null;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_DELIVERY_NUMBER", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string deliveryNumber = objOracleCommand.Parameters["P_DELIVERY_NUMBER"].Value.ToString();
                    string message = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();

                    return new Tuple<string, string>(message, deliveryNumber);
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

        public async Task<string> StoreDeliveryDelete(string deliveryNumber, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_STORE_DELIVERY_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(deliveryNumber) ? deliveryNumber : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
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

                    string strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();

                    return deliveryNumber;
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

        public async Task<string> StoreDeliveryItemSave(List<ProductInventoryModel> objDeliveryItem, string deliveryNumber, int deliveryShopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_STORE_DELIVERY_ITEM_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objDeliveryItem)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_STORE_DELIVERY_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = null;
                            objCommand.Parameters.Add("P_STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(deliveryNumber) ? deliveryNumber : null;
                            objCommand.Parameters.Add("P_DELIVERY_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = deliveryShopId;
                            objCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ItemId > 0 ? data.ItemId : (object)null;
                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductId > 0 ? data.ProductId : (object) null;
                            objCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.ItemName) ? data.ItemName : null;
                            objCommand.Parameters.Add("P_BARCODE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.Barcode) ? data.Barcode : null;
                            objCommand.Parameters.Add("P_DELIVERY_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.DeliveryQuantity > 0 ? data.DeliveryQuantity : (object)null;
                            objCommand.Parameters.Add("P_PURCHASE_PRICE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.PurchasePrice > 0 ? data.PurchasePrice : (object) null;
                            objCommand.Parameters.Add("P_SALE_PRICE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.SalePrice > 0 ? data.SalePrice : (object) null;
                            objCommand.Parameters.Add("P_VAT ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.Vat > 0 ? data.Vat : (object) null;

                            objCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                            using (_trans = objConnection.BeginTransaction())
                            {
                                try
                                {
                                    await objCommand.ExecuteNonQueryAsync();
                                    _trans.Commit();

                                    vMessage = objCommand.Parameters["P_MESSAGE"].Value.ToString();
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

        public async Task<string> StoreDeliveryItemDelete(string deliveryNumber, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_STORE_DELIVERY_ITEM_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = deliveryNumber;
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

        public IEnumerable<StoreDeliveryGrid> GetStoreDeliveryGrids(StoreDeliveryGrid objStoreDeliveryGrid)
        {
            var sql = "SELECT " +
                      "STORE_DELIVERY_ID," +
                        "STORE_DELIVERY_NUMBER," +
                        "DELIVERY_SHOP_ID," +
                        "DELIVERY_SHOP_NAME," +
                        "PURCHASE_RECEIVE_ID," +
                        "PURCHASE_RECEIVE_NUMBER," +
                        "to_char(DELIVERY_DATE,'dd/mm/yyyy') DELIVERY_DATE ," +
                        "REQUISTION_NO," +
                        "RECEIVE_CHALLAN_DELIVERY," +
                        "RECEIVE_YN," +
                        "RECEIVE_BY," +
                        "UPDATE_BY," +
                        "WARE_HOUSE_ID," +
                        "SHOP_ID " +
                        "FROM VEW_STOREDELIVERY_GRID WHERE 1=1 ";


            if (!string.IsNullOrWhiteSpace(objStoreDeliveryGrid.SearchBy))
            {
                sql +=
                    "and ( (lower(DELIVERY_DATE) like lower(:SearchBy)  or upper(DELIVERY_DATE)like upper(:SearchBy) )" +
                    "or (lower(PURCHASE_RECEIVE_NUMBER) like lower(:SearchBy)  or upper(PURCHASE_RECEIVE_NUMBER)like upper(:SearchBy) )" +
                    "or (lower(STORE_DELIVERY_NUMBER) like lower(:SearchBy)  or upper(STORE_DELIVERY_NUMBER)like upper(:SearchBy) )" +
                    "or (lower(RECEIVE_YN) like lower(:SearchBy)  or upper(RECEIVE_YN)like upper(:SearchBy) )" +
                    "or (lower(RECEIVE_BY) like lower(:SearchBy)  or upper(RECEIVE_BY)like upper(:SearchBy) )" +
                    "or (lower(DELIVERY_SHOP_NAME) like lower(:SearchBy)  or upper(DELIVERY_SHOP_NAME)like upper(:SearchBy) )" +
                    "or (lower(DELIVERY_DATE) like lower(:SearchBy)  or upper(DELIVERY_DATE)like upper(:SearchBy) )" +
                    "or (lower(REQUISTION_NO) like lower(:SearchBy)  or upper(REQUISTION_NO)like upper(:SearchBy) ))";

            }

            if (objStoreDeliveryGrid.DeliveryNumber != null)
            {
                sql += "ORDER BY " + objStoreDeliveryGrid.DeliveryNumber + "DESC ";
            }
            if (objStoreDeliveryGrid.DeliveryNumber == null)
            {
                sql += "  ORDER BY STORE_DELIVERY_NUMBER DESC ";
            }
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    if (!string.IsNullOrWhiteSpace(objStoreDeliveryGrid.SearchBy))
                    {
                        objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = $"%{objStoreDeliveryGrid.SearchBy}%";
                    }
                    

                    objConnection.Open();

                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        List<StoreDeliveryGrid> grid = new List<StoreDeliveryGrid>();
                        try
                        {
                            while (objDataReader.Read())
                            {
                                objStoreDeliveryGrid = new StoreDeliveryGrid();

                                objStoreDeliveryGrid.StoreDeliveryId = objDataReader["STORE_DELIVERY_ID"].ToString();
                                objStoreDeliveryGrid.DeliveryNumber = objDataReader["STORE_DELIVERY_NUMBER"].ToString();
                                objStoreDeliveryGrid.DeliveryShopId = objDataReader["DELIVERY_SHOP_ID"].ToString();
                                objStoreDeliveryGrid.DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString();
                                objStoreDeliveryGrid.PurchaseReceiveId = objDataReader["PURCHASE_RECEIVE_ID"].ToString();
                                objStoreDeliveryGrid.PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString();
                                objStoreDeliveryGrid.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                objStoreDeliveryGrid.RequistionNo = objDataReader["REQUISTION_NO"].ToString();
                                objStoreDeliveryGrid.Receive_YN = objDataReader["RECEIVE_YN"].ToString();
                                objStoreDeliveryGrid.ReceivedBy = objDataReader["RECEIVE_BY"].ToString();
                                //objStoreDeliveryGrid.ReceiveChallanDeliveryStatus = objDataReader["RECEIVE_CHALLAN_DELIVERY"].ToString() == "Y";
                               
// objStoreDeliveryGrid.UpdateBy = objDataReader["UpdateBy"].ToString();
                                //objStoreDeliveryGrid.WareHouseId = objDataReader["WareHouseId"].ToString();
                                //objStoreDeliveryGrid.ShopId = objDataReader["ShopId"].ToString();

                                grid.Add(objStoreDeliveryGrid);
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

        public async Task<List<StoreDeliveryItemModel>> GetStoreDeliveryItemList(string storeDeliveryNumber)
        {
            var sql = "SELECT " +
                      "STORE_DELIVERY_ITEM_ID," +
                        "STORE_DELIVERY_NUMBER," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "DELIVERY_QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "VAT " +
                      "FROM STORE_DELIVERY_ITEM where STORE_DELIVERY_NUMBER =:STORE_DELIVERY_NUMBER ORDER BY STORE_DELIVERY_ITEM_ID desc";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":STORE_DELIVERY_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = storeDeliveryNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<StoreDeliveryItemModel> objStoreDeliveryItemModels = new List<StoreDeliveryItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                StoreDeliveryItemModel model = new StoreDeliveryItemModel
                                {
                                    StoreDeliveryItemId = Convert.ToInt32(objDataReader["STORE_DELIVERY_ITEM_ID"].ToString()),
                                    StoreDeliveryNumber = objDataReader["STORE_DELIVERY_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    DeliveryQuantity =objDataReader["DELIVERY_QUANTITY"].ToString(),
                                    PurchasePrice = objDataReader["PURCHASE_PRICE"].ToString(),
                                    SalePrice = objDataReader["SALE_PRICE"].ToString(),
                                    Vat = objDataReader["VAT"].ToString()
                                };
                                objStoreDeliveryItemModels.Add(model);
                            }
                            return objStoreDeliveryItemModels;
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