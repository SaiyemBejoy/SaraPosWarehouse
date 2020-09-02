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
    public class DamageProductDal
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

        public async Task<string> GetMaxChallanNumber()
        {

            string challanNo = "";

            var sql = " SELECT    LPAD (TO_CHAR (SYSDATE, 'dd'), 2, 0) " +
                      "|| LPAD(TO_CHAR(SYSDATE, 'MM'), 2, 0)  " +
                      "|| TO_CHAR(SYSDATE, 'yy')  " +
                      "|| (SELECT LPAD(NVL( " +
                      "MAX(SUBSTR((DAMAGE_TRANSFER_CHALLAN_NUM), 9, 8)),0)+ 1,8,0) " +
                      "FROM DAMAGE_TRANSFER_MAIN)  " +
                      "DAMAGE_TRANSFER_CHALLAN_NUM  " +
                      "FROM DUAL ";

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
                                challanNo = objDataReader["DAMAGE_TRANSFER_CHALLAN_NUM"].ToString();
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


        public async Task<string> SaveAllDamageProductMain(DamageProductModel objDamageProductModel)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_PRODUCT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DAMAGE_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductModel.DamageId;
            objOracleCommand.Parameters.Add("P_DAMAGE_CHALLAN_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageProductModel.DamageChallanNo) ? objDamageProductModel.DamageChallanNo : null;
            objOracleCommand.Parameters.Add("P_CREATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductModel.CreatedBy;
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

        public async Task<string> SaveAllDamageProductMainItem(DamageProductItemModel objDamageProductItemModel)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_PRODUCT_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DAMAGE_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.DamageItemId;
            objOracleCommand.Parameters.Add("P_DAMAGE_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.DamageId;
            objOracleCommand.Parameters.Add("P_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageProductItemModel.Barcode) ? objDamageProductItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageProductItemModel.ItemName) ? objDamageProductItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_PRICE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.Price;
            objOracleCommand.Parameters.Add("P_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDamageProductItemModel.Quantity;
            objOracleCommand.Parameters.Add("P_REMARKS", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDamageProductItemModel.Remarks) ? objDamageProductItemModel.Remarks : null;
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

        public async Task<string> DeleteDamageProductMain(string damageId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_DAMAGE_PRODUCT_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DAMAGE_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = damageId;
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


        public async Task<List<DamageProductModel>> GetAllDamagelist()
        {
            var sql = "SELECT " +
                      "DAMAGE_TRANSFER_ID,"+
                        "DAMAGE_TRANSFER_CHALLAN_NUM,"+
                        "CREATED_BY, "+
                        "CREATED_DATE  " +
                      "FROM DAMAGE_TRANSFER_MAIN WHERE  REQUISITION_NUM IS NULL AND TRANSFERED_BY IS NULL AND TRANSFER_DATE IS NULL ORDER BY DAMAGE_TRANSFER_CHALLAN_NUM  DESC ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DamageProductModel> objDamageProductModel = new List<DamageProductModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DamageProductModel model = new DamageProductModel
                                {
                                    DamageId = Convert.ToInt32(objDataReader["DAMAGE_TRANSFER_ID"].ToString()),
                                    DamageChallanNo = objDataReader["DAMAGE_TRANSFER_CHALLAN_NUM"].ToString(),
                                    CreatedBy = objDataReader["CREATED_BY"].ToString(),
                                    CreatedDate = objDataReader["CREATED_DATE"].ToString(),
                                };
                                objDamageProductModel.Add(model);
                            }
                            return objDamageProductModel;
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

        public async Task<List<DamageProductModel>> GetAllDamagelistForApproval()
        {
            var sql = "SELECT " +
                      "DAMAGE_TRANSFER_ID," +
                        "DAMAGE_TRANSFER_CHALLAN_NUM," +
                        "CREATED_BY, " +
                        "CREATED_DATE  " +
                      "FROM DAMAGE_TRANSFER_MAIN WHERE  REQUISITION_NUM IS NULL AND TRANSFERED_BY IS NULL AND TRANSFER_DATE IS NULL AND APPROVED_YN IS NULL ORDER BY DAMAGE_TRANSFER_CHALLAN_NUM  DESC ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DamageProductModel> objDamageProductModel = new List<DamageProductModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DamageProductModel model = new DamageProductModel
                                {
                                    DamageId = Convert.ToInt32(objDataReader["DAMAGE_TRANSFER_ID"].ToString()),
                                    DamageChallanNo = objDataReader["DAMAGE_TRANSFER_CHALLAN_NUM"].ToString(),
                                    CreatedBy = objDataReader["CREATED_BY"].ToString(),
                                    CreatedDate = objDataReader["CREATED_DATE"].ToString(),
                                };
                                objDamageProductModel.Add(model);
                            }
                            return objDamageProductModel;
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

        public async Task<List<DamageProductItemModel>> GetDamageProductItem(string damageProductChallanNo)
        {
            var sql = "SELECT " +
                      "DAMAGE_TRANSFER_ITEM_ID," +
                        "DAMAGE_TRANSFER_CHALLAN_NUM," +
                        "CREATED_BY," +
                        "CREATED_DATE," +
                        "DAMAGE_TRANSFER_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY," +
                        "SALE_PRICE," +
                        "REMARKS " +
                      "FROM VEW_DAMAGE_ITEM_DETAILS where DAMAGE_TRANSFER_CHALLAN_NUM =:DAMAGE_TRANSFER_CHALLAN_NUM";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DAMAGE_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = damageProductChallanNo;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DamageProductItemModel> objDamageItemModels = new List<DamageProductItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DamageProductItemModel model = new DamageProductItemModel
                                {
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Remarks = objDataReader["REMARKS"].ToString(),
                                    Price = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    Quantity = Convert.ToDouble(objDataReader["QUANTITY"].ToString())
                                };
                                objDamageItemModels.Add(model);
                            }
                            return objDamageItemModels;
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

        public async Task<string> UpdateDamageProductByChallanNo(string challanNo, string strEmployeeId)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_DMG_APPROVED")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_CHALLAN_NO ", OracleDbType.Varchar2, ParameterDirection.Input).Value = challanNo;
            objOracleCommand.Parameters.Add("P_APPROVED_BY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = strEmployeeId;
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
    }
}