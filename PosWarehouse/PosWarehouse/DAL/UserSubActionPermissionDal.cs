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




    }
}