using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class ShopToShopRequisitionDAL
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

        public async Task<List<ShopModel>> GetShopList()
        {
            var sql = "SELECT * FROM VEW_SHOP WHERE ACTIVE_YN = 'Y' ";

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
                                ShopModel model = new ShopModel()
                                {
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    WareHouseId = Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString()),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString(),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),
                                    WareHouseName = objDataReader["WARE_HOUSE_NAME"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    PostalCode = objDataReader["POSTAL_CODE"].ToString(),
                                    DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString(),
                                    VatNo = objDataReader["VAT_NO"].ToString(),
                                    TINNo = objDataReader["TIN_NO"].ToString(),
                                    BINNo = objDataReader["BIN_NO"].ToString(),
                                    FAXNo = objDataReader["FAX_NO"].ToString(),
                                    EmailAddress = objDataReader["EMAIL_ADDRESS"].ToString(),
                                    ShopAddress = objDataReader["SHOP_ADDRESS"].ToString(),
                                    IsActive = objDataReader["ACTIVE_YN"].ToString() == "Y",
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString()

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
        public async Task<List<ShopToShopRequisitionModel>> GetShopStockList( int shopId)
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "SHOP_ID," +
                        "PRODUCT_CATEGORY," +
                        "PRODUCT_SUB_CATEGORY  " +
                      "FROM VEW_ALL_SHOP_STOCK where SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequisitionModel> objShopToShopRequisitionModel = new List<ShopToShopRequisitionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequisitionModel model = new ShopToShopRequisitionModel
                                {
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    ShoId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    Category = objDataReader["PRODUCT_CATEGORY"].ToString(),
                                    SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString()

                                };
                                objShopToShopRequisitionModel.Add(model);
                            }
                            return objShopToShopRequisitionModel;
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
        public async Task<List<ShopToShopRequisitionModel>> GetShopSearchProductList(int shopId, string styleName)
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "PURCHASE_PRICE," +
                        "SALE_PRICE," +
                        "SHOP_ID," +
                        "PRODUCT_CATEGORY," +
                        "PRODUCT_SUB_CATEGORY  " +
                      "FROM VEW_ALL_SHOP_STOCK where SHOP_ID = :SHOP_ID AND " +
                      " ((lower(ITEM_NAME) like lower('" + styleName.Trim() + " %'))  or upper(ITEM_NAME)like upper('" + styleName.Trim() + " %') OR " +
                      " (lower(BARCODE) like lower('" + styleName.Trim() + "%'))  or upper(BARCODE)like upper('" + styleName.Trim() + "%')) ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequisitionModel> objShopToShopRequisitionModel = new List<ShopToShopRequisitionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequisitionModel model = new ShopToShopRequisitionModel
                                {
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    ShoId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    Category = objDataReader["PRODUCT_CATEGORY"].ToString(),
                                    SubCategory = objDataReader["PRODUCT_SUB_CATEGORY"].ToString()

                                };
                                objShopToShopRequisitionModel.Add(model);
                            }
                            return objShopToShopRequisitionModel;
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

        public async Task<Tuple<string, string>> SaveShopRequisitionMainModel(ShopToShopRequisitionMainModel objShopToShopRequisitionMainModel)
        {
            string strMessage;
            string challanNo;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_REQUISITION_MAIN_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_REQUISITION_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainModel.RequisitionId;
            objOracleCommand.Parameters.Add("P_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainModel.RequisitionNumber) ? objShopToShopRequisitionMainModel.RequisitionNumber : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainModel.RequisitionShopIdFrom) ? objShopToShopRequisitionMainModel.RequisitionShopIdFrom : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_SHOPID_FROM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainModel.RequisitionShopIdTo) ? objShopToShopRequisitionMainModel.RequisitionShopIdTo : null;
             objOracleCommand.Parameters.Add("P_CREATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainModel.CreatedBy) ? objShopToShopRequisitionMainModel.CreatedBy : null;
            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_MAX_REQUISITION_NUMNER", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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
                    challanNo = objOracleCommand.Parameters["P_MAX_REQUISITION_NUMNER"].Value.ToString();
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
            //return strMessage;
            return new Tuple<string, string>(strMessage, challanNo);
        }

        public async Task<string> SaveShopRequisitionMainItemModel(ShopToShopRequisitionMainItemModel objShopToShopRequisitionMainItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_REQUI_MAIN_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_REQUISITION_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.ShopRequisitionMainItemId;
            objOracleCommand.Parameters.Add("P_REQUISITION_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.RequisitionId;
            objOracleCommand.Parameters.Add("P_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainItemModel.Barcode) ? objShopToShopRequisitionMainItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequisitionMainItemModel.ItemName) ? objShopToShopRequisitionMainItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_PRICE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.Price;
            objOracleCommand.Parameters.Add("P_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequisitionMainItemModel.Quantity;
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


        public async Task<List<ShopToShopRequisitionMainModel>> GetShopRequisitionMainData(int fromShopId)
        {
            var sql = "SELECT " +
                      "REQUISITION_ID," +
                        "REQUISITION_NUMNER," +
                        "REQUISITION_SHOPID_TO," +
                        "TO_SHOP_NAME," +
                        "REQUISITION_SHOPID_FROM," +
                        "FROM_SHOP_NAME," +
                        "REQUISITION_DATE," +
                        "CREATED_BY, " +
                        "DELIVERY_YN, " +
                        "TRANSFER_CHALLAN_NUM "+
                      "FROM VEW_SHOP_REQUISITION_MAIN where REQUISITION_SHOPID_FROM = :REQUISITION_SHOPID_FROM ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REQUISITION_SHOPID_FROM", OracleDbType.Varchar2, ParameterDirection.Input).Value = fromShopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequisitionMainModel> objShopToShopRequisitionMainModel = new List<ShopToShopRequisitionMainModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequisitionMainModel model = new ShopToShopRequisitionMainModel
                                {
                                    RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString()),
                                    RequisitionNumber = objDataReader["REQUISITION_NUMNER"].ToString(),
                                    RequisitionShopIdTo = objDataReader["REQUISITION_SHOPID_TO"].ToString(),
                                    RequisitionShopNameTo = objDataReader["TO_SHOP_NAME"].ToString(),
                                    RequisitionShopIdFrom = objDataReader["REQUISITION_SHOPID_FROM"].ToString(),
                                    RequisitionShopNameFrom = objDataReader["FROM_SHOP_NAME"].ToString(),
                                    RequisitionDate = objDataReader["REQUISITION_DATE"].ToString(),
                                    CreatedBy = objDataReader["CREATED_BY"].ToString(),
                                    DeliveryStatus = objDataReader["DELIVERY_YN"].ToString(),
                                    TransferChallanNumber = objDataReader["TRANSFER_CHALLAN_NUM"].ToString(),

                                };
                                

                                objShopToShopRequisitionMainModel.Add(model);
                            }
                            return objShopToShopRequisitionMainModel;
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

        public async Task<List<ShopToShopRequisitionMainItemModel>> GetShopRequisitionMainItemData(int requisitionId)
        {
            var sql = "SELECT " +
                      "REQUISITION_ITEM_ID," +
                        "REQUISITION_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "BARCODE," +
                        "ITEM_NAME," +
                        "PRICE," +
                        "QUANTITY,VAT " +
                      "FROM VEW_SHOP_REQUISITION_MAIN_ITEM where REQUISITION_ID = :REQUISITION_ID ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REQUISITION_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = requisitionId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequisitionMainItemModel> objShopToShopRequisitionMainItemModel = new List<ShopToShopRequisitionMainItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequisitionMainItemModel model = new ShopToShopRequisitionMainItemModel
                                {
                                    ShopRequisitionMainItemId = Convert.ToInt32(objDataReader["REQUISITION_ITEM_ID"].ToString()),
                                    RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    Price = Convert.ToDouble(objDataReader["PRICE"].ToString()),
                                    Vat = objDataReader["VAT"].ToString(),
                                };


                                objShopToShopRequisitionMainItemModel.Add(model);
                            }
                            return objShopToShopRequisitionMainItemModel;
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


        public async Task<List<ShopToShopRequisitionMainModel>> GetFromShopRequisitionMainData(int toShopId)
        {
            var sql = "SELECT " +
                      "REQUISITION_ID," +
                        "REQUISITION_NUMNER," +
                        "REQUISITION_SHOPID_TO," +
                        "TO_SHOP_NAME," +
                        "REQUISITION_SHOPID_FROM," +
                        "FROM_SHOP_NAME," +
                        "REQUISITION_DATE," +
                        "CREATED_BY " +
                      "FROM VEW_SHOP_REQUISITION_MAIN where REQUISITION_SHOPID_TO = :REQUISITION_SHOPID_TO AND DELIVERY_YN ='N' ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REQUISITION_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = toShopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequisitionMainModel> objShopToShopRequisitionMainModel = new List<ShopToShopRequisitionMainModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequisitionMainModel model = new ShopToShopRequisitionMainModel
                                {
                                    RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString()),
                                    RequisitionNumber = objDataReader["REQUISITION_NUMNER"].ToString(),
                                    RequisitionShopIdTo = objDataReader["REQUISITION_SHOPID_TO"].ToString(),
                                    RequisitionShopNameTo = objDataReader["TO_SHOP_NAME"].ToString(),
                                    RequisitionShopIdFrom = objDataReader["REQUISITION_SHOPID_FROM"].ToString(),
                                    RequisitionShopNameFrom = objDataReader["FROM_SHOP_NAME"].ToString(),
                                    RequisitionDate = objDataReader["REQUISITION_DATE"].ToString(),
                                    CreatedBy = objDataReader["CREATED_BY"].ToString(),

                                };


                                objShopToShopRequisitionMainModel.Add(model);
                            }
                            return objShopToShopRequisitionMainModel;
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

        #region GetMax ReQUISITION NUMBER FOR API

        public async Task<string> GetMaxRequisitionNumber()
        {
            string reqNumber = "";

            var sql =
                "SELECT LPAD (NVL (MAX (SUBSTR ( (REQUISITION_NUMNER), 3, 8)), 0) + 1, 8, 0)  REQUISITION_NUMNER FROM SHOP_REQUISITION_MAIN ";

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
                                reqNumber = objDataReader["REQUISITION_NUMNER"].ToString();
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

        #endregion
    }
}