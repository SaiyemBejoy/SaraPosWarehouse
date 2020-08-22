using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;

namespace PosWarehouse.DAL
{
    public class ProductDal
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

        public IEnumerable<ProductGrid> GetProductGrids(ProductGrid objProductGrid)
        {


            var sql = "SELECT " +
                        "ProductId," +
                        "ProductName," +
                        "ProductStyle," +
                        "ProductDescription," +
                        "CategoryName," +
                        "SubCategoryName," +
                        "IsActive," +
                        "ProductImage," +
                        "IncludeVat," +
                        "MATERIALCOST," +
                        "CM," +
                        "PurchasePrice," +
                        "SellPrice," +
                        "UpdateBy," +
                        "WareHouseId," +
                        "ShopId " +
                        "FROM VEW_PRODUCT_GRID WHERE 1=1 ";


            if (!string.IsNullOrWhiteSpace(objProductGrid.SearchBy))
            {
                sql += "and ( (lower(ProductName) like lower(:SearchBy)  or upper(ProductName)like upper(:SearchBy) )" +
                          "or (lower(ProductStyle) like lower(:SearchBy)  or upper(ProductStyle)like upper(:SearchBy) )" +
                          "or (lower(PurchasePrice) like lower(:SearchBy)  or upper(PurchasePrice)like upper(:SearchBy) )" +
                          "or (lower(SellPrice) like lower(:SearchBy)  or upper(SellPrice)like upper(:SearchBy) )" +
                          "or (lower(CategoryName) like lower(:SearchBy)  or upper(CategoryName)like upper(:SearchBy) )" +
                          "or (lower(SubCategoryName) like lower(:SearchBy)  or upper(SubCategoryName)like upper(:SearchBy) )" +
                          "or (lower(ProductDescription) like lower(:SearchBy)  or upper(ProductDescription)like upper(:SearchBy) ) )";
            }

            //sql += "AND ROWNUM <= 100 ";

            if (objProductGrid.OrderByName != null && objProductGrid.OrderByDirection != null)
            {
                sql += "ORDER BY " + objProductGrid.OrderByName + " " + objProductGrid.OrderByDirection;
            }

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductGrid.SearchBy) ?
                        $"%{objProductGrid.SearchBy}%"
                        : null;

