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
    public class EmployeeDistributionDal
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

        public async Task<string> SaveAllEmployeeInfo(EmployeeDistributionModel objEmployeeDistributionModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_EmployeeInfo_Save")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_employee_dis_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.EmployeeDisId;
            objOracleCommand.Parameters.Add("p_employee_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.EmployeeId;
            objOracleCommand.Parameters.Add("p_employee_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.EmployeeName) ? objEmployeeDistributionModel.EmployeeName : null;
            objOracleCommand.Parameters.Add("p_designation", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.Designation) ? objEmployeeDistributionModel.Designation : null;
            objOracleCommand.Parameters.Add("p_contact_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.ContactNo) ? objEmployeeDistributionModel.ContactNo : null;
            objOracleCommand.Parameters.Add("p_email", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.Email) ? objEmployeeDistributionModel.Email : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.ShopId;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.EmployeeRole;
            objOracleCommand.Parameters.Add("p_employee_password", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.Password;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.ActiveYn) ? objEmployeeDistributionModel.ActiveYn : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.UpdateBy) ? objEmployeeDistributionModel.UpdateBy : null;
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

        public async Task<List<EmployeeDistributionModel>> GetAllEmployeeInfoList()
        {
            var sql = "SELECT " +
                      "EMPLOYEE_DIS_ID, " +
                        "EMPLOYEE_ID," +
                        "EMPLOYEE_NAME," +
                        "SHOP_ID," +
                        "SHOP_NAME," +
                        "EMPLOYEE_ROLE," +
                        "EMPLOYEE_PASSWORD," +
                        "ACTIVE_YN," +
                        "DESIGNATION," +
                        "CONTACT_NO," +
                        "EMAIL " +
                      "FROM VEW_EMPLOYEE_DISTRIBUTION WHERE SUPER_ADMIN is null ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<EmployeeDistributionModel> objEmployeeDistributionModels = new List<EmployeeDistributionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                EmployeeDistributionModel model = new EmployeeDistributionModel
                                {
                                    EmployeeDisId = Convert.ToInt32(objDataReader["EMPLOYEE_DIS_ID"].ToString()),
                                    EmployeeId = objDataReader["EMPLOYEE_ID"].ToString(),
                                    EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString(),
                                    Designation = objDataReader["DESIGNATION"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    Email = objDataReader["EMAIL"].ToString(),
                                    EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString(),
                                    ActiveYn = objDataReader["ACTIVE_YN"].ToString(),
                                    Password = objDataReader["EMPLOYEE_PASSWORD"].ToString(),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString()
                                };
                                objEmployeeDistributionModels.Add(model);
                            }
                            return objEmployeeDistributionModels;
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

        public async Task<List<EmployeeDistributionModel>> GetAllEmployeeInfoList(string shopId)
        {
            var sql = "SELECT " +
                      "EMPLOYEE_DIS_ID, " +
                        "EMPLOYEE_ID," +
                        "EMPLOYEE_NAME," +
                        "SHOP_ID," +
                        "SHOP_NAME," +
                        "EMPLOYEE_ROLE," +
                        "EMPLOYEE_PASSWORD," +
                        "ACTIVE_YN," +
                        "DESIGNATION," +
                        "CONTACT_NO," +
                        "EMAIL " +
                      "FROM VEW_EMPLOYEE_DISTRIBUTION WHERE SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection){ CommandType = CommandType.Text})
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<EmployeeDistributionModel> objEmployeeDistributionModels = new List<EmployeeDistributionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                EmployeeDistributionModel model = new EmployeeDistributionModel
                                {
                                    EmployeeDisId = Convert.ToInt32(objDataReader["EMPLOYEE_DIS_ID"].ToString()),
                                    EmployeeId = objDataReader["EMPLOYEE_ID"].ToString(),
                                    EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString(),
                                    Designation = objDataReader["DESIGNATION"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    Email = objDataReader["EMAIL"].ToString(),
                                    EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString(),
                                    ActiveYn = objDataReader["ACTIVE_YN"].ToString(),
                                    Password = objDataReader["EMPLOYEE_PASSWORD"].ToString(),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString()
                                };
                                objEmployeeDistributionModels.Add(model);
                            }
                            return objEmployeeDistributionModels;
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

        public async Task<EmployeeDistributionModel> GetstaticListValueforSearchById(string employeeId)
        {
            var sql = "SELECT " +
                      "EMPLOYEE_DIS_ID," +
                        "EMPLOYEE_ID," +
                        "EMPLOYEE_NAME," +
                        "SHOP_ID," +
                        "SHOP_NAME," +
                        "EMPLOYEE_ROLE," +
                        "EMPLOYEE_PASSWORD," +
                        "ACTIVE_YN," +
                        "UPDATE_BY," +
                        "UPDATE_DATE," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "DESIGNATION," +
                        "CONTACT_NO," +
                        "EMAIL " +
                      "FROM VEW_EMPLOYEE_DISTRIBUTION where EMPLOYEE_ID = :EMPLOYEE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":EMPLOYEE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = employeeId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        EmployeeDistributionModel objEmployeeDistributionModel = new EmployeeDistributionModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objEmployeeDistributionModel.EmployeeId = objDataReader["EMPLOYEE_ID"].ToString();
                                objEmployeeDistributionModel.EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString();
                                objEmployeeDistributionModel.Designation = objDataReader["DESIGNATION"].ToString();
                                objEmployeeDistributionModel.EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString();
                                objEmployeeDistributionModel.ActiveYn = objDataReader["ACTIVE_YN"].ToString();
                                objEmployeeDistributionModel.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                objEmployeeDistributionModel.ShopName = objDataReader["SHOP_NAME"].ToString();
                            }
                            return objEmployeeDistributionModel;
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


        public async Task<string> GetShopUrlByEmployeeId(string employeeId)
        {
            string shopUrl = null;
            var sql = "SELECT SHOP_URL FROM VEW_SHOP where SHOP_ID = (SELECT SHOP_ID FROM VEW_EMPLOYEE_DISTRIBUTION WHERE EMPLOYEE_ID = :EMPLOYEE_ID) AND ACTIVE_YN = 'Y' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                    { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":EMPLOYEE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = employeeId;
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

        public async Task<string> UpdateEmployeeInfo(EmployeeDistributionModel objEmployeeDistributionModel)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_EMPLOYEE_DIS_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_EMPLOYEE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.EmployeeId;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objEmployeeDistributionModel.ShopId;
            objOracleCommand.Parameters.Add("P_ACTIVE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objEmployeeDistributionModel.ActiveYn) ? objEmployeeDistributionModel.ActiveYn : null;   
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
    }
}