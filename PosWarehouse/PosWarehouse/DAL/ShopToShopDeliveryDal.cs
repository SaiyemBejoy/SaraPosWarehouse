using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class ShopToShopDeliveryDal
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



        public async Task<string> SaveShopToShopDeliveryData(ShopToShopDeliveryModel objShopToShopDeliveryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_TO_SHOP_DELIVERY_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DELIVERY_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopDeliveryModel.DeliveryId;
            objOracleCommand.Parameters.Add("P_DELIVERY_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.DeliveryNumber) ? objShopToShopDeliveryModel.DeliveryNumber : null;
            objOracleCommand.Parameters.Add("P_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.RequisitionNumber) ? objShopToShopDeliveryModel.RequisitionNumber : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.DeliveryShopIdTo) ? objShopToShopDeliveryModel.DeliveryShopIdTo : null;
            objOracleCommand.Parameters.Add("P_DELIVERY_SHOPID_FROM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.DeliveryShopIdFrom) ? objShopToShopDeliveryModel.DeliveryShopIdFrom : null;
            objOracleCommand.Parameters.Add("P_DELIVERED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.DeliveredBy) ? objShopToShopDeliveryModel.DeliveredBy : null;
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

        public async Task<string> SaveShopToShopDeliveryItem(ShopToShopRequDeliveryItemModel objShopToShopRequDeliveryItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_SHOP_DELIVY_ITEM_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_DELIVERY_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.DeliveryItemId;
            objOracleCommand.Parameters.Add("P_DELIVERY_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.DeliveryId;
            objOracleCommand.Parameters.Add("P_ITEM_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.ItemId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.ProductId;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequDeliveryItemModel.Barcode) ? objShopToShopRequDeliveryItemModel.Barcode : null;
            objOracleCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopRequDeliveryItemModel.ItemName) ? objShopToShopRequDeliveryItemModel.ItemName : null;
            objOracleCommand.Parameters.Add("P_PRICE ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.Price;
            objOracleCommand.Parameters.Add("P_QUANTITY ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopToShopRequDeliveryItemModel.Quantity;
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

        public async Task<ShopToShopDeliveryModel> GetAllShopToShopDeliveryData(string toShopId,string requisitionNumber)
        {
            var sql = "SELECT " +
                      "DELIVERY_ID," +
                        "DELIVERY_NUMNER," +
                        "REQUISITION_NUMNER," +
                        "DELIVERY_SHOPID_TO," +
                        "DELIVERY_SHOPNAME_TO," +
                        "DELIVERY_SHOPID_FROM," +
                        "DELIVERY_SHOPNAME_FROM," +
                        "DELIVERY_DATE," +
                        "DELIVERED_BY," +
                        "RECEIVED_YN," +
                        "RECEIVED_DATE," +
                        "RECEIVED_BY," +
                        "RETURN_YN," +
                        "RETURN_DATE," +
                        "RETURN_BY " +
                      "FROM VEW_SHOP_REQ_DELIVERY_MAIN where DELIVERY_SHOPID_TO = :DELIVERY_SHOPID_TO AND REQUISITION_NUMNER =:REQUISITION_NUMNER ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DELIVERY_SHOPID_TO", OracleDbType.Varchar2, ParameterDirection.Input).Value = toShopId;
                    objCommand.Parameters.Add(":REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = requisitionNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                       ShopToShopDeliveryModel objShopToShopDeliveryModel = new ShopToShopDeliveryModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {

                                objShopToShopDeliveryModel.DeliveryId = Convert.ToInt32(objDataReader["DELIVERY_ID"].ToString());
                                objShopToShopDeliveryModel.DeliveryNumber = objDataReader["DELIVERY_NUMNER"].ToString();
                                objShopToShopDeliveryModel.RequisitionNumber = objDataReader["REQUISITION_NUMNER"].ToString();
                                objShopToShopDeliveryModel.DeliveryShopIdTo = objDataReader["DELIVERY_SHOPID_TO"].ToString();
                                objShopToShopDeliveryModel.DeliveryShopNameTo = objDataReader["DELIVERY_SHOPNAME_TO"].ToString();
                                objShopToShopDeliveryModel.DeliveryShopIdFrom = objDataReader["DELIVERY_SHOPID_FROM"].ToString();
                                objShopToShopDeliveryModel.DeliveryShopNameFrom = objDataReader["DELIVERY_SHOPNAME_FROM"].ToString();
                                objShopToShopDeliveryModel.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                objShopToShopDeliveryModel.DeliveredBy = objDataReader["DELIVERED_BY"].ToString();
                                objShopToShopDeliveryModel.ReceivedYN = objDataReader["RECEIVED_YN"].ToString();
                                objShopToShopDeliveryModel.ReceivedBy = objDataReader["RECEIVED_BY"].ToString();
                                objShopToShopDeliveryModel.ReceivedDate = objDataReader["RECEIVED_DATE"].ToString();
                                objShopToShopDeliveryModel.ReturnYN = objDataReader["RETURN_YN"].ToString();
                                objShopToShopDeliveryModel.ReturnBy = objDataReader["RETURN_BY"].ToString();
                                objShopToShopDeliveryModel.ReturnDate = objDataReader["RETURN_DATE"].ToString();
                            }
                            return objShopToShopDeliveryModel;
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

        public async Task<List<ShopToShopRequDeliveryItemModel>> GetAllShopToShopDeliveryItemData(int deliveryId)
        {
            var sql = "SELECT " +
                      "DELIVERY_ITEM_ID," +
                        "DELIVERY_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "BARCODE," +
                        "ITEM_NAME," +
                        "PRICE," +
                        "QUANTITY " +
                      "FROM VEW_SHOP_REQ_DELIVERY_ITEM where DELIVERY_ID = :DELIVERY_ID ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DELIVERY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = deliveryId;
                  
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopToShopRequDeliveryItemModel> objShopToShopRequDeliveryItemModel = new List<ShopToShopRequDeliveryItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopToShopRequDeliveryItemModel model = new ShopToShopRequDeliveryItemModel
                                {
                                    DeliveryId = Convert.ToInt32(objDataReader["DELIVERY_ITEM_ID"].ToString()),
                                    DeliveryItemId = Convert.ToInt32(objDataReader["DELIVERY_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Price = Convert.ToDouble(objDataReader["PRICE"].ToString()),
                                    Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),

                                };


                                objShopToShopRequDeliveryItemModel.Add(model);
                            }
                            return objShopToShopRequDeliveryItemModel;
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

        public async Task<string> UpdateReQuiAndDeliveryTable(ShopToShopDeliveryModel objShopToShopDeliveryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_REQ_DELIVE_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_REQUISITION_NUMNER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.RequisitionNumber) ? objShopToShopDeliveryModel.RequisitionNumber : null;
            objOracleCommand.Parameters.Add("P_RCEIVED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopToShopDeliveryModel.ReceivedBy) ? objShopToShopDeliveryModel.ReceivedBy : null;
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