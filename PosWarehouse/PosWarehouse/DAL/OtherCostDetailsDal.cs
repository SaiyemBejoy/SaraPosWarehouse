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
    public class OtherCostDetailsDal
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

        public async Task<List<OtherCostDetailsModel>> GetOtherCostList()
        {


            var sql = "SELECT " +
                      "OTHER_COST_ID," +
                      "COST_PURPOSE," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE " +
                      "FROM VEW_OTHER_COST_DETAILS ORDER BY OTHER_COST_ID DESC";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<OtherCostDetailsModel> objOtherCostDetailsModels = new List<OtherCostDetailsModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                OtherCostDetailsModel model = new OtherCostDetailsModel
                                {
                                    OtherCostId = Convert.ToInt32(objDataReader["OTHER_COST_ID"].ToString()),
                                    CostPurpose = objDataReader["COST_PURPOSE"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    CreateBy = objDataReader["CREATE_BY"].ToString(),
                                    CreateDate = objDataReader["CREATE_DATE"].ToString()
                                };
                                objOtherCostDetailsModels.Add(model);
                            }
                            return objOtherCostDetailsModels;
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

        public async Task<OtherCostDetailsModel> GetAOtherCost(int otherCostId)
        {

            var sql = "SELECT " +
                      "OTHER_COST_ID," +
                      "COST_PURPOSE," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE " +
                      "FROM VEW_OTHER_COST_DETAILS where OTHER_COST_ID = :OTHER_COST_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":OTHER_COST_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = otherCostId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        OtherCostDetailsModel objOtherCostDetailsModels = new OtherCostDetailsModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objOtherCostDetailsModels.OtherCostId =
                                    Convert.ToInt32(objDataReader["OTHER_COST_ID"].ToString());
                                objOtherCostDetailsModels.CostPurpose = objDataReader["COST_PURPOSE"].ToString();
                                objOtherCostDetailsModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objOtherCostDetailsModels.UpdateDate = objDataReader["UPDATE_DATE"].ToString();
                                objOtherCostDetailsModels.CreateBy = objDataReader["CREATE_BY"].ToString();
                                objOtherCostDetailsModels.CreateDate = objDataReader["CREATE_DATE"].ToString();
                            }

                            return objOtherCostDetailsModels;
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

        public async Task<string> SaveAndUpdateOterCost(OtherCostDetailsModel objOtherCostDetailsModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_OTHER_COST_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_OTHER_COST_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherCostDetailsModels.OtherCostId;
            objOracleCommand.Parameters.Add("P_COST_PURPOSE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCostDetailsModels.CostPurpose) ? objOtherCostDetailsModels.CostPurpose.Trim() : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCostDetailsModels.UpdateBy) ? objOtherCostDetailsModels.UpdateBy.Trim() : null;
            objOracleCommand.Parameters.Add("P_UPDATE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCostDetailsModels.UpdateDate) ? objOtherCostDetailsModels.UpdateDate.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCostDetailsModels.CreateBy) ? objOtherCostDetailsModels.CreateBy.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCostDetailsModels.CreateDate) ? objOtherCostDetailsModels.CreateDate.Trim() : null;
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

    }
}