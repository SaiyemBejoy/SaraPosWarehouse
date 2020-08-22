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
    public class PoCuttingDal
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

        public async Task<List<FabricModel>> SearchPoFabricByOrderNumber(string purchaseOrderNumber)
        {
            var sql = "SELECT " +
                        "FABRIC_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "FABRIC_TYPE," +
                        "FABRIC_CODE," +
                        "CONSUMPTION," +
                        "FABRIC_QUANTITY " +
                        "FROM VEW_PURCHASE_ORDER_FABRIC WHERE PURCHASE_ORDER_NUMBER = : PURCHASE_ORDER_NUMBER ";
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseOrderNumber;

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
                                    FabricId = Convert.ToInt32(objDataReader["FABRIC_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),
                                    FabricType = objDataReader["FABRIC_TYPE"].ToString(),
                                    FabricCode = objDataReader["FABRIC_CODE"].ToString(),
                                    Consumption = objDataReader["CONSUMPTION"].ToString(),
                                    FabricQuantity = objDataReader["FABRIC_QUANTITY"].ToString(),
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

        public async Task<string> PoCuttingSave(PoCuttingModel objPoCuttingModel)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PO_CUTTING_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PO_CUTTING_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPoCuttingModel.PoCuttingId;
            objOracleCommand.Parameters.Add("P_PURCHASE_ORDER_NUMBER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPoCuttingModel.PurchaseOrderNumber) ? objPoCuttingModel.PurchaseOrderNumber : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPoCuttingModel.UpdateBy) ? objPoCuttingModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPoCuttingModel.WareHouseId) ? objPoCuttingModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPoCuttingModel.ShopId) ? objPoCuttingModel.ShopId : null;
            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("P_CUTTING_ID", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    string poCuttingId = objOracleCommand.Parameters["P_CUTTING_ID"].Value.ToString();

                    return poCuttingId;
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

        public async Task<string> PoCuttingItemSave(List<PoCuttingItem> objPoCuttingItem, int poCuttingId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PO_CUTTING_ITEM_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objPoCuttingItem)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PO_CUTTING_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.PoCuttingItemId > 0 ? data.PoCuttingItemId : (object)null;
                            objCommand.Parameters.Add("P_PO_CUTTING_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = poCuttingId > 0 ? poCuttingId : (object)null;
                            objCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ItemId > 0 ? data.ItemId : (object)null;
                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductId > 0 ? data.ProductId : (object)null;
                            objCommand.Parameters.Add("P_ITEM_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.Barcode) ? data.Barcode : null;
                            objCommand.Parameters.Add("P_PO_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.PoQuantity > 0 ? data.PoQuantity : (object)null;
                            objCommand.Parameters.Add("P_CUTTING_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.CuttingQuantity > 0 ? data.CuttingQuantity : (object)null;
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

        public async Task<int> DeletePoCutting(int poCuttingId, string wareHouseId, string shopId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PO_CUTTING_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PO_CUTTING_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = poCuttingId > 0 ? poCuttingId : (object)null;
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
                    return poCuttingId;
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

        public async Task<List<PoCuttingModel>> PoCuttingList(string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                        "PO_CUTTING_ID," +
                        "PURCHASE_ORDER_NUMBER," +
                        "UPDATE_BY," +
                        "to_char (UPDATE_DATE, 'dd/mm/yyyy') UPDATE_DATE," +
                        "WARE_HOUSE_ID," +
                        "SHOP_ID " +
                      "FROM VEW_PO_CUTTING where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ORDER BY PO_CUTTING_ID DESC ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PoCuttingModel> objPoCuttingModel = new List<PoCuttingModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PoCuttingModel model = new PoCuttingModel
                                {
                                    PoCuttingId = Convert.ToInt32(objDataReader["PO_CUTTING_ID"].ToString()),
                                    PurchaseOrderNumber = objDataReader["PURCHASE_ORDER_NUMBER"].ToString(),                                              
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objPoCuttingModel.Add(model);
                            }
                            return objPoCuttingModel;
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