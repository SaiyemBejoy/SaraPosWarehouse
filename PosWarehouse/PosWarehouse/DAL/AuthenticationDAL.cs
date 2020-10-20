using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class AuthenticationDAL
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

        public async Task<AuthModel> Login(string employeeId, string employeePassword)
        {
            AuthModel model = new AuthModel();
            OracleCommand objOracleCommand = new OracleCommand("pro_check_valid_user")
            {
                CommandType = CommandType.StoredProcedure
            };


            objOracleCommand.Parameters.Add("p_employee_id", OracleDbType.Varchar2, ParameterDirection.InputOutput)
                .Value = !string.IsNullOrWhiteSpace(employeeId) ? employeeId : null;
            objOracleCommand.Parameters.Add("p_employee_password", OracleDbType.Varchar2, ParameterDirection.Input)
                .Value = !string.IsNullOrWhiteSpace(employeePassword) ? employeePassword : null;

            objOracleCommand.Parameters.Add("p_employee_name", OracleDbType.Varchar2, 2000).Direction =
                ParameterDirection.Output;
            objOracleCommand.Parameters.Add("p_employee_email", OracleDbType.Varchar2, 2000).Direction =
                ParameterDirection.Output;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, 2000).Direction =
                ParameterDirection.Output;

            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Int32).Direction =
                ParameterDirection.Output;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Int32).Direction = ParameterDirection.Output;

            objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction =
                ParameterDirection.Output;

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

                    model.EmployeeId = objOracleCommand.Parameters["p_employee_id"].Value.ToString();
                    model.EmployeeName = objOracleCommand.Parameters["p_employee_name"].Value.ToString();
                    model.EmployeeEmail = objOracleCommand.Parameters["p_employee_email"].Value.ToString();
                    model.EmployeeRole = objOracleCommand.Parameters["p_employee_role"].Value.ToString();

                    model.WareHouseId = objOracleCommand.Parameters["p_ware_house_id"].Value.ToString();
                    model.ShopId = objOracleCommand.Parameters["p_shop_id"].Value.ToString();

                    var strMsg = objOracleCommand.Parameters["p_message"].Value.ToString();

                    model.Message = strMsg == "TRUE";
                }

                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }

                finally
                {

                    strConn.Close();
                }

            }

            return model;
        }

        public async Task<IEnumerable<RequisitionMainModel>> GetAllRequisition()
        {
            const string sql = "SELECT " +
                               "REQUISITION_ID," +
                               "REQUISITION_NO ," +
                               "REQUISITION_DATE," +
                               "CREATED_BY," +
                               "SHOP_ID " +
                               "FROM VEW_REQUISITION_MAIN WHERE DELIVERY_YN = 'N' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader) await objCommand.ExecuteReaderAsync())
                    {
                        List<RequisitionMainModel> objRequisitionMainModel = new List<RequisitionMainModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                RequisitionMainModel model = new RequisitionMainModel();

                                model.RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString());
                                model.RequisitionNo = objDataReader["REQUISITION_NO"].ToString();
                                model.RequisitionDate = objDataReader["REQUISITION_DATE"].ToString();
                                model.CreatedBy = objDataReader["CREATED_BY"].ToString();
                                model.ShopId = objDataReader["SHOP_ID"].ToString();

                                objRequisitionMainModel.Add(model);
                            }

                            return objRequisitionMainModel;
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

        public async Task<IEnumerable<StockTransferModel>> GetAllStockTransfer()
        {
            const string sql = "SELECT " +
                               "STOCK_TRANSFER_ID," +
                               "STOCK_TRANSFER_CHALLAN_NUM," +
                               "REQUISITION_NUM," +
                               "TRANSFER_SHOPID_TO," +
                               "TRANSFER_SHOPID_FROM," +
                               "TRANSFERED_BY," +
                               "TRANSFER_DATE," +
                               "RECEIVE_YN " +
                               "FROM VEW_STOCK_TRAN_FOR_WAREHOUSE WHERE RECEIVE_YN = 'N' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader) await objCommand.ExecuteReaderAsync())
                    {
                        List<StockTransferModel> objStockTransferModel = new List<StockTransferModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                StockTransferModel model = new StockTransferModel();

                                model.StockTransferId = Convert.ToInt32(objDataReader["STOCK_TRANSFER_ID"].ToString());
                                model.StockTransferChallanNumber =
                                    objDataReader["STOCK_TRANSFER_CHALLAN_NUM"].ToString();
                                model.RequisitionNumber = objDataReader["REQUISITION_NUM"].ToString();
                                model.TransferToShopId = objDataReader["TRANSFER_SHOPID_TO"].ToString();
                                model.TransferFromShopId = objDataReader["TRANSFER_SHOPID_FROM"].ToString();
                                model.ReceivedYN = objDataReader["RECEIVE_YN"].ToString();
                                model.TransferDate = objDataReader["TRANSFER_DATE"].ToString();
                                model.TransferedBy = objDataReader["TRANSFERED_BY"].ToString();

                                objStockTransferModel.Add(model);
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

        public async Task<IEnumerable<LowStockModel>> GetAllLowStockStyle()
        {
            const string sql = "  SELECT SHOP_ID, SHOP_NAME, COUNT (PRODUCT_STYLE) STYLE_COUNT FROM VEW_RPT_SHOP_LOWSTOCK WHERE SHOP_ID<> 331 " +
                               " GROUP BY SHOP_ID, SHOP_NAME ORDER BY SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<LowStockModel> objLowStockModels = new List<LowStockModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                LowStockModel model = new LowStockModel();
                              
                                model.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                model.ShopName = objDataReader["SHOP_NAME"].ToString();
                                model.ProductStyleCount =
                                    Convert.ToInt32(objDataReader["STYLE_COUNT"].ToString());
                                objLowStockModels.Add(model);
                            }
                            return objLowStockModels;
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

        public async Task<string> ChangePassword(ChangePasswordModel oChangePasswordModel)
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_EMPLOYEE_PASS_CHANGE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("p_employee_id", OracleDbType.Varchar2, ParameterDirection.InputOutput)
                .Value = !string.IsNullOrWhiteSpace(oChangePasswordModel.EmployeeId)
                ? oChangePasswordModel.EmployeeId
                : null;
            objOracleCommand.Parameters
                    .Add("p_employee_old_pass", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value =
                !string.IsNullOrWhiteSpace(oChangePasswordModel.OldPassword) ? oChangePasswordModel.OldPassword : null;
            objOracleCommand.Parameters.Add("p_employee_password", OracleDbType.Varchar2, ParameterDirection.Input)
                .Value = !string.IsNullOrWhiteSpace(oChangePasswordModel.NewPassword)
                ? oChangePasswordModel.NewPassword
                : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                !string.IsNullOrWhiteSpace(oChangePasswordModel.ShopId) ? oChangePasswordModel.ShopId : null;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction =
                ParameterDirection.Output;

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

                    strMsg = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                }
            }

            return strMsg;
        }

        public async Task<string> LogAction(LogActionModel objLogActionModel, AuthModel objAuthModel)
        {
            string strMsg;
            OracleCommand objOracleCommand = new OracleCommand("PRO_USER_ACTION_HIST_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("p_user_id", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value =
                !string.IsNullOrWhiteSpace(objAuthModel.EmployeeId) ? objAuthModel.EmployeeId : null;
            objOracleCommand.Parameters.Add("p_user_role", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value
                = !string.IsNullOrWhiteSpace(objAuthModel.EmployeeRole) ? objAuthModel.EmployeeRole : null;
            objOracleCommand.Parameters.Add("p_user_Name", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                !string.IsNullOrWhiteSpace(objAuthModel.EmployeeName) ? objAuthModel.EmployeeName : null;
            objOracleCommand.Parameters.Add("p_controller_name", OracleDbType.Varchar2, ParameterDirection.Input).Value
                = !string.IsNullOrWhiteSpace(objLogActionModel.ControllerName)
                    ? objLogActionModel.ControllerName
                    : "unknown";
            objOracleCommand.Parameters.Add("p_action_name", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                !string.IsNullOrWhiteSpace(objLogActionModel.ActionName) ? objLogActionModel.ActionName : "unknown";
            objOracleCommand.Parameters.Add("p_menu_url", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                !string.IsNullOrWhiteSpace(objLogActionModel.MenuUrl) ? objLogActionModel.MenuUrl : "";
            objOracleCommand.Parameters.Add("p_ip_address", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                !string.IsNullOrWhiteSpace(objLogActionModel.IpAddress) ? objLogActionModel.IpAddress : "0";
            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction =
                ParameterDirection.Output;

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

                    strMsg = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error : " + ex.Message);
                }
                finally
                {
                    strConn.Close();
                }
            }

            return strMsg;
        }

        public async Task<string> RoleWiseActionPermision(string controllerAction,string userRole)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_PERMISION_CHECK")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_Menu_URL", OracleDbType.Varchar2, ParameterDirection.Input).Value =controllerAction;
            objOracleCommand.Parameters.Add("p_User_Role", OracleDbType.Varchar2, ParameterDirection.Input).Value =userRole;
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

        #region UserList
        public async Task<List<EmployeeDistributionModel>> GetUserList()
        {


            var sql = "SELECT " +
                      "EMPLOYEE_ID," +
                      "EMPLOYEE_NAME," +
                      "EMPLOYEE_EMAIL," +
                      "EMPLOYEE_ROLE," +
                      "ACTIVE_YN," +
                      "CREATED_BY," +
                      "CREATED_DATE," +
                      "UPDATE_BY," +
                      "UPDATE_DATE " +
                      "FROM ADMIN_LOGIN ORDER BY UPDATE_DATE DESC";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<EmployeeDistributionModel> objUserListModels = new List<EmployeeDistributionModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                EmployeeDistributionModel model = new EmployeeDistributionModel
                                {
                                    EmployeeId = objDataReader["EMPLOYEE_ID"].ToString(),
                                    EmployeeName = objDataReader["EMPLOYEE_NAME"].ToString(),
                                    Email = objDataReader["EMPLOYEE_EMAIL"].ToString(),
                                    EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString(),
                                    ActiveYn = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    CreateBy = objDataReader["CREATED_BY"].ToString(),
                                    CreateDate = objDataReader["CREATED_DATE"].ToString()
                                };
                                objUserListModels.Add(model);
                            }
                            return objUserListModels;
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