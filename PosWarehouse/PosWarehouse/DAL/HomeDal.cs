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
    public class HomeDal
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


        #region Menu Section

        public async Task<List<MenuMain>> GetMenuMain(string userId, string headOfficeId, string branchOfficeId)
        {
            const string sql = "SELECT " +
                               "MENU_ID," +
                               "MENU_NAME," +
                               "MENU_URL," +
                               "MENU_ICON," +
                               "UPDATE_BY," +
                               "UPDATE_DATE," +
                               "HEAD_OFFICE_ID," +
                               "BRANCH_OFFICE_ID " +
                               "FROM VEW_MENU_MAIN WHERE HEAD_OFFICE_ID = :HEAD_OFFICE_ID AND BRANCH_OFFICE_ID = :BRANCH_OFFICE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                   // objCommand.Parameters.Add(":UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = userId;
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = headOfficeId;
                    objCommand.Parameters.Add(":BRANCH_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = branchOfficeId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuMain> objMenuModel = new List<MenuMain>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuMain menuMain = new MenuMain();

                                menuMain.MenuMainId = Convert.ToInt32(objDataReader["MENU_ID"].ToString());
                                menuMain.MenuMainName = objDataReader["MENU_NAME"].ToString();
                                menuMain.MenuUrl = objDataReader["MENU_URL"].ToString();
                                menuMain.MenuIcon = objDataReader["MENU_ICON"].ToString();
                               // menuMain.UpdateBy = objDataReader["UPDATE_DATE"].ToString();
                                menuMain.HeadOfficeId = objDataReader["HEAD_OFFICE_ID"].ToString();
                                menuMain.BranchOfficeId = objDataReader["BRANCH_OFFICE_ID"].ToString();

                                objMenuModel.Add(menuMain);
                            }
                            return objMenuModel;
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

        public async Task<List<MenuSub>> GetMenuSub(int menuId, string userId, string headOfficeId, string branchOfficeId)
        {
            const string sql = "SELECT " +
                               "MENU_SUB_ID," +
                               "MENU_MAIN_ID," +
                               "MENU_SUB_NAME," +
                               "MENU_SUB_URL," +
                               "MENU_SUB_ICON," +
                               "UPDATE_BY," +
                               "UPDATE_DATE," +
                               "HEAD_OFFICE_ID," +
                               "BRANCH_OFFICE_ID " +
                               "FROM VEW_MENU_SUB WHERE MENU_MAIN_ID = :MENU_MAIN_ID  AND HEAD_OFFICE_ID = :HEAD_OFFICE_ID AND BRANCH_OFFICE_ID = :BRANCH_OFFICE_ID ORDER BY MENU_ORDER ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":MENU_MAIN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;
                    //objCommand.Parameters.Add(":UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = userId;
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = headOfficeId;
                    objCommand.Parameters.Add(":BRANCH_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = branchOfficeId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuSub> objMenuModel = new List<MenuSub>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuSub menuSub = new MenuSub();

                                menuSub.MenuMainId = Convert.ToInt32(objDataReader["MENU_MAIN_ID"].ToString());
                                menuSub.MenuSubId = Convert.ToInt32(objDataReader["MENU_SUB_ID"].ToString());
                                menuSub.MenuSubName = objDataReader["MENU_SUB_NAME"].ToString();
                                menuSub.MenuUrl = objDataReader["MENU_SUB_URL"].ToString();
                                menuSub.MenuIcon = objDataReader["MENU_SUB_ICON"].ToString();
                               // menuSub.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                menuSub.HeadOfficeId = objDataReader["HEAD_OFFICE_ID"].ToString();
                                menuSub.BranchOfficeId = objDataReader["BRANCH_OFFICE_ID"].ToString();

                                objMenuModel.Add(menuSub);
                            }
                            return objMenuModel;
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

        public async Task<List<MenuMain>> GetMenuMainRoleWise(string userRole)
        {
            const string sql = "SELECT * FROM MENU_MAIN_PERMISION WHERE ROLE_NAME = :ROLE_NAME ORDER BY MENU_ORDER";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":ROLE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = userRole;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuMain> objMenuModel = new List<MenuMain>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuMain menuMain = new MenuMain();

                                menuMain.MenuMainId = Convert.ToInt32(objDataReader["MENU_ID"].ToString());
                                menuMain.MenuMainName = objDataReader["MENU_NAME"].ToString();
                                menuMain.MenuUrl = objDataReader["MENU_URL"].ToString();
                                menuMain.MenuIcon = objDataReader["MENU_ICON"].ToString();
                                objMenuModel.Add(menuMain);
                            }
                            return objMenuModel;
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

        public async Task<List<MenuSub>> GetMenuSubRoleWise(int menuId, string userRole)
        {
            const string sql = "SELECT  * FROM MENU_SUB_PERMISION WHERE MENU_MAIN_ID = :MENU_MAIN_ID  AND ROLE_NAME = :ROLE_NAME  ORDER BY MENU_ORDER ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":MENU_MAIN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;
                    objCommand.Parameters.Add(":ROLE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = userRole;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuSub> objMenuModel = new List<MenuSub>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuSub menuSub = new MenuSub();

                                menuSub.MenuMainId = Convert.ToInt32(objDataReader["MENU_MAIN_ID"].ToString());
                                menuSub.MenuSubId = Convert.ToInt32(objDataReader["MENU_SUB_ID"].ToString());
                                menuSub.MenuSubName = objDataReader["MENU_SUB_NAME"].ToString();
                                menuSub.MenuUrl = objDataReader["MENU_SUB_URL"].ToString();
                                menuSub.MenuIcon = objDataReader["MENU_SUB_ICON"].ToString();
                                objMenuModel.Add(menuSub);
                            }
                            return objMenuModel;
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

        public async Task<List<MenuSub>> GetMenuSubByMenuId(int menuId)
        {
            const string sql = "SELECT " +
                               "MENU_SUB_ID," +
                               "MENU_MAIN_ID," +
                               "MENU_SUB_NAME," +
                               "MENU_SUB_URL," +
                               "MENU_SUB_ICON," +
                               "UPDATE_BY," +
                               "UPDATE_DATE," +
                               "HEAD_OFFICE_ID," +
                               "BRANCH_OFFICE_ID " +
                               "FROM VEW_MENU_SUB WHERE MENU_MAIN_ID = :MENU_MAIN_ID ORDER BY MENU_ORDER ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":MENU_MAIN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuSub> objMenuModel = new List<MenuSub>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuSub menuSub = new MenuSub();

                                menuSub.MenuMainId = Convert.ToInt32(objDataReader["MENU_MAIN_ID"].ToString());
                                menuSub.MenuSubId = Convert.ToInt32(objDataReader["MENU_SUB_ID"].ToString());
                                menuSub.MenuSubName = objDataReader["MENU_SUB_NAME"].ToString();
                                menuSub.MenuUrl = objDataReader["MENU_SUB_URL"].ToString();
                                menuSub.MenuIcon = objDataReader["MENU_SUB_ICON"].ToString();
                                menuSub.HeadOfficeId = objDataReader["HEAD_OFFICE_ID"].ToString();
                                menuSub.BranchOfficeId = objDataReader["BRANCH_OFFICE_ID"].ToString();

                                objMenuModel.Add(menuSub);
                            }
                            return objMenuModel;
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

        public async Task<MenuMain> GetMenuInfoByMenuId(int menuId)
        {
            var sql = "SELECT * FROM VEW_MENU_MAIN where MENU_ID = :MENU_ID  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":MENU_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;                
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        MenuMain objMenuMain = new MenuMain();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objMenuMain.MenuMainId =Convert.ToInt32(objDataReader["MENU_ID"].ToString());
                                objMenuMain.MenuMainName =objDataReader["MENU_NAME"].ToString();
                                objMenuMain.MenuIcon = objDataReader["MENU_ICON"].ToString();
                                objMenuMain.MenuUrl = objDataReader["MENU_URL"].ToString();                   
                            }
                            return objMenuMain;
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

        public async Task<string> SaveUserMenuPermision(MenuMain objMenuMain)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_PERMISION_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_main_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuMainId;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.EmployeeRole;
            objOracleCommand.Parameters.Add("p_main_menu_Name", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuMainName;
            objOracleCommand.Parameters.Add("p_Menu_Url", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuUrl;
            objOracleCommand.Parameters.Add("p_main_menu_Icon", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuIcon;
            objOracleCommand.Parameters.Add("p_Updated_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.UpdateBy;
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

        public async Task<string> SaveUserSubMenuPermision(MenuSub objMenuSub)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SUBMENU_PERMISION_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_main_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.MenuMainId;
            objOracleCommand.Parameters.Add("p_sub_main_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.MenuSubId;
            objOracleCommand.Parameters.Add("p_sub_menu_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.MenuSubName;
            objOracleCommand.Parameters.Add("p_sub_menu_url", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.MenuUrl;
            objOracleCommand.Parameters.Add("p_sub_menu_icon", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.MenuIcon;
            objOracleCommand.Parameters.Add("p_Updated_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.UpdateBy;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.EmployeeRole;
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

        public async Task<string> DeleteUserSubMenuPermision(MenuSubPermesionDelete objMenuMain)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_PERMISION_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_main_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuMainId;
            objOracleCommand.Parameters.Add("p_sub_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuSubId;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.EmployeeRole;
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

        public async Task<string> DeleteUserMenuAndSubMenuPermision(MenuSubPermesionDelete objMenuMain)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_All_PERMISION_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_main_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuMainId;
            objOracleCommand.Parameters.Add("p_sub_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.MenuSubId;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuMain.EmployeeRole;
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

        public async Task<MenuMain> GetInfoByUserId(string userId)
        {
            var sql = "SELECT * FROM ADMIN_LOGIN where EMPLOYEE_ID = :EMPLOYEE_ID  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":EMPLOYEE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = userId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        MenuMain objMenuMain = new MenuMain();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objMenuMain.UserId = objDataReader["EMPLOYEE_ID"].ToString();
                                objMenuMain.EmployeeRole = objDataReader["EMPLOYEE_ROLE"].ToString();
                                objMenuMain.UserName = objDataReader["EMPLOYEE_NAME"].ToString();
                                objMenuMain.ActiveYn = objDataReader["ACTIVE_YN"].ToString();
                            }
                            return objMenuMain;
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

        public async Task<string> UpdateUserRoleByUserId(MenuMain objMenuSub)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_USER_ROLE_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_user_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.UserId;
            objOracleCommand.Parameters.Add("p_employee_role", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.EmployeeRole;
            objOracleCommand.Parameters.Add("p_user_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.UserName;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuSub.ActiveYn;
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
        #endregion

        #region Dashboard Section

        public async Task<DashboardModel> GetAllSaleInfoData()
        {

            var sql = "SELECT " +
                      "NVL (SUM (TOTAL_SALE), 0) TOTAL_SALE," +
                      "NVL (SUM (TODAYS_SALE), 0) TODAYS_SALE," +
                      "NVL (SUM (CURRENT_MONTH_SALE), 0) CURRENT_MONTH_SALE," +
                      "NVL (SUM (LAST_SAVEN_DAYS_SALE), 0) LAST_SAVEN_DAYS_SALE " +
                      "FROM VEW_DASHBOARD_SALE_INFO ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                   
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        DashboardModel objDashboardModel = new DashboardModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objDashboardModel.TotalSale = Convert.ToDouble(objDataReader["TOTAL_SALE"].ToString());
                                objDashboardModel.ToDaysSale = Convert.ToDouble(objDataReader["TODAYS_SALE"].ToString());
                                objDashboardModel.CurrentMonthSale = Convert.ToDouble(objDataReader["CURRENT_MONTH_SALE"].ToString());
                                objDashboardModel.LastSevenDaysSale = Convert.ToDouble(objDataReader["LAST_SAVEN_DAYS_SALE"].ToString());
                                
                            }

                            return objDashboardModel;
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

        public async Task<DashboardModel> AllDashboardInfoByShopId(string shopId)
        {
            var sql = "SELECT " +
                      "TOTAL_SALE," +
                      "TODAYS_SALE," +
                      "CURRENT_MONTH_SALE," +
                      "LAST_SAVEN_DAYS_SALE," +
                      "WAREHOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_DASHBOARD_SALE_INFO where SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        DashboardModel objDashboardModel = new DashboardModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objDashboardModel.TotalSale = Convert.ToDouble(objDataReader["TOTAL_SALE"].ToString());
                                objDashboardModel.ToDaysSale = Convert.ToDouble(objDataReader["TODAYS_SALE"].ToString());
                                objDashboardModel.CurrentMonthSale = Convert.ToDouble(objDataReader["CURRENT_MONTH_SALE"].ToString());
                                objDashboardModel.LastSevenDaysSale = Convert.ToDouble(objDataReader["LAST_SAVEN_DAYS_SALE"].ToString());
                                objDashboardModel.WareHouseId = objDataReader["WAREHOUSE_ID"].ToString();
                                objDashboardModel.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objDashboardModel;
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

        public async Task<List<HotSaleAndLowStockModel>> AllLowStockByShopId(string shopId)
        {
            var sql = "SELECT " +
                      "BARCODE," +
                      "ITEM_NAME," +
                      "QUANTITY," +
                      "SALE_PRICE " +
                      "FROM VEW_ALL_SHOP_STOCK  WHERE SHOP_ID = :SHOP_ID AND " +
                      " ROWNUM <= 20 AND QUANTITY <= 5 ORDER BY QUANTITY DESC ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<HotSaleAndLowStockModel> objHotSaleAndLowStockModels = new List<HotSaleAndLowStockModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                HotSaleAndLowStockModel model = new HotSaleAndLowStockModel
                                {
                                    BarCode = objDataReader["BARCODE"].ToString(),
                                    StyleName = objDataReader["ITEM_NAME"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    Price = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    
                                };
                                objHotSaleAndLowStockModels.Add(model);
                            }
                            return objHotSaleAndLowStockModels;
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

        public async Task<List<HotSaleModel>> AllHotSaleByShopId(string shopId)
        {
            var sql = "SELECT " +
                        "ITEM_NAME," +
                        "BARCODE," +
                        "QUANTITY " +
                      "FROM VEW_SALE_SHOP_QTY where SHOP_ID =:SHOP_ID AND ROWNUM <= 20 order by QUANTITY desc ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<HotSaleModel> objHotSaleModel = new List<HotSaleModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                HotSaleModel model = new HotSaleModel
                                {
                                    BarCode = objDataReader["BARCODE"].ToString(),
                                    StyleName = objDataReader["ITEM_NAME"].ToString(),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
                                    //Price = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),

                                };
                                objHotSaleModel.Add(model);
                            }
                            return objHotSaleModel;
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

        public async Task<string> GetShopUrlByShopId(string shopId)
        {
            string shopUrl = null;
            var sql = "SELECT SHOP_URL FROM VEW_SHOP where SHOP_ID = :SHOP_ID AND ACTIVE_YN = 'Y' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
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

        public async Task<string> GetShopUrlByChallanNo(string challanNo)
        {
            string shopUrl = null;
            var sql = "SELECT SHOP_URL FROM VEW_SHOP where SHOP_ID = (SELECT TRANSFER_SHOPID_FROM FROM VEW_STOCK_TRANSFER_MAIN WHERE STOCK_TRANSFER_CHALLAN_NUM = :STOCK_TRANSFER_CHALLAN_NUM) AND ACTIVE_YN = 'Y' ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                    { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":STOCK_TRANSFER_CHALLAN_NUM", OracleDbType.Varchar2, ParameterDirection.Input).Value = challanNo;
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

        #endregion
    }
}