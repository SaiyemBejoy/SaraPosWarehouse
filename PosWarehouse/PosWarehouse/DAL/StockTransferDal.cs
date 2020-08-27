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
    public class StockTransferDal
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

        public async Task<Tuple<string, string>> SaveAllStockTransferMainData(StockTransferModel objStockTransferModel)
        {
            string strMessage;
            string strMessage2;

            OracleCommand objOracleCommand = new OracleCommand("PRO_STOCK_TRANSFER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_STOCK_TRANSFER_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferModel.StockTransferId;
            objOracleCommand.Parameters.Add("P_STOCK_TRANSFER_CHALLAN_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.StockTransferChallanNumber) ? objStockTransferModel.StockTransferChallanNumber : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.RequisitionNumber) ? objStockTransferModel.RequisitionNumber : null;
            objOracleCommand.Parameters.Add("P_SHOP_SHOP_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.ShopToShopRequisitionNumber) ? objStockTransferModel.ShopToShopRequisitionNumber : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.TransferToShopId) ? objStockTransferModel.TransferToShopId : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_SHOPID_FROM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.TransferFromShopId) ? objStockTransferModel.TransferFromShopId : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferModel.TransferedBy) ? objStockTransferModel.TransferedBy : null;
            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_CHALLAN_NO", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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
                    strMessage2 = objOracleCommand.Parameters["P_CHALLAN_NO"].Value.ToString();

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
            return new Tuple<string, string>(strMessage, strMessage2);
        }

        public async Task<string> SaveAllStockTransferItemData(StockTransferItemModel objStockTransferItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_STOCK_TRANSFER_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_STOCK_TRANSFER_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.StockTransferItemId;
            objOracleCommand.Parameters.Add("P_STOCK_TRANSFER_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.StockTransferId;
            objOracleCommand.Parameters.Add("P_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferItemModel.ItemName) ? objStockTransferItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objStockTransferItemModel.Barcode) ? objStockTransferItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.TransferQuantity;
            objOracleCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.SalePrice;
            objOracleCommand.Parameters.Add("P_VAT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objStockTransferItemModel.Vat;
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
            return strMessage;
        }

        public async Task<StockTransferModel> GetAllStockTarnsferInfo(string stockTransferChallanNo)
        {
            var sql = "SELECT " +
                      "STOCK_TRANSFER_ID, " +
                      "STOCK_TRANSFER_CHALLAN_NUM, " +
                      "REQUISITION_NUM, " +
                      "TRANSFER_SHOPID_TO, " +
                      "TRANSFER_SHOPID_FROM, " +
                      "TRANSFER_SHOPNAME_FROM, " +
                      "TRANSFERED_BY, " +
                      "TRANSFER_DATE, " +
                      "RECEIVE_YN  " +
                      "FROM VEW_STOCK_TRANSFER_MAIN Where STOCK_TRANSFER_CHALLAN_NUM =:STOCK_TRANSFER_CHALLAN_NUM ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":STOCK_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = stockTransferChallanNo;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                       StockTransferModel objStockTransferModel = new StockTransferModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {

                                objStockTransferModel.StockTransferId = Convert.ToInt32(objDataReader["STOCK_TRANSFER_ID"].ToString());
                                objStockTransferModel.StockTransferChallanNumber = objDataReader["STOCK_TRANSFER_CHALLAN_NUM"].ToString();
                                objStockTransferModel.RequisitionNumber = objDataReader["REQUISITION_NUM"].ToString();
                                objStockTransferModel.TransferDate = objDataReader["TRANSFER_DATE"].ToString();
                                objStockTransferModel.TransferToShopId = objDataReader["TRANSFER_SHOPID_TO"].ToString();
                                objStockTransferModel.TransferFromShopId = objDataReader["TRANSFER_SHOPID_FROM"].ToString();
                                objStockTransferModel.TransferFromShopName = objDataReader["TRANSFER_SHOPNAME_FROM"].ToString();
                                objStockTransferModel.TransferedBy = objDataReader["TRANSFERED_BY"].ToString();

                            }
                            return objStockTransferModel;
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

        public async Task<List<StockTransferItemModel>> GetAllStockTarnsferItemInfo(int transferId)
        {
            var sql = " SELECT " +
                      "STOCK_TRANSFER_ITEM_ID, " +
                      "STOCK_TRANSFER_ID, " +
                      "ITEM_ID, " +
                      "PRODUCT_ID, " +
                      "ITEM_NAME, " +
                      "BARCODE, " +
                      "TRANSFER_QUANTITY, " +
                      "SALE_PRICE, " +
                      "VAT " +
                      "FROM VEW_STOCK_TRANSFER_ITEM  where STOCK_TRANSFER_ID =:STOCK_TRANSFER_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":STOCK_TRANSFER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = transferId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<StockTransferItemModel> objStockTransferItemModel = new List<StockTransferItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                StockTransferItemModel model = new StockTransferItemModel
                                {
                                    StockTransferItemId = Convert.ToInt32(objDataReader["STOCK_TRANSFER_ITEM_ID"].ToString()),
                                    StockTransferId = Convert.ToInt32(objDataReader["STOCK_TRANSFER_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    TransferQuantity = Convert.ToInt32(objDataReader["TRANSFER_QUANTITY"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString())
                                };
                                objStockTransferItemModel.Add(model);
                            }
                            return objStockTransferItemModel;
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