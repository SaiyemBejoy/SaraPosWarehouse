using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class StockReturnReceiveDal
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


        public async Task<string> SaveStockReturnReceive(StockReceiveModel objStockReceiveModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_STOCK_TR_RECEIVE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STOCK_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveModel.StockReceiveId;
            objOracleCommand.Parameters.Add("P_STOCK_RECEIVE_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveModel.StockReceiveNumber) ? objStockReceiveModel.StockTransferChallanNo : null;
            objOracleCommand.Parameters.Add("P_STOCK_TRANS_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveModel.StockTransferChallanNo) ? objStockReceiveModel.StockTransferChallanNo : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveModel.RequisitionNo) ? objStockReceiveModel.RequisitionNo : null;
            objOracleCommand.Parameters.Add("P_STOCK_TRANS_FROM_SHOPID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveModel.StockTransferFromShopId) ? objStockReceiveModel.StockTransferFromShopId : null;
            objOracleCommand.Parameters.Add("P_RECEIVED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveModel.ReceivedBy) ? objStockReceiveModel.ReceivedBy : null;
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

        public async Task<string> SaveStockReturnReceiveItem(StockReceiveItemModel objStockReceiveItemModel)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_STOCK_TR_RECEIVE_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STOCK_RECEIVE_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.StockReceiveItemId;
            objOracleCommand.Parameters.Add("P_STOCK_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.StockReceiveId;
            objOracleCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveItemModel.Barcode) ? objStockReceiveItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockReceiveItemModel.ItemName) ? objStockReceiveItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_RECEIVE_QTY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.ReceiveQuantity;
            objOracleCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.SalePrice;
            objOracleCommand.Parameters.Add("P_VAT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockReceiveItemModel.Vat;

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

        public async Task<string> UpdateShopReceive(string shopUrlApi, string challanNo)
        {

            string returnMessage = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(shopUrlApi);

                    var responseTask = client.GetAsync("StockReturnReceive?challanNo=" + challanNo);

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.ReasonPhrase;
                        returnMessage = Convert.ToString(readTask);
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return returnMessage;

        }

        public async Task<string> UpdateShopForDmgReceive(string shopUrlApi, string challanNo)
        {

            string returnMessage = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(shopUrlApi);

                    var responseTask = client.GetAsync("DamageReturnReceive?damageChallanNo=" + challanNo);

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.ReasonPhrase;
                        returnMessage = Convert.ToString(readTask);
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return returnMessage;

        }

        public async Task<List<StockReceiveModel>> GetReceivelist()
        {
            var sql = "SELECT "+
                        "TRANSFER_ID, " +
                        "CHALLAN_NUMBER, " +
                        "REQUISITION_NUMBER, " +
                        "SHOP_ID_FROM, " +
                        "SHOP_FROM, " +
                        "TRANSFERED_BY, " +
                        "TRANSFER_DATE, " +
                        "RECEIVED_STATUS " +
                        "FROM VEW_SHOP_TO_WAREHOUSE_TRANSFER WHERE REQUISITION_NUMBER IS Not Null ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) )
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<StockReceiveModel> objStockReceiveModel = new List<StockReceiveModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                StockReceiveModel model = new StockReceiveModel
                                {
                                    StockReceiveId = Convert.ToInt32(objDataReader["TRANSFER_ID"].ToString()),
                                    StockReceiveNumber = objDataReader["TRANSFER_ID"].ToString(),
                                    StockTransferChallanNo = objDataReader["CHALLAN_NUMBER"].ToString(),
                                    RequisitionNo = objDataReader["REQUISITION_NUMBER"].ToString(),
                                    StockTransferFromShopId = objDataReader["SHOP_ID_FROM"].ToString(),
                                    StockTransferFromShopName = objDataReader["SHOP_FROM"].ToString(),
                                    ReceivedBy = objDataReader["TRANSFERED_BY"].ToString(),
                                    ReceivedDate = objDataReader["TRANSFER_DATE"].ToString(),
                                    ReceivedStatus = objDataReader["RECEIVED_STATUS"].ToString()
                                };
                                objStockReceiveModel.Add(model);
                            }
                            return objStockReceiveModel;
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

        public async Task<List<string>> GetAllDamageChallan()
        {
            var sql = "SELECT  DAMAGE_TRANSFER_CHALLAN_NUM FROM DAMAGE_TRANSFER_MAIN Where RECEIVE_YN ='N' ";
                      
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<string> objStockReceiveModel = new List<string>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                string damageTrChallanNo = null;

                                damageTrChallanNo = objDataReader["DAMAGE_TRANSFER_CHALLAN_NUM"].ToString();

                                objStockReceiveModel.Add(damageTrChallanNo);
                            }
                            return objStockReceiveModel;
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

        public async Task<string> SaveDamageReturnReceive(string challanNo, string employeeId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_TR_RECEIVE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DAMAGE_TRANS_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = challanNo;
            objOracleCommand.Parameters.Add("P_RECEIVED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = employeeId;
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

        public async Task<string> GetAStockReceiveInfo(string challanNo)
        {
            var sql = "SELECT " +
                        "STOCK_RECEIVE_ID," +
                        "STOCK_RECEIVE_NUMBER," +
                        "STOCK_TRANSFER_CHALLAN_NUM," +
                        "REQUISITION_NUM  " +
                      "FROM VEW_STOCK_TR_RECEIVE_MAIN where STOCK_TRANSFER_CHALLAN_NUM = :STOCK_TRANSFER_CHALLAN_NUM ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":STOCK_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = challanNo;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        string stockTrReceiveId = "";

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {                     
                                stockTrReceiveId = objDataReader["STOCK_RECEIVE_ID"].ToString();  
                            }
                            return stockTrReceiveId;
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

        public async Task<List<StockReceiveItemModel>> GetAllReceiveItemByChallanNo(string stockReceiveId)
        {
            var sql = " SELECT " +
                        "STOCK_RECEIVE_ITEM_ID," +
                        "STOCK_RECEIVE_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "RECEIVE_QUANTITY," +
                        "SALE_PRICE," +
                        "VAT " +
                      "FROM VEW_STOCK_TR_RECEIVE_ITEM  where STOCK_RECEIVE_ID =:STOCK_RECEIVE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":STOCK_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = stockReceiveId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<StockReceiveItemModel> objStockReceiveItemModel = new List<StockReceiveItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                StockReceiveItemModel model = new StockReceiveItemModel
                                {
                                    StockReceiveItemId = Convert.ToInt32(objDataReader["STOCK_RECEIVE_ITEM_ID"].ToString()),
                                    StockReceiveId = Convert.ToInt32(objDataReader["STOCK_RECEIVE_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    ReceiveQuantity = Convert.ToInt32(objDataReader["RECEIVE_QUANTITY"].ToString()),                                 
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString())
                                };
                                objStockReceiveItemModel.Add(model);
                            }
                            return objStockReceiveItemModel;
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

        #region ShopURL
        public async Task<string> GetShopUrlForUpdate(string shopId)
        {
            string shopUrl = null;
            var sql = "SELECT SHOP_URL FROM VEW_SHOP where SHOP_ID = :SHOP_ID AND ACTIVE_YN = 'Y' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                shopUrl = objDataReader["SHOP_URL"].ToString();
                            }

                            return shopUrl;
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