                    objConnection.Open();

                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        List<ProductGrid> grid = new List<ProductGrid>();
                        try
                        {
                            while (objDataReader.Read())
                            {
                                objProductGrid = new ProductGrid();

                                objProductGrid.ProductId = objDataReader["ProductId"].ToString();
                                objProductGrid.ProductName = objDataReader["ProductName"].ToString();
                                objProductGrid.ProductStyle = objDataReader["ProductStyle"].ToString();
                                objProductGrid.ProductDescription = objDataReader["ProductDescription"].ToString();

                                objProductGrid.CategoryName = objDataReader["CategoryName"].ToString();
                                objProductGrid.SubCategoryName = objDataReader["SubCategoryName"].ToString();
                                

                                objProductGrid.MaterialCost = objDataReader["MATERIALCOST"].ToString();
                                objProductGrid.CM = objDataReader["CM"].ToString();
                                objProductGrid.PurchasePrice = objDataReader["PurchasePrice"].ToString();
                                objProductGrid.SellPrice = objDataReader["SellPrice"].ToString();
                                objProductGrid.IncludeVat = objDataReader["IncludeVat"].ToString() == "Y";

                                objProductGrid.ProductImage = Path.Combine("/Files/ProductImage/", objDataReader["ProductImage"].ToString());

                                objProductGrid.IsActive = objDataReader["IsActive"].ToString() == "Y";

                                //ProductQuantity = objDataReader.GetValue(5).ToString(),
                                objProductGrid.UpdateBy = objDataReader["UpdateBy"].ToString();
                                objProductGrid.WareHouseId = objDataReader["WareHouseId"].ToString();
                                objProductGrid.ShopId = objDataReader["ShopId"].ToString();

                                grid.Add(objProductGrid);
                            }

                            return grid;
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

        public async Task<List<string>> SaveProductInfo(ProductModel objProductModel)
        {
            string strMsg;
            string productId;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.ProductId != 0 ? objProductModel.ProductId.ToString() : null;
            objOracleCommand.Parameters.Add("P_PRODUCT_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.ProductName.Trim()) ? objProductModel.ProductName.Trim() : null;
            objOracleCommand.Parameters.Add("P_PRODUCT_STYLE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.ProductStyle.Trim()) ? objProductModel.ProductStyle.Trim(): null;
            objOracleCommand.Parameters.Add("P_PRODUCT_DESCRIPTION", OracleDbType.Clob, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(objProductModel.ProductDescription) ? objProductModel.ProductDescription : null;

            objOracleCommand.Parameters.Add("P_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.CategoryId != 0 ? objProductModel.CategoryId.ToString() : null;
            objOracleCommand.Parameters.Add("P_SUB_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.SubCategoryId != 0 ? objProductModel.SubCategoryId.ToString() : null;

            objOracleCommand.Parameters.Add("P_ACTIVE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.IsActive ? "Y" : "N";

            objOracleCommand.Parameters.Add("P_PRODUCT_IMAGE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.ProductImageString) ? objProductModel.ProductImageString : null;

            objOracleCommand.Parameters.Add("P_VAT_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.IncludeVat ? "Y" : "N";

            objOracleCommand.Parameters.Add("P_PURCHASE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.PurchasePrice.ToString()) ? objProductModel.PurchasePrice.ToString() : null;
            objOracleCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.SalePrice.ToString()) ? objProductModel.SalePrice.ToString() : null;
            objOracleCommand.Parameters.Add("P_MATERIAL_COST", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.MaterialCost.ToString()) ? objProductModel.MaterialCost.ToString() : null;
            objOracleCommand.Parameters.Add("P_CM", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.CM.ToString()) ? objProductModel.CM.ToString() : null;
            objOracleCommand.Parameters.Add("P_KARCUPI", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.Karchupi ? "Y" : "N";
            objOracleCommand.Parameters.Add("P_WASH", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.Wash) ? objProductModel.Wash : null;
            objOracleCommand.Parameters.Add("P_PRINT", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.Print) ? objProductModel.Print : null;
            objOracleCommand.Parameters.Add("P_EMBROIDERY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.Embroidery) ? objProductModel.Embroidery : null;

            objOracleCommand.Parameters.Add("P_DESIGNER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.DesignerId) ? objProductModel.DesignerId : null;
            objOracleCommand.Parameters.Add("P_MERCHANDISER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objProductModel.MerchandiserId) ? objProductModel.MerchandiserId : null;
            objOracleCommand.Parameters.Add("P_BRAND_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.BrandId != 0 ? objProductModel.BrandId.ToString() : null;
            objOracleCommand.Parameters.Add("p_PurcMeasurUnit_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.PurchaseMeasuringUnitId != 0 ? objProductModel.PurchaseMeasuringUnitId.ToString() : null;
            objOracleCommand.Parameters.Add("p_SaleMeasurUnit_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objProductModel.SalesMeasuringUnitId != 0 ? objProductModel.SalesMeasuringUnitId.ToString() : null;
            objOracleCommand.Parameters.Add("p_PurcUnitAndSaleUnit", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(objProductModel.PurchaseUnitSaleUnit) ? objProductModel.PurchaseUnitSaleUnit : null;

            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(objProductModel.UpdateBy) ? objProductModel.UpdateBy : null;
            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(objProductModel.WareHouseId) ? objProductModel.WareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(objProductModel.ShopId) ? objProductModel.ShopId : null;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;
            objOracleCommand.Parameters.Add("O_PRODUCT_ID", OracleDbType.Varchar2, 500).Direction = ParameterDirection.Output;

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
                    productId = objOracleCommand.Parameters["O_PRODUCT_ID"].Value.ToString();
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
            return  new List<string>{ strMsg, productId };
        }

        public async Task<ProductModel> GetAProduct(int productId, string wareHouseId, string shopId)
        {
            var sql = "SELECT " +
                      "PRODUCT_ID," +
                      "PRODUCT_NAME," +
                      "PRODUCT_STYLE," +
                      "PRODUCT_DESCRIPTION," +
                      "CATEGORY_ID," +
                      "SUB_CATEGORY_ID," +
                      "BRAND_ID," +
                      "DESIGNER_ID," +
                      "MERCHANDISER_ID," +
                      "PurcMeasurUnit_ID," +
                      "SaleMeasurUnit_ID," +
                      "PurcUnitAndSaleUnit," +
                      "ACTIVE_YN," +
                      "PRODUCT_IMAGE," +
                      "VAT_YN," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE," +
                      "MATERIAL_COST," +
                        "CM," +
                        "KARCUPI," +
                        "WASH," +
                        "PRINT," +
                        "EMBROIDERY," +
                      "UPDATE_BY," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PRODUCT where PRODUCT_ID = :PRODUCT_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID ";

            //OracleCommand objCommand = new OracleCommand(sql);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        ProductModel objProductModel = new ProductModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objProductModel.ProductId =
                                    Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                objProductModel.CategoryId =
                                    Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString());
                                objProductModel.SubCategoryId =
                                    Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString());
                                objProductModel.ProductName = objDataReader["PRODUCT_NAME"].ToString();
                                objProductModel.ProductStyle = objDataReader["PRODUCT_STYLE"].ToString();
                                objProductModel.ProductDescription = objDataReader["PRODUCT_DESCRIPTION"].ToString();
                                objProductModel.PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString());
                                objProductModel.SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                                objProductModel.MaterialCost = Convert.ToDouble(objDataReader["MATERIAL_COST"].ToString());
                                objProductModel.CM = Convert.ToDouble(objDataReader["CM"].ToString());
                                objProductModel.Wash = objDataReader["WASH"].ToString();
                                objProductModel.Print = objDataReader["PRINT"].ToString();
                                objProductModel.Embroidery = objDataReader["EMBROIDERY"].ToString();
                                objProductModel.ProductImageString = objDataReader["PRODUCT_IMAGE"].ToString();
                                objProductModel.DesignerId = objDataReader["DESIGNER_ID"].ToString();
                                objProductModel.MerchandiserId = objDataReader["MERCHANDISER_ID"].ToString();
                                objProductModel.BrandId =
                                    Convert.ToInt32(objDataReader["BRAND_ID"].ToString());
                                objProductModel.PurchaseMeasuringUnitId =
                                    Convert.ToInt32(objDataReader["PurcMeasurUnit_ID"].ToString());
                                objProductModel.SalesMeasuringUnitId =
                                    Convert.ToInt32(objDataReader["SaleMeasurUnit_ID"].ToString());
                                objProductModel.PurchaseUnitSaleUnit = objDataReader["PurcUnitAndSaleUnit"].ToString();
                                //objProductModel.IsActive = objProductModel.IsActive == "Y") ? true : false;
                                // objProductModel.IncludeVat = objProductModel.IncludeVat == "Y" ? true : false;

                                objProductModel.IsActive = objDataReader["ACTIVE_YN"].ToString() == "Y";
                                objProductModel.Karchupi = objDataReader["KARCUPI"].ToString() == "Y";
                                objProductModel.IncludeVat = objDataReader["VAT_YN"].ToString() == "Y";
                                objProductModel.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objProductModel.WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString();
                                objProductModel.ShopId = objDataReader["SHOP_ID"].ToString();
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

        public async Task<IEnumerable<ProductAttribute>> GetAttributeValue(int attributeId, string wareHouseId, string shopId)
        {
            const string sql = "SELECT " +
                               "ATTRIBUTE_ID," +
                               "ATTRIBUTE_NAME," +
                               "ATTRIBUTE_VALUE_ID," +
                               "ATTRIBUTE_VALUE_NAME," +
                               "UPDATE_BY," +
                               "WARE_HOUSE_ID," +
                               "SHOP_ID " +
                               "FROM VEW_ATTRIBUTE_VALUE where ATTRIBUTE_ID = :ATTRIBUTE_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID ";

            //OracleCommand objCommand = new OracleCommand(sql);

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductAttribute> objProductAttribute = new List<ProductAttribute>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductAttribute model = new ProductAttribute();

                                model.AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString());
                                model.AttributeValueId = Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString());
                                model.Attribute = objDataReader["ATTRIBUTE_NAME"].ToString();
                                model.AttributeValue = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString();
                                objProductAttribute.Add(model);
                            }

                            return objProductAttribute;
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

        public async Task<string> SaveProductAttributes(List<ProductAttribute> objProductAttributes, string updateBy, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PRODUCT_ATTRIBUTE_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objProductAttributes)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductId != 0 ? data.ProductId.ToString() : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.AttributeId != 0 ? data.AttributeId.ToString() : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_VALUE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.AttributeValueId != 0 ? data.AttributeValueId.ToString() : null;

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

        public async Task<List<Attributes>> GetProductAttribute(int productId, string wareHouseId, string shopId)
        {
            const string sql = "SELECT distinct " +
                                "PRODUCT_ID," +
                                "ATTRIBUTE_ID," +
                                "ATTRIBUTE_NAME," +
                                "ATTRIBUTE_ORDER " +
                                "FROM VEW_PRODUCT_ATTRIBUTE where PRODUCT_ID = :PRODUCT_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<Attributes> objProductAttribute = new List<Attributes>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                Attributes model = new Attributes();

                                model.AttributeId = Convert.ToInt32(objDataReader["ATTRIBUTE_ID"].ToString());
                                model.AttributeOrder = Convert.ToInt32(objDataReader["ATTRIBUTE_ORDER"].ToString());
                                model.AttributeName = objDataReader["ATTRIBUTE_NAME"].ToString();
                                objProductAttribute.Add(model);
                            }

                            return objProductAttribute;
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

        public List<AttributesValue> GetProductAttributeValue(int productId, int attributeId, string wareHouseId, string shopId, out List<string>value)
        {
            value = new List<string>();
            const string sql = "SELECT " +
                                "PRODUCT_ID," +
                                "ATTRIBUTE_ID," +
                                "ATTRIBUTE_VALUE_ID," +
                                "ATTRIBUTE_VALUE_NAME " +
                                "FROM VEW_PRODUCT_ATTRIBUTE where PRODUCT_ID = :PRODUCT_ID AND ATTRIBUTE_ID = :ATTRIBUTE_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = attributeId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    objConnection.Open();
                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        List<AttributesValue> objProductAttribute = new List<AttributesValue>();
                        try
                        {
                            while (objDataReader.Read())
                            {
                                AttributesValue model = new AttributesValue();

                                model.AttributeValueId = Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString());
                                model.AttributeValueName = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString();
                                value.Add(objDataReader["ATTRIBUTE_VALUE_NAME"].ToString());
                                objProductAttribute.Add(model);
                            }

                            return objProductAttribute;
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

        public List<AttributesValue> GetProductAttributeValue(int productId, string wareHouseId, string shopId)
        {
            const string sql = "SELECT " +
                                "PRODUCT_ID," +
                                "ATTRIBUTE_ID," +
                                "ATTRIBUTE_VALUE_ID," +
                                "ATTRIBUTE_VALUE_NAME " +
                                "FROM VEW_PRODUCT_ATTRIBUTE where PRODUCT_ID = :PRODUCT_ID AND WARE_HOUSE_ID =:WARE_HOUSE_ID  AND SHOP_ID = :SHOP_ID ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;

                    objConnection.Open();
                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        List<AttributesValue> objProductAttribute = new List<AttributesValue>();
                        try
                        {
                            while (objDataReader.Read())
                            {
                                AttributesValue model = new AttributesValue();

                                model.AttributeValueId = Convert.ToInt32(objDataReader["ATTRIBUTE_VALUE_ID"].ToString());
                                model.AttributeValueName = objDataReader["ATTRIBUTE_VALUE_NAME"].ToString();
                                objProductAttribute.Add(model);
                            }

                            return objProductAttribute;
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

        public async Task<string> DeleteProductAttribute(string productId, string attributeId, string attributeValueId, string wareHouseId, string shopId)
        {
            string strMsg;

            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_ATTRIBUTE_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(productId) ? productId : null;
            objOracleCommand.Parameters.Add("P_ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(attributeId) ? attributeId : null;
            objOracleCommand.Parameters.Add("P_ATTRIBUTE_VALUE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(attributeValueId) ? attributeValueId : null;

            objOracleCommand.Parameters.Add("P_WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

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

        public async Task<string> SaveProductItem(ProductItem item, string employeeId, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PRODUCT_ITEM_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        if (item.IsActive)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.ProductId != 0 ? item.ProductId.ToString() : null;
                            objCommand.Parameters.Add("P_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.ItemId != 0 ? item.ItemId.ToString() : null;


                            objCommand.Parameters.Add("P_PRODUCT_CODE", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(item.ProductCode) ? item.ProductCode.Trim() : null;
                            objCommand.Parameters.Add("P_PRODUCT_NAME", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(item.ProductName) ? item.ProductName.Trim() : null;

                            objCommand.Parameters.Add("P_MATERIAL_COST", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.MaterialCost > 0 ? item.MaterialCost.ToString(CultureInfo.InvariantCulture) : "0";
                            objCommand.Parameters.Add("P_CM", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.CM > 0 ? item.CM.ToString(CultureInfo.InvariantCulture) : "0";
                            objCommand.Parameters.Add("P_PURCHASE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.PurchasePrice > 0 ? item.PurchasePrice.ToString(CultureInfo.InvariantCulture) : "0";
                            objCommand.Parameters.Add("P_SALE_PRICE", OracleDbType.Varchar2, ParameterDirection.Input).Value = item.SalePrice > 0 ? item.SalePrice.ToString(CultureInfo.InvariantCulture) : "0";

                            objCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.InputOutput).Value = !string.IsNullOrWhiteSpace(employeeId) ? employeeId : null;
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

        public async Task<string> DeleteProductItemAndAttribute(int productId, string productCode)
        {
            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_ITEM_ATR_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };
            string strMessage = null;

            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
            objOracleCommand.Parameters.Add("P_PRODUCT_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(productCode) ? productCode.Trim() : null;

            objOracleCommand.Parameters.Add("P_MESSAGE", OracleDbType.Varchar2, 500).Direction =
                ParameterDirection.Output;

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

        public async Task<string> SaveProductItemAttributes(List<ItemAttribute> objItemAttr, string updateBy, string wareHouseId, string shopId)
        {
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand("PRO_PRODUCT_ITEM_ATTR_SAVE", objConnection) { CommandType = CommandType.StoredProcedure })
                {
                    await objConnection.OpenAsync();

                    try
                    {
                        string vMessage = null;

                        foreach (var data in objItemAttr)
                        {
                            objCommand.Parameters.Clear();

                            objCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductId != 0 ? data.ProductId.ToString() : null;
                            objCommand.Parameters.Add("P_PRODUCT_ITEM_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.ProductItemId != 0 ? data.ProductItemId.ToString() : null;
                            objCommand.Parameters.Add("P_PRODUCT_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.ProductCode) ? data.ProductCode : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.AttributeId != 0 ? data.AttributeId.ToString() : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.AttributeName) ? data.AttributeName : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_VALUE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = data.AttributeValueId != 0 ? data.AttributeValueId.ToString() : null;
                            objCommand.Parameters.Add("P_ATTRIBUTE_VALUE_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(data.AttributeValueName) ? data.AttributeValueName : null;

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

        public int GetMaxItemCode()
        {
            var sql = "SELECT PRODUCT_CODE FROM VEW_MAX_PRODUCT_CODE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    objConnection.Open();
                    using (OracleDataReader objDataReader = objCommand.ExecuteReader())
                    {
                        var itemCode = 0;
                        try
                        {
                            while (objDataReader.Read())
                            {
                                itemCode = Convert.ToInt32(objDataReader["PRODUCT_CODE"].ToString());
                            }
                            return itemCode;
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

        public async Task<List<ProductItem>> GetProductItem(int productId,string wareHouseId, string shopId)
        {


            var sql = "SELECT " +
                      "ITEM_ID," +
                      "PRODUCT_ID," +
                      "PRODUCT_CODE," +
                      "PRODUCT_NAME," +
                      "ITEM_NAME," +
                      "MATERIAL_COST," +
                        "CM," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE," +
                      "UPDATE_BY," +
                     "UPDATE_DATE," +
                     "CREATE_BY," +
                     "CREATE_DATE," +
                      "WARE_HOUSE_ID," +
                      "SHOP_ID " +
                      "FROM VEW_PRODUCT_ITEM where PRODUCT_ID =:PRODUCT_ID AND WARE_HOUSE_ID = :WARE_HOUSE_ID AND SHOP_ID =:SHOP_ID ";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
                    objCommand.Parameters.Add(":WARE_HOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = wareHouseId;
                    objCommand.Parameters.Add(":SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<ProductItem> objProductItem = new List<ProductItem>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                ProductItem model = new ProductItem
                                {
                                    ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString()),
                                    ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString()),
                                    ProductCode = objDataReader["PRODUCT_CODE"].ToString(),
                                    ProductName = objDataReader["ITEM_NAME"].ToString(),
                                    ItemName = objDataReader["PRODUCT_NAME"].ToString(),
                                    MaterialCost = Convert .ToDouble(objDataReader["MATERIAL_COST"].ToString()),
                                    CM = Convert .ToDouble(objDataReader["CM"].ToString()),
                                    PurchasePrice = Convert .ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
                                    SalePrice = Convert .ToDouble(objDataReader["SALE_PRICE"].ToString()),
                                    IsActive = true,
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WARE_HOUSE_ID"].ToString(),
                                    ShopId = objDataReader["SHOP_ID"].ToString()
                                };
                                objProductItem.Add(model);
                            }
                            return objProductItem;
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

        public async Task<string> DeleteProductItem(int productId, string wareHouseId, string shopId)
        {
         

            OracleCommand objOracleCommand = new OracleCommand("PRO_PRODUCT_ITEM_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };
            string strMessage = null;

            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = productId;
            objOracleCommand.Parameters.Add("p_ware_house_id", OracleDbType.Varchar2, ParameterDirection.Input)
                    .Value = !string.IsNullOrWhiteSpace(wareHouseId) ? wareHouseId : null;
                objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value =
                    !string.IsNullOrWhiteSpace(shopId) ? shopId : null;

                objOracleCommand.Parameters.Add("p_message", OracleDbType.Varchar2, 500).Direction =
                    ParameterDirection.Output;

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


    }
}