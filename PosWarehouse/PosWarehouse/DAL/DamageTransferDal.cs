using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class DamageTransferDal
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


        public async Task<string> SaveAllDamageTransferMainData(DamageTransferMain objDamageTransferMainModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_TRANSFER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DAMAGE_TRANSFER_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferMainModel.DamageTransferId;
            objOracleCommand.Parameters.Add("P_DAMAGE_TRANSFER_CHALLAN_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferMainModel.DamageTrschallanNo) ? objDamageTransferMainModel.DamageTrschallanNo : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferMainModel.RequisitionNo) ? objDamageTransferMainModel.RequisitionNo : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_SHOPID_FROM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferMainModel.TransferShopIdfrom) ? objDamageTransferMainModel.TransferShopIdfrom : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferMainModel.TransferedBy) ? objDamageTransferMainModel.TransferedBy : null;
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


        public async Task<string> SaveAllDamageTransferItemData(DamageTransferItem objDamageTransferItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_TRANSFER_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_DAMAGE_TRANSFER_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.DamageTransferItemId;
            objOracleCommand.Parameters.Add("P_DAMAGE_TRANSFER_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.DamageTransferId;
            objOracleCommand.Parameters.Add("P_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferItemModel.ItemName) ? objDamageTransferItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferItemModel.Barcode) ? objDamageTransferItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_TRANSFER_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.Quantity;
            objOracleCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageTransferItemModel.Price;
            objOracleCommand.Parameters.Add("P_REMARKS", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageTransferItemModel.Remarks) ? objDamageTransferItemModel.Remarks : null;
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

        public async Task<DamageTransferMain> GetAllDamageTransferMainInfo(string damageChallan)
        {
            var sql = "SELECT " +
                      "DAMAGE_TRANSFER_ID," +
                        "DAMAGE_TRANSFER_CHALLAN_NUM," +
                        "REQUISITION_NUM," +
                        "TRANSFER_SHOPID_FROM," +
                        "TRANSFER_SHOPNAME_FROM," +
                        "TRANSFERED_BY," +
                        "TRANSFER_DATE," +
                        "RECEIVE_YN," +
                        "RECEIVED_DATE," +
                        "RECEIVED_BY " +
                      "FROM VEW_DAMAGE_TRANSFER_MAIN Where DAMAGE_TRANSFER_CHALLAN_NUM =:DAMAGE_TRANSFER_CHALLAN_NUM ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DAMAGE_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = damageChallan;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        DamageTransferMain objDamageTransferMain = new DamageTransferMain();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {

                                objDamageTransferMain.DamageTransferId = Convert.ToInt32(objDataReader["DAMAGE_TRANSFER_ID"].ToString());
                                objDamageTransferMain.DamageTrschallanNo = objDataReader["DAMAGE_TRANSFER_CHALLAN_NUM"].ToString();
                                objDamageTransferMain.RequisitionNo = objDataReader["REQUISITION_NUM"].ToString();
                                objDamageTransferMain.TransferDate = objDataReader["TRANSFER_DATE"].ToString();
                                objDamageTransferMain.TransferShopIdfrom = objDataReader["TRANSFER_SHOPID_FROM"].ToString();
                                objDamageTransferMain.TransferShopNamefrom = objDataReader["TRANSFER_SHOPNAME_FROM"].ToString();
                                objDamageTransferMain.TransferedBy = objDataReader["TRANSFERED_BY"].ToString();

                            }
                            return objDamageTransferMain;
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

        public async Task<List<DamageTransferItem>> GetAllDamageTransferItemInfo(int damageTransferId)
        {
            var sql = " SELECT " +
                      "DAMAGE_TRANSFER_ITEM_ID," +
                        "DAMAGE_TRANSFER_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "TRANSFER_QUANTITY," +
                        "SALE_PRICE," +
                        "REMARKS," +
                        "VAT " +
                      "FROM VEW_DAMAGE_TRANSFER_ITEM  where DAMAGE_TRANSFER_ID =:DAMAGE_TRANSFER_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DAMAGE_TRANSFER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = damageTransferId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DamageTransferItem> objDamageTransferItem = new List<DamageTransferItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DamageTransferItem model = new DamageTransferItem
                                {
                                    DamageTransferItemId = Convert.ToInt32(objDataReader["DAMAGE_TRANSFER_ITEM_ID"].ToString()),
                                    DamageTransferId = objDataReader["DAMAGE_TRANSFER_ID"].ToString(),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    Remarks = objDataReader["REMARKS"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["TRANSFER_QUANTITY"].ToString()),
                                    Price = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString())
                                };
                                objDamageTransferItem.Add(model);
                            }
                            return objDamageTransferItem;
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