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
    public class QuickSearchDal
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

        #region Product Search

        //public async Task<List<QuickSearchModelDataTable>> GetAllProductList(QuickSearchModel objQuickSearchModel)
        //{
        //    var sql = "SELECT " +
        //                "ITEM_ID," +
        //                "PRODUCT_ID," +
        //                "MERCHANDISER_ID," +
        //                "DESIGNER_ID," +
        //                "PRODUCT_STYLE," +
        //                "ITEM_NAME," +
        //                "BARCODE," +
        //                "PURCHASE_PRICE," +
        //                "SALE_PRICE," +
        //                "VAT," +
        //                "PRODUCT_CATEGORY," +
        //                "CATEGORY_ID," +
        //                "PRODUCT_SUB_CATEGORY," +
        //                "SUB_CATEGORY_ID," +
        //                "QUANTITY, " +
        //                //"WAREHOUSE_STOCK," +
        //                //"SHOP_STOCK," +
        //                "SHOP_ID, " +
        //                "SHOP_NAME  " +
        //                //"WARE_HOUSE_ID, " +
        //                //"WARE_HOUSE_NAME  " +
        //                "FROM VEW_STOCK_QUICK_SEARCH where 1 = 1 ";

        //    if (objQuickSearchModel.ShopId > 0)
        //    {
        //        sql = sql + "and SHOP_ID = '" + objQuickSearchModel.ShopId + "' ";
        //    }

        //    if (objQuickSearchModel.CategoryId > 0)
        //    {
        //        sql = sql + "and CATEGORY_ID = '" + objQuickSearchModel.CategoryId + "' ";
        //    }
        //    if (objQuickSearchModel.SubCategoryId > 0)
        //    {
        //        sql = sql + "and SUB_CATEGORY_ID = '" + objQuickSearchModel.SubCategoryId + "' ";
        //    }
        //    if (objQuickSearchModel.ProductStyle != null)
        //    {
        //        sql = sql + "and PRODUCT_STYLE = '" + objQuickSearchModel.ProductStyle + "' ";
        //    }
        //    if (objQuickSearchModel.MerchandiserId != null)
        //    {
        //        sql = sql + "and MERCHANDISER_ID = '" + objQuickSearchModel.MerchandiserId + "' ";
        //    }
        //    if (objQuickSearchModel.DesignerId != null)
        //    {
        //        sql = sql + "and DESIGNER_ID = '" + objQuickSearchModel.DesignerId + "' ";
        //    }

        //    using (OracleConnection objConnection = GetConnection())
        //    {

        //        using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
        //        {
        //            //objCommand.Parameters.Add(":PURCHASE_RECEIVE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = purchaseReceiveId;

        //            await objConnection.OpenAsync();
        //            using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
        //            {
        //                List<QuickSearchModelDataTable> objQuickSearchModelDataTable = new List<QuickSearchModelDataTable>();
        //                try
        //                {
        //                    while (await objDataReader.ReadAsync())
        //                    {
        //                        QuickSearchModelDataTable model = new QuickSearchModelDataTable
        //                        {

        //                            ItemName = objDataReader["ITEM_NAME"].ToString(),
        //                            Barcode = objDataReader["BARCODE"].ToString(),
        //                            CategoryName = objDataReader["PRODUCT_CATEGORY"].ToString(),
        //                            SubCategoryName = objDataReader["PRODUCT_SUB_CATEGORY"].ToString(),
        //                            ShopName = objDataReader["SHOP_NAME"].ToString(),              
        //                            ProductStyle = objDataReader["PRODUCT_STYLE"].ToString(),
        //                            PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString()),
        //                            SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString()),
        //                            Vat = Convert.ToDouble(objDataReader["VAT"].ToString()),
        //                            Stock = Convert.ToInt32(objDataReader["QUANTITY"].ToString()),
        //                        };
        //                        objQuickSearchModelDataTable.Add(model);
        //                    }
        //                    return objQuickSearchModelDataTable;
        //                }
        //                catch (Exception ex)
        //                {
        //                    throw new Exception("Error : " + ex.Message);
        //                }
        //                finally
        //                {
        //                    objDataReader.Dispose();
        //                    objCommand.Dispose();
        //                    objConnection.Dispose();
        //                }
        //            }


        //        }
        //    }
        //}

        public async Task<DataTableQuickSearch> GetAllProductList(DataTableQuickSearch objDataTableQuickSearch)
        {
            int totalRow = 0;
            string sql = "SELECT COUNT(*) COUNT_OF_ROW FROM VEW_STOCK_SEARCH_DATATABLE WHERE 1=1 ";

            var query = "Select " +
                        "ROWNUM RN, " +
                        "ITEMID, " +
                        "PRODUCTID, " +
                        "ITEMNAME, " +
                        "PRODUCTSTYLE, " +
                        "BARCODE, " +
                        "QUANTITY, " +
                        "PURCHASEPRICE, " +
                        "SALEPRICE, " +
                        "VAT, " +
                        "CATEGORYID, " +
                        "PRODUCTCATEGORY, " +
                        "SUBCATEGORYID, " +
                        "PRODUCTSUBCATEGORY, " +
                        "MERCHANDISERID, " +
                        "DESIGNERID, " +
                        "SHOPID, " +
                        "SHOPNAME  " +
                        "from VEW_STOCK_SEARCH_DATATABLE WHERE 1=1 ";

            
            if (objDataTableQuickSearch.QuickSearchModel.ShopId > 0)
            {
                sql = sql + "and SHOPID = '" + objDataTableQuickSearch.QuickSearchModel.ShopId + "' ";
                query = query + "and SHOPID = '" + objDataTableQuickSearch.QuickSearchModel.ShopId + "' ";
            }

            if (objDataTableQuickSearch.QuickSearchModel.CategoryId > 0)
            {
                sql = sql + "and CATEGORYID = '" + objDataTableQuickSearch.QuickSearchModel.CategoryId + "' ";
                query = query + "and CATEGORYID = '" + objDataTableQuickSearch.QuickSearchModel.CategoryId + "' ";
            }
            if (objDataTableQuickSearch.QuickSearchModel.SubCategoryId > 0)
            {
                sql = sql + "and SUBCATEGORYID = '" + objDataTableQuickSearch.QuickSearchModel.SubCategoryId + "' ";
                query = query + "and SUBCATEGORYID = '" + objDataTableQuickSearch.QuickSearchModel.SubCategoryId + "' ";
            }
            if (objDataTableQuickSearch.QuickSearchModel.ProductStyle != null)
            {
                sql = sql + "and PRODUCTSTYLE = '" + objDataTableQuickSearch.QuickSearchModel.ProductStyle + "' ";
                query = query + "and PRODUCTSTYLE = '" + objDataTableQuickSearch.QuickSearchModel.ProductStyle + "' ";
            }
            if (objDataTableQuickSearch.QuickSearchModel.MerchandiserId != null)
            {
                sql = sql + "and MERCHANDISERID = '" + objDataTableQuickSearch.QuickSearchModel.MerchandiserId + "' ";
                query = query + "and MERCHANDISERID = '" + objDataTableQuickSearch.QuickSearchModel.MerchandiserId + "' ";
            }
            if (objDataTableQuickSearch.QuickSearchModel.DesignerId != null)
            {
                sql = sql + "and DESIGNERID = '" + objDataTableQuickSearch.QuickSearchModel.DesignerId + "' ";
                query = query + "and DESIGNERID = '" + objDataTableQuickSearch.QuickSearchModel.DesignerId + "' ";
            }

            if (!string.IsNullOrWhiteSpace(objDataTableQuickSearch.search.value))
            {
                query +=
                    "and ( (lower(ITEMNAME) like lower(:SearchBy)  or upper(ITEMNAME)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTSTYLE) like lower(:SearchBy)  or upper(PRODUCTSTYLE)like upper(:SearchBy) )" +
                    "or (lower(BARCODE) like lower(:SearchBy)  or upper(BARCODE)like upper(:SearchBy) )" +
                    "or (lower(PURCHASEPRICE) like lower(:SearchBy)  or upper(PURCHASEPRICE)like upper(:SearchBy) )" +
                    "or (lower(SALEPRICE) like lower(:SearchBy)  or upper(SALEPRICE)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTCATEGORY) like lower(:SearchBy)  or upper(PRODUCTCATEGORY)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTSUBCATEGORY) like lower(:SearchBy)  or upper(PRODUCTSUBCATEGORY)like upper(:SearchBy) )" +
                    "or (lower(SHOPNAME) like lower(:SearchBy)  or upper(SHOPNAME)like upper(:SearchBy) )" +
                    "or (lower(VAT) like lower(:SearchBy)  or upper(VAT)like upper(:SearchBy) ) )";

                sql +=
                    "and ( (lower(ITEMNAME) like lower(:SearchBy)  or upper(ITEMNAME)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTSTYLE) like lower(:SearchBy)  or upper(PRODUCTSTYLE)like upper(:SearchBy) )" +
                    "or (lower(BARCODE) like lower(:SearchBy)  or upper(BARCODE)like upper(:SearchBy) )" +
                    "or (lower(PURCHASEPRICE) like lower(:SearchBy)  or upper(PURCHASEPRICE)like upper(:SearchBy) )" +
                    "or (lower(SALEPRICE) like lower(:SearchBy)  or upper(SALEPRICE)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTCATEGORY) like lower(:SearchBy)  or upper(PRODUCTCATEGORY)like upper(:SearchBy) )" +
                    "or (lower(PRODUCTSUBCATEGORY) like lower(:SearchBy)  or upper(PRODUCTSUBCATEGORY)like upper(:SearchBy) )" +
                    "or (lower(SHOPNAME) like lower(:SearchBy)  or upper(SHOPNAME)like upper(:SearchBy) )" +
                    "or (lower(VAT) like lower(:SearchBy)  or upper(VAT)like upper(:SearchBy) ) )";
            }
            if (objDataTableQuickSearch.order != null)
            {
                query += "ORDER BY " + objDataTableQuickSearch.columns[objDataTableQuickSearch.order[0].column].data + " " +
                         objDataTableQuickSearch.order[0].dir.ToUpper();
            }

            query = "SELECT * FROM (" + query + ") WHERE RN BETWEEN  '" + objDataTableQuickSearch.start + "' AND '" +
                    (objDataTableQuickSearch.start + objDataTableQuickSearch.length) + "' ";



            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) {CommandType = CommandType.Text})
                {
                    objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDataTableQuickSearch.search.value) ? $"%{objDataTableQuickSearch.search.value}%" : null;

                    await objConnection.OpenAsync();

                    using (OracleDataReader objDataReader = (OracleDataReader) await objCommand.ExecuteReaderAsync())
                    {
                        int recordCount = 0;
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                recordCount = Convert.ToInt32(objDataReader["COUNT_OF_ROW"].ToString());
                            }
                            totalRow = recordCount;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error : " + e.Message);
                        }
                        finally
                        {
                            objDataReader.Dispose();
                            objCommand.Dispose();
                            objConnection.Close();
                        }
                    }
                }
                using (OracleCommand objCommand = new OracleCommand(query, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDataTableQuickSearch.search.value) ? $"%{objDataTableQuickSearch.search.value}%" : null;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<QuickSearchModelDataTable> objQuickSearchModelDataTable = new List<QuickSearchModelDataTable>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                QuickSearchModelDataTable model = new QuickSearchModelDataTable
                                {
                                    ItemName = objDataReader["ITEMNAME"].ToString(),
                                    Barcode = objDataReader["BARCODE"].ToString(),
                                    CategoryName = objDataReader["PRODUCTCATEGORY"].ToString(),
                                    SubCategoryName = objDataReader["PRODUCTSUBCATEGORY"].ToString(),
                                    ShopName = objDataReader["SHOPNAME"].ToString(),
                                    ProductStyle = objDataReader["PRODUCTSTYLE"].ToString(),
                                    PurchasePrice = Convert.ToDouble(objDataReader["PURCHASEPRICE"].ToString()),
                                    SalePrice = Convert.ToDouble(objDataReader["SALEPRICE"].ToString()),
                                    Vat = Convert.ToDouble(objDataReader["VAT"].ToString()),
                                    Stock = Convert.ToInt32(objDataReader["QUANTITY"].ToString())
                                };
                                objQuickSearchModelDataTable.Add(model);
                            }

                            objDataTableQuickSearch.ListOfData = objQuickSearchModelDataTable.ToList();
                            objDataTableQuickSearch.recordsTotal = totalRow;
                            objDataTableQuickSearch.recordsFiltered = totalRow;
                            return objDataTableQuickSearch;
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

        public async Task<DataTablesSaleCustomerSearch> GetAllSaleCustomerList(DataTablesSaleCustomerSearch objDataTablesSaleCustomerSearch)
        {
            int totalRow = 0;
            string sql = "SELECT COUNT(*) COUNT_OF_ROW FROM CUSTOMER_SALE WHERE 1=1 ";

            var query = @"SELECT ROWNUM RN, CUSTOMER_ID,
                          CUSTOMER_CODE,
                          CUSTOMER_NAME,
                          CONTACT_NO,
                          EMAIL,
                          ADDRESS,
                          DISCOUNT,
                          TO_CHAR (ENROLMENT_DATE, 'dd/mm/yyyy') ENROLMENT_DATE,
                          CUSTOMER_INFO_ID,
                          CUSTOMER_AUTO_ID,
                          SHOP_ID,
                          SHOP_NAME
                          FROM VEW_SALE_CUSTOMER_INFO s WHERE 1 = 1  ";

            query = query.Replace(System.Environment.NewLine, string.Empty);

            if (!string.IsNullOrWhiteSpace(objDataTablesSaleCustomerSearch.search.value))
            {
                query +=
                    "and ( (lower(CUSTOMER_CODE) like lower(:SearchBy)  or upper(CUSTOMER_CODE)like upper(:SearchBy) )" +
                    "or (lower(CUSTOMER_NAME) like lower(:SearchBy)  or upper(CUSTOMER_NAME)like upper(:SearchBy) )" +
                    "or (lower(CONTACT_NO) like lower(:SearchBy)  or upper(CONTACT_NO)like upper(:SearchBy) )" +
                    "or (lower(EMAIL) like lower(:SearchBy)  or upper(EMAIL)like upper(:SearchBy) )" +
                    "or (lower(ADDRESS) like lower(:SearchBy)  or upper(ADDRESS)like upper(:SearchBy) )" +
                    "or (lower(DISCOUNT) like lower(:SearchBy)  or upper(DISCOUNT)like upper(:SearchBy) )" +
                    "or (lower(ENROLMENT_DATE) like lower(:SearchBy)  or upper(ENROLMENT_DATE)like upper(:SearchBy) )" +
                    "or (lower(SHOP_NAME) like lower(:SearchBy)  or upper(SHOP_NAME)like upper(:SearchBy) ) )";
            }
            if (objDataTablesSaleCustomerSearch.order != null)
            {
                query += "ORDER BY " + objDataTablesSaleCustomerSearch.columns[objDataTablesSaleCustomerSearch.order[0].column].data + " " +
                         objDataTablesSaleCustomerSearch.order[0].dir.ToUpper();
            }
            query = "SELECT * FROM (" + query + ") WHERE RN BETWEEN  '" + objDataTablesSaleCustomerSearch.start + "' AND '" +
                    (objDataTablesSaleCustomerSearch.start + objDataTablesSaleCustomerSearch.length) + "'  ORDER BY ENROLMENT_DATE DESC  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDataTablesSaleCustomerSearch.search.value) ? $"%{objDataTablesSaleCustomerSearch.search.value}%" : null;

                    await objConnection.OpenAsync();

                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        int recordCount = 0;
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                recordCount = Convert.ToInt32(objDataReader["COUNT_OF_ROW"].ToString());
                            }
                            totalRow = recordCount;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Error : " + e.Message);
                        }
                        finally
                        {
                            objDataReader.Dispose();
                            objCommand.Dispose();
                            objConnection.Close();
                        }
                    }
                }
                using (OracleCommand objCommand = new OracleCommand(query, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":SearchBy", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objDataTablesSaleCustomerSearch.search.value) ? $"%{objDataTablesSaleCustomerSearch.search.value}%" : null;

                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SaleCustomerSearchModelDataTable> objSaleCustomerSearchModelDataTable = new List<SaleCustomerSearchModelDataTable>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SaleCustomerSearchModelDataTable model = new SaleCustomerSearchModelDataTable
                                {
                                    CUSTOMER_CODE = objDataReader["CUSTOMER_CODE"].ToString(),
                                    CUSTOMER_NAME = objDataReader["CUSTOMER_NAME"].ToString(),
                                    CONTACT_NO = objDataReader["CONTACT_NO"].ToString(),
                                    EMAIL = objDataReader["EMAIL"].ToString(),
                                    SHOP_NAME = objDataReader["SHOP_NAME"].ToString(),
                                    ENROLMENT_DATE = objDataReader["ENROLMENT_DATE"].ToString(),
                                    ADDRESS = objDataReader["ADDRESS"].ToString(),
                                    DISCOUNT = objDataReader["DISCOUNT"].ToString()

                                };
                                objSaleCustomerSearchModelDataTable.Add(model);
                            }

                            objDataTablesSaleCustomerSearch.ListOfData = objSaleCustomerSearchModelDataTable.ToList();
                            objDataTablesSaleCustomerSearch.recordsTotal = totalRow;
                            objDataTablesSaleCustomerSearch.recordsFiltered = totalRow;
                            return objDataTablesSaleCustomerSearch;
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