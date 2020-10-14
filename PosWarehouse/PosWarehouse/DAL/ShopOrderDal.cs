using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class ShopOrderDal
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

        public async Task<List<RequisitionMainModel>> ShopOrderList()
        {

            var sql = "SELECT " +
                      "REQUISITION_ID," +
                        "REQUISITION_NO," +
                        "to_date(REQUISITION_DATE,'dd/mm/yyyy')REQUISITION_DATE," +
                        "CREATED_BY," +
                        "REQUISITION_AUTO_ID," +
                        "DELIVERY_YN," +
                        "SHOP_ID," +
                        "SHOP_NAME " +
                      "FROM VEW_REQUISITION_MAIN ";
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<RequisitionMainModel> objRequisitionMainModel = new List<RequisitionMainModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                RequisitionMainModel model = new RequisitionMainModel
                                {
                                    RequisitionAutoId = Convert.ToInt32(objDataReader["REQUISITION_AUTO_ID"].ToString()),
                                    RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString()),
                                    RequisitionNo = objDataReader["REQUISITION_NO"].ToString(),
                                    RequisitionDate = objDataReader["REQUISITION_DATE"].ToString(),              
                                    CreatedBy = objDataReader["CREATED_BY"].ToString(),
                                    Delivery_YN = objDataReader["DELIVERY_YN"].ToString(),
                                    ShopId = objDataReader["SHOP_NAME"].ToString()
                                };
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
        
        public async Task<List<RequisitionMainItemModel>> GetShopOrderItemList(int requisitionId)
        {
            var sql = "SELECT " +
                      "REQUISITION_MAIN_ITEM_ID," +
                        "REQUISITION_AUTO_ID," +
                        "BARCODE," +
                        "ITEM_NAME," +
                        "PRICE," +
                        "QUANTITY "+
                      "FROM VEW_REQUISITION_MAIN_ITEM where REQUISITION_AUTO_ID =:REQUISITION_AUTO_ID ";
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REQUISITION_AUTO_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = requisitionId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<RequisitionMainItemModel> objRequisitionMainItemModel = new List<RequisitionMainItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                RequisitionMainItemModel model = new RequisitionMainItemModel
                                {
                                    RequisitionMainItemId = Convert.ToInt32(objDataReader["REQUISITION_MAIN_ITEM_ID"].ToString()),
                                    RequisitionAutoId = Convert.ToInt32(objDataReader["REQUISITION_AUTO_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    Price = objDataReader["PRICE"].ToString(),
                                    RqsnQuantity = objDataReader["QUANTITY"].ToString(),

                                };
                                objRequisitionMainItemModel.Add(model);
                            }
                            return objRequisitionMainItemModel;
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

        public async Task<RequisitionMainModel> ShopOrderListByRequisitionNum(string requisitionNumber)
        {
            var sql = "SELECT " +
                      "REQUISITION_ID," +
                        "REQUISITION_NO," +
                        "to_date(REQUISITION_DATE,'dd/mm/yyyy')REQUISITION_DATE," +
                        "CREATED_BY," +
                        "SHOP_ID," +
                        "SHOP_NAME " +
                      "FROM VEW_REQUISITION_MAIN where REQUISITION_NO =:REQUISITION_NO ";
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {

                    objCommand.Parameters.Add(":REQUISITION_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = requisitionNumber;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                       RequisitionMainModel objRequisitionMainModel = new RequisitionMainModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {

                                objRequisitionMainModel.RequisitionId =
                                    Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString());
                                objRequisitionMainModel.RequisitionNo = objDataReader["REQUISITION_NO"].ToString();
                                objRequisitionMainModel.RequisitionDate = objDataReader["REQUISITION_DATE"].ToString();
                                objRequisitionMainModel.CreatedBy = objDataReader["CREATED_BY"].ToString();
                                objRequisitionMainModel.ShopId = objDataReader["SHOP_ID"].ToString();
                                objRequisitionMainModel.ShopName = objDataReader["SHOP_NAME"].ToString();
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

        public async Task<List<RequisitionDeliveryModel>> GetShopOrderItemInfoDelivery(int requisitionId)
        {
            var sql = "SELECT " +
                      "REQUISITION_MAIN_ITEM_ID," +
                        "REQUISITION_ID," +
                        "ITEM_ID," +
                        "PRODUCT_ID," +
                        "BARCODE," +
                        "ITEM_NAME," +
                        "PRICE," +
                        "PRODUCT_STOCK, " +
                        "VAT " +
                      "FROM VEW_REQUIS_DELIV_ITEMINFO where REQUISITION_ID =:REQUISITION_ID ";
            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REQUISITION_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = requisitionId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<RequisitionDeliveryModel> objRequisitionDeliveryModel = new List<RequisitionDeliveryModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                RequisitionDeliveryModel model = new RequisitionDeliveryModel
                                {
                                    RequisitionMainItemId = Convert.ToInt32(objDataReader["REQUISITION_MAIN_ITEM_ID"].ToString()),
                                    RequisitionId = Convert.ToInt32(objDataReader["REQUISITION_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    Price = objDataReader["PRICE"].ToString(),
                                    ProductStock = Convert.ToInt32(objDataReader["PRODUCT_STOCK"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString()),

                                };
                                objRequisitionDeliveryModel.Add(model);
                            }
                            return objRequisitionDeliveryModel;
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