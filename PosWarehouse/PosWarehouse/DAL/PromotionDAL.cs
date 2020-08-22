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
    public class PromotionDAL
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

        #region "Price Change"

        public DataTable GetShopListDropdown()
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
                    strConn.Open();
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
        #endregion


        #region "Circular Price Change"

        public async Task<List<CircularPriceChangeMain>> GetCircularPriceChangeList(string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "CIRCULAR_ID," +
                      "CIRCULAR_NAME," +
                      "to_char(EFFECT_DATE,'dd/mm/yyyy') EFFECT_DATE ," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIRCULAR_PRICE_CHANGE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularPriceChangeMain> objCircularPriceChangeMainModels = new List<CircularPriceChangeMain>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularPriceChangeMain model = new CircularPriceChangeMain
                                {
                                    CircularId = Convert.ToInt32(objDataReader["CIRCULAR_ID"].ToString()),
                                    CircularName = objDataReader["CIRCULAR_NAME"].ToString(),
                                    EffectiveDate = objDataReader["EFFECT_DATE"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCircularPriceChangeMainModels.Add(model);
                            }
                            return objCircularPriceChangeMainModels;
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

        public async Task<CircularPriceChangeMain> GetACircularPriceChangeMain(int circularId, string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "CIRCULAR_ID," +
                      "CIRCULAR_NAME," +
                      "to_char(EFFECT_DATE,'dd/mm/yyyy') EFFECT_DATE ," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIRCULAR_PRICE_CHANGE where CIRCULAR_ID = :CIRCULAR_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {

                    objCommand.Parameters.Add(":CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CircularPriceChangeMain objCircularPriceChangeMainModels = new CircularPriceChangeMain();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCircularPriceChangeMainModels.CircularId =
                                    Convert.ToInt32(objDataReader["CIRCULAR_ID"].ToString());
                                objCircularPriceChangeMainModels.CircularName = objDataReader["CIRCULAR_NAME"].ToString();
                                objCircularPriceChangeMainModels.EffectiveDate = objDataReader["EFFECT_DATE"].ToString();
                                objCircularPriceChangeMainModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCircularPriceChangeMainModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCircularPriceChangeMainModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCircularPriceChangeMainModels;
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
        public async Task<List<CircularPriceChangeSub>> GetCircularPriceChangeSubList(int circularId, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "CIRCULAR_ID," +
                      "CIRCULAR_PRICE_SUB_ID," +
                      "BARCODE_NO," +
                      "PURCHASE_PRICE," +
                      "PRE_SALE_PRICE," +
                      "NEW_SALE_PRICE," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM CIRCULAR_PRICE_CHANGE_SUB where CIRCULAR_ID =:CIRCULAR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularPriceChangeSub> objCircularPriceChangeSubModels = new List<CircularPriceChangeSub>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularPriceChangeSub model = new CircularPriceChangeSub
                                {
                                    CircularId = Convert.ToInt32(objDataReader["CIRCULAR_ID"].ToString()),
                                    CircularPriceSubId = Convert.ToInt32(objDataReader["CIRCULAR_PRICE_SUB_ID"].ToString()),
                                    BarcodeNo = objDataReader["BARCODE_NO"].ToString(),
                                    PurchasePrice = objDataReader["PURCHASE_PRICE"].ToString(),
                                    PreSalePrice = objDataReader["PRE_SALE_PRICE"].ToString(),
                                    NewSalePrice = objDataReader["NEW_SALE_PRICE"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCircularPriceChangeSubModels.Add(model);
                            }
                            return objCircularPriceChangeSubModels;
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

        public async Task<List<CircularPriceChangeShop>> GetALLCircularPriceChangeShopList(int circularId )
        {
            var sql = "SELECT " +
                      "CIRCULAR_ID," +
                        "CIRCULAR_PRICE_CHANGE_SHOP_ID," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "UPDATE_BY," +
                        "UPDATE_DATE," +
                        "SHOP_ID," +
                        "SHOP_NAME," +
                        "WARE_HOUSE_ID " +
                      "FROM VEW_CIR_PRICE_CHAG_SHOP where CIRCULAR_ID =:CIRCULAR_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;
                    //objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;   
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularPriceChangeShop> objCircularPriceChangeShop = new List<CircularPriceChangeShop>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularPriceChangeShop model = new CircularPriceChangeShop
                                {
                                    CircularId = Convert.ToInt32(objDataReader["CIRCULAR_ID"].ToString()),
                                    CircularPriceChangeShopId  = Convert.ToInt32(objDataReader["CIRCULAR_PRICE_CHANGE_SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString(),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString() 
                                };
                                objCircularPriceChangeShop.Add(model);
                            }
                            return objCircularPriceChangeShop;
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
        public async Task<List<int>> GetCircularPriceChangeShopList(int circularId, string wareHouseId)
        {
            var sql = "SELECT " +
                      "CIRCULAR_ID," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM CIRCULAR_PRICE_CHANGE_SHOP where CIRCULAR_ID = :CIRCULAR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) {CommandType = CommandType.Text})
                {
                    objCommand.Parameters.Add(":CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<int> shopId = new List<int>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                //CircularPriceChangeShop model = new CircularPriceChangeShop
                                //{
                                //    CircularId = Convert.ToInt32(objDataReader["CIRCULAR_ID"].ToString()),
                                //    CircularPriceChangeShopId = Convert.ToInt32(objDataReader["CIRCULAR_PRICE_CHANGE_SHOP_ID"].ToString()),
                                //    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                //    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                //    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString()
                                
                                //};
                                int shop = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                shopId.Add(shop);
                            }
                            return shopId;
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

        public async Task<string> SaveAndUpdateCircularPriceChangeMain(CircularPriceChangeMain objCircularPriceChangeMainModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIRPRICHANGEMAIN_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_circular_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeMainModel.CircularId;
            objOracleCommand.Parameters.Add("p_circular_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.CircularName) ? objCircularPriceChangeMainModel.CircularName : null;
            objOracleCommand.Parameters.Add("p_effect_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.EffectiveDate) ? objCircularPriceChangeMainModel.EffectiveDate : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.UpdateBy) ? objCircularPriceChangeMainModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.WareHouseId) ? objCircularPriceChangeMainModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_SELECT_ALL_SHOP", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.SelectALlShop) ? objCircularPriceChangeMainModel.SelectALlShop : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeMainModel.ShopId) ? objCircularPriceChangeMainModel.ShopId : null;

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

        public async Task<string> SaveAndUpdateSubCircularPriceChangeSub(CircularPriceChangeSub objCircularPriceChangeSubModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_PRICE_CHANGE_SUB_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.CircularId;
            objOracleCommand.Parameters.Add("p_circular_price_sub_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.CircularPriceSubId;

            objOracleCommand.Parameters.Add("p_barcode_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.BarcodeNo) ? objCircularPriceChangeSubModel.BarcodeNo : null;
            objOracleCommand.Parameters.Add("p_purchase_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.PurchasePrice;
            objOracleCommand.Parameters.Add("p_pre_sale_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.PreSalePrice;
            objOracleCommand.Parameters.Add("p_new_sale_price", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.NewSalePrice;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.UpdateBy) ? objCircularPriceChangeSubModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.WareHouseId) ? objCircularPriceChangeSubModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.ShopId) ? objCircularPriceChangeSubModel.ShopId : null;

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

        public async Task<string> ChangeAllWarehousePrice(CircularPriceChangeSub objCircularPriceChangeSubModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_ALL_PRICE_CHANGE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_BARCODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.BarcodeNo) ? objCircularPriceChangeSubModel.BarcodeNo : null;
            objOracleCommand.Parameters.Add("P_NEW_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.NewSalePrice;
            objOracleCommand.Parameters.Add("P_OLD_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeSubModel.PreSalePrice;
            objOracleCommand.Parameters.Add("P_EFFECTIVE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeSubModel.EffectiveDate) ? objCircularPriceChangeSubModel.EffectiveDate : null;

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

        public async Task<string> SaveAndUpdateSubCircularPriceChangeForShop(CircularPriceChangeShop objCircularPriceChangeShopModel)
        {

            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_circular_price_change_shop")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeShopModel.CircularId;
            objOracleCommand.Parameters.Add("p_price_change_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeShopModel.CircularPriceChangeShopId;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularPriceChangeShopModel.ShopId;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeShopModel.UpdateBy) ? objCircularPriceChangeShopModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularPriceChangeShopModel.WareHouseId) ? objCircularPriceChangeShopModel.WareHouseId : null;

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

        public async Task<string> DeleteCircularPriceChange(int circularId, string wareHouseId, string shopId)
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_PRICE_CHANGE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;

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

                    strMsg = objOracleCommand.Parameters["p_message"].Value.ToString();
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
            return strMsg;
        }

        public async Task<List<ShopViewModel>> GetShopList()
        {
            var sql = "SELECT " +
                      "SHOP_ID," +
                      "SHOP_NAME " +
                         "FROM VEW_SHOP WHERE ACTIVE_YN = 'Y' ORDER BY SHOP_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopViewModel> objShopModels = new List<ShopViewModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopViewModel model = new ShopViewModel()
                                {
                                  
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString(),
                                   
                                };
                                objShopModels.Add(model);
                            }

                            return objShopModels;
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

        public async Task<BarCodeModel> SearchProductItemByBarcode(string barcode, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "PRODUCT_ID," +
                      "PRODUCT_CODE," +
                      "PRODUCT_NAME," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PRODUCT_ITEM where PRODUCT_CODE = :PRODUCT_CODE  AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = barcode;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        BarCodeModel objCategoryModels = new BarCodeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCategoryModels.ProductId =
                                    Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                objCategoryModels.ItemCode = objDataReader["PRODUCT_CODE"].ToString();
                                objCategoryModels.ProductName = objDataReader["PRODUCT_NAME"].ToString();
                                objCategoryModels.PurchasePrice = objDataReader["PURCHASE_PRICE"].ToString();
                                objCategoryModels.SalePrice = objDataReader["SALE_PRICE"].ToString();
                                objCategoryModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCategoryModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCategoryModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCategoryModels;
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

            #region ShopURL
            public async Task<string> GetAllShopUrl(int shopId)
            {
            var sql = "SELECT " +
                      "SHOP_URL " +
                      "FROM VEW_SHOP WHERE SHOP_ID =:SHOP_ID AND ACTIVE_YN ='Y' ";
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection){CommandType = CommandType.Text})
                {

                        objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;


                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                       string objShopModels = string.Empty;
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objShopModels = objDataReader["SHOP_URL"].ToString();   
                            }
                            return objShopModels;
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

            public async Task<string> GetAllShopName(int shopId)
            {
                var sql = "SELECT " +
                          "SHOP_NAME  " +
                          "FROM VEW_SHOP WHERE SHOP_ID =:SHOP_ID AND ACTIVE_YN ='Y' ";
                using (OracleConnection objConnection = GetConnection())
                {
                    using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                    {

                        objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;


                        await objConnection.OpenAsync();
                        using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                        {
                            string objShopModels = string.Empty;
                            try
                            {
                                while (await objDataReader.ReadAsync())
                                {
                                    objShopModels = objDataReader["SHOP_NAME"].ToString();
                                }
                                return objShopModels;
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

        #region Circular_Discount_Promotion


        public async Task<List<CircularDiscountPromotionMain>> CircularDiscountPromotionList(string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "DISCOUNT_CIRCULAR_ID," +
                      "DISCOUNT_CIRCULAR_NAME," +
                      "to_char(VALID_FROM,'dd/mm/yyyy') VALID_FROM ," +
                      "to_char(VALID_TO,'dd/mm/yyyy') VALID_TO ," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIR_DISCOUNT_PROMOTION where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularDiscountPromotionMain> objCircularDiscountPromotionMainModels = new List<CircularDiscountPromotionMain>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularDiscountPromotionMain model = new CircularDiscountPromotionMain
                                {
                                    DiscountCircularId = Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_ID"].ToString()),
                                    DiscountCircularName = objDataReader["DISCOUNT_CIRCULAR_NAME"].ToString(),
                                    ValidFrom = objDataReader["VALID_FROM"].ToString(),
                                    ValidTo = objDataReader["VALID_TO"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCircularDiscountPromotionMainModels.Add(model);
                            }
                            return objCircularDiscountPromotionMainModels;
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

        public async Task<string> SaveAndUpdateCircularDiscountPromotionMain(CircularDiscountPromotionMain objCircularDiscountPromotionMainModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_DIS_PROMOTION_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_circular_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionMainModel.DiscountCircularId;
            objOracleCommand.Parameters.Add("p_discount_circular_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.DiscountCircularName) ? objCircularDiscountPromotionMainModel.DiscountCircularName : null;
            objOracleCommand.Parameters.Add("p_valid_from", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.ValidFrom) ? objCircularDiscountPromotionMainModel.ValidFrom : null;
            objOracleCommand.Parameters.Add("p_valid_to", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.ValidTo) ? objCircularDiscountPromotionMainModel.ValidTo : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.UpdateBy) ? objCircularDiscountPromotionMainModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.WareHouseId) ? objCircularDiscountPromotionMainModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionMainModel.ShopId) ? objCircularDiscountPromotionMainModel.ShopId : null;

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

        public async Task<string> SaveAndUpdateSubCircularDiscountPromotionSub(CircularDiscountPromotionItem objCircularDiscountPromotionItemModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_DIS_PROMOTION_SUB_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.DiscountCircularId;
            objOracleCommand.Parameters.Add("p_discount_circular_sub_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.DiscountCircularItemId;
            objOracleCommand.Parameters.Add("p_Barcode", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.Barcode;
            objOracleCommand.Parameters.Add("p_PURCHASE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.PurchasePrice;
            // style er table nai..Tai subcategoryID save kora hoise for testing.
            objOracleCommand.Parameters.Add("p_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.SalePrice;
            objOracleCommand.Parameters.Add("p_DISCOUNT_PERCENT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.DiscountPercent;
            objOracleCommand.Parameters.Add("p_DISCOUNT_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionItemModel.DiscountAmount;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionItemModel.UpdateBy) ? objCircularDiscountPromotionItemModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionItemModel.WareHouseId) ? objCircularDiscountPromotionItemModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionItemModel.ShopId) ? objCircularDiscountPromotionItemModel.ShopId : null;

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

        public async Task<string> SaveAndUpdateCircularDiscountPromotionForShop(CircularDiscountPromotionShop objCircularDiscountPromotionShopModel)
        {

            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_cir_dis_Promotion_shop")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionShopModel.DiscountCircularId;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCircularDiscountPromotionShopModel.ShopId;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionShopModel.UpdateBy) ? objCircularDiscountPromotionShopModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCircularDiscountPromotionShopModel.WareHouseId) ? objCircularDiscountPromotionShopModel.WareHouseId : null;

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

        public async Task<string> DeleteCircularDiscountPromotion(int discountCircularId, string wareHouseId, string shopId)
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_DIS_PROMOTION_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_circular_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountCircularId;

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

                    strMsg = objOracleCommand.Parameters["p_message"].Value.ToString();
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
            return strMsg;
        }


        public async Task<CircularDiscountPromotionMain> GetACircularDiscountPromotonMain(int discountCircularId, string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "DISCOUNT_CIRCULAR_ID," +
                      "DISCOUNT_CIRCULAR_NAME," +
                      "to_char(VALID_FROM,'dd/mm/yyyy') VALID_FROM ," +
                      "to_char(VALID_TO,'dd/mm/yyyy') VALID_TO ," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIR_DISCOUNT_PROMOTION where DISCOUNT_CIRCULAR_ID = :DISCOUNT_CIRCULAR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DISCOUNT_CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountCircularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CircularDiscountPromotionMain objCircularDiscountPromotionMainModels = new CircularDiscountPromotionMain();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCircularDiscountPromotionMainModels.DiscountCircularId =
                                    Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_ID"].ToString());
                                objCircularDiscountPromotionMainModels.DiscountCircularName = objDataReader["DISCOUNT_CIRCULAR_NAME"].ToString();
                                objCircularDiscountPromotionMainModels.ValidFrom = objDataReader["VALID_FROM"].ToString();
                                objCircularDiscountPromotionMainModels.ValidTo = objDataReader["VALID_TO"].ToString();
                                objCircularDiscountPromotionMainModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCircularDiscountPromotionMainModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCircularDiscountPromotionMainModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCircularDiscountPromotionMainModels;
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


        public async Task<List<CircularDiscountPromotionSub>> GetACircularDiscountPromotionSubList(int discountCircularId, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "DISCOUNT_CIRCULAR_ID," +
                      "DISCOUNT_CIRCULAR_SUB_ID," +
                      "CATEGORY_ID," +
                      "CATEGORY_NAME," +
                      "SUB_CATEGORY_ID," +
                      "SUB_CATEGORY_NAME," +
                      "STYLE_ID," +
                      "DISCOUNT," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIR_DIS_PROMOTION_SUB where DISCOUNT_CIRCULAR_ID =:DISCOUNT_CIRCULAR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                   // objCommand.Parameters.Add(":CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = categoryId;
                    objCommand.Parameters.Add(":DISCOUNT_CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountCircularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularDiscountPromotionSub> objCircularDiscountPromotionSubModels = new List<CircularDiscountPromotionSub>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularDiscountPromotionSub model = new CircularDiscountPromotionSub
                                {
                                    DiscountCircularId = Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_ID"].ToString()),
                                    DiscountCircularSubId = Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_SUB_ID"].ToString()),
                                    CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    SubCategoryId = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString()),
                                    StyleId = Convert.ToInt32(objDataReader["STYLE_ID"].ToString()),
                                    //CategoryId = objDataReader["CATEGORY_ID"].ToString(),
                                    //SubCategoryId =objDataReader["SUB_CATEGORY_ID"].ToString(),
                                    //StyleId = objDataReader["STYLE_ID"].ToString(),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString(),
                                    SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString(),
                                    Discount = objDataReader["DISCOUNT"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCircularDiscountPromotionSubModels.Add(model);
                            }
                            return objCircularDiscountPromotionSubModels;
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

        public async Task<List<int>> GetACircularDiscountPromotionShopList(int discountCircularId, string wareHouseId)
        {
            var sql = "SELECT " +
                      "DISCOUNT_CIRCULAR_ID," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CIR_DIS_PROMOTION_SHOP where DISCOUNT_CIRCULAR_ID = :DISCOUNT_CIRCULAR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DISCOUNT_CIRCULAR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountCircularId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<int> shopId = new List<int>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                int shop = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                shopId.Add(shop);
                            }
                            return shopId;
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

        public async Task<List<CircularDiscountPromotionModelApi>> GetAllPromotionProduct(int shopId)
        {
            var sql = "SELECT " +
                                "DISCOUNT_CIRCULAR_ID," +
                                "DISCOUNT_CIRCULAR_NAME," +
                                "VALID_FROM, " +
                                "VALID_TO, " +
                                "UPDATE_BY," +
                                "DISCOUNT_CIRCULAR_SUB_ID," +
                                "BARCODE," +
                                "PRUCHASE_PRICE," +
                                "SALE_PRICE," +
                                "DISCOUNT_PERCENT," +
                                "DISCOUNT_AMOUNT," +
                                "WARE_HOUSE_ID," +
                                "SHOP_ID," +
                                "SHOP_NAME, " +
                                "SHOP_SAVE_YN "+
                                "FROM VEW_DIS_PROMOTION_API WHERE SHOP_ID = :SHOP_ID  AND SHOP_SAVE_YN IS NULL ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CircularDiscountPromotionModelApi> objCircularDiscountPromotionModelApi = new List<CircularDiscountPromotionModelApi>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CircularDiscountPromotionModelApi model = new CircularDiscountPromotionModelApi();
                                model.DiscountCircularId = !string.IsNullOrWhiteSpace(objDataReader["DISCOUNT_CIRCULAR_ID"].ToString()) ? Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_ID"].ToString()) : 0;
                                model.DiscountCircularItemId = !string.IsNullOrWhiteSpace(objDataReader["DISCOUNT_CIRCULAR_SUB_ID"].ToString()) ? Convert.ToInt32(objDataReader["DISCOUNT_CIRCULAR_SUB_ID"].ToString()) : 0;
                                model.ShopId = !string.IsNullOrWhiteSpace(objDataReader["SHOP_ID"].ToString()) ? Convert.ToInt32(objDataReader["SHOP_ID"].ToString()) : 0;
                                model.DiscountCircularName = objDataReader["DISCOUNT_CIRCULAR_NAME"].ToString();
                                model.ValidFrom = !string.IsNullOrWhiteSpace(objDataReader["VALID_FROM"].ToString()) ? objDataReader["VALID_FROM"].ToString() : "";
                                model.ValidTo = !string.IsNullOrWhiteSpace(objDataReader["VALID_TO"].ToString()) ? objDataReader["VALID_TO"].ToString() : "";
                                model.Barcode = objDataReader["BARCODE"].ToString();
                                model.DiscountPercent = !string.IsNullOrWhiteSpace(objDataReader["DISCOUNT_PERCENT"].ToString()) ? Convert.ToDouble(objDataReader["DISCOUNT_PERCENT"].ToString()) : 0;
                                model.DiscountAmount = !string.IsNullOrWhiteSpace(objDataReader["DISCOUNT_AMOUNT"].ToString()) ? Convert.ToDouble(objDataReader["DISCOUNT_AMOUNT"].ToString()) : 0;
                                model.PurchasePrice = !string.IsNullOrWhiteSpace(objDataReader["PRUCHASE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["PRUCHASE_PRICE"].ToString()) : 0;
                                model.SalePrice = !string.IsNullOrWhiteSpace(objDataReader["SALE_PRICE"].ToString()) ? Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()) : 0;
                                model.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                model.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCircularDiscountPromotionModelApi.Add(model);
                            }
                            return objCircularDiscountPromotionModelApi;
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

        public async Task<string> UpdateWarehousePromotion(int circularId,int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CIR_DIS_PROMOTION_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_circular_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = circularId;
            objOracleCommand.Parameters.Add("p_shop_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
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