using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel.Ecom;

namespace PosWarehouse.DAL.Ecom
{
    public class EcomDal
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

        #region ProductInfo Masum Work

        public async Task<List<Product>> GetAllProductInfo()
        {
            var sql = @"SELECT PRODUCT_ID,
                        PRODUCT_NAME,
                        PRODUCT_STYLE,
                        PRODUCT_DESCRIPTION,
                        CATEGORY_NAME,
                        SUB_CATEGORY_NAME 
                        FROM PRODUCT_FOR_ECOM
                        WHERE RECEIVE_YN = 'N' AND ROWNUM <= 100
                        ORDER BY PRODUCT_ID ";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<Product> objProductModel = new List<Product>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                Product model = new Product
                                {
                                    WhProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ProductName = objDataReader["PRODUCT_NAME"].ToString().Trim(),
                                    ProductStyle= objDataReader["PRODUCT_STYLE"].ToString().Trim(),
                                    ShortDescription = objDataReader["PRODUCT_DESCRIPTION"].ToString().Trim(),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString().Trim(),
                                    SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString().Trim()
                                };
                                objProductModel.Add(model);
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

        public async Task<bool> UpdateEcomProduct(int id)
        {
            var sql = "UPDATE PRODUCT_FOR_ECOM SET RECEIVE_YN = :RECEIVE_YN WHERE PRODUCT_ID = :PRODUCT_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":RECEIVE_YN", OracleDbType.Char, ParameterDirection.Input).Value = "Y";
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Int32, ParameterDirection.Input).Value = id;

                    await objConnection.OpenAsync();

                    try
                    {
                        await objCommand.ExecuteReaderAsync();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        objCommand.Dispose();
                        objConnection.Dispose();
                    }
                    return false;
                }
            }
        }

        public async Task<ICollection<ProductItem>> GetAllProductItemInfo(int productId)
        {
            var sql = @"SELECT PRODUCT_ID,
                       ITEM_ID,
                       PRODUCT_CODE,
                       PRODUCT_NAME,
                       ITEM_NAME
                       FROM PRODUCT_ITEM_FOR_ECOM 
                       WHERE PRODUCT_ID = :PRODUCT_ID ";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Int32, ParameterDirection.Input).Value = productId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductItem> productItems = new List<ProductItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductItem model = new ProductItem();
                                model.WhProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                model.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                model.Barcode = objDataReader["PRODUCT_CODE"].ToString().Trim();
                                model.ProductName = objDataReader["PRODUCT_NAME"].ToString().Trim();
                                model.ItemName = objDataReader["ITEM_NAME"].ToString().Trim();

                                var itemAttr = model.ItemName.Split('|');

                                for (int i = 0; i < itemAttr.Length; i++)
                                {
                                    if (i == 0)
                                        model.SizeName = itemAttr[i].Trim();
                                    if (i == 1)
                                        model.ColorName = itemAttr[i].Trim();
                                    if (i == 2)
                                        model.FabricName = itemAttr[i].Trim();
                                }

                                productItems.Add(model);
                            }

                            return productItems;
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

        public async Task<string> LoadData()
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_FOR_ECOM")
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

        #region Category Info

        public async Task<List<ProductCategory>> GetProductCategory()
        {
            var sql = "SELECT CATEGORY_ID, CATEGORY_NAME FROM L_CATEGORY ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductCategory> objCategoryModel = new List<ProductCategory>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductCategory model = new ProductCategory
                                {
                                    Id = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    Name = objDataReader["CATEGORY_NAME"].ToString().Trim()
                                };
                                objCategoryModel.Add(model);
                            }

                            return objCategoryModel;
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

        public async Task<List<ProductSubCategory>> GetProductSubCategory(int categoryId)
        {
            var sql = "SELECT SUB_CATEGORY_ID, SUB_CATEGORY_NAME FROM L_SUB_CATEGORY WHERE CATEGORY_ID = :CATEGORY_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":CATEGORY_ID", OracleDbType.Int32, ParameterDirection.Input).Value = categoryId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductSubCategory> objSubCategoryModel = new List<ProductSubCategory>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductSubCategory model = new ProductSubCategory
                                {
                                    Id = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString()),
                                    Name = objDataReader["SUB_CATEGORY_NAME"].ToString().Trim()
                                };
                                objSubCategoryModel.Add(model);
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


        #endregion

        #region E-Com Sazzad Vai

        public async Task<List<ProductInfoEcom>> GetAllProductInfoForEcom()
        {
            var sql = @"SELECT PRODUCT_ID,
                       PRODUCT_NAME,
                       PRODUCT_STYLE,
                       PRODUCT_DESCRIPTION,
                       CATEGORY_ID,
                       CATEGORY_NAME,
                       SUB_CATEGORY_ID,
                       SUB_CATEGORY_NAME,
                       ACTIVE_YN,
                       PRODUCT_IMAGE,
                       VAT_YN,
                       PURCHASE_PRICE,
                       SALE_PRICE,
                       WARE_HOUSE_ID,
                       SHOP_ID,
                       BRAND_ID,
                       DESIGNER_ID,
                       MERCHANDISER_ID,
                       MATERIAL_COST,
                       ITEM_ID,
                       PRODUCT_CODE,
                       ITEM_NAME,
                       ITEM_ATTRIBUTE_ID,
                       PRODUCT_ITEM_ID,
                       ATTRIBUTE_ID,
                       ATTRIBUTE_NAME,
                       ATTRIBUTE_VALUE_ID,
                       ATTRIBUTE_VALUE_NAME 
                       FROM VEW_ECOM_PRODUCT ";

            sql = sql.Replace(System.Environment.NewLine, string.Empty);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductInfoEcom> objProductInfoEcom = new List<ProductInfoEcom>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductInfoEcom model = new ProductInfoEcom
                                {
                                    
                                    ProductId  = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ProductName = objDataReader["PRODUCT_NAME"].ToString().Trim(),
                                    ProductStyle= objDataReader["PRODUCT_STYLE"].ToString().Trim(),
                                    ProductDescription = objDataReader["PRODUCT_DESCRIPTION"].ToString().Trim(),
                                    CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString()),
                                    CategoryName = objDataReader["CATEGORY_NAME"].ToString().Trim(),
                                    SubCategoryId = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString()),
                                    SubCategoryName = objDataReader["SUB_CATEGORY_NAME"].ToString().Trim(),

                                    ActiveYN = objDataReader["ACTIVE_YN"].ToString().Trim(),
                                    ProductImage = objDataReader["PRODUCT_IMAGE"].ToString().Trim(),
                                    VatYN = objDataReader["VAT_YN"].ToString().Trim(),
                                    PurchasePrice = Convert.ToInt32(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert.ToInt32(objDataReader["SALE_PRICE"].ToString()),

                                    WarehouseId = objDataReader["WARE_HOUSE_ID"].ToString().Trim(),
                                    ShopId = objDataReader["SHOP_ID"].ToString().Trim(),
                                    BrandId = Convert.ToInt32(objDataReader["BRAND_ID"].ToString()),
                                    DesignerId = objDataReader["DESIGNER_ID"].ToString().Trim(),
                                    MerchandiserId = objDataReader["MERCHANDISER_ID"].ToString().Trim(),

                                    MaterialCost = Convert.ToInt32(objDataReader["MATERIAL_COST"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductCode = objDataReader["PRODUCT_CODE"].ToString().Trim(),
                                    ItemName = objDataReader["ITEM_NAME"].ToString().Trim(),
                                    ItemAttributeId = Convert.ToInt32(objDataReader["ITEM_ATTRIBUTE_ID"].ToString()),
                                    ProductItemId = Convert.ToInt32(objDataReader["PRODUCT_ITEM_ID"].ToString()),

                                    AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString()),
                                    AttributeName = objDataReader["ATTRIBUTE_NAME"].ToString().Trim(),
                                    AttributeValueId = Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString()),
                                    AttributeValueName = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString().Trim(),
                                };
                                objProductInfoEcom.Add(model);
                            }

                            return objProductInfoEcom;
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