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
    public class MenuAndSubMenuDal
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

        #region "Menu section"
        public async Task<List<MenuSetUpModel>> GetMenuList(string wareHouseId, string shopId)
        {

        
            var sql = "SELECT " +
                      "MENU_ID," +
                      "MENU_NAME," +
                      "MENU_URL," +
                      "MENU_ICON," +
                      "MENU_ORDER," +
                      " ''ACTIVE_YN," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "HEAD_OFFICE_ID," +
                      "BRANCH_OFFICE_ID " +
                      "FROM VEW_MENU_MAIN where HEAD_OFFICE_ID = :HEAD_OFFICE_ID ORDER BY MENU_ID DESC";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                   // objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MenuSetUpModel> objMenuModels = new List<MenuSetUpModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MenuSetUpModel model = new MenuSetUpModel
                                {
                                    MenuId = Convert.ToInt32(objDataReader["MENU_ID"].ToString()),
                                    MenuName = objDataReader["MENU_NAME"].ToString(),
                                    MenuURL = objDataReader["MENU_URL"].ToString(),
                                    MenuIcon = objDataReader["MENU_ICON"].ToString(),
                                    MenuOrder = Convert.ToInt32(objDataReader["MENU_ORDER"].ToString()),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    WareHouseId = objDataReader["HEAD_OFFICE_ID"].ToString(),
                                    ShopId = objDataReader["BRANCH_OFFICE_ID"].ToString()
                                };
                                objMenuModels.Add(model);
                            }
                            return objMenuModels;
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

        public async Task<MenuSetUpModel> GetAMenu(int menuId, string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "MENU_ID," +
                      "MENU_NAME," +
                      "MENU_URL," +
                      "MENU_ICON," +
                      "MENU_ORDER," +
                      " ''ACTIVE_YN," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "HEAD_OFFICE_ID," +
                      "BRANCH_OFFICE_ID " +
                      "FROM VEW_MENU_MAIN where MENU_ID = :MENU_ID AND HEAD_OFFICE_ID = :HEAD_OFFICE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":MENU_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    //objCommand.Parameters.Add(":BRANCH_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        MenuSetUpModel objMenuModels = new MenuSetUpModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objMenuModels.MenuId =
                                    Convert.ToInt32(objDataReader["MENU_ID"].ToString());
                                objMenuModels.MenuName = objDataReader["MENU_NAME"].ToString();
                                objMenuModels.MenuURL = objDataReader["MENU_URL"].ToString();
                                objMenuModels.MenuIcon = objDataReader["MENU_ICON"].ToString();
                                objMenuModels.MenuOrder = Convert.ToInt32(objDataReader["MENU_ORDER"].ToString());
                                objMenuModels.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objMenuModels.ActiveStatus = objMenuModels.Active_YN == "Y" ? true : false;
                                objMenuModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objMenuModels.UpdateDate = objDataReader["UPDATE_DATE"].ToString();
                                objMenuModels.WareHouseId = objDataReader["HEAD_OFFICE_ID"].ToString();
                                objMenuModels.ShopId = objDataReader["BRANCH_OFFICE_ID"].ToString();
                            }

                            return objMenuModels;
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


        public async Task<string> SaveAndUpdateMenu(MenuSetUpModel objMenuModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_Menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMenuModel.MenuId;
            objOracleCommand.Parameters.Add("p_Menu_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.MenuName) ? objMenuModel.MenuName.Trim() : null;
            objOracleCommand.Parameters.Add("p_Menu_url", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.MenuURL) ? objMenuModel.MenuURL.Trim() : null;
            objOracleCommand.Parameters.Add("p_Menu_icon", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.MenuIcon) ? objMenuModel.MenuIcon.Trim() : null;
            objOracleCommand.Parameters.Add("p_Menu_order", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(Convert.ToInt32(objMenuModel.MenuOrder).ToString()) ? Convert.ToInt32(objMenuModel.MenuOrder).ToString().Trim() : null;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.Active_YN) ? objMenuModel.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.UpdateBy) ? objMenuModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.WareHouseId) ? objMenuModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMenuModel.ShopId) ? objMenuModel.ShopId : null;

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


        public async Task<string> DeleteMenu(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MENU_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

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


        //public async Task<List<string>> GetMaxOrderNumberForMenu()
        //{
        //    var sql = "SELECT " +
        //              "MAX(MENU_ORDER) + 1 " +
        //                 "FROM VEW_MENU_MAIN";
        //    using (OracleConnection objConnection = GetConnection())
        //    {
        //        using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
        //        {

        //            await objConnection.OpenAsync();
        //            using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
        //            {
        //                List<string> objMenuOrderNumber = new List<string>();
        //                try
        //                {
        //                    while (await objDataReader.ReadAsync())
        //                    {
        //                        string maxOrder = objDataReader["MENU_ORDER"].ToString();
        //                        objMenuOrderNumber.Add(maxOrder);
        //                    }

        //                    return objMenuOrderNumber;
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw new Exception("Error : " + ex.Message);
        //                }
        //                finally
        //                {
        //                    objDataReader.Dispose();
        //                    objCommand.Dispose();
        //                    objConnection.Dispose();
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        #region "SubMenu Section"

        public async Task<List<SubMenuSetUpModel>> GetSubMenuList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                         "MENU_SUB_ID," +
                         "MENU_MAIN_ID," +
                         "MENU_MAIN_NAME," +
                         "MENU_SUB_NAME," +
                         "MENU_SUB_URL," +
                         "MENU_SUB_ICON," +
                         "MENU_ORDER," +
                         "UPDATE_BY," +
                         "HEAD_OFFICE_ID," +
                         "BRANCH_OFFICE_ID " +
                         "FROM VEW_MENU_SUB where HEAD_OFFICE_ID = :HEAD_OFFICE_ID ORDER BY MENU_SUB_ID DESC";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    //objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SubMenuSetUpModel> objSubMenu = new List<SubMenuSetUpModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SubMenuSetUpModel model = new SubMenuSetUpModel
                                {
                                    MenuId = Convert.ToInt32(objDataReader["MENU_MAIN_ID"].ToString()),
                                    SubMenuId = Convert.ToInt32(objDataReader["MENU_SUB_ID"].ToString()),
                                    MenuName = objDataReader["MENU_MAIN_NAME"].ToString(),
                                    SubMenuName = objDataReader["MENU_SUB_NAME"].ToString(),
                                    SubMenuURL = objDataReader["MENU_SUB_URL"].ToString(),
                                    SubMenuIcon = objDataReader["MENU_SUB_ICON"].ToString(),
                                    SubMenuOrder = Convert.ToInt32(objDataReader["MENU_ORDER"].ToString()),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["HEAD_OFFICE_ID"].ToString(),
                                    ShopId = objDataReader["BRANCH_OFFICE_ID"].ToString()
                                };
                                objSubMenu.Add(model);
                            }

                            return objSubMenu;
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

        public async Task<SubMenuSetUpModel> GetASubMenu(int menuId, int subMenuId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                         "MENU_SUB_ID," +
                         "MENU_MAIN_ID," +
                         "MENU_MAIN_NAME," +
                         "MENU_SUB_NAME," +
                         "MENU_SUB_URL," +
                         "MENU_SUB_ICON," +
                         "MENU_ORDER," +
                         "UPDATE_BY," +
                         "HEAD_OFFICE_ID," +
                         "BRANCH_OFFICE_ID " +
                         "FROM VEW_MENU_SUB where MENU_MAIN_ID = :MENU_MAIN_ID AND MENU_SUB_ID = :MENU_SUB_ID AND HEAD_OFFICE_ID = :HEAD_OFFICE_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text

                })
                {

                    objCommand.Parameters.Add(":MENU_MAIN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;
                    objCommand.Parameters.Add(":MENU_SUB_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = subMenuId;
                    objCommand.Parameters.Add(":HEAD_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    //objCommand.Parameters.Add(":BRANCH_OFFICE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        SubMenuSetUpModel objSubMenuModel = new SubMenuSetUpModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objSubMenuModel.MenuId =
                                    Convert.ToInt32(objDataReader["MENU_MAIN_ID"].ToString());
                                objSubMenuModel.SubMenuId =
                                    Convert.ToInt32(objDataReader["MENU_SUB_ID"].ToString());
                                objSubMenuModel.MenuName = objDataReader["MENU_MAIN_NAME"].ToString();
                                objSubMenuModel.SubMenuName = objDataReader["MENU_SUB_NAME"].ToString();
                                objSubMenuModel.SubMenuURL = objDataReader["MENU_SUB_URL"].ToString();
                                objSubMenuModel.SubMenuIcon = objDataReader["MENU_SUB_ICON"].ToString();
                                objSubMenuModel.SubMenuOrder =
                                    Convert.ToInt32(objDataReader["MENU_ORDER"].ToString());
                                objSubMenuModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objSubMenuModel.WareHouseId = objDataReader["HEAD_OFFICE_ID"].ToString();
                                objSubMenuModel.ShopId = objDataReader["BRANCH_OFFICE_ID"].ToString();
                            }
                            return objSubMenuModel;
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

        public async Task<string> SaveAndUpdateSubMenu(SubMenuSetUpModel objSubMenuModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_sub_menu_save")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSubMenuModel.MenuId;
            objOracleCommand.Parameters.Add("p_sub_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSubMenuModel.SubMenuId;
            objOracleCommand.Parameters.Add("p_sub_menu_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.SubMenuName) ? objSubMenuModel.SubMenuName.Trim() : null;
            objOracleCommand.Parameters.Add("p_sub_menu_url", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.SubMenuURL) ? objSubMenuModel.SubMenuURL.Trim() : null;
            objOracleCommand.Parameters.Add("p_sub_menu_icon", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.SubMenuIcon) ? objSubMenuModel.SubMenuIcon.Trim() : null;
            objOracleCommand.Parameters.Add("p_sub_menu_order", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(Convert.ToInt32(objSubMenuModel.SubMenuOrder).ToString()) ? Convert.ToInt32(objSubMenuModel.SubMenuOrder).ToString().Trim() : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.UpdateBy) ? objSubMenuModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.WareHouseId) ? objSubMenuModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubMenuModel.ShopId) ? objSubMenuModel.ShopId : null;

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

        public async Task<string> DeleteSubMenu(int menuId, int subMenuId, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SUB_MENU_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_sub_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = subMenuId;
            objOracleCommand.Parameters.Add("p_menu_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = menuId;

            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

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
    }
}