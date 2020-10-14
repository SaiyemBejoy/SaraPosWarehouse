using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;

namespace PosWarehouse.DAL
{
    public class DropdownDAL
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

        //For All Dropdown Start
        public async Task<DataTable> GetCategoryListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CATEGORY_ID, " +
                      "CATEGORY_NAME " +
                      "FROM VEW_CATEGORY ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetUserRoleListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT DISTINCT " +
                      "EMPLOYEE_ROLE, " +
                      "EMPLOYEE_ROLE  " +
                      "FROM ADMIN_LOGIN ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetUserMenuListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "MENU_ID, " +
                      "MENU_NAME  " +
                      "FROM MENU_MAIN ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetUserIdDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "EMPLOYEE_ID, " +
                      "EMPLOYEE_ID  " +
                      "FROM ADMIN_LOGIN ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetCategoryListDropdownForRpt()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CATEGORY_NAME, " +
                      "CATEGORY_NAME " +
                      "FROM VEW_CATEGORY ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetAttributeListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "ATTRIBUTE_ID, " +
                      "ATTRIBUTE_NAME " +
                      "FROM VEW_ATTRIBUTE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetMaterialListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "MATERIAL_ID, " +
                      "MATERIAL_NAME " +
                      "FROM L_MATERIAL_DETAILS ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetOtherCostListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "OTHER_COST_ID, " +
                      "COST_PURPOSE  " +
                      "FROM L_OTHER_COST_DETAILS ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetCountryListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "COUNTRY_ID, " +
                      "COUNTRY_NAME " +
                      "FROM VEW_COUNTRY ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetWareHouseListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "WARE_HOUSE_ID, " +
                      "WARE_HOUSE_NAME " +
                      "FROM VEW_WARE_HOUSE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetCardtypeListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CARD_TYPE_ID, " +
                      "CARD_TYPE_NAME " +
                      "FROM VEW_CARD_TYPE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetCardTypeListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CARD_TYPE_ID, " +
                      "CARD_TYPE_NAME " +
                      "FROM VEW_CARD_TYPE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetShopListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "SHOP_ID, " +
                      "SHOP_NAME " +
                      "FROM VEW_SHOP ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetWarehouseListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "WARE_HOUSE_ID, " +
                      "WARE_HOUSE_NAME " +
                      "FROM WARE_HOUSE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetGiftVouchervalueListDropdown()
        {
            DataTable dt = new DataTable();
            //var sql = "SELECT GIFT_VOUCHER_ID, GIFT_VOUCHER_VALUE" +
            //          " FROM GIFT_VOUCHER_GENERATE WHERE GIFT_VOUCHER_VALUE " +
            //          "NOT IN(SELECT GIFT_VOUCHER_VALUE FROM GIFT_VOUCHER_DELIVERY)";

            var sql = "SELECT GIFT_VOUCHER_ID, GIFT_VOUCHER_VALUE  " +
                " FROM GIFT_VOUCHER_GENERATE g WHERE (SELECT COUNT(*)   " +
                " FROM GIFT_VOUCHER_ITEM  " +
                " WHERE GIFT_VOUCHER_ID = G.GIFT_VOUCHER_ID) <>  " +
                " (SELECT COUNT(*) " +
                " FROM GIFT_VOUCHER_DELIVERY   " +
                " WHERE GIFT_VOUCHER_ID = G.GIFT_VOUCHER_ID)";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetActiveShopListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "SHOP_ID, " +
                      "SHOP_NAME " +
                      "FROM VEW_SHOP WHERE ACTIVE_YN = 'Y' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetPurchaseOrderNumberDropdownDub()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "PURCHASE_ORDER_ID," +
                      "PURCHASE_ORDER_NUMBER " +
                      "FROM VEW_PURCHASE_ORDER " +
                      "WHERE PURCHASE_ORDER_NUMBER NOT IN " +
                      "(SELECT PURCHASE_ORDER_NUMBER FROM VEW_PO_CUTTING) " +
                      "ORDER BY PURCHASE_ORDER_NUMBER ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetPurchaseOrderNumberDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "PURCHASE_ORDER_ID," +
                      "PURCHASE_ORDER_NUMBER " +
                      "FROM VEW_PURCHASE_ORDER " +
                      "WHERE PURCHASE_ORDER_NUMBER NOT IN " +
                        "(SELECT PURCHASE_ORDER_NUMBER FROM VEW_PURCHASE_RECEIVE) " +
                        "ORDER BY PURCHASE_ORDER_NUMBER ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetSubCategoryListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "SUB_CATEGORY_ID, " +
                      "SUB_CATEGORY_NAME " +
                      "FROM VEW_SUB_CATEGORY ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetOtherPurchaseReceiveNumberDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "O_PURCHASE_RECEIVE_NUMBER, " +
                      "O_PURCHASE_RECEIVE_NUMBER " +
                      "FROM OTHER_PURCHASE_RECEIVE WHERE O_RECEIVE_YN = 'N' AND HOLD_YN = 'N' AND SCAN_TYPE <> 'H' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetSubCategoryListDropdownForRpt()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "SUB_CATEGORY_NAME, " +
                      "SUB_CATEGORY_NAME " +
                      "FROM VEW_SUB_CATEGORY ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetSubCategoryListDropdown(int categoryId)
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CATEGORY_ID, " +
                      "SUB_CATEGORY_ID, " +
                      "SUB_CATEGORY_NAME " +
                      "FROM L_SUB_CATEGORY WHERE CATEGORY_ID = :CATEGORY_ID  order by SUB_CATEGORY_NAME ";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };
            objCommand.Parameters.Add("CATEGORY_ID", categoryId);

            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetSubCategoryListDropdownByName(string categoryName)
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CATEGORY_NAME, " +
                      "SUB_CATEGORY_ID, " +
                      "SUB_CATEGORY_NAME " +
                      "FROM VEW_SUB_CATEGORY WHERE CATEGORY_NAME = :CATEGORY_NAME  order by SUB_CATEGORY_ID ";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };
            objCommand.Parameters.Add("CATEGORY_NAME", categoryName);

            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetStyleListByCategorySubCategoryDropdown(int categoryId, int subCategoryId)
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CATEGORY_ID, " +
                      "SUB_CATEGORY_ID, " +
                      "PRODUCT_ID, " +
                      "PRODUCT_STYLE " +
                      "FROM PRODUCT WHERE CATEGORY_ID = :CATEGORY_ID AND SUB_CATEGORY_ID = :SUB_CATEGORY_ID  order by PRODUCT_STYLE ";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };
            objCommand.Parameters.Add("CATEGORY_ID", categoryId);
            objCommand.Parameters.Add("SUB_CATEGORY_ID", subCategoryId);

            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetBarcodeListByStyleNameDropdown(string styleName)
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "PI.PRODUCT_CODE " +
                      " FROM PRODUCT_ITEM PI JOIN PRODUCT P ON (PI.PRODUCT_ID = P.PRODUCT_ID) WHERE P.PRODUCT_STYLE = :PRODUCT_STYLE order by PI.PRODUCT_CODE ";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };
            objCommand.Parameters.Add("PRODUCT_STYLE", styleName);

            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetCustomerTypeListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "CUSTOMER_TYPE_ID, " +
                      "CUSTOMER_TYPE_NAME " +
                      "FROM VEW_CUSTOMER_TYPE ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetItemCodeList()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "ITEM_CODE, " +
                      "ITEM_SEARCH_NAME " +
                      "FROM VEW_PRODUCT_ITEM_INFO ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetAllProductStyleList()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "PRODUCT_ID, " +
                      "PRODUCT_STYLE " +
                      "FROM VEW_PRODUCT WHERE ACTIVE_YN = 'Y' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetBrandListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "BRAND_ID, " +
                      "BRAND_NAME " +
                      "FROM VEW_BRAND ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetMerchandiserListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "EMPLOYEE_ID, " +
                      "EMPLOYEE_NAME " +
                      "FROM VEW_MERCHANDISER ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetDesignerListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "EMPLOYEE_ID, " +
                      "EMPLOYEE_NAME " +
                      "FROM VEW_DESIGNER ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetMeasuringUnitListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "UNIT_ID, " +
                      "UNIT_NAME " +
                      "FROM VEW_UNIT ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetVendorListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "VENDOR_ID, " +
                      "VENDOR_NAME " +
                      "FROM VEW_VENDOR ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetPurchaseReceiveListDropdown(string purchaseDate)
        {
            DataTable dt = new DataTable();
            var sql = "SELECT  pr.PURCHASE_RECEIVE_NUMBER, pr.PURCHASE_RECEIVE_NUMBER " +
                      "FROM VEW_PURCHASE_RECEIVE pr LEFT JOIN STORE_DELIVERY sd ON (pr.PURCHASE_RECEIVE_NUMBER = sd.PURCHASE_RECEIVE_NUMBER) " +
                      "WHERE sd.STORE_DELIVERY_NUMBER IS NULL AND PURCHASE_DATE = TO_DATE (:PURCHASE_DATE, 'dd/mm/yyyy') ";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };
            objCommand.Parameters.Add("PURCHASE_DATE", purchaseDate);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetPurchaseReceiveListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT DISTINCT " +
                      "PURCHASE_RECEIVE_NUMBER, " +
                      "PURCHASE_RECEIVE_NUMBER " +
                      "FROM VEW_PURCHASE_RECEIVE " +
                      "WHERE PURCHASE_RECEIVE_NUMBER NOT IN" +
                      "(SELECT PURCHASE_RECEIVE_NUMBER FROM VEW_STOREDELIVERY_GRID)";

            OracleCommand objCommand = new OracleCommand(sql) { CommandType = CommandType.Text };

            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetRegisterInfoListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "REGISTER_ID, " +
                      "REGISTER_PERSON_NAME " +
                      "FROM VEW_REGISTER_INFO ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetAllShopRequisitionListDropDown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "REQUISITION_ID, " +
                      "REQUISITION_NO " +
                      "FROM VEW_REQUISITION_MAIN WHERE DELIVERY_YN = 'N' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetSeasonListDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "SEASON_ID, " +
                      "SEASON_NAME " +
                      "FROM VEW_SEASON ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetStockTransferChallanNo()
        {
            DataTable dt = new DataTable();
            var sql = " SELECT " +
                      " STOCK_TRANSFER_CHALLAN_NUM, " +
                      " STOCK_TRANSFER_CHALLAN_NUM " +
                      " FROM VEW_STOCK_TRANSFER_MAIN " +
                      " WHERE TRANSFER_SHOPID_TO = (SELECT WARE_HOUSE_ID FROM WARE_HOUSE)  AND RECEIVE_YN = 'N' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetColorForDropdown()
        {
            DataTable dt = new DataTable();
            var sql = " SELECT " +
                      " ATTRIBUTE_VALUE_NAME " +
                      " FROM L_ATTRIBUTE_VALUE " +
                      " WHERE ATTRIBUTE_ID = 5 ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }

        public async Task<DataTable> GetHoldChallanNoDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT " +
                      "O_PURCHASE_RECEIVE_NUMBER " +
                      "FROM OTHER_PURCHASE_RECEIVE WHERE HOLD_YN = 'Y' ";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn; 
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }
        
        public async Task<DataTable> GetLogedInEmployeeDropdown()
        {
            DataTable dt = new DataTable();
            var sql = "SELECT EMPLOYEE_ID AS VALUE ,EMPLOYEE_ID ||'--'|| EMPLOYEE_NAME || ' (' || EMPLOYEE_ROLE || ')' AS TEXT  FROM ADMIN_LOGIN";

            OracleCommand objCommand = new OracleCommand(sql);
            OracleDataAdapter objDataAdapter = new OracleDataAdapter(objCommand);
            using (OracleConnection strConn = GetConnection())
            {
                try
                {
                    objCommand.Connection = strConn;
                    await strConn.OpenAsync();
                    objDataAdapter.Fill(dt);
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
            return dt;
        }
        //End

    }
}