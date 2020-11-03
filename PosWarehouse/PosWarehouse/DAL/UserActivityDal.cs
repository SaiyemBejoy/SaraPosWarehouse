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
    public class UserActivityDal
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

        public async Task<List<UserActivityModel>> GetUserActivityList()
        {


            var sql = "SELECT " +
                      "EMPLOYEE_ID," +
                      "EMPLOYEE_ROLE," +
                      "EMPLOYEE_NAME," +
                      "USE_CONTROLLER," +
                      "USE_ACTION," +
                      "IP_ADDRESS," +
                      "CREATED_DATE " +
                      "FROM USER_ACTION_HISTORY ORDER BY CREATED_DATE DESC";

            
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<UserActivityModel> objUserActivityModels = new List<UserActivityModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                UserActivityModel model = new UserActivityModel
                                {
                                    EmployeeId = objDataReader["EMPLOYEE_ID"].ToString(),
                                    EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString(),
                                    UsedAction = objDataReader["USE_ACTION"].ToString(),
                                    UsedController = objDataReader["USE_CONTROLLER"].ToString(),
                                    IpAddress = objDataReader["IP_ADDRESS"].ToString(),
                                    ActivityDate = objDataReader["CREATED_DATE"].ToString(),
                                    EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString()
                                };
                                objUserActivityModels.Add(model);
                            }
                            return objUserActivityModels;
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

        public async Task<List<UserActivityModel>> GetUserActivityListById(UserActivityModel objUserActivityModel)
        {
            var sql = "SELECT " +
                      "EMPLOYEE_ID," +
                      "EMPLOYEE_ROLE," +
                      "EMPLOYEE_NAME," +
                      "USE_CONTROLLER," +
                      "USE_ACTION," +
                      "IP_ADDRESS," +
                      "CREATED_DATE " +
                      "FROM USER_ACTION_HISTORY where EMPLOYEE_ID = :EMPLOYEE_ID  OR TO_CHAR (CREATED_DATE, 'DD/MM/YYYY') BETWEEN :FROM_DATE AND :TO_DATE ORDER BY CREATED_DATE DESC";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":EMPLOYEE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objUserActivityModel.EmployeeId;
                    objCommand.Parameters.Add(":FROM_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objUserActivityModel.FromDate;
                    objCommand.Parameters.Add(":TO_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objUserActivityModel.ToDate;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<UserActivityModel> userActivityList = new List<UserActivityModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                UserActivityModel objUserActivityModels = new UserActivityModel()
                                {
                                    EmployeeId = objDataReader["EMPLOYEE_ID"].ToString(),
                                    EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString(),
                                    IpAddress = objDataReader["IP_ADDRESS"].ToString(),
                                    EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString(),
                                    UsedController = objDataReader["USE_CONTROLLER"].ToString(),
                                    UsedAction = objDataReader["USE_ACTION"].ToString(),
                                    ActivityDate = objDataReader["CREATED_DATE"].ToString()
                                };
                                userActivityList.Add(objUserActivityModels);
                            }
                            return userActivityList;
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