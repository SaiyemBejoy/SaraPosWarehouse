using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;

namespace PosWarehouse.DAL
{
    public class CloudPosDataEntryDal
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

        public async Task<List<WareHouseBarcodeUpdate>> GetItemFromDatabase()
        {
            var colorObj = await GetAllColor();
            var colorList = colorObj.ToList();
            var sizeObj = await GetAllSize();
            var sizeList = sizeObj.ToList();

            var sql = "SELECT * FROM VEW_PRODUCT_BARCODE_CHANGE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<WareHouseBarcodeUpdate> objProductModel = new List<WareHouseBarcodeUpdate>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                WareHouseBarcodeUpdate modelData = new WareHouseBarcodeUpdate();
                                string attribute = "";
                                string[] attributeArray = new string[2];
                                modelData.Attribute = new string[2];

                                modelData.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                modelData.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                modelData.Style = objDataReader["STYLE"].ToString();
                                attribute = objDataReader["ATTRIBUTE_NAME"].ToString();
                                modelData.BarCode = objDataReader["BARCODE"].ToString();

                                attributeArray = attribute.Split('|').Select(a => a.Trim()).Take(2).ToArray();

                                for (int i = 0; i < attributeArray.Length; i++)
                                {
                                    switch (i)
                                    {
                                        case 0:
                                        {
                                            var size = sizeList.FirstOrDefault(s => s.AttributeName.ToLower().Trim() == attributeArray[i].ToLower().Trim());

                                            if (size != null)
                                                modelData.Attribute[i] = size.AttributeName;
                                            break;
                                        }
                                        case 1:
                                        {
                                            var color = colorList.FirstOrDefault(s => s.AttributeName.ToLower().Trim() == attributeArray[i].ToLower().Trim());

                                            if (color != null)
                                                modelData.Attribute[i] = color.AttributeName;
                                            break;
                                        }
                                    }
                                }

                                objProductModel.Add(modelData);
                            }

                            return objProductModel;
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

        public async Task<IEnumerable<AttributeModel>> GetAllColor()
        {
            var sql = "SELECT * FROM VEW_PRODUCT_COLOR ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<AttributeModel> objColorModel = new List<AttributeModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                AttributeModel modelData = new AttributeModel();

                                modelData.AttributeId = Convert.ToInt32(objDataReader["COLOR_ID"].ToString());
                                modelData.AttributeName = objDataReader["COLOR_NAME"].ToString();

                                objColorModel.Add(modelData);
                            }

                            return objColorModel;
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

        public async Task<IEnumerable<AttributeModel>> GetAllSize()
        {
            var sql = "SELECT * FROM VEW_PRODUCT_SIZE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<AttributeModel> objColorModel = new List<AttributeModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                AttributeModel modelData = new AttributeModel();

                                modelData.AttributeId = Convert.ToInt32(objDataReader["SIZE_ID"].ToString());
                                modelData.AttributeName = objDataReader["SIZE_NAME"].ToString();

                                objColorModel.Add(modelData);
                            }

                            return objColorModel;
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

        public async Task<string> UpdateBarCodeInProductItem(WareHouseBarcodeUpdate model)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_BARCODE_CHANGE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.ProductId != 0 ? model.ProductId.ToString() : null;
            objOracleCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = model.ItemId != 0 ? model.ItemId.ToString() : null;
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(model.BarCode.Trim()) ? model.BarCode.Trim() : null;

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

                    string strMsg = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();

                    return strMsg;
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
        }
    }
}