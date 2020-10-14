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
    public class SetupDAL
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



        #region "Categoty Section"

        public async Task<List<CategoryModel>> GetCategoryList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "CATEGORY_ID," +
                      "CATEGORY_NAME," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CATEGORY where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CategoryModel> objCategoryModels = new List<CategoryModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CategoryModel model = new CategoryModel
                                {
                                    CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCategoryModels.Add(model);
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

        public async Task<CategoryModel> GetACategory(int categoryId, string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "CATEGORY_ID," +
                      "CATEGORY_NAME," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_CATEGORY where CATEGORY_ID = :CATEGORY_ID  AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = categoryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CategoryModel objCategoryModels = new CategoryModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCategoryModels.CategoryId =
                                    Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString());
                                objCategoryModels.CategoryName = objDataReader["CATEGORY_NAME"].ToString();
                                objCategoryModels.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objCategoryModels.ActiveStatus = objCategoryModels.Active_YN == "Y" ? true : false;
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

        public async Task<string> SaveAndUpdateCategory(CategoryModel objCategoryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_category_save")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCategoryModel.CategoryId;
            objOracleCommand.Parameters.Add("p_category_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCategoryModel.CategoryName) ? objCategoryModel.CategoryName.Trim() : null;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCategoryModel.Active_YN) ? objCategoryModel.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCategoryModel.UpdateBy) ? objCategoryModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCategoryModel.WareHouseId) ? objCategoryModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCategoryModel.ShopId) ? objCategoryModel.ShopId : null;

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

        public async Task<string> DeleteCategory(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_category_delete")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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


        #region "Sub_Category"

        public async Task<List<SubCategoryModel>> GetSubCategoryList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                         "SUB_CATEGORY_ID," +
                         "SUB_CATEGORY_NAME," +
                         "CATEGORY_ID," +
                         "CATEGORY_NAME," +
                         "ACTIVE_YN," +
                         "UPDATE_BY," +
                         "WARE_HOUSE_ID," +
                         "SHOP_ID " +
                         "FROM VEW_SUB_CATEGORY where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID  ORDER BY SUB_CATEGORY_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SubCategoryModel> objSubCategory = new List<SubCategoryModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SubCategoryModel model = new SubCategoryModel
                                {
                                    CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    SubCategoryId = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString()),
                                    SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString(),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objSubCategory.Add(model);
                            }

                            return objSubCategory;
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

        public async Task<SubCategoryModel> GetASubCategory(int categoryId, int subCategoryId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "SUB_CATEGORY_ID," +
                      "SUB_CATEGORY_NAME," +
                      "CATEGORY_ID," +
                      "CATEGORY_NAME," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_SUB_CATEGORY where CATEGORY_ID = :CATEGORY_ID AND SUB_CATEGORY_ID = :SUB_CATEGORY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID  ORDER BY CATEGORY_NAME ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = categoryId;
                    objCommand.Parameters.Add(":SUB_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = subCategoryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        SubCategoryModel objSubCategoryModel = new SubCategoryModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objSubCategoryModel.CategoryId =
                                    Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString());
                                objSubCategoryModel.SubCategoryId =
                                    Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString());
                                objSubCategoryModel.SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString();
                                objSubCategoryModel.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objSubCategoryModel.ActiveStatus = objSubCategoryModel.Active_YN == "Y" ? true : false;
                                objSubCategoryModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objSubCategoryModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objSubCategoryModel.ShopId = objDataReader["SHOP_ID"].ToString();
                            }
                            return objSubCategoryModel;
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

        public async Task<string> SaveAndUpdateSubCategory(SubCategoryModel objSubCategoryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_sub_category_save")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSubCategoryModel.CategoryId;
            objOracleCommand.Parameters.Add("p_sub_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSubCategoryModel.SubCategoryId;
            objOracleCommand.Parameters.Add("p_sub_category_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubCategoryModel.SubCategoryName) ? objSubCategoryModel.SubCategoryName.Trim() : null;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubCategoryModel.Active_YN) ? objSubCategoryModel.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubCategoryModel.UpdateBy) ? objSubCategoryModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubCategoryModel.WareHouseId) ? objSubCategoryModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSubCategoryModel.ShopId) ? objSubCategoryModel.ShopId : null;

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

        public async Task<string> DeleteSubCategory(int catagoryId, int SubcatagoryId, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SUB_CATEGORY_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_sub_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = SubcatagoryId;
            objOracleCommand.Parameters.Add("p_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = catagoryId;

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


        #region "Brand Section"

        public async Task<List<BrandModel>> GetBrandList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "BRAND_ID," +
                      "BRAND_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_BRAND where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<BrandModel> objCategoryModels = new List<BrandModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                BrandModel model = new BrandModel
                                {
                                    BrandId = Convert.ToInt32(objDataReader["BRAND_ID"].ToString()),
                                    BrandName = objDataReader["BRAND_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCategoryModels.Add(model);
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

        public async Task<BrandModel> GetABrand(int brandId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "BRAND_ID," +
                      "BRAND_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_BRAND where BRAND_ID = :BRAND_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":BRAND_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = brandId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        BrandModel objBrandModels = new BrandModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objBrandModels.BrandId = Convert.ToInt32(objDataReader["BRAND_ID"].ToString());
                                objBrandModels.BrandName = objDataReader["BRAND_NAME"].ToString();
                                objBrandModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objBrandModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objBrandModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objBrandModels;
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

        public async Task<string> SaveAndUpdateBrand(BrandModel objBrandModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_BRAND_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_brand_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objBrandModels.BrandId;
            objOracleCommand.Parameters.Add("p_brand_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objBrandModels.BrandName) ? objBrandModels.BrandName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objBrandModels.UpdateBy) ? objBrandModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objBrandModels.WareHouseId) ? objBrandModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objBrandModels.ShopId) ? objBrandModels.ShopId : null;

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

        public async Task<string> DeleteBrand(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_BRAND_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_brand_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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

        #region "Attribute Section"

        public async Task<List<AttributeModel>> GetAttributeList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "ATTRIBUTE_ID," +
                      "ATTRIBUTE_NAME," +
                      "DISPLAY_ORDER," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_ATTRIBUTE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID  order by DISPLAY_ORDER ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<AttributeModel> objAttributeModels = new List<AttributeModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                AttributeModel model = new AttributeModel()
                                {
                                    AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString()),
                                 
                                    AttributeName = objDataReader["ATTRIBUTE_NAME"].ToString(),
                                    DisplayOrder = objDataReader["DISPLAY_ORDER"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objAttributeModels.Add(model);
                            }
                            return objAttributeModels;
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

        public async Task<AttributeModel> GetAAttribute(int attributeId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "ATTRIBUTE_ID," +
                      "ATTRIBUTE_NAME," +
                      "DISPLAY_ORDER," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_ATTRIBUTE where ATTRIBUTE_ID = :ATTRIBUTE_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        AttributeModel objAttributeModels = new AttributeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objAttributeModels.AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString());
                                objAttributeModels.AttributeName = objDataReader["ATTRIBUTE_NAME"].ToString();
                                objAttributeModels.DisplayOrder =objDataReader["DISPLAY_ORDER"].ToString();
                                objAttributeModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objAttributeModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objAttributeModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objAttributeModels;
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

        public async Task<string> SaveAndUpdateAttribute(AttributeModel objAttributeModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_ATTRIBUTE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_attribute_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objAttributeModels.AttributeId;
            objOracleCommand.Parameters.Add("p_attribute_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeModels.AttributeName) ? objAttributeModels.AttributeName.Trim() : null;
            objOracleCommand.Parameters.Add("p_display_order", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeModels.DisplayOrder) ? objAttributeModels.DisplayOrder : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeModels.UpdateBy) ? objAttributeModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeModels.WareHouseId) ? objAttributeModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeModels.ShopId) ? objAttributeModels.ShopId : null;

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

        public async Task<string> DeleteAttribute(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_ATTRIBUTE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_attribute_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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


        #region "Attribute_Value Section"

        public async Task<List<AttributeValue>> GetAttributeValueList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                         "ATTRIBUTE_VALUE_ID," +
                         "ATTRIBUTE_VALUE_NAME," +
                         "ATTRIBUTE_ID," +
                         "ATTRIBUTE_NAME," +
                         "UPDATE_BY," +
                         "WARE_HOUSE_ID," +
                         "SHOP_ID " +
                         "FROM VEW_ATTRIBUTE_VALUE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ORDER BY ATTRIBUTE_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<AttributeValue> objAttributeValue = new List<AttributeValue>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                AttributeValue model = new AttributeValue()
                                {
                                    AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString()),
                                    AttributeValueId = Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString()),
                                    AttributeValueName = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString(),
                                    AttributeName = objDataReader["ATTRIBUTE_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objAttributeValue.Add(model);
                            }

                            return objAttributeValue;
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

        public async Task<AttributeValue> GetAAttributeValue(int attributeId, int attributeValueId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "ATTRIBUTE_VALUE_ID," +
                      "ATTRIBUTE_VALUE_NAME," +
                      "ATTRIBUTE_ID," +
                      "ATTRIBUTE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_ATTRIBUTE_VALUE where ATTRIBUTE_ID =:ATTRIBUTE_ID AND ATTRIBUTE_VALUE_ID = :ATTRIBUTE_VALUE_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ORDER BY ATTRIBUTE_NAME ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeId;
                    objCommand.Parameters.Add(":ATTRIBUTE_VALUE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeValueId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        AttributeValue objAttributeValue = new AttributeValue();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objAttributeValue.AttributeId =
                                    Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString());
                                objAttributeValue.AttributeValueId =
                                    Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString());
                                objAttributeValue.AttributeValueName = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString();
                                objAttributeValue.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objAttributeValue.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objAttributeValue.ShopId = objDataReader["SHOP_ID"].ToString();
                            }
                            return objAttributeValue;
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

        public async Task<string> SaveAndUpdateAtributeValue(AttributeValue objAttributeValue)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_ATTRIBUTE_VALUE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_attribute_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objAttributeValue.AttributeId;
            objOracleCommand.Parameters.Add("p_attribute_value_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objAttributeValue.AttributeValueId;
            objOracleCommand.Parameters.Add("p_attribute_value_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeValue.AttributeValueName) ? objAttributeValue.AttributeValueName.Trim() : null;

            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeValue.UpdateBy) ? objAttributeValue.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeValue.WareHouseId) ? objAttributeValue.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objAttributeValue.ShopId) ? objAttributeValue.ShopId : null;

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


        public async Task<string> DeleteAttributeValue(int attributeId, int attributeValueId, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_ATTRIBUTE_VALUE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_attribute_value_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeValueId;
            objOracleCommand.Parameters.Add("p_attribute_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeId;

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

        #region "Vendor Section"

        public async Task<List<VendorModel>> GetVendorList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "VENDOR_ID," +
                      "VENDOR_NAME," +
                      "ADDRESS," +
                      "CONTACT_NO," +
                      "EMAIL," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_VENDOR where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<VendorModel> objVendorModels = new List<VendorModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                VendorModel model = new VendorModel
                                {
                                    VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString()),
                                    VendorName = objDataReader["VENDOR_NAME"].ToString(),
                                    Address = objDataReader["ADDRESS"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    Email = objDataReader["EMAIL"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objVendorModels.Add(model);
                            }
                            return objVendorModels;
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

        public async Task<VendorModel> GetAVendor(int vendorId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "VENDOR_ID," +
                      "VENDOR_NAME," +
                      "ADDRESS," +
                      "CONTACT_NO," +
                      "EMAIL," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_VENDOR where VENDOR_ID = :VENDOR_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":VENDOR_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = vendorId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        VendorModel objVendorModels = new VendorModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objVendorModels.VendorId = Convert.ToInt32(objDataReader["VENDOR_ID"].ToString());
                                objVendorModels.VendorName = objDataReader["VENDOR_NAME"].ToString();
                                objVendorModels.Address = objDataReader["ADDRESS"].ToString();
                                objVendorModels.ContactNo = objDataReader["CONTACT_NO"].ToString();
                                objVendorModels.Email = objDataReader["EMAIL"].ToString();
                                objVendorModels.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objVendorModels.ActiveStatus = objVendorModels.Active_YN == "Y" ? true : false;
                                objVendorModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objVendorModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objVendorModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objVendorModels;
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

        public async Task<string> SaveAndUpdateVendor(VendorModel objVendorModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_VENDOR_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_vendor_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objVendorModels.VendorId;
            objOracleCommand.Parameters.Add("p_vendor_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.VendorName) ? objVendorModels.VendorName : null;
            objOracleCommand.Parameters.Add("p_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.Address) ? objVendorModels.Address : null;
            objOracleCommand.Parameters.Add("p_conatct_NO", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.ContactNo) ? objVendorModels.ContactNo : null;
            objOracleCommand.Parameters.Add("p_email", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.Email) ? objVendorModels.Email : null;
            objOracleCommand.Parameters.Add("p_active_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.Active_YN) ? objVendorModels.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.UpdateBy) ? objVendorModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.WareHouseId) ? objVendorModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVendorModels.ShopId) ? objVendorModels.ShopId : null;

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

        public async Task<string> DeleteVendor(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_VENDOR_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_vendor_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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

        #region "Unit Section"

        public async Task<List<UnitModel>> GetUnitList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "UNIT_ID," +
                      "UNIT_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_UNIT where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<UnitModel> objUnitModels = new List<UnitModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                UnitModel model = new UnitModel()
                                {
                                    UnitId = Convert.ToInt32(objDataReader["UNIT_ID"].ToString()),
                                    UnitName = objDataReader["UNIT_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objUnitModels.Add(model);
                            }
                            return objUnitModels;
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

        public async Task<UnitModel> GetAUnit(int unitId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "UNIT_ID," +
                      "UNIT_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_UNIT where UNIT_ID = :UNIT_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":UNIT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = unitId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        UnitModel objUnitModels = new UnitModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objUnitModels.UnitId = Convert.ToInt32(objDataReader["UNIT_ID"].ToString());
                                objUnitModels.UnitName = objDataReader["UNIT_NAME"].ToString();
                                objUnitModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objUnitModels.ShopId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objUnitModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objUnitModels;
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

        public async Task<string> SaveAndUpdateUnit(UnitModel objUnitModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_UNIT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_unit_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objUnitModels.UnitId;
            objOracleCommand.Parameters.Add("p_unit_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objUnitModels.UnitName) ? objUnitModels.UnitName.Trim() : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objUnitModels.UpdateBy) ? objUnitModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objUnitModels.WareHouseId) ? objUnitModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objUnitModels.ShopId) ? objUnitModels.ShopId : null;

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

        public async Task<string> DeleteUnit(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_UNIT_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_unit_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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

        #region "Card Type Section"

        public async Task<List<CardTypeModel>> GetCardTypeList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "CARD_TYPE_ID," +
                      "CARD_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CARD_TYPE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CardTypeModel> objCardTypeModels = new List<CardTypeModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CardTypeModel model = new CardTypeModel()
                                {
                                    CardTypeId = Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString()),
                                    CardTypeName = objDataReader["CARD_TYPE_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCardTypeModels.Add(model);
                            }
                            return objCardTypeModels;
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

        public async Task<CardTypeModel> GetACardType(int cardTypeId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "CARD_TYPE_ID," +
                      "CARD_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_CARD_TYPE where CARD_TYPE_ID = :CARD_TYPE_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":CARD_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = cardTypeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CardTypeModel objCardTypeModels = new CardTypeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCardTypeModels.CardTypeId = Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString());
                                objCardTypeModels.CardTypeName = objDataReader["CARD_TYPE_NAME"].ToString();
                                objCardTypeModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCardTypeModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCardTypeModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCardTypeModels;
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

        public async Task<string> SaveAndUpdateCardType(CardTypeModel objCardTypeModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CARD_TYPE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_card_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCardTypeModels.CardTypeId;
            objOracleCommand.Parameters.Add("p_card_type_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCardTypeModels.CardTypeName) ? objCardTypeModels.CardTypeName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCardTypeModels.UpdateBy) ? objCardTypeModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCardTypeModels.WareHouseId) ? objCardTypeModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCardTypeModels.ShopId) ? objCardTypeModels.ShopId : null;

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

        public async Task<string> DeleteCardType(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CARD_TYPE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_card_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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

        #region "Customer Type Section"

        public async Task<List<CustomerTypeModel>> GetCustomerTypeList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CUSTOMER_TYPE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CustomerTypeModel> objCustomerTypeModels = new List<CustomerTypeModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CustomerTypeModel model = new CustomerTypeModel()
                                {
                                    CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString()),
                                    CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCustomerTypeModels.Add(model);
                            }
                            return objCustomerTypeModels;
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

        public async Task<CustomerTypeModel> GetACustomerType(int customerTypeId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_CUSTOMER_TYPE where CUSTOMER_TYPE_ID = :CUSTOMER_TYPE_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":CUSTOMER_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = customerTypeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CustomerTypeModel objCustomerTypeModels = new CustomerTypeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCustomerTypeModels.CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString());
                                objCustomerTypeModels.CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString();
                                objCustomerTypeModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCustomerTypeModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCustomerTypeModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCustomerTypeModels;
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

        public async Task<string> SaveAndUpdateCustomerType(CustomerTypeModel objCustomerTypeModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CUSTOMER_TYPE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_customer_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerTypeModels.CustomerTypeId;
            objOracleCommand.Parameters.Add("p_customer_type_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerTypeModels.CustomerTypeName) ? objCustomerTypeModels.CustomerTypeName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerTypeModels.UpdateBy) ? objCustomerTypeModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerTypeModels.WareHouseId) ? objCustomerTypeModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerTypeModels.ShopId) ? objCustomerTypeModels.ShopId : null;

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

        public async Task<string> DeleteCustomerType(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CUSTOMER_TYPE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_customer_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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


        #region "Payment Type Section"

        public async Task<List<PaymentTypeModel>> GetPaymentTypeList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "PAYMENT_TYPE_ID," +
                      "PAYMENT_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PAYMENT_TYPE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<PaymentTypeModel> objPaymentTypeModels = new List<PaymentTypeModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                PaymentTypeModel model = new PaymentTypeModel()
                                {
                                    PaymentTypeId = Convert.ToInt32(objDataReader["PAYMENT_TYPE_ID"].ToString()),
                                    PaymentTypeName = objDataReader["PAYMENT_TYPE_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objPaymentTypeModels.Add(model);
                            }
                            return objPaymentTypeModels;
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

        public async Task<PaymentTypeModel> GetAPaymentType(int paymentTypeId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "PAYMENT_TYPE_ID," +
                      "PAYMENT_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_PAYMENT_TYPE where PAYMENT_TYPE_ID = :PAYMENT_TYPE_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":PAYMENT_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = paymentTypeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        PaymentTypeModel objPaymentTypeModels = new PaymentTypeModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objPaymentTypeModels.PaymentTypeId = Convert.ToInt32(objDataReader["PAYMENT_TYPE_ID"].ToString());
                                objPaymentTypeModels.PaymentTypeName = objDataReader["PAYMENT_TYPE_NAME"].ToString();
                                objPaymentTypeModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objPaymentTypeModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objPaymentTypeModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objPaymentTypeModels;
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

        public async Task<string> SaveAndUpdatePaymentType(PaymentTypeModel objPaymentTypeModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PAYMENT_TYPE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_payment_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPaymentTypeModels.PaymentTypeId;
            objOracleCommand.Parameters.Add("p_payment_type_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPaymentTypeModels.PaymentTypeName) ? objPaymentTypeModels.PaymentTypeName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPaymentTypeModels.UpdateBy) ? objPaymentTypeModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPaymentTypeModels.WareHouseId) ? objPaymentTypeModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objPaymentTypeModels.ShopId) ? objPaymentTypeModels.ShopId : null;

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

        public async Task<string> DeletePaymentType(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PAYMENT_TYPE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_payment_Type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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


        #region "Country Section"

        public async Task<List<CountryModel>> GetCountryList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_COUNTRY where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CountryModel> objCountryModels = new List<CountryModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CountryModel model = new CountryModel()
                                {
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCountryModels.Add(model);
                            }
                            return objCountryModels;
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

        public async Task<CountryModel> GetACountry(int countryId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM L_COUNTRY where COUNTRY_ID = :COUNTRY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":COUNTRY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = countryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CountryModel objCountryModels = new CountryModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objCountryModels.CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString());
                                objCountryModels.CountryName = objDataReader["COUNTRY_NAME"].ToString();
                                objCountryModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCountryModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCountryModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objCountryModels;
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

        public async Task<string> SaveAndUpdateCountry(CountryModel objCountryModels)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_COUNTRY_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCountryModels.CountryId;
            objOracleCommand.Parameters.Add("p_country_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCountryModels.CountryName) ? objCountryModels.CountryName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCountryModels.UpdateBy) ? objCountryModels.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCountryModels.WareHouseId) ? objCountryModels.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCountryModels.ShopId) ? objCountryModels.ShopId : null;

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

        public async Task<string> DeleteCountry(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_COUNTRY_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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


        #region "WareHouse Section"

        public async Task<List<WareHouseModel>> GetWareHouseList()
        {


            var sql = "SELECT " +
                         "WARE_HOUSE_ID," +
                         "WARE_HOUSE_NAME," +
                         "COUNTRY_ID," +
                         "COUNTRY_NAME," +
                         "CONTACT_NO," +
                         "POSTAL_CODE," +
                         "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                         "VAT_NO," +
                         "TIN_NO," +
                         "BIN_NO," +
                         "FAX_NO," +
                         "EMAIL_ADDRESS," +
                         "WARE_HOUSE_ADDRESS," +
                         "UPDATE_BY " +
                         "FROM VEW_WARE_HOUSE ORDER BY COUNTRY_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<WareHouseModel> objWareHouseModels = new List<WareHouseModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                WareHouseModel model = new WareHouseModel
                                {
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    WareHouseId = Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString()),
                                    WareHouseName = objDataReader["WARE_HOUSE_NAME"].ToString(),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),

                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    PostalCode = objDataReader["POSTAL_CODE"].ToString(),
                                    DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString(),
                                    VatNo = objDataReader["VAT_NO"].ToString(),
                                    TINNo = objDataReader["TIN_NO"].ToString(),
                                    BINNo = objDataReader["BIN_NO"].ToString(),
                                    FAXNo = objDataReader["FAX_NO"].ToString(),
                                    EmailAddress = objDataReader["EMAIL_ADDRESS"].ToString(),
                                    WareHouseAddress = objDataReader["WARE_HOUSE_ADDRESS"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString()

                                };
                                objWareHouseModels.Add(model);
                            }

                            return objWareHouseModels;
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

        public async Task<WareHouseModel> GetAWareHouse(int countryId, int wareHouseId)
        {


            var sql = "SELECT " +
                      "WARE_HOUSE_ID," +
                      "WARE_HOUSE_NAME," +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "CONTACT_NO," +
                      "POSTAL_CODE," +
                      "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                      "VAT_NO," +
                      "TIN_NO," +
                      "BIN_NO," +
                      "FAX_NO," +
                      "EMAIL_ADDRESS," +
                      "WARE_HOUSE_ADDRESS," +
                      "UPDATE_BY " +
                      "FROM VEW_WARE_HOUSE where COUNTRY_ID = :COUNTRY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":COUNTRY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = countryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        WareHouseModel objWareHouseModel = new WareHouseModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objWareHouseModel.CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString());
                                objWareHouseModel.WareHouseId = Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString());
                                objWareHouseModel.WareHouseName = objDataReader["WARE_HOUSE_NAME"].ToString();

                                objWareHouseModel.ContactNo = objDataReader["CONTACT_NO"].ToString();
                                objWareHouseModel.PostalCode = objDataReader["POSTAL_CODE"].ToString();
                                objWareHouseModel.DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString();
                                objWareHouseModel.VatNo = objDataReader["VAT_NO"].ToString();
                                objWareHouseModel.TINNo = objDataReader["TIN_NO"].ToString();
                                objWareHouseModel.BINNo = objDataReader["BIN_NO"].ToString();
                                objWareHouseModel.FAXNo = objDataReader["FAX_NO"].ToString();
                                objWareHouseModel.EmailAddress = objDataReader["EMAIL_ADDRESS"].ToString();
                                objWareHouseModel.WareHouseAddress = objDataReader["WARE_HOUSE_ADDRESS"].ToString();
                                objWareHouseModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();

                            }
                            return objWareHouseModel;
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

        public async Task<string> SaveAndUpdateWareHouse(WareHouseModel objWareHouseModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_WARE_HOUSE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objWareHouseModel.CountryId;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objWareHouseModel.WareHouseId;
            objOracleCommand.Parameters.Add("p_ware_house_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.WareHouseName) ? objWareHouseModel.WareHouseName : null;

            objOracleCommand.Parameters.Add("p_contact_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.ContactNo) ? objWareHouseModel.ContactNo : null;
            objOracleCommand.Parameters.Add("p_postal_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.PostalCode) ? objWareHouseModel.PostalCode : null;
            objOracleCommand.Parameters.Add("p_date_of_enrollment", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.DateOfEnrollment) ? objWareHouseModel.DateOfEnrollment : null;
            objOracleCommand.Parameters.Add("p_vat_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.VatNo) ? objWareHouseModel.VatNo : null;
            objOracleCommand.Parameters.Add("p_tin_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.TINNo) ? objWareHouseModel.TINNo : null;
            objOracleCommand.Parameters.Add("p_bin_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.BINNo) ? objWareHouseModel.BINNo : null;
            objOracleCommand.Parameters.Add("p_fax_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.FAXNo) ? objWareHouseModel.FAXNo : null;
            objOracleCommand.Parameters.Add("p_email_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.EmailAddress) ? objWareHouseModel.EmailAddress : null;
            objOracleCommand.Parameters.Add("p_ware_house_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.WareHouseAddress) ? objWareHouseModel.WareHouseAddress : null;

            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objWareHouseModel.UpdateBy) ? objWareHouseModel.UpdateBy : null;
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


        public async Task<string> DeleteWareHouse(int countryId, int wareHouseId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_WARE_HOUSE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = countryId;
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

        #region "Shop Section"

        public async Task<List<ShopModel>> GetShopList(int? shopId)
        {
            var sql = "SELECT " +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                         "WARE_HOUSE_ID," +
                         "WARE_HOUSE_NAME," +
                         "COUNTRY_ID," +
                         "COUNTRY_NAME," +
                         "CONTACT_NO," +
                         "POSTAL_CODE," +
                         "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                         "VAT_NO," +
                         "TIN_NO," +
                         "BIN_NO," +
                         "FAX_NO," +
                         "EMAIL_ADDRESS," +
                         "SHOP_ADDRESS," +
                         "ACTIVE_YN," +
                         "UPDATE_BY " +
                         "FROM VEW_SHOP WHERE 1=1 ";

            if (shopId != null && shopId > 0)
            {
                sql += "AND SHOP_ID = :SHOP_ID ORDER BY COUNTRY_ID ";
            }
            else
            {
                sql += "ORDER BY COUNTRY_ID ";
            }


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection){CommandType = CommandType.Text})
                {
                    if (shopId != null && shopId > 0)
                    {
                        objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    }

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ShopModel> objShopModels = new List<ShopModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ShopModel model = new ShopModel()
                                {
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    WareHouseId = Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString()),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    ShopName = objDataReader["SHOP_NAME"].ToString(),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),
                                    WareHouseName = objDataReader["WARE_HOUSE_NAME"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    PostalCode = objDataReader["POSTAL_CODE"].ToString(),
                                    DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString(),
                                    VatNo = objDataReader["VAT_NO"].ToString(),
                                    TINNo = objDataReader["TIN_NO"].ToString(),
                                    BINNo = objDataReader["BIN_NO"].ToString(),
                                    FAXNo = objDataReader["FAX_NO"].ToString(),
                                    EmailAddress = objDataReader["EMAIL_ADDRESS"].ToString(),
                                    ShopAddress = objDataReader["SHOP_ADDRESS"].ToString(),
                                    IsActive = objDataReader["ACTIVE_YN"].ToString() == "Y",
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString()

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

        public async Task<ShopModel> GetAShop(int countryId, int warehoueId, int shopId)
        {


            var sql = "SELECT " +
                      "WARE_HOUSE_ID," +
                      "WARE_HOUSE_NAME," +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                      "CONTACT_NO," +
                      "POSTAL_CODE," +
                      "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                      "VAT_NO," +
                      "TIN_NO," +
                      "BIN_NO," +
                      "FAX_NO," +
                      "EMAIL_ADDRESS," +
                      "SHOP_ADDRESS," +
                      "ACTIVE_YN," +
                      "UPDATE_BY " +
                      "FROM VEW_SHOP where COUNTRY_ID = :COUNTRY_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":COUNTRY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = countryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = warehoueId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ShopModel objShopModel = new ShopModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objShopModel.CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString());
                                objShopModel.WareHouseId = Convert.ToInt32(objDataReader["WARE_HOUSE_ID"].ToString());
                                objShopModel.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                objShopModel.ShopName = objDataReader["SHOP_NAME"].ToString();

                                objShopModel.ContactNo = objDataReader["CONTACT_NO"].ToString();
                                objShopModel.PostalCode = objDataReader["POSTAL_CODE"].ToString();
                                objShopModel.DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString();
                                objShopModel.VatNo = objDataReader["VAT_NO"].ToString();
                                objShopModel.TINNo = objDataReader["TIN_NO"].ToString();
                                objShopModel.BINNo = objDataReader["BIN_NO"].ToString();
                                objShopModel.FAXNo = objDataReader["FAX_NO"].ToString();
                                objShopModel.EmailAddress = objDataReader["EMAIL_ADDRESS"].ToString();
                                objShopModel.ShopAddress = objDataReader["SHOP_ADDRESS"].ToString();
                                objShopModel.IsActive = objDataReader["ACTIVE_YN"].ToString() == "Y";
                                objShopModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();

                            }
                            return objShopModel;
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

        public async Task<string> SaveAndUpdateShop(ShopModel objShopModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopModel.CountryId;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopModel.WareHouseId;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopModel.ShopId;
            objOracleCommand.Parameters.Add("p_shop_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.ShopName) ? objShopModel.ShopName.Trim() : null;

            objOracleCommand.Parameters.Add("p_contact_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.ContactNo) ? objShopModel.ContactNo : null;
            objOracleCommand.Parameters.Add("p_postal_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.PostalCode) ? objShopModel.PostalCode : null;
            objOracleCommand.Parameters.Add("p_date_of_enrollment", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.DateOfEnrollment) ? objShopModel.DateOfEnrollment : null;
            objOracleCommand.Parameters.Add("p_vat_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.VatNo) ? objShopModel.VatNo : null;
            objOracleCommand.Parameters.Add("p_tin_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.TINNo) ? objShopModel.TINNo : null;
            objOracleCommand.Parameters.Add("p_bin_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.BINNo) ? objShopModel.BINNo : null;
            objOracleCommand.Parameters.Add("p_fax_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.FAXNo) ? objShopModel.FAXNo : null;
            objOracleCommand.Parameters.Add("p_email_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.EmailAddress) ? objShopModel.EmailAddress : null;
            objOracleCommand.Parameters.Add("p_shop_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.ShopAddress) ? objShopModel.ShopAddress : null;

            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.UpdateBy) ? objShopModel.UpdateBy : null;
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

        public async Task<string> DeleteShop(int countryId, int wareHouseId, int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = countryId;
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

        public async Task<string> UpdateShop(ShopModel objShopModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SHOP_UPDATE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objShopModel.ShopId;
            objOracleCommand.Parameters.Add("p_shop_url", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.ShopUrl) ? objShopModel.ShopUrl : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objShopModel.UpdateBy) ? objShopModel.UpdateBy : null;
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

        #region "Voucher Generate Section "

        public async Task<List<GiftVoucherGenerateModel>> GetGiftVoucherGenerateList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                         "GIFT_VOUCHER_ID," +
                         "NO_OF_CARD," +
                         "CARD_TYPE_ID," +
                         "START_FROM," +
                         "END_FROM," +
                         "GIFT_VOUCHER_VALUE, "+
                         "UPDATE_BY," +
                         "WARE_HOUSE_ID," +
                         "SHOP_ID " +
                         "FROM GIFT_VOUCHER_GENERATE where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ORDER BY CARD_TYPE_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<GiftVoucherGenerateModel> objGiftVoucherGenerateModel = new List<GiftVoucherGenerateModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                GiftVoucherGenerateModel model = new GiftVoucherGenerateModel
                                {
                                    CardTypeId = Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString()),
                                    GiftVoucherId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString()),
                                    NoOfCard = objDataReader["NO_OF_CARD"].ToString(),
                                    StartFrom = objDataReader["START_FROM"].ToString(),
                                    EndFrom = objDataReader["END_FROM"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    GiftVoucherValue = Convert.ToInt32(objDataReader["GIFT_VOUCHER_VALUE"].ToString()),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objGiftVoucherGenerateModel.Add(model);
                            }

                            return objGiftVoucherGenerateModel;
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

        public async Task<GiftVoucherGenerateModel> GetAGiftVoucherGenerate(int cardTypeId, int giftVoucherId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "GIFT_VOUCHER_ID," +
                      "NO_OF_CARD," +
                      "CARD_TYPE_ID," +
                      "CARD_TYPE_NAME," +
                      "START_FROM," +
                      "END_FROM," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_GIFT_VOUCHER_GENERATE where CARD_TYPE_ID = :CARD_TYPE_ID AND GIFT_VOUCHER_ID =:GIFT_VOUCHER_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ORDER BY CARD_TYPE_NAME ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":CARD_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = cardTypeId;
                    objCommand.Parameters.Add(":GIFT_VOUCHER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        GiftVoucherGenerateModel objgiftVoucherGenerateModel = new GiftVoucherGenerateModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objgiftVoucherGenerateModel.CardTypeId =
                                    Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString());
                                objgiftVoucherGenerateModel.GiftVoucherId =
                                    Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString());
                                objgiftVoucherGenerateModel.NoOfCard = objDataReader["NO_OF_CARD"].ToString();
                                objgiftVoucherGenerateModel.StartFrom = objDataReader["START_FROM"].ToString();
                                objgiftVoucherGenerateModel.EndFrom = objDataReader["END_FROM"].ToString();
                                objgiftVoucherGenerateModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objgiftVoucherGenerateModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objgiftVoucherGenerateModel.ShopId = objDataReader["SHOP_ID"].ToString();
                            }
                            return objgiftVoucherGenerateModel;
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

        public async Task<string> SaveAndUpdateGiftVoucherGenerate(GiftVoucherGenerateModel objGiftVoucherGenerateModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFT_VOUCHER_GENERATE_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_card_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherGenerateModel.CardTypeId;
            objOracleCommand.Parameters.Add("p_gift_voucher_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherGenerateModel.GiftVoucherId;
            objOracleCommand.Parameters.Add("p_no_of_card", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.NoOfCard) ? objGiftVoucherGenerateModel.NoOfCard : null;
            objOracleCommand.Parameters.Add("p_start_from", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.StartFrom) ? objGiftVoucherGenerateModel.StartFrom : null;
            objOracleCommand.Parameters.Add("p_end_from", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.EndFrom) ? objGiftVoucherGenerateModel.EndFrom : null;
            objOracleCommand.Parameters.Add("p_gift_voucher_value", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherGenerateModel.GiftVoucherValue;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.UpdateBy) ? objGiftVoucherGenerateModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.WareHouseId) ? objGiftVoucherGenerateModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.ShopId) ? objGiftVoucherGenerateModel.ShopId : null;

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


        public async Task<string> SaveGiftVoucherItem(GiftVoucherGenerateModel objGiftVoucherGenerateModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFT_VOUCHER_Item_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

           
            objOracleCommand.Parameters.Add("p_gift_voucher_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherGenerateModel.GiftVoucherId;
            objOracleCommand.Parameters.Add("p_start_from", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.StartFrom) ? objGiftVoucherGenerateModel.StartFrom : null;
            objOracleCommand.Parameters.Add("p_end_from", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.EndFrom) ? objGiftVoucherGenerateModel.EndFrom : null;
            objOracleCommand.Parameters.Add("p_gift_voucher_value", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherGenerateModel.GiftVoucherValue;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.UpdateBy) ? objGiftVoucherGenerateModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherGenerateModel.WareHouseId) ? objGiftVoucherGenerateModel.WareHouseId : null;
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
        public async Task<string> DeleteGiftVoucherGenerate(int cardTypeId, int giftVoucherId, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFT_VOUCHER_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_gift_voucher_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherId;
            objOracleCommand.Parameters.Add("p_card_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = cardTypeId;

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

        public async Task<List<GiftVoucherGenerateItemModel>> GetGiftVoucherGenerateItemList(string giftVoucherId)
        {
            //var sql = "SELECT * FROM GIFT_VOUCHER_ITEM where GIFT_VOUCHER_ID = :GIFT_VOUCHER_ID ";

            var sql = "SELECT * FROM GIFT_VOUCHER_ITEM" +
                " WHERE GIFT_VOUCHER_ID = :GIFT_VOUCHER_ID" +
                " AND GIFT_VOUCHER_CODE NOT IN" +
                " (SELECT GIFT_VOUCHER_CODE FROM GIFT_VOUCHER_DELIVERY" +
                " WHERE GIFT_VOUCHER_ID = :GIFT_VOUCHER_ID)";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":GIFT_VOUCHER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherId;
                 
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<GiftVoucherGenerateItemModel> objGiftVoucherGenerateItemModel = new List<GiftVoucherGenerateItemModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                GiftVoucherGenerateItemModel model = new GiftVoucherGenerateItemModel
                                {
                                    GiftVoucherId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString()),
                                    GiftVoucherCode = objDataReader["GIFT_VOUCHER_CODE"].ToString(),
                                    GiftVoucherValue = Convert.ToInt32(objDataReader["GIFT_VOUCHER_VALUE"].ToString())
                                };
                                objGiftVoucherGenerateItemModel.Add(model);
                            }

                            return objGiftVoucherGenerateItemModel;
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

        public async Task<GiftVoucherGenerateModel> GiftVoucherLastStartFrom()
        {
            var sql = "SELECT END_FROM FROM  VEW_LAST_GIFT_END  WHERE ROWNUM = 1  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        GiftVoucherGenerateModel objgiftVoucherGenerateModel = new GiftVoucherGenerateModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objgiftVoucherGenerateModel.EndFrom = objDataReader["END_FROM"].ToString();
                            }
                            return objgiftVoucherGenerateModel;
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

        public async Task<string> SavePrintedGiftVoucherCode(List<GiftVoucherGeneratePrintItemModel> objGiftVoucherGeneratePrintItemModel, string updateBy)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PRINT_GIFTVOU_CODE_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objGiftVoucherGeneratePrintItemModel)
                        {
                            for (int i = 0; i < Convert.ToInt32(data.Quantity); i++)
                            {
                                objCommand.Parameters.Clear();

                                objCommand.Parameters.Add("P_GIFT_VOUCHER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.GiftVoucherId != 0 ? data.GiftVoucherId.ToString() : null;
                                objCommand.Parameters.Add("P_GIFT_VOUCHER_CODE", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(data.GiftVoucherCode) ? data.GiftVoucherCode : null;
                                objCommand.Parameters.Add("P_GIFT_VOUCHER_VALUE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.GiftVoucherValue) ? data.GiftVoucherValue : null;
                                objCommand.Parameters.Add("P_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.Quantity != 0 ? data.Quantity.ToString() : null;
                                objCommand.Parameters.Add("P_BARCODE_IMAGE", OracleDbType.Blob, ParameterDirection.Input).Value = data.GiftVoucherCodeImageArray;
                                objCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(updateBy) ? updateBy : null;
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
        #endregion

        #region "Gift Voucher Delivery Section"

        public async Task<List<GiftVoucherDeliveryModel>> GetGiftVoucherDeliveryList(string wareHouseId)
        {


            var sql = "SELECT " +
                      "GIFT_VOUCHER_DELIVERY_ID,GIFT_VOUCHER_ID,GIFT_VOUCHER_ITEM_NUM,DELIVERY_DATE, " +
                    "GIFT_VOUCHER_CODE,GIFT_VOUCHER_VALUE,GIFT_VOUCHER_REMAINING_VALUE,CREATE_BY,CREATE_DATE, " +
                    "UPDATE_BY,UPDATE_DATE, DELIVERY_SHOP_NAME,WARE_HOUSE_ID  " +
                    "FROM VEW_GIFT_VOUCHER_DELIVERY where WARE_HOUSE_ID = :WARE_HOUSE_ID ORDER BY GIFT_VOUCHER_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;


                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<GiftVoucherDeliveryModel> objGiftVoucherDelivery = new List<GiftVoucherDeliveryModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                GiftVoucherDeliveryModel model = new GiftVoucherDeliveryModel
                                {
                                    //CardTYpeId = Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString()),
                                    //ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    GiftVoucherDeliveryId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_DELIVERY_ID"].ToString()),
                                    GiftVoucherCode = objDataReader["GIFT_VOUCHER_CODE"].ToString(),
                                    GiftVoucherValue = objDataReader["GIFT_VOUCHER_VALUE"].ToString(),
                                    GiftVoucherRemainingValue = objDataReader["GIFT_VOUCHER_REMAINING_VALUE"].ToString(),
                                    DeliveryDate = objDataReader["DELIVERY_DATE"].ToString(),
                                    CreatedBy = objDataReader["CREATE_BY"].ToString(),
                                    ShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),

                                };
                                objGiftVoucherDelivery.Add(model);
                            }

                            return objGiftVoucherDelivery;
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

        public async Task<GiftVoucherDeliveryModel> GetAGiftVoucherDelivery(int cardTypeId, int shopId, int giftVoucherDeliveryId, string wareHouseId)
        {


            var sql = "SELECT " +
                      "CARD_TYPE_ID," +
                      "CARD_TYPE_NAME," +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                      "GIFT_VOUCHER_DELIVERY_ID," +
                      "NO_OF_CARD," +
                      "to_char(DELIVERY_DATE,'dd/mm/yyyy') DELIVERY_DATE ," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID " +
                      "FROM VEW_GIFT_VOUCHER_DELIVERY where CARD_TYPE_ID = :CARD_TYPE_ID AND GIFT_VOUCHER_DELIVERY_ID = :GIFT_VOUCHER_DELIVERY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ORDER BY CARD_TYPE_NAME ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {
                    objCommand.Parameters.Add(":CARD_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = cardTypeId;
                    objCommand.Parameters.Add(":GIFT_VOUCHER_DELIVERY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherDeliveryId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        GiftVoucherDeliveryModel objGiftVoucherDeliveryModel = new GiftVoucherDeliveryModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                //objGiftVoucherDeliveryModel.CardTYpeId = Convert.ToInt32(objDataReader["CARD_TYPE_ID"].ToString());
                                objGiftVoucherDeliveryModel.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                objGiftVoucherDeliveryModel.GiftVoucherDeliveryId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_DELIVERY_ID"].ToString());
                                //objGiftVoucherDeliveryModel.NoOfCard = objDataReader["NO_OF_CARD"].ToString();
                                //objGiftVoucherDeliveryModel.DeliveryDate = objDataReader["DELIVERY_DATE"].ToString();
                                objGiftVoucherDeliveryModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objGiftVoucherDeliveryModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();


                            }
                            return objGiftVoucherDeliveryModel;
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

        public async Task<string> SaveAndUpdateGiftVoucherDelivery(GiftVoucherDeliveryModel objGiftVoucherDeliveryModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFT_VOUCHER_DELIVERY_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_gift_voucher_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherDeliveryModel.GiftVoucherId;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherDeliveryModel.ShopId;
            objOracleCommand.Parameters.Add("p_gift_voucher_delivery_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objGiftVoucherDeliveryModel.GiftVoucherDeliveryId;
            objOracleCommand.Parameters.Add("p_gift_voucher_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.GiftVoucherCode) ? objGiftVoucherDeliveryModel.GiftVoucherCode : null;
            objOracleCommand.Parameters.Add("p_gift_voucher_value", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.GiftVoucherValue) ? objGiftVoucherDeliveryModel.GiftVoucherValue : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.WareHouseId) ? objGiftVoucherDeliveryModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDeliveryModel.UpdateBy) ? objGiftVoucherDeliveryModel.UpdateBy : null;

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



        public async Task<string> DeleteGiftVoucherDelivery(int cardTypeId, int shopId, int giftvoucherdeliveryId, string wareHouseId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_VOUCHER_DELIVERY_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_gift_voucher_delivery_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftvoucherdeliveryId;
            //objOracleCommand.Parameters.Add("p_card_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = cardTypeId;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;

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

        #region "Discount Policy Section"
        public async Task<List<DiscountPolicyModel>> GetDiscountPolicyList(string wareHouseId)
        {
            var sql = "SELECT " +
                      "DISCOUNT_POLICY_ID," +
                      "DISCOUNT_PERCENT," +
                      "to_char (START_DATE, 'dd/mm/yyyy') START_DATE," +
                      "to_char(END_DATE, 'dd/mm/yyyy') END_DATE," +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID " +
                      "FROM VEW_DISCOUNT_POLICY where WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DiscountPolicyModel> objDiscountPolicyModel = new List<DiscountPolicyModel>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DiscountPolicyModel model = new DiscountPolicyModel
                                {
                                    DiscountPolicyId = Convert.ToInt32(objDataReader["DISCOUNT_POLICY_ID"].ToString()),
                                    ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString()),
                                    CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString()),
                                    DiscountPercent = objDataReader["DISCOUNT_PERCENT"].ToString(),
                                    StartDate = objDataReader["START_DATE"].ToString(),
                                    EndDate = objDataReader["END_DATE"].ToString(),
                                    ShopName = objDataReader["SHOP_NAME"].ToString(),
                                    CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString()

                                };
                                objDiscountPolicyModel.Add(model);
                            }
                            return objDiscountPolicyModel;
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

        public async Task<DiscountPolicyModel> GetADiscountPolicy(int discountPolicyId, string wareHouseId)
        {

            var sql = "SELECT " +
                      "DISCOUNT_POLICY_ID," +
                      "DISCOUNT_PERCENT," +
                      "to_char (START_DATE, 'dd/mm/yyyy') START_DATE," +
                      "to_char(END_DATE, 'dd/mm/yyyy') END_DATE," +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID " +
                      "FROM VEW_DISCOUNT_POLICY where DISCOUNT_POLICY_ID = :DISCOUNT_POLICY_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID ";          

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {

                    objCommand.Parameters.Add(":DISCOUNT_POLICY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountPolicyId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        DiscountPolicyModel objDiscountPolicyModel = new DiscountPolicyModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objDiscountPolicyModel.DiscountPolicyId = Convert.ToInt32(objDataReader["DISCOUNT_POLICY_ID"].ToString());
                                objDiscountPolicyModel.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                objDiscountPolicyModel.CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString());
                                objDiscountPolicyModel.DiscountPercent = objDataReader["DISCOUNT_PERCENT"].ToString();
                                objDiscountPolicyModel.StartDate = objDataReader["START_DATE"].ToString();
                                objDiscountPolicyModel.EndDate = objDataReader["END_DATE"].ToString();
                                objDiscountPolicyModel.ShopName = objDataReader["SHOP_NAME"].ToString();
                                objDiscountPolicyModel.CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString();
                                objDiscountPolicyModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objDiscountPolicyModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();                        
                            }

                            return objDiscountPolicyModel;
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

        public async Task<List<int>> GetDiscountPolicyCategoryList(int discountPolicyId, string wareHouseId)
        {
            var sql = "SELECT " +
                      "DISCOUNT_POLICY_CATEGORY_ID," +
                      "DISCOUNT_POLICY_ID," +
                      "CATEGORY_ID," +
                      "WARE_HOUSE_ID " +
                      "FROM VEW_DISCOUNT_POLICY_CATEGORY where DISCOUNT_POLICY_ID = :DISCOUNT_POLICY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DISCOUNT_POLICY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountPolicyId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<int> categoryId = new List<int>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                int category = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString());
                                categoryId.Add(category);
                            }
                            return categoryId;
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

        public async Task<List<int>> GetDiscountPolicySubCategoryList(int discountPolicyId, string wareHouseId)
        {
            var sql = "SELECT " +
                      "DIS_POLICY_SUB_CATEGORY_ID," +
                      "DISCOUNT_POLICY_ID," +
                      "SUB_CATEGORY_ID," +
                      "WARE_HOUSE_ID " +
                      "FROM VEW_DIS_POLICY_SUB_CATEGORY where DISCOUNT_POLICY_ID = :DISCOUNT_POLICY_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":DISCOUNT_POLICY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountPolicyId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<int> subCategoryId = new List<int>();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                int subCategory = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString());
                                subCategoryId.Add(subCategory);
                            }
                            return subCategoryId;
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
        public async Task<string> SaveAndUpdateDiscountPolicy(DiscountPolicyModel objDiscountPolicyModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DISCOUNT_POLICY_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_policy_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyModel.DiscountPolicyId;
            objOracleCommand.Parameters.Add("p_discount_percent", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyModel.DiscountPercent) ? objDiscountPolicyModel.DiscountPercent : null;
            objOracleCommand.Parameters.Add("p_start_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyModel.StartDate) ? objDiscountPolicyModel.StartDate : null;
            objOracleCommand.Parameters.Add("p_end_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyModel.EndDate) ? objDiscountPolicyModel.EndDate : null;
            objOracleCommand.Parameters.Add("p_shop_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyModel.ShopId;
            objOracleCommand.Parameters.Add("p_customer_type_id ", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyModel.CustomerTypeId;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyModel.UpdateBy) ? objDiscountPolicyModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyModel.WareHouseId) ? objDiscountPolicyModel.WareHouseId : null;
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
        public async Task<string> SaveAndUpdateSubDiscountPolicyCategory(DiscountPolicyCategory objDiscountPolicyCategory)
        {

            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_discount_policy_category")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_policy_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyCategory.DiscountPolicyId;
            objOracleCommand.Parameters.Add("p_discount_policy_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyCategory.DiscountPolicyCategoryId;
            objOracleCommand.Parameters.Add("p_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicyCategory.CategoryId;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyCategory.UpdateBy) ? objDiscountPolicyCategory.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicyCategory.WareHouseId) ? objDiscountPolicyCategory.WareHouseId : null;

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
        public async Task<string> SaveAndUpdateSubDiscountPolicySubCategory(DiscountPolicySubCategory objDiscountPolicySubCategory)
        {

            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_discount_poli_sub_category")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_policy_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicySubCategory.DiscountPolicyId;
            objOracleCommand.Parameters.Add("p_dis_poli_sub_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicySubCategory.DiscountPolicySubCategoryId;
            objOracleCommand.Parameters.Add("p_sub_category_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objDiscountPolicySubCategory.SubCategoryId;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicySubCategory.UpdateBy) ? objDiscountPolicySubCategory.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDiscountPolicySubCategory.WareHouseId) ? objDiscountPolicySubCategory.WareHouseId : null;

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

        public async Task<string> DeleteDiscountPolicy(int discountPolicyId, string wareHouseId)
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DISCOUNT_POLICY_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_discount_policy_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = discountPolicyId;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
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

        public async Task<List<CategoryViewModel>> GetselectedCategoryList()
        {


            var sql = "SELECT " +
                      "CATEGORY_ID," +
                      "CATEGORY_NAME " +
                      "FROM VEW_CATEGORY";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader) await objCommand.ExecuteReaderAsync())
                    {
                        List<CategoryViewModel> objCategoryViewModel = new List<CategoryViewModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CategoryViewModel model = new CategoryViewModel()
                                {

                                    CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString(),

                                };
                                objCategoryViewModel.Add(model);
                            }

                            return objCategoryViewModel;
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

        public async Task<List<SubCategoryViewModel>> GetselectedSubCategoryList()
        {


            var sql = "SELECT " +
                      "SUB_CATEGORY_ID," +
                      "SUB_CATEGORY_NAME " +
                      "FROM VEW_SUB_CATEGORY";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SubCategoryViewModel> objSubCategoryViewModel = new List<SubCategoryViewModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SubCategoryViewModel model = new SubCategoryViewModel()
                                {

                                    SubCategoryId = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString()),
                                    SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString(),

                                };
                                objSubCategoryViewModel.Add(model);
                            }

                            return objSubCategoryViewModel;
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

        #region VatSection

        public async Task<List<VatModel>> GetVatList(string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "VAT_ID," +
                      "VAT_PERCENT," +
                      "INCLUDE_VAT," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_VAT where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<VatModel> objVatModel = new List<VatModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                VatModel model = new VatModel()
                                {
                                    VatId = Convert.ToInt32(objDataReader["VAT_ID"].ToString()),
                                    VatPercent = objDataReader["VAT_PERCENT"].ToString(),
                                    IncludeVat = objDataReader["INCLUDE_VAT"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objVatModel.Add(model);
                            }
                            return objVatModel;
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

        public async Task<VatModel> GetAVat(int vatId, string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "VAT_ID," +
                      "VAT_PERCENT," +
                      "INCLUDE_VAT," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_VAT where VAT_ID = :VAT_ID  AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":VAT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = vatId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        VatModel objVatModel = new VatModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objVatModel.VatId =
                                    Convert.ToInt32(objDataReader["VAT_ID"].ToString());
                                objVatModel.VatPercent = objDataReader["VAT_PERCENT"].ToString();
                                objVatModel.IncludeVat = objDataReader["INCLUDE_VAT"].ToString();
                                objVatModel.VatStatus = objVatModel.IncludeVat == "Y" ? true : false;
                                objVatModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objVatModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objVatModel.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objVatModel;
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

        public async Task<string> SaveAndUpdateVat(VatModel objVatModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_VAT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_vat_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objVatModel.VatId;
            objOracleCommand.Parameters.Add("p_vat_percent", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVatModel.VatPercent) ? objVatModel.VatPercent : null;
            objOracleCommand.Parameters.Add("p_include_vat", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVatModel.IncludeVat) ? objVatModel.IncludeVat : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVatModel.UpdateBy) ? objVatModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVatModel.WareHouseId) ? objVatModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objVatModel.ShopId) ? objVatModel.ShopId : null;

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

        #region "Customer"
        public async Task<List<CustomerModel>> GetCustomerList()
        {

            var sql = "SELECT " +
                      "CUSTOMER_ID," +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "CUSTOMER_CODE," +
                      "CUSTOMER_FIRST_NAME," +
                      "CUSTOMER_LAST_NAME," +
                      "CONTACT_NO," +
                      "EMAIL," +
                      "ADDRESS," +
                      "CITY," +
                      "POSTAL_CODE," +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "DISCOUNT_PERCENT," +
                      "to_char(DATE_OF_BIRTH,'dd/mm/yyyy') DATE_OF_BIRTH," +
                      "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                      "to_char(DATE_OF_EXPIRE,'dd/mm/yyyy') DATE_OF_EXPIRE ," +
                      "WHOLE_SALE_CUSTOMER," +
                      "STORE_CUSTOMER," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                         "FROM VEW_CUSTOMER  WHERE ACTIVE_YN ='Y'";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CustomerModel> objCustomerModel = new List<CustomerModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CustomerModel model = new CustomerModel
                                {
                                    CustomerId = Convert.ToInt32(objDataReader["CUSTOMER_ID"].ToString()),
                                    CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString()),
                                    CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString(),
                                    CustomerCode = objDataReader["CUSTOMER_CODE"].ToString(),
                                    CustomerFirstName = objDataReader["CUSTOMER_FIRST_NAME"].ToString(),
                                    CustomerLastName = objDataReader["CUSTOMER_LAST_NAME"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    Email = objDataReader["EMAIL"].ToString(),
                                    Address = objDataReader["ADDRESS"].ToString(),
                                    City = objDataReader["CITY"].ToString(),
                                    PostalCode = objDataReader["POSTAL_CODE"].ToString(),
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),
                                    DiscountPercent = objDataReader["DISCOUNT_PERCENT"].ToString(),
                                    DateOfBirth = objDataReader["DATE_OF_BIRTH"].ToString(),
                                    DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString(),
                                    DateOfExpire = objDataReader["DATE_OF_EXPIRE"].ToString(),
                                    WholeSaleCustomer = objDataReader["WHOLE_SALE_CUSTOMER"].ToString(),
                                    StoreCustomer = objDataReader["STORE_CUSTOMER"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCustomerModel.Add(model);
                            }

                            return objCustomerModel;
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
        public async Task<List<CustomerModel>> GetcustomerList(string wareHouseId, string shopId)
        {

            var sql = "SELECT " +
                      "CUSTOMER_ID," +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "CUSTOMER_CODE," +
                      "CUSTOMER_FIRST_NAME," +
                      "CUSTOMER_LAST_NAME," +
                      "CONTACT_NO," +
                      "EMAIL," +
                      "ADDRESS," +
                      "CITY," +
                      "POSTAL_CODE," +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "DISCOUNT_PERCENT," +
                      "to_char(DATE_OF_BIRTH,'dd/mm/yyyy') DATE_OF_BIRTH," +
                      "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                      "to_char(DATE_OF_EXPIRE,'dd/mm/yyyy') DATE_OF_EXPIRE ," +
                      "WHOLE_SALE_CUSTOMER," +
                      "STORE_CUSTOMER," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                         "FROM VEW_CUSTOMER where WARE_HOUSE_ID = :WARE_HOUSE_ID ORDER BY CUSTOMER_ID ";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    //objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<CustomerModel> objCustomerModel = new List<CustomerModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                CustomerModel model = new CustomerModel
                                {
                                    CustomerId = Convert.ToInt32(objDataReader["CUSTOMER_ID"].ToString()),
                                    CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString()),
                                    CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString(),
                                    CustomerCode = objDataReader["CUSTOMER_CODE"].ToString(),
                                    CustomerFirstName = objDataReader["CUSTOMER_FIRST_NAME"].ToString(),
                                    CustomerLastName = objDataReader["CUSTOMER_LAST_NAME"].ToString(),
                                    ContactNo = objDataReader["CONTACT_NO"].ToString(),
                                    Email = objDataReader["EMAIL"].ToString(),
                                    Address = objDataReader["ADDRESS"].ToString(),
                                    City = objDataReader["CITY"].ToString(),
                                    PostalCode = objDataReader["POSTAL_CODE"].ToString(),
                                    CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString()),
                                    CountryName = objDataReader["COUNTRY_NAME"].ToString(),
                                    DiscountPercent = objDataReader["DISCOUNT_PERCENT"].ToString(),
                                    DateOfBirth = objDataReader["DATE_OF_BIRTH"].ToString(),
                                    DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString(),
                                    DateOfExpire = objDataReader["DATE_OF_EXPIRE"].ToString(),
                                    WholeSaleCustomer = objDataReader["WHOLE_SALE_CUSTOMER"].ToString(),
                                    StoreCustomer = objDataReader["STORE_CUSTOMER"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objCustomerModel.Add(model);
                            }

                            return objCustomerModel;
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

        public async Task<CustomerModel> GetAcustomer(int customerId, int customerTypeId, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "CUSTOMER_ID," +
                      "CUSTOMER_TYPE_ID," +
                      "CUSTOMER_TYPE_NAME," +
                      "CUSTOMER_CODE," +
                      "CUSTOMER_FIRST_NAME," +
                      "CUSTOMER_LAST_NAME," +
                      "CONTACT_NO," +
                      "EMAIL," +
                      "ADDRESS," +
                      "CITY," +
                      "POSTAL_CODE," +
                      "COUNTRY_ID," +
                      "COUNTRY_NAME," +
                      "DISCOUNT_PERCENT," +
                      "to_char(DATE_OF_BIRTH,'dd/mm/yyyy') DATE_OF_BIRTH," +
                      "to_char(DATE_OF_ENROLLMENT,'dd/mm/yyyy') DATE_OF_ENROLLMENT ," +
                      "to_char(DATE_OF_EXPIRE,'dd/mm/yyyy') DATE_OF_EXPIRE ," +
                      "WHOLE_SALE_CUSTOMER," +
                      "STORE_CUSTOMER," +
                      "ACTIVE_YN," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_CUSTOMER where CUSTOMER_TYPE_ID = :CUSTOMER_TYPE_ID AND CUSTOMER_ID = :CUSTOMER_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                {
                    CommandType = CommandType.Text
                })
                {

                    objCommand.Parameters.Add(":CUSTOMER_TYPE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = customerTypeId;
                    objCommand.Parameters.Add(":CUSTOMER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = customerId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    //objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        CustomerModel objCustomerModel = new CustomerModel();

                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {

                                objCustomerModel.CustomerId = Convert.ToInt32(objDataReader["CUSTOMER_ID"].ToString());
                                objCustomerModel.CustomerTypeId = Convert.ToInt32(objDataReader["CUSTOMER_TYPE_ID"].ToString());
                                objCustomerModel.CustomerTypeName = objDataReader["CUSTOMER_TYPE_NAME"].ToString();
                                objCustomerModel.CustomerCode = objDataReader["CUSTOMER_CODE"].ToString();
                                objCustomerModel.CustomerFirstName = objDataReader["CUSTOMER_FIRST_NAME"].ToString();
                                objCustomerModel.CustomerLastName = objDataReader["CUSTOMER_LAST_NAME"].ToString();
                                objCustomerModel.ContactNo = objDataReader["CONTACT_NO"].ToString();
                                objCustomerModel.Email = objDataReader["EMAIL"].ToString();
                                objCustomerModel.Address = objDataReader["ADDRESS"].ToString();
                                objCustomerModel.City = objDataReader["CITY"].ToString();
                                objCustomerModel.PostalCode = objDataReader["POSTAL_CODE"].ToString();
                                objCustomerModel.CountryId = Convert.ToInt32(objDataReader["COUNTRY_ID"].ToString());
                                objCustomerModel.CountryName = objDataReader["COUNTRY_NAME"].ToString();
                                objCustomerModel.DiscountPercent = objDataReader["DISCOUNT_PERCENT"].ToString();
                                objCustomerModel.DateOfBirth = objDataReader["DATE_OF_BIRTH"].ToString();
                                objCustomerModel.DateOfEnrollment = objDataReader["DATE_OF_ENROLLMENT"].ToString();
                                objCustomerModel.DateOfExpire = objDataReader["DATE_OF_EXPIRE"].ToString();                                
                                objCustomerModel.WholeSaleCustomer = objDataReader["WHOLE_SALE_CUSTOMER"].ToString();
                                objCustomerModel.WholeSaleCustomerStatus = objCustomerModel.WholeSaleCustomer == "Y" ? true : false;
                                objCustomerModel.StoreCustomer = objDataReader["STORE_CUSTOMER"].ToString();
                                objCustomerModel.StoreCustomerStatus = objCustomerModel.StoreCustomer == "Y" ? true : false;
                                objCustomerModel.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objCustomerModel.ActiveStatus = objCustomerModel.Active_YN == "Y" ? true : false;
                                objCustomerModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objCustomerModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objCustomerModel.ShopId = objDataReader["SHOP_ID"].ToString();
                                
                            }
                            return objCustomerModel;
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

        public async Task<string> SaveAndUpdateCustomer(CustomerModel objCustomerModel)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_CUSTOMER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_customer_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CustomerId;
            objOracleCommand.Parameters.Add("p_customer_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CustomerTypeId;
            objOracleCommand.Parameters.Add("p_customer_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerCode) ? objCustomerModel.CustomerCode : null;
            objOracleCommand.Parameters.Add("p_customer_first_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerFirstName) ? objCustomerModel.CustomerFirstName.Trim() : null;
            objOracleCommand.Parameters.Add("p_customer_last_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerLastName) ? objCustomerModel.CustomerLastName : null;
            objOracleCommand.Parameters.Add("p_date_of_birth", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfBirth) ? objCustomerModel.DateOfBirth : null;
            objOracleCommand.Parameters.Add("p_date_of_enrollment", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfEnrollment) ? objCustomerModel.DateOfEnrollment : null;
            objOracleCommand.Parameters.Add("p_date_of_expire", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfExpire) ? objCustomerModel.DateOfExpire : null;
            objOracleCommand.Parameters.Add("p_contact_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.ContactNo) ? objCustomerModel.ContactNo : null;
            objOracleCommand.Parameters.Add("p_email", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Email) ? objCustomerModel.Email : null;
            objOracleCommand.Parameters.Add("p_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Address) ? objCustomerModel.Address : null;
            objOracleCommand.Parameters.Add("p_city", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.City) ? objCustomerModel.City : null;
            objOracleCommand.Parameters.Add("p_postal_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.PostalCode) ? objCustomerModel.PostalCode : null;
            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CountryId;
            objOracleCommand.Parameters.Add("p_discount_percent", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DiscountPercent) ? objCustomerModel.DiscountPercent : null;
            objOracleCommand.Parameters.Add("p_whole_sale_customer", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.WholeSaleCustomer) ? objCustomerModel.WholeSaleCustomer : null;
            objOracleCommand.Parameters.Add("p_store_customer", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.StoreCustomer) ? objCustomerModel.StoreCustomer : null;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Active_YN) ? objCustomerModel.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.UpdateBy) ? objCustomerModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.WareHouseId) ? objCustomerModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.ShopId) ? objCustomerModel.ShopId : null;

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

        public async Task<string> SavePrivilegeCustomer(CustomerModel objCustomerModel)
        {
            string strMessage;
            OracleCommand objOracleCommand = new OracleCommand("PRO_PRIVI_CUSTOMER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_customer_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CustomerId;
            objOracleCommand.Parameters.Add("p_customer_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CustomerTypeId;
            objOracleCommand.Parameters.Add("p_customer_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerCode) ? objCustomerModel.CustomerCode : null;
            objOracleCommand.Parameters.Add("p_customer_first_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerFirstName) ? objCustomerModel.CustomerFirstName.Trim() : null;
            objOracleCommand.Parameters.Add("p_customer_last_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.CustomerLastName) ? objCustomerModel.CustomerLastName : null;
            objOracleCommand.Parameters.Add("p_date_of_birth", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfBirth) ? objCustomerModel.DateOfBirth : null;
            objOracleCommand.Parameters.Add("p_date_of_enrollment", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfEnrollment) ? objCustomerModel.DateOfEnrollment : null;
            objOracleCommand.Parameters.Add("p_date_of_expire", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DateOfExpire) ? objCustomerModel.DateOfExpire : null;
            objOracleCommand.Parameters.Add("p_contact_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.ContactNo) ? objCustomerModel.ContactNo : null;
            objOracleCommand.Parameters.Add("p_email", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Email) ? objCustomerModel.Email : null;
            objOracleCommand.Parameters.Add("p_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Address) ? objCustomerModel.Address : null;
            objOracleCommand.Parameters.Add("p_city", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.City) ? objCustomerModel.City : null;
            objOracleCommand.Parameters.Add("p_postal_code", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.PostalCode) ? objCustomerModel.PostalCode : null;
            objOracleCommand.Parameters.Add("p_country_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objCustomerModel.CountryId;
            objOracleCommand.Parameters.Add("p_discount_percent", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.DiscountPercent) ? objCustomerModel.DiscountPercent : null;
            objOracleCommand.Parameters.Add("p_whole_sale_customer", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.WholeSaleCustomer) ? objCustomerModel.WholeSaleCustomer : null;
            objOracleCommand.Parameters.Add("p_store_customer", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.StoreCustomer) ? objCustomerModel.StoreCustomer : null;
            objOracleCommand.Parameters.Add("p_active_yn", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.Active_YN) ? objCustomerModel.Active_YN : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.UpdateBy) ? objCustomerModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.WareHouseId) ? objCustomerModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objCustomerModel.ShopId) ? objCustomerModel.ShopId : null;

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
        public async Task<string> DeleteCustomer(int customerId, int customerTypeId, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_CUSTOMER_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_customer_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = customerId;
            objOracleCommand.Parameters.Add("p_customer_type_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = customerTypeId;

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

        #region "Register info Section"

        public async Task<List<RegisterInfoModel>> GetRegisterInfoList()
        {
            var sql = "SELECT " +
                      "REGISTER_ID," +
                        "REGISTER_PERSON_NAME," +
                        "BIN_NO," +
                        "ADDRESS," +
                        "UPDATE_BY," +
                        "UPDATE_DATE " +
                      "FROM VEW_REGISTER_INFO ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<RegisterInfoModel> objRegisterInfoModel= new List<RegisterInfoModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                RegisterInfoModel model = new RegisterInfoModel
                                {
                                    RegisterId = Convert.ToInt32(objDataReader["REGISTER_ID"].ToString()),
                                    RegisterPersonName = objDataReader["REGISTER_PERSON_NAME"].ToString(),
                                    BinNO = objDataReader["BIN_NO"].ToString(),
                                    Address = objDataReader["ADDRESS"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString()
                                   
                                };
                                objRegisterInfoModel.Add(model);
                            }
                            return objRegisterInfoModel;
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

        public async Task<RegisterInfoModel> GetARegisterInfo(int registerId)
        {

            var sql = "SELECT " +
                      "REGISTER_ID," +
                      "REGISTER_PERSON_NAME," +
                      "BIN_NO," +
                      "ADDRESS," +
                      "UPDATE_BY," +
                      "UPDATE_DATE " +
                      "FROM VEW_REGISTER_INFO where REGISTER_ID = :REGISTER_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":REGISTER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = registerId;
                   
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        RegisterInfoModel objRegisterInfoModel = new RegisterInfoModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objRegisterInfoModel.RegisterId = Convert.ToInt32(objDataReader["REGISTER_ID"].ToString());
                                objRegisterInfoModel.RegisterPersonName = objDataReader["REGISTER_PERSON_NAME"].ToString();
                                objRegisterInfoModel.BinNO = objDataReader["BIN_NO"].ToString();
                                objRegisterInfoModel.Address = objDataReader["ADDRESS"].ToString();                              
                                objRegisterInfoModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                               
                            }

                            return objRegisterInfoModel;
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

        public async Task<string> SaveAndUpdateRegisterInfo(RegisterInfoModel objRegisterInfoModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_REGISTER_INFO_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_register_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objRegisterInfoModel.RegisterId;
            objOracleCommand.Parameters.Add("p_register_person_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRegisterInfoModel.RegisterPersonName) ? objRegisterInfoModel.RegisterPersonName : null;
            objOracleCommand.Parameters.Add("p_bin_no", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRegisterInfoModel.BinNO) ? objRegisterInfoModel.BinNO : null;
            objOracleCommand.Parameters.Add("p_address", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRegisterInfoModel.Address) ? objRegisterInfoModel.Address : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objRegisterInfoModel.UpdateBy) ? objRegisterInfoModel.UpdateBy : null;
            
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

        public async Task<string> DeleteRegisterInfo(int registerId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_REGISTER_INFO_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_register_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = registerId;
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

        #region "season Section"

        public async Task<List<SeasonModel>> GetSeasonList(string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "SEASON_ID," +
                        "SEASON_NAME," +
                        "UPDATE_BY," +
                        "UPDATE_DATE," +
                        "CREATE_BY," +
                        "CREATE_DATE," +
                        "WARE_HOUSE_ID," +
                        "SHOP_ID " +
                      "FROM VEW_SEASON where WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SeasonModel> objSeasonModels = new List<SeasonModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SeasonModel model = new SeasonModel
                                {
                                    SeasonId = Convert.ToInt32(objDataReader["SEASON_ID"].ToString()),
                                    SeasonName = objDataReader["SEASON_NAME"].ToString(),                                  
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objSeasonModels.Add(model);
                            }
                            return objSeasonModels;
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

        public async Task<SeasonModel> GetASeason(int seasonId, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "SEASON_ID," +
                      "SEASON_NAME," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_SEASON  where SEASON_ID=:SEASON_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SEASON_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = seasonId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        SeasonModel objSeasonModels = new SeasonModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objSeasonModels.SeasonId =
                                    Convert.ToInt32(objDataReader["SEASON_ID"].ToString());
                                objSeasonModels.SeasonName = objDataReader["SEASON_NAME"].ToString();                              
                                objSeasonModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objSeasonModels.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objSeasonModels.ShopId = objDataReader["SHOP_ID"].ToString();
                            }

                            return objSeasonModels;
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

        public async Task<string> SaveAndUpdateSeason(SeasonModel objSeasonModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SEASON_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_season_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSeasonModel.SeasonId;
            objOracleCommand.Parameters.Add("p_season_name", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSeasonModel.SeasonName) ? objSeasonModel.SeasonName : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSeasonModel.UpdateBy) ? objSeasonModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSeasonModel.WareHouseId) ? objSeasonModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objSeasonModel.ShopId) ? objSeasonModel.ShopId : null;

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

        public async Task<string> DeleteSeason(int id, string wareHouseId, string shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_SEASON_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_season_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = id;

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