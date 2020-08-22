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
    public class PurchaseReceiveDal
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

        public async Task<int> GetTotalRowCount(DataTableAjaxPostModel model)
        {
            string sql = "SELECT COUNT(*) COUNT FROM VEW_ITEM_PURCHASE_RECEIVE WHERE 1=1 ";

            if (!string.IsNullOrWhiteSpace(model.search.value))
            {
                sql += "and ( (lower(ProductCode) like lower(:searchBy)  or upper(ProductCode)like upper(:searchBy) )" +
                       "or (lower(ProductName) like lower(:searchBy)  or upper(ProductName)like upper(:searchBy) )" +
                       "or (lower(PurchasePrice) like lower(:searchBy)  or upper(PurchasePrice)like upper(:searchBy) )" +
                       "or (lower(SalePrice) like lower(:searchBy)  or upper(SalePrice)like upper(:searchBy) )" +
                       "or (lower(Style) like lower(:searchBy)  or upper(Style)like upper(:searchBy) )" +
                       "or (lower(ItemColor) like lower(:searchBy)  or upper(ItemColor)like upper(:searchBy) )" +
                       "or (lower(ItemSize) like lower(:searchBy)  or upper(ItemSize)like upper(:searchBy)) ) ";
            }

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    if (!string.IsNullOrWhiteSpace(model.search.value))
                        objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = $"%{model.search.value}%";

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        try
                        {
                            int rowCount = 0;
                            while (await objDataReader.ReadAsync())
                            {
                                rowCount = Convert.ToInt32(objDataReader["COUNT"].ToString());
                            }
                            return rowCount;
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

        public async Task<List<ProductItem>> GetAllProductItemForDirectPurchaseReceive(DataTableAjaxPostModel model)
        {
            //var query = "SELECT * FROM VEW_ITEM_PURCHASE_RECEIVE where 1=1 ";

            var query = "SELECT " +
                        "ROW_NUMBER () OVER (ORDER BY ProductCode) AS RN, " +
                        "ItemId," +
                      "ProductId," +
                      "ProductCode," +
                      "ProductName," +
                      "ItemName," +
                      "PurchasePrice," +
                      "SalePrice," +
                      "Style," +
                      "ItemColor," +
                      "ItemSize," +
                      "Total," +
                      "VATPERCENT," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_ITEM_PURCHASE_RECEIVE where 1=1 ";

            //if (!string.IsNullOrWhiteSpace(searchBy))
            //{
            //    query += "and ( (lower(ProductCode) like lower(:searchBy)  or upper(ProductCode)like upper(:searchBy) )" +
            //           "or (lower(ProductName) like lower(:searchBy)  or upper(ProductName)like upper(:searchBy) )" +
            //           "or (lower(PurchasePrice) like lower(:searchBy)  or upper(PurchasePrice)like upper(:searchBy) )" +
            //           "or (lower(SalePrice) like lower(:searchBy)  or upper(SalePrice)like upper(:searchBy) )" +
            //           "or (lower(Style) like lower(:searchBy)  or upper(Style)like upper(:searchBy) )" +
            //           "or (lower(ItemColor) like lower(:searchBy)  or upper(ItemColor)like upper(:searchBy) )" +
            //           "or (lower(ItemSize) like lower(:searchBy)  or upper(ItemSize)like upper(:searchBy)) ) ";
            //}

            if (!string.IsNullOrWhiteSpace(model.search.value))
            {
                query += "and ( (lower(ProductCode) like lower(:searchBy)  or upper(ProductCode)like upper(:searchBy) )" +
                         "or (lower(ProductName) like lower(:searchBy)  or upper(ProductName)like upper(:searchBy) )" +
                         "or (lower(PurchasePrice) like lower(:searchBy)  or upper(PurchasePrice)like upper(:searchBy) )" +
                         "or (lower(SalePrice) like lower(:searchBy)  or upper(SalePrice)like upper(:searchBy) )" +
                         "or (lower(Style) like lower(:searchBy)  or upper(Style)like upper(:searchBy) )" +
                         "or (lower(ItemColor) like lower(:searchBy)  or upper(ItemColor)like upper(:searchBy) )" +
                         "or (lower(ItemSize) like lower(:searchBy)  or upper(ItemSize)like upper(:searchBy)) ) ";
            }

            //query += "AND ROWNUM BETWEEN  '" + model.start + "' AND '" + (model.start + model.length) + "' ";

            if (model.order != null)
            {
                query += "ORDER BY " + model.columns[model.order[0].column].data + " " + model.order[0].dir.ToUpper();
            }

            query = "SELECT * FROM (" + query + ") WHERE RN BETWEEN  '" + model.start + "' AND '" + (model.start + model.length) + "' ";

            //query += "AND ROWNUM <= 100 ";          

            //if (orderByName != null && orderByDirection != null)
            //{
            //    query += "ORDER BY " + orderByName + " " + orderByDirection;
            //}

            var totalRow = await GetTotalRowCount(model);

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(query, objConnection) { CommandType = CommandType.Text })
                {
                    if (!string.IsNullOrWhiteSpace(model.search.value))
                    {
                        objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = $"%{model.search.value}%";
                    }                   

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {                        
                        try
                        {
                            List<ProductItem> objProductItem = new List<ProductItem>();
                            while (await objDataReader.ReadAsync())
                            {
                                ProductItem item = new ProductItem
                                {
                                    ProductId = Convert.ToInt32(objDataReader["ProductId"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ItemId"].ToString()),
                                    TotalItem = totalRow,
                                    ProductCode = objDataReader["PRODUCTCODE"].ToString(),
                                    ProductName = objDataReader["ITEMNAME"].ToString(),
                                    ItemName = objDataReader["PRODUCTNAME"].ToString(),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASEPRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALEPRICE"].ToString()),
                                    Style = objDataReader["STYLE"].ToString(),
                                    ItemColor = objDataReader["ITEMCOLOR"].ToString(),
                                    ItemSize = objDataReader["ITEMSIZE"].ToString(),
                                    VatPercent = Convert.ToDouble(objDataReader["VATPERCENT"].ToString()),
                                    IsActive = true,
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objProductItem.Add(item);
                            }
                            return objProductItem;
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

        public async Task<List<PurchaseReceiveModel>> PurchaseReceiveList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                        "PURCHASE_RECEIVE_ID," +
                        "PURCHASE_RECEIVE_NUMBER," +
                        "VENDOR_ID," +
                        "DELIVERY_SHOP_ID," +
                        "DELIVERY_SHOP_NAME," +
                        "PURCHASE_ORDER_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "to_char (PURCHASE_DATE, 'dd/mm/yyyy') PURCHASE_DATE," +
                        "WARE_HOUSE_ID," +
                        "SHOP_ID " +
                      "FROM VEW_PURCHASE_RECEIVE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ORDER BY PURCHASE_RECEIVE_NUMBER DESC ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseReceiveModel> objPurchaseReceiveModels = new List<PurchaseReceiveModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseReceiveModel model = new PurchaseReceiveModel
                                {
                                    PurchaseReceiveId = Convert.ToInt32(objDataReader["PURCHASE_RECEIVE_ID"].ToString()),
                                    PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString(),
                                    VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    DeliveryShopId = objDataReader["DELIVERY_SHOP_ID"].ToString(),
                                    DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    PurchaseOrderId = objDataReader["PURCHASE_ORDER_ID"].ToString(),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    PurchaseDate = objDataReader["PURCHASE_DATE"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objPurchaseReceiveModels.Add(model);
                            }
                            return objPurchaseReceiveModels;
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

        public async Task<List<ReceiveItem>> GetPurchaseReceiveItemList(string purchaseReceiveId)
        {
            var sql = "SELECT " +
                      "PURCHASE_RECEIVE_ITEM_ID," +
                        "PURCHASE_RECEIVE_NUMBER," +
                        "PURCHASE_RECEIVE_ID," +
                        "PRODUCT_ID," +
                        "ITEM_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "ORDER_QUANTITY," +
                        "RECEIVE_QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "VAT " +
                      "FROM VEW_PURCHASE_RECEIVE_ITEM where PURCHASE_RECEIVE_ID =:PURCHASE_RECEIVE_ID";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseReceiveId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ReceiveItem> objPurchaseReceiveItemModels = new List<ReceiveItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ReceiveItem model = new ReceiveItem
                                {
                                    PurchaseReceiveItemId = Convert.ToInt32(objDataReader["PURCHASE_RECEIVE_ITEM_ID"].ToString()),
                                    PurchaseReceiveId = Convert.ToInt32(objDataReader["PURCHASE_RECEIVE_ID"].ToString()),
                                    PurchaseReceiveNumber = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    OrderQuantity = Convert.ToInt32(objDataReader["ORDER_QUANTITY"].ToString()),
                                    ReceiveQuantity = Convert.ToInt32(objDataReader["RECEIVE_QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString())
                                };
                                objPurchaseReceiveItemModels.Add(model);
                            }
                            return objPurchaseReceiveItemModels;
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
        public async Task<List<PurchaseItem>> SearchProductByOrderNumber(string purchaseOrderNumber)
        {
            var sql = "SELECT " +
                            "PURCHASE_ORDER_ITEM_ID," +
                            "PURCHASE_ORDER_NUMBER," +
                            "PRODUCT_ID," +
                            "VENDOR_ID," +
                            "VENDOR_NAME," +
                            "TO_CHAR(DELIVERY_DATE,'dd/mm/yyyy') DELIVERY_DATE, " +
                            "TO_CHAR(ORDER_DATE,'dd/mm/yyyy') ORDER_DATE, " +
                            "TO_CHAR(SHOP_DISPLAY_DATE,'dd/mm/yyyy') SHOP_DISPLAY_DATE, " +
                            "SEASON_NAME," +
                            "DELIVERY_SHOP_ID," +
                            "DELIVERY_SHOP_NAME," +
                            "ITEM_ID," +
                            "ITEM_NAME," +
                            "ITEM_CODE," +
                            "UOM," +
                            "PURCHASE_PRICE," +
                            "SALE_PRICE," +
                            "QUANTITY," +
                            "VAT " +
                            "FROM VEW_PURCHASE_ORDER_ITEM WHERE PURCHASE_ORDER_NUMBER = : PURCHASE_ORDER_NUMBER ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseOrderNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItem> objPurchaseItem = new List<PurchaseItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItem model = new PurchaseItem
                                {
                                    PurchaseOrderItemId = Convert.ToInt32(objDataReader["PURCHASE_ORDER_ITEM_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    VendorName =objDataReader["VENDOR_NAME"].ToString(),
                                    DeliveryShopId = Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString()),
                                    DeliveryShopName =objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    OrderDate = objDataReader["ORDER_DATE"].ToString(),
                                    DisplayDate = objDataReader["SHOP_DISPLAY_DATE"].ToString(),
                                    DeliveryDate = objDataReader["DELIVERY_DATE"].ToString(),
                                    SeasonName = objDataReader["SEASON_NAME"].ToString(),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["ITEM_CODE"].ToString(),
                                    Uom = objDataReader["UOM"].ToString(),
                                    PurchasePrice = Convert.ToInt32(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToInt32(objDataReader["SALE_PRICE"].ToString()),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    VatPercent = Convert.ToDouble(objDataReader["VAT"].ToString())

                                };
                                objPurchaseItem.Add(model);
                            }
                            return objPurchaseItem;
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

        public async Task<Tuple<string,string>> SavePurchaseReceive(PurchaseReceiveModel objPurchaseReceiveModel)
        {
            string strMessage;
            string receiveId;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PURCHASE_RECEIVE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_purchase_receive_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseReceiveModel.PurchaseReceiveId;
            objOracleCommand.Parameters.Add("p_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.PurchaseReceiveNumber) ? objPurchaseReceiveModel.PurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("p_o_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.OtherPurchaseReceiveNumber) ? objPurchaseReceiveModel.OtherPurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("p_purchase_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.PurchaseDate) ? objPurchaseReceiveModel.PurchaseDate : null;
            objOracleCommand.Parameters.Add("p_vendor_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPurchaseReceiveModel.VendorId;
            objOracleCommand.Parameters.Add("p_delivery_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.DeliveryShopId) ? objPurchaseReceiveModel.DeliveryShopId : null;
            objOracleCommand.Parameters.Add("p_purchase_order_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.PurchaseOrderId) ? objPurchaseReceiveModel.PurchaseOrderId : null;
            objOracleCommand.Parameters.Add("p_purchase_order_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.PurchaseOrderNumber) ? objPurchaseReceiveModel.PurchaseOrderNumber : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.UpdateBy) ? objPurchaseReceiveModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.WareHouseId) ? objPurchaseReceiveModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPurchaseReceiveModel.ShopId) ? objPurchaseReceiveModel.ShopId : null;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_RECEIVE_NUMBER", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("p_RECEIVE_ID", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    strMessage = objOracleCommand.Parameters["P_RECEIVE_NUMBER"].Value.ToString();
                    receiveId = objOracleCommand.Parameters["p_RECEIVE_ID"].Value.ToString();
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
            return new Tuple<string, string>(strMessage, receiveId);
        }

        public async Task<string> SavePurchaseReceiveItemList(ReceiveItem objReceiveItem)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PURCHASE_RECEIVE_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_purchase_receive_item_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.PurchaseReceiveItemId;
            objOracleCommand.Parameters.Add("p_purchase_receive_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.PurchaseReceiveId;
            objOracleCommand.Parameters.Add("p_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objReceiveItem.PurchaseReceiveNumber) ? objReceiveItem.PurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("p_item_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.ItemId;
            objOracleCommand.Parameters.Add("p_product_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.ProductId;
            objOracleCommand.Parameters.Add("p_item_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objReceiveItem.ItemName) ? objReceiveItem.ItemName : null;
            objOracleCommand.Parameters.Add("p_order_quantity", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.OrderQuantity;
            objOracleCommand.Parameters.Add("p_purchase_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.PurchasePrice;
            objOracleCommand.Parameters.Add("p_barcode", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objReceiveItem.Barcode) ? objReceiveItem.Barcode : null;
            objOracleCommand.Parameters.Add("p_receive_quantity", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.ReceiveQuantity;
            objOracleCommand.Parameters.Add("p_vat", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.Vat;
            objOracleCommand.Parameters.Add("p_sale_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objReceiveItem.SalePrice;
             objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objReceiveItem.WareHouseId) ? objReceiveItem.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objReceiveItem.ShopId) ? objReceiveItem.ShopId : null;

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

                    strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();
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

        public async Task<ProductItem> SelectProductItemByCheckBox(int itemId)
        {
            var sql = "SELECT " +
                      "ItemId," +
                      "ProductId," +
                      "ProductCode," +
                      "ProductName," +
                      "ItemName," +
                      "PurchasePrice," +
                      "SalePrice," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_ITEM_PURCHASE_RECEIVE where ItemId = :ItemId ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":ItemId", OracleDbType.Varchar2, ParameterDirection.Input).Value = itemId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ProductItem objProductItemModels = new ProductItem();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {


                                objProductItemModels.ProductId = Convert.ToInt32(objDataReader["ProductId"].ToString());
                                objProductItemModels.ItemId = Convert.ToInt32(objDataReader["ItemId"].ToString());
                                objProductItemModels.ProductCode = objDataReader["PRODUCTCODE"].ToString();
                                objProductItemModels.ProductName = objDataReader["ITEMNAME"].ToString();
                                objProductItemModels.ItemName = objDataReader["PRODUCTNAME"].ToString();
                                objProductItemModels.PurchasePrice =
                                    Convert.ToDouble(objDataReader["PURCHASEPRICE"].ToString());
                                objProductItemModels.SalePrice =
                                    Convert.ToDouble(objDataReader["SALEPRICE"].ToString());
                                objProductItemModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objProductItemModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objProductItemModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objProductItemModels;
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

            var sql = "SELECT  LPAD ((NVL (MAX (PURCHASE_RECEIVE_NUMBER), 0) + 1), 8, 0) PURCHASE_RECEIVE_NUMBER  FROM PURCHASE_RECEIVE ";

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
                                challanNo = objDataReader["PURCHASE_RECEIVE_NUMBER"].ToString();
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

        public async Task<ProductItem> GetProductInfoForScanReceive(string barcode)
        {
            var sql = "SELECT " +
                      "ITEMID," +
                        "PRODUCTID," +
                        "PRODUCTCODE," +
                        "PRODUCTNAME," +
                        "ITEMNAME," +
                        "PURCHASEPRICE," +
                        "SALEPRICE," +
                        "VATPERCENT " +
                      "FROM VEW_ITEM_PURCHASE_RECEIVE where PRODUCTCODE = :PRODUCTCODE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCTCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = barcode;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ProductItem objProductItem = new ProductItem();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objProductItem.ProductId = Convert.ToInt32(objDataReader["PRODUCTID"].ToString());
                                objProductItem.ItemId = Convert.ToInt32(objDataReader["ITEMID"].ToString());
                                objProductItem.ItemName = objDataReader["PRODUCTNAME"].ToString();
                                objProductItem.ProductCode = objDataReader["PRODUCTCODE"].ToString();
                                objProductItem.PurchasePrice = Convert.ToDouble(objDataReader["PURCHASEPRICE"].ToString());
                                objProductItem.SalePrice = Convert.ToDouble(objDataReader["SALEPRICE"].ToString());
                                objProductItem.VatPercent = Convert.ToDouble(objDataReader["VATPERCENT"].ToString());
                            }
                            return objProductItem;
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

        public async Task<List<PurchaseItem>> GetProductInfoForSOLScanReceive(string OtherPurchaseReceiveNumber)
        {
            var sql = "SELECT " +
                            "O_PURCHASE_RECEIVE_ITEM_ID," +
                            "O_PURCHASE_RECEIVE_NUMBER," +
                            "ITEM_ID," +
                            "ITEM_NAME," +
                            "PRODUCT_ID," +
                            "BARCODE," +
                            "RECEIVE_QUANTITY," +
                            "SALE_PRICE," +
                            "PURCHASE_PRICE," +
                            "VAT "+
                            "FROM VEW_OTHER_PO_RECEIVE_ITEM WHERE O_PURCHASE_RECEIVE_NUMBER = : O_PURCHASE_RECEIVE_NUMBER ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":O_PURCHASE_RECEIVE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = OtherPurchaseReceiveNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PurchaseItem> objPurchaseItem = new List<PurchaseItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PurchaseItem model = new PurchaseItem
                                {
                                    PurchaseOrderItemId = Convert.ToInt32(objDataReader["O_PURCHASE_RECEIVE_ITEM_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["O_PURCHASE_RECEIVE_NUMBER"].ToString(),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    //VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    //VendorName = objDataReader["VENDOR_NAME"].ToString(),
                                    //DeliveryShopId = Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString()),
                                    //DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    //OrderDate = objDataReader["ORDER_DATE"].ToString(),
                                    //DisplayDate = objDataReader["SHOP_DISPLAY_DATE"].ToString(),
                                    //DeliveryDate = objDataReader["DELIVERY_DATE"].ToString(),
                                    //SeasonName = objDataReader["SEASON_NAME"].ToString(),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    //Uom = objDataReader["UOM"].ToString(),
                                    PurchasePrice = Convert.ToInt32(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToInt32(objDataReader["SALE_PRICE"].ToString()),
                                    Quantity = Convert.ToInt32(objDataReader["RECEIVE_QUANTITY"].ToString()),
                                    VatPercent = Convert.ToDouble(objDataReader["VAT"].ToString())

                                };
                                objPurchaseItem.Add(model);
                            }
                            return objPurchaseItem;
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

        public async Task<string> UpdateOtherPurchaseReceive(string OtherPurchaseReceiveNumber ,string UpdatedBy)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_UPDATE_OTHER_PORECEIVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_O_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = OtherPurchaseReceiveNumber;
            objOracleCommand.Parameters.Add("p_Updated_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = UpdatedBy;
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

                    strMessage = objOracleCommand.Parameters["p_message"].Value.ToString();
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

    }
}