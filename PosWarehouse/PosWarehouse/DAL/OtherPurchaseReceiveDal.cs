using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PosWarehouse.DAL
{
    public class OtherPurchaseReceiveDal
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

        public async Task<string> GetMaxChallanNumber()
        {
            string challanNo = "";

            var sql = "SELECT LPAD ( (NVL (MAX (O_PURCHASE_RECEIVE_NUMBER), 0) + 1), 9, 0) O_PURCHASE_RECEIVE_NUMBER   FROM OTHER_PURCHASE_RECEIVE";

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
                                challanNo = objDataReader["O_PURCHASE_RECEIVE_NUMBER"].ToString();
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

        public async Task<Tuple<string, string>> SaveOtherPurchaseReceive(OtherPurchaseReceiveModel objOtherPurchaseReceiveModel)
        {
            string strMessage;
            string receiveId;

            OracleCommand objOracleCommand = new OracleCommand("PRO_OTHER_PURCH_RECEIVE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_o_purchase_receive_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveModel.OtherPurchaseReceiveId;
            objOracleCommand.Parameters.Add("p_o_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber) ? objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("p_vendor_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveModel.VendorId;
            objOracleCommand.Parameters.Add("p_delivery_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.DeliveryShopId) ? objOtherPurchaseReceiveModel.DeliveryShopId : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.UpdateBy) ? objOtherPurchaseReceiveModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.WareHouseId) ? objOtherPurchaseReceiveModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_HOLD_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.Hold_YN) ? objOtherPurchaseReceiveModel.Hold_YN : null;
            objOracleCommand.Parameters.Add("P_SCAN_TYPE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.Scan_Type) ? objOtherPurchaseReceiveModel.Scan_Type : null;
            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_RECEIVE_NUMBER", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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
                    receiveId = "";
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

        public async Task<string> SaveOtherPurchaseReceiveItemList(OtherPurchaseReceiveItemModel objOtherPurchaseReceiveItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_OTH_PURCH_REC_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_o_purchase_receive_item_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveItemModel.OtherPurchaseReceiveItemId;
            objOracleCommand.Parameters.Add("p_o_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveItemModel.OtherPurchaseReceiveNumber) ? objOtherPurchaseReceiveItemModel.OtherPurchaseReceiveNumber : null;
            objOracleCommand.Parameters.Add("p_item_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveItemModel.ItemId;
            objOracleCommand.Parameters.Add("p_product_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveItemModel.ProductId;         
            objOracleCommand.Parameters.Add("p_barcode", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveItemModel.Barcode) ? objOtherPurchaseReceiveItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("p_receive_quantity", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveItemModel.ReceiveQuantity;
            objOracleCommand.Parameters.Add("p_sale_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherPurchaseReceiveItemModel.SalePrice;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveItemModel.WareHouseId) ? objOtherPurchaseReceiveItemModel.WareHouseId : null;
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


        public async Task<string> DeleteMainAndItem(OtherPurchaseReceiveModel objOtherPurchaseReceiveModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DELETE_MAIN_AND_ITEM")
            {
                CommandType = CommandType.StoredProcedure
            };

          
            objOracleCommand.Parameters.Add("p_o_purchase_receive_number", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber) ? objOtherPurchaseReceiveModel.OtherPurchaseReceiveNumber : null;
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

        public async Task<List<OtherPurchaseReceiveModel>> OtherPurchaseReceiveList()
        {
            var sql =
                "SELECT O_PURCHASE_RECEIVE_ID," +
                "O_PURCHASE_RECEIVE_NUMBER," +
                "VENDOR_ID,DELIVERY_SHOP_ID," +
                "(SELECT SHOP_NAME FROM SHOP WHERE SHOP_ID = S.DELIVERY_SHOP_ID)DELIVERY_SHOP_NAME," +
                "UPDATE_BY," +
                "TRUNC(UPDATE_DATE) UPDATE_DATE," +
                "CREATE_BY," +
                "CREATE_DATE," +
                "WARE_HOUSE_ID," +
                "O_RECEIVE_YN, " +
                "TOTAL_CHALLAN_QTY,"+
                "HOLD_YN " +
                "FROM VEW_O_PURCHASE_RECEIVE_LIST s ORDER BY O_PURCHASE_RECEIVE_NUMBER DESC";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                   
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<OtherPurchaseReceiveModel> objOtherPurchaseReceiveModels = new List<OtherPurchaseReceiveModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                OtherPurchaseReceiveModel model = new OtherPurchaseReceiveModel
                                {
                                    OtherPurchaseReceiveId = Convert.ToInt32(objDataReader["O_PURCHASE_RECEIVE_ID"].ToString()),
                                    OtherPurchaseReceiveNumber = objDataReader["O_PURCHASE_RECEIVE_NUMBER"].ToString(),
                                    VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    TotalChallanQty = Convert.ToInt32(objDataReader["TOTAL_CHALLAN_QTY"].ToString()),
                                    DeliveryShopId = objDataReader["DELIVERY_SHOP_ID"].ToString(),
                                    DeliveryShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    CreatedBy = objDataReader["CREATE_BY"].ToString(),
                                    CreatedDate = objDataReader["CREATE_DATE"].ToString(),
                                    ReceiveYN = objDataReader["O_RECEIVE_YN"].ToString(),
                                    Hold_YN = objDataReader["HOLD_YN"].ToString().Trim()
                                };
                                objOtherPurchaseReceiveModels.Add(model);
                            }
                            return objOtherPurchaseReceiveModels;
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


        public async Task<OtherPurchaseReceiveItemScanModel> GetProductInfoForHoldScanReceive(string barcode)
        {
            var sql = "SELECT " +
                      "ITEM_ID," +
                        "PRODUCT_ID," +
                        "PRODUCT_STYLE," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "SALE_PRICE " +
                      "FROM VEW_O_PURCHASE_HOLD_ITEM where BARCODE = :BARCODE AND QUANTITY > 0 ";

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
                        OtherPurchaseReceiveItemScanModel objOtherPurchaseReceiveItemScanModel = new OtherPurchaseReceiveItemScanModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objOtherPurchaseReceiveItemScanModel.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                objOtherPurchaseReceiveItemScanModel.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                objOtherPurchaseReceiveItemScanModel.ItemName = objDataReader["ITEM_NAME"].ToString();
                                objOtherPurchaseReceiveItemScanModel.Style = objDataReader["PRODUCT_STYLE"].ToString();
                                objOtherPurchaseReceiveItemScanModel.ProductCode = objDataReader["BARCODE"].ToString();
                                objOtherPurchaseReceiveItemScanModel.Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString());
                                objOtherPurchaseReceiveItemScanModel.SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                            }
                            return objOtherPurchaseReceiveItemScanModel;
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