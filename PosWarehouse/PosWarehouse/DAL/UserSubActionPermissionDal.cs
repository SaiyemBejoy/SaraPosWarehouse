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
    public class UserSubActionPermissionDal
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

        public async Task<List<UserSubActionPermissionModel>> GetSubActionPermissionList()
        {


            var sql = "SELECT " +
                        "AUTO_ID," +
                        "ROLE_NAME," +
                        "ACTION_NAME," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "ACTIVE_YN " +
                        "FROM VEW_USR_ACTN_PRMSN ORDER BY AUTO_ID DESC";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<UserSubActionPermissionModel> objSubPermissionModels = new List<UserSubActionPermissionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                UserSubActionPermissionModel model = new UserSubActionPermissionModel
                                {
                                    PermissionId = Convert.ToInt32(objDataReader["AUTO_ID"].ToString()),
                                    RoleName = objDataReader["ROLE_NAME"].ToString(),
                                    ActioinName = objDataReader["ACTION_NAME"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    CreateBy = objDataReader["CREATE_BY"].ToString(),
                                    CreateDate = objDataReader["CREATE_DATE"].ToString()
                                };
                                objSubPermissionModels.Add(model);
                            }
                            return objSubPermissionModels;
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

        public async Task<UserSubActionPermissionModel> GetASubActionPermission(int permissionId)
        {

            var sql = "SELECT " +
                       "AUTO_ID," +
                        "ROLE_NAME," +
                        "ACTION_NAME," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "ACTIVE_YN " +
                      "FROM VEW_USR_ACTN_PRMSN where AUTO_ID = :AUTO_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = permissionId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        UserSubActionPermissionModel objSubPermissionModels = new UserSubActionPermissionModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objSubPermissionModels.PermissionId =
                                    Convert.ToInt32(objDataReader["AUTO_ID"].ToString());
                                objSubPermissionModels.RoleName = objDataReader["ROLE_NAME"].ToString();
                                objSubPermissionModels.ActioinName = objDataReader["ACTION_NAME"].ToString();
                                objSubPermissionModels.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objSubPermissionModels.ActiveStatus = objSubPermissionModels.Active_YN == "Y" ? true : false;
                                objSubPermissionModels.CreateBy = objDataReader["CREATE_BY"].ToString();
                                objSubPermissionModels.CreateDate = objDataReader["CREATE_DATE"].ToString();
                            }

                            return objSubPermissionModels;
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

        public async Task<string> SaveAndUpdateSubActionPermission(UserSubActionPermissionModel objSubPermissionModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_USR_PRMSN_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSubPermissionModel.PermissionId;
            objOracleCommand.Parameters.Add("P_ROLE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubPermissionModel.RoleName) ? objSubPermissionModel.RoleName.Trim() : null;
            objOracleCommand.Parameters.Add("P_ACTION_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubPermissionModel.ActioinName) ? objSubPermissionModel.ActioinName.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubPermissionModel.CreateBy) ? objSubPermissionModel.CreateBy.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubPermissionModel.CreateDate) ? objSubPermissionModel.CreateDate.Trim() : null;
            objOracleCommand.Parameters.Add("P_ACTIVE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubPermissionModel.Active_YN) ? objSubPermissionModel.Active_YN : null;

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