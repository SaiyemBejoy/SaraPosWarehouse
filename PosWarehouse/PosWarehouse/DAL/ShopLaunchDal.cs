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
    public class ShopLaunchDal
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

        public async Task<string> SaveData(ShopLaunchModel objShopLaunchModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_LAUNCH")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_SHOP_LAUNCH_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopLaunchModel.ShopLaunchId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopLaunchModel.ProductId;
            objOracleCommand.Parameters.Add("P_LAUNCH_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopLaunchModel.LaunchDate) ? objShopLaunchModel.LaunchDate : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopLaunchModel.ShopId;
            objOracleCommand.Parameters.Add("P_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopLaunchModel.UpdatedBy) ? objShopLaunchModel.UpdatedBy : null;
            objOracleCommand.Parameters.Add("P_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopLaunchModel.WarehouseId) ? objShopLaunchModel.WarehouseId : null;
            objOracleCommand.Parameters.Add("P_message", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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

                    strMessage = objOracleCommand.Parameters["P_message"].Value.ToString();
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