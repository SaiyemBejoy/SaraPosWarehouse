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
    public class BarCodeDAL
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

        #region BarCode

        public async Task<BarCodeModel> GetADataByBarcode(string productstyle, string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "PRODUCT_ID," +
                      "ITEM_ID," +
                      "ITEM_NAME," +
                      "ITEM_CODE," +
                      "SALE_PRICE," +
                      "PRODUCT_STYLE," +
                      "BRAND_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PRODUCT_ITEM_INFO where PRODUCT_STYLE = :PRODUCT_STYLE AND WARE_HOUSE_ID =:WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {

                    objCommand.Parameters.Add(":PRODUCT_STYLE", OracleDbType.Varchar2, ParameterDirection.Input).Value = productstyle;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        BarCodeModel objBarCodeModel = new BarCodeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objBarCodeModel.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                objBarCodeModel.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                objBarCodeModel.ItemName = objDataReader["ITEM_NAME"].ToString();
                                objBarCodeModel.ItemCode = objDataReader["ITEM_CODE"].ToString();
                                objBarCodeModel.ProductStyle = objDataReader["PRODUCT_STYLE"].ToString();
                                objBarCodeModel.SalePrice = objDataReader["SALE_PRICE"].ToString();
                                objBarCodeModel.BrandName = objDataReader["BRAND_NAME"].ToString();
                                objBarCodeModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objBarCodeModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objBarCodeModel.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objBarCodeModel;
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

        public async Task<List<BarCodeModel>> GetDataListByProductStyle(string productstyle, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "PRODUCT_ID," +
                      "ITEM_ID," +
                      "ITEM_NAME," +
                      "ITEM_CODE," +
                      "SALE_PRICE," +
                      "PRODUCT_STYLE," +
                      "BRAND_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PRODUCT_ITEM_INFO where PRODUCT_STYLE = :PRODUCT_STYLE AND WARE_HOUSE_ID =:WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ORDER BY ITEM_NAME  ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_STYLE", OracleDbType.Varchar2, ParameterDirection.Input).Value = productstyle;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<BarCodeModel> objBarCodeModel = new List<BarCodeModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                BarCodeModel model = new BarCodeModel
                                {
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemName = objDataReader["ITEM_NAME"].ToString(),
                                    ItemCode = objDataReader["ITEM_CODE"].ToString(),
                                    ProductStyle = objDataReader["PRODUCT_STYLE"].ToString(),
                                    SalePrice = objDataReader["SALE_PRICE"].ToString(),
                                    BrandName = objDataReader["BRAND_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objBarCodeModel.Add(model);
                            }
                            return objBarCodeModel;
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

        public async Task<string> SavePrintedBarCode(List<BarCodeTableModel> objBarCodeModel, string updateBy, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PRINTED_BARCODE_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objBarCodeModel)
                        {
                            for (int i = 0; i < Convert.ToInt32(data.Quantity); i++)
                            {
                                objCommand.Parameters.Clear();

                                objCommand.Parameters.Add("P_PRINTED_BARCODE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.PrintedBarCodeId != 0 ? data.PrintedBarCodeId.ToString() : null;
                                objCommand.Parameters.Add("P_ITEM_CODE", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(data.ItemCode) ? data.ItemCode : null;
                                objCommand.Parameters.Add("P_ITEM_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.ItemName) ? data.ItemName : null;
                                objCommand.Parameters.Add("P_BRAND_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.BrandName) ? data.BrandName : null;

                                objCommand.Parameters.Add("P_BARCODE_IMAGE", OracleDbType.Blob, ParameterDirection.Input).Value = data.BarCodeImageArray;

                                objCommand.Parameters.Add("P_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.Quantity) ? data.Quantity : null;
                                objCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.SalePrice) ? data.SalePrice : null;
                                objCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(updateBy) ? updateBy : null;
                                objCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
                                objCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

                                objCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

                                using (_trans = objConnection.BeginTransaction())
                                {
                                    try
                                    {
                                        await objCommand.ExecuteNonQueryAsync();
                                        _trans.Commit();

                                        vMessage = objCommand.Parameters["P_MESSAGE"].Value.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        _trans.Rollback();
                                        throw new Exception("Error : " + ex.Message);
                                    }
                                    finally
                                    {
                                        _trans.Dispose();
                                    }
                                }
                            }


                        }
                        return vMessage;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error : " + ex.Message);
                    }
                    finally
                    {
                        objConnection.Close();
                        objCommand.Dispose();
                        objConnection.Dispose();
                    }
                }
            }
        }

        public async Task<string> DeletePreviousBarCodeDatafromTable()
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PRINTED_BARCODE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };
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



        #endregion


    }
}