using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;
using PosWarehouse.ViewModel.ExcelReport;

namespace PosWarehouse.DAL
{
    public class ReportDal
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

        public async Task<DataSet> BarcodePrint()
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                                "PRINTED_BARCODE_ID," +
                                "ITEM_CODE," +
                                "ITEM_NAME," +
                                "BRAND_NAME," +
                                "BARCODE_IMAGE," +
                                "QUANTITY," +
                                "SALE_PRICE," +
                                "UPDATE_BY," +
                                "WARE_HOUSE_ID," +
                                "SHOP_ID " +
                                "from VEW_PRINTED_BARCODE where 1=1 ";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_PRINTED_BARCODE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> GiftVoucherGenerateCodePrint(string giftVoucherId)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT  * from VEW_RPT_GIFTVOU_CODE_FRINT WHERE GIFT_VOUCHER_ID = '" + giftVoucherId.Trim() + "' ";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_GIFTVOU_CODE_FRINT"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SaleDetailsSummary(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " '  Sale Details Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                           " SHOP_NAME, " +
                    " WARE_HOUSE_NAME, " +
                    " WARE_HOUSE_ADDRESS, " +
                    " SALE_ITEM_ID, " +
                    " SALE_INFO_ID, " +
                    " BARCODE, " +
                    " STYLE_NAME, " +
                    " PRICE, " +
                    " QUANTITY, " +
                    " VAT, " +
                    " TOTAL, " +
                    " ITEM_ID, " +
                    " PRODUCT_ID, " +
                    " INVOICE_NO, " +
                    " INVOICE_DATE, " +
                    "  SHOP_ID, " +
                    " WAREHOUSE_ID, " +
                    "  PURCHASE_PRICE, " +
                    "  DISCOUNT_P, " +
                    "  DISCOUNT_A, " +
                    "  NET_AMOUNT " +
                      " From vew_rpt_sale_detail where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                   
                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "vew_rpt_sale_detail");
                            await Task.Run(() => objDataAdapter.Fill(ds, "vew_rpt_sale_detail"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> SaleSummary(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                        " '  Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +                  
                        "SHOP_ID," +
                        "SHOP_NAME," +
                        "SHOP_ADDRESS," +
                        "WAREHOUSE_ID," +
                        "WAREHOUSE_NAME," +
                        "WAREHOUSE_ADDRESS," +
                        "INVOICE_NUMBER," +
                        "INVOICE_DATE," +
                        "TOTAL_ITEM," +
                        "TOTAL_AMOUNT," +
                        "EXG_RTN_AMOUNT," +
                        "ACT_AMOUNT, " +
                        "VAT," +
                        "DISCOUNT_P, " +
                        "DISCOUNT_A, " +
                        "PRE_DISCOUNT, " +
                        "ACT_DISCOUNT, " +
                        "SUB_TOTAL," +
                        "NET_AMOUNT," +
                        "PAYMENT_TYPE," +
                        "CASHAMOUNT, " +
                        "BKASHAMOUNT, " +
                        "BRACKBANKAMOUNT, " +
                        "ROCKETAMOUNT, " +
                        "DBBLAMOUNT, " +
                        "SCBAMOUNT, " +
                        "SIBLAMOUNT, " +
                        "CITYAMOUNT, " +
                        "EBLAMOUNT, " +
                        "GIFTVOUCHER, " +
                        "SHOPARU, " +
                        "BAGDOOM, " +
                        "PRIYOSHOP, " +
                        "KABLEWALA, " +
                        "DARAZ, " +
                        "OTHOBA, " +
                        "DELIGRAM, " +
                        "BESHIDESHI, " +
                        "AJKERDEAL, " +
                        "AADI, " +
                        "EORANGE, " +
                        "CLOTHO, " +
                        "EVALY, " +
                        "DBBLGATEWAY, " +
                        "CITYGATEWAY, " +
                        "REDX, " +
                        "TOTAL_PAYMENT_TYPE_AMOUNT, " +
                        "PAYMENT_AMOUNT_ALL  " +
                      
                        " From VEW_RPT_SALE_SUMMARY s where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }


                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY"));
                            //objDataAdapter.Fill(ds, "");
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> SaleSummaryDateWise(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    string sql = "";

                    if (saleDetailsSummary.FromDate.Length > 6 && saleDetailsSummary.ToDate.Length > 6 &&
                        saleDetailsSummary.ShopId > 0)
                    {

                         sql = "SELECT " +
                                  " '  Date Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate +
                                  "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate +
                                  "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                  " to_date( '" + saleDetailsSummary.FromDate +
                                  "', 'DD MM, YYYY') || ' to '|| to_date( '" + saleDetailsSummary.ToDate +
                                  "', 'DD MM, YYYY')  PERIOD, " +
                                  "SHOP_ID," +
                                  "SHOP_NAME," +
                                  "SHOP_ADDRESS," +
                                  "WAREHOUSE_ID," +
                                  "WAREHOUSE_NAME," +
                                  "WAREHOUSE_ADDRESS," +
                                  "CENTRAL_BIN_NO," +
                                  "INVOICE_DATE," +
                                  "TOTAL_ITEM," +
                                  "TOTAL_AMOUNT," +
                                  "VAT_AMT," +
                                  "DISCOUNT_AMT," +
                                  "SUB_TOTAL," +
                                  "NET_AMOUNT, " +

                                  " (select sum(NET_AMOUNT) from VEW_RPT_SALE_CTGY_SUBCTGY_WVAT where INVOICE_DATE BETWEEN  to_date('" +
                                  saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                  saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  and SHOP_ID = '" +
                                  saleDetailsSummary.ShopId + "')TOTAL_VALUE_WV, " +

                                  "  (select sum(NET_AMOUNT) from VEW_RPT_SALE_SUMMARY_DATE where INVOICE_DATE BETWEEN  to_date('" +
                                  saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                  saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  and SHOP_ID = '" +
                                  saleDetailsSummary.ShopId + "'  ) -  (select sum(NET_AMOUNT) from VEW_RPT_SALE_CTGY_SUBCTGY_WVAT where INVOICE_DATE BETWEEN  to_date('" +
                                  saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                  saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  and SHOP_ID = '" +
                                  saleDetailsSummary.ShopId + "') REMAIN_AMOUNT " +



                                  " From VEW_RPT_SALE_SUMMARY_DATE where INVOICE_DATE BETWEEN  to_date('" +
                                  saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" +
                                  saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                        if (saleDetailsSummary.ShopId > 0)
                        {
                            sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                        }

                    }
                    else
                    {

                       sql = "SELECT " +
                                 " '  Date Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate +
                                 "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate +
                                 "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                 " to_date( '" + saleDetailsSummary.FromDate +
                                 "', 'DD MM, YYYY') || ' to '|| to_date( '" + saleDetailsSummary.ToDate +
                                 "', 'DD MM, YYYY')  PERIOD, " +
                                 "SHOP_ID," +
                                 "SHOP_NAME," +
                                 "SHOP_ADDRESS," +
                                 "WAREHOUSE_ID," +
                                 "WAREHOUSE_NAME," +
                                 "WAREHOUSE_ADDRESS," +
                                 "CENTRAL_BIN_NO," +
                                 "INVOICE_DATE," +
                                 "TOTAL_ITEM," +
                                 "TOTAL_AMOUNT," +
                                 "VAT_AMT," +
                                 "DISCOUNT_AMT," +
                                 "SUB_TOTAL," +
                                 "NET_AMOUNT, " +

                                 " (select sum(NET_AMOUNT) from VEW_RPT_SALE_CTGY_SUBCTGY_WVAT where INVOICE_DATE BETWEEN  to_date('" +
                                 saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                 saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')   )TOTAL_VALUE_WV, " +

                                 "  (select sum(NET_AMOUNT) from VEW_RPT_SALE_SUMMARY_DATE where INVOICE_DATE BETWEEN  to_date('" +
                                 saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                 saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')   ) - (select sum(NET_AMOUNT) from VEW_RPT_SALE_CTGY_SUBCTGY_WVAT where INVOICE_DATE BETWEEN  to_date('" +
                                 saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND to_date('" +
                                 saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')   )REMAIN_AMOUNT " +


                                 " From VEW_RPT_SALE_SUMMARY_DATE where INVOICE_DATE BETWEEN  to_date('" +
                                 saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" +
                                 saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                        if (saleDetailsSummary.ShopId > 0)
                        {
                            sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                        }

                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet PoNumberWiseDetails(PurchaseOrderReport purchaseOrderReport)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " 'PO Number Wise Details' RPT_TITLE , " +                                
                                "PURCHASE_ORDER_NUMBER," +                  
                                "ITEM_NAME," +
                                "STYLE," +
                                "ITEM_SIZE," +
                                "ITEM_COLOR," +
                                "ITEM_FABRIC," +
                                "FIT," +
                                "ITEM_CODE," +
                                "CATEGORY_NAME," +
                                "SUB_CATEGORY_NAME," +
                                "PRODUCT_NAME," +
                                "PURCHASE_PRICE," +
                                "SALE_PRICE," +
                                "UOM," +
                                "QUANTITY," +
                                "VAT," +                             
                                "VENDOR_NAME," +
                                "ORDER_DATE," +
                                "DELIVERY_DATE," +
                                "SHOP_DISPLAY_DATE," +
                                "SHOP_NAME," +
                                "SEASON_NAME," +
                                "DESIGNER_ID," +
                                "DESIGNER_NAME," +
                                "MERCHANDISER_ID," +
                                "MERCHANDISER_NAME," +
                                "KARCUPI," +
                                "PRINT," +
                                "WASH," +
                                "EMBROIDERY," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS " +
                              " from VEW_RPT_PURCHASE_ORDER_DETAILS where PURCHASE_ORDER_NUMBER='" + purchaseOrderReport.PurchaseOrderNumber + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            strConn.Open();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            objDataAdapter.Fill(ds, "VEW_RPT_PURCHASE_ORDER_DETAILS");
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataSet DateAndShopWiseDetails(PurchaseOrderReport purchaseOrderReport)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " '  Purchase Order Details Report  between ' || to_date( '" + purchaseOrderReport.OrderDatefrom + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + purchaseOrderReport.OrderDateTo + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                "PURCHASE_ORDER_NUMBER," +
                                "ITEM_NAME," +
                                "ITEM_CODE," +
                                "PURCHASE_PRICE," +
                                "SALE_PRICE," +
                                "UOM," +
                                "QUANTITY," +
                                "VAT," +
                                "VENDOR_NAME," +
                                "ORDER_DATE," +
                                "DELIVERY_DATE," +
                                "DELIVERY_SHOP_ID," +
                                "SHOP_NAME," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS " +
                                " From VEW_RPT_PURCHASE_ORDER_DETAILS where ORDER_DATE BETWEEN  to_date('" + purchaseOrderReport.OrderDatefrom.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + purchaseOrderReport.OrderDateTo.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (!string.IsNullOrEmpty(purchaseOrderReport.PurchaseOrderNumber))
                    {
                        sql = sql + "and PURCHASE_ORDER_NUMBER = '" + purchaseOrderReport.PurchaseOrderNumber + "' ";
                    }
                    if (purchaseOrderReport.ShopId > 0)
                    {
                        sql = sql + "and DELIVERY_SHOP_ID = '" + purchaseOrderReport.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            strConn.Open();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            objDataAdapter.Fill(ds, "VEW_RPT_PURCHASE_ORDER_DETAILS");
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> DeliveryNumberWiseDetails(string storeDeliveryNo)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " 'Challan Number Wise Details' RPT_TITLE, " +
                              "STORE_DELIVERY_ITEM_ID," +
                                "STORE_DELIVERY_NUMBER," +
                                "ITEM_ID," +
                                "ITEM_NAME," +
                                "PRODUCT_ID," +
                                "PRODUCT_STYLE," +
                                "BARCODE," +
                                "DELIVERY_QUANTITY," +
                                "PURCHASE_PRICE," +
                                "ACTUAL_PRICE," +
                                "TOTAL_VALUE," +
                                "VAT_AMOUNT," +
                                "SALE_PRICE," +
                                "VAT," +
                                "DELIVERY_SHOP_ID," +
                                "SHOP_NAME," +
                                "REGISTER_ID," +
                                "REGISTER_PERSON_NAME," +
                                "DELIVERY_DATE," +
                                "REQUISTION_NO," +
                                "SEASON_ID," +
                                "SEASON_NAME," +
                                "WARE_HOUSE_ID," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS_DELIRPT,"+
                                "WAREHOUSE_ADDRESS, " +
                                "Org_Name," +
                                "Org_Bin_No," +
                                "SHOP_ADDRESS " +
                              " from VEW_RPT_STORE_DELIVERY_DETAILS where STORE_DELIVERY_NUMBER='" + storeDeliveryNo + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_STORE_DELIVERY_DETAILS");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STORE_DELIVERY_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> ChallanNumberWiseDamageDetails(DamageProductModel damageProductModel)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                                " 'Challan Number Wise Details' RPT_TITLE , " +
                                "DAMAGE_TRANSFER_ITEM_ID," +
                                "DAMAGE_TRANSFER_CHALLAN_NUM," +
                                "CREATED_BY," +
                                "CREATED_DATE," +
                                "DAMAGE_TRANSFER_ID," +
                                "ITEM_ID," +
                                "PRODUCT_ID," +
                                "ITEM_NAME," +
                                "BARCODE," +
                                "TRANSFER_QUANTITY," +
                                "SALE_PRICE," +
                                "REMARKS " +
                              " from VEW_RPT_DAMAGE_DETAILS where DAMAGE_TRANSFER_CHALLAN_NUM='" + damageProductModel.DamageChallanNo + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DAMAGE_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> ChallanNumberWiseReturnReceiveDetails(StockReceiveModel stockReceiveModel)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                                " 'Challan Number Wise Details' RPT_TITLE , " +
                                "RECEIVE_ID," +
                                "RECEIVE_NUMBER," +
                                "CHALLAN_NUM," +
                                "REQUISITION_NUM," +
                                "SHOPID_FROM," +
                                "TR_FROM_SHOP_NAME," +
                                "RECEIVED_BY," +
                                "RECEIVED_DATE," +
                                "ITEM_ID," +
                                "PRODUCT_ID," +
                                "ITEM_NAME," +
                                "BARCODE," +
                                "QUANTITY," +
                                "SALE_PRICE," +
                                "REMARKS " +
                              " from VEW_RPT_STOCK_TR_REC_DETAILS where CHALLAN_NUM='" + stockReceiveModel.StockTransferChallanNo + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_TR_REC_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> PurchaseReceiveNumberWiseDetails(string purchaseReceiveNumber)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              "RPT_TITLE," +
                                "PURCHASE_RECEIVE_ITEM_ID," +
                                "PURCHASE_RECEIVE_NUMBER," +
                                "ITEM_ID," +
                                "ITEM_NAME," +
                                "PRODUCT_ID," +
                                "PRODUCT_STYLE," +
                                "BARCODE," +
                                "ORDER_QUANTITY," +
                                "RECEIVE_QUANTITY," +
                                "PURCHASE_PRICE," +
                                "SALE_PRICE," +
                                "VAT," +
                                "PURCHASE_RECEIVE_ID," +
                                "DELIVERY_SHOP_ID," +
                                "SHOP_NAME," +
                                "PURCHASE_DATE," +
                                "WARE_HOUSE_ID," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS " +
                              " from VEW_RPT_PURCHASE_RECEIVE where PURCHASE_RECEIVE_NUMBER='" + purchaseReceiveNumber + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_PURCHASE_RECEIVE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_PURCHASE_RECEIVE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> OtherPurchaseReceiveNumberWiseDetails(string OtherPurchaseReceiveNumber)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              "RPT_TITLE," +
                              "DELIVERY_SHOP_ID," +
                              "SHOP_NAME," +
                              "CREATE_DATE," +
                              "WARE_HOUSE_ID," +
                              "WAREHOUSE_NAME," +
                              "WAREHOUSE_ADDRESS," +
                              "O_PURCHASE_RECEIVE_ITEM_ID," +
                              "O_PURCHASE_RECEIVE_NUMBER," +
                              "ITEM_ID,ITEM_NAME,PRODUCT_ID," +
                              "PRODUCT_STYLE,BARCODE,RECEIVE_QUANTITY,SALE_PRICE" +
                              " from VEW_RPT_OTHER_PURCHASE_RECEIVE where O_PURCHASE_RECEIVE_NUMBER='" + OtherPurchaseReceiveNumber + "'";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_OTHER_PURCHASE_RECEIVE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockDetailsSummary(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                 sql = "SELECT " +
                          " '  Stock Details Report  ' RPT_TITLE, " +
                          "SHOP_ID,  " +
                          "SHOP_NAME, " +
                          "WARE_HOUSE_NAME, " +
                          "WARE_HOUSE_ADDRESS, " +
                          "PRODUCT_ID, " +
                          "ITEM_ID, " +
                          "ITEM_NAME, " +
                          "BARCODE, " +
                          "DELIVERY_QTY, " +
                          "SHOP_SHOP_EX_QTY, " +
                          "SALE_QTY, " +
                          "SHOP_TRANSFAR_QTY, " +
                          "SHOP_TO_SHOP_RECEIVE_QTY, " +
                          "DAMAGE_QTY, " +
                          "CURRENT_STOCK, " +
                          "PURCHASE_PRICE, " +
                          "SALE_PRICE, " +
                          "COST_VALUE, " +
                          "SALE_VALUE, " +
                          "PRODUCT_CATEGORY, " +
                          "PRODUCT_SUB_CATEGORY  " +
                          " From VEW_RPT_CURRENT_STOCK_SUMMARY WHERE 1=1 ";
                        
                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    if (objStockSummaryReport.SubCategoryId  != null)
                    {
                        sql = sql + "and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockDetailsSummaryWithOutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                             " '  Stock Details Report  ' RPT_TITLE, " +
                             "SHOP_ID,  " +
                             "SHOP_NAME, " +
                             "WARE_HOUSE_NAME, " +
                             "WARE_HOUSE_ADDRESS, " +
                             "PRODUCT_ID, " +
                             "ITEM_ID, " +
                             "ITEM_NAME, " +
                             "BARCODE, " +
                             "DELIVERY_QTY, " +
                             "SHOP_SHOP_EX_QTY, " +
                             "SALE_QTY, " +
                             "SHOP_TRANSFAR_QTY, " +
                             "SHOP_TO_SHOP_RECEIVE_QTY, " +
                             "DAMAGE_QTY, " +
                             "CURRENT_STOCK, " +
                             "PURCHASE_PRICE, " +
                             "SALE_PRICE, " +
                             "COST_VALUE, " +
                             "SALE_VALUE, " +
                             "PRODUCT_CATEGORY, " +
                             "PRODUCT_SUB_CATEGORY  " +
                             " From VEW_RPT_CURRENT_STOCK_SUMMARY WHERE CURRENT_STOCK > 0 ";

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + "and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockDetailsSummaryByBarcode(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                        sql = "SELECT " +
                              " '  Barcode Wise Stock DetailsReport  ' RPT_TITLE, " +
                              "SHOP_ID,  " +
                              "SHOP_NAME,  " +
                              "WARE_HOUSE_NAME, " +
                              "WARE_HOUSE_ADDRESS, " +
                              "PRODUCT_ID, " +
                              "ITEM_ID, " +
                              "ITEM_NAME, " +
                              "BARCODE, " +
                              "DELIVERY_QTY, " +
                              "SHOP_SHOP_EX_QTY ," +
                              "SALE_QTY, " +
                              "SHOP_TRANSFAR_QTY, " +
                              "SHOP_TO_SHOP_RECEIVE_QTY, " +
                              "DAMAGE_QTY, " +
                              "CURRENT_STOCK, " +
                              "PURCHASE_PRICE, " +
                              "SALE_PRICE, " +
                              "COST_VALUE, " +
                              "SALE_VALUE, " +
                              "PRODUCT_CATEGORY, " +
                              "PRODUCT_SUB_CATEGORY  " +
                              " From VEW_RPT_CURRENT_STOCK_SUMMARY WHERE BARCODE = '" + objStockSummaryReport.Barcode.Trim() + "' ";

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }
                  
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockDetailsSummaryByStyleName(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " '  Style Wise Stock DetailsReport  ' RPT_TITLE, " +
                          "SHOP_ID,  " +
                          "SHOP_NAME,  " +
                          "WARE_HOUSE_NAME, " +
                          "WARE_HOUSE_ADDRESS, " +
                          "PRODUCT_ID, " +
                          "ITEM_ID, " +
                          "ITEM_NAME, " +
                          "BARCODE, " +
                          "DELIVERY_QTY, " +
                          "SHOP_SHOP_EX_QTY ," +
                          "SALE_QTY, " +
                          "SHOP_TRANSFAR_QTY, " +
                          "SHOP_TO_SHOP_RECEIVE_QTY, " +
                          "DAMAGE_QTY, " +
                          "CURRENT_STOCK, " +
                          "PURCHASE_PRICE, " +
                          "SALE_PRICE, " +
                          "COST_VALUE, " +
                          "SALE_VALUE, " +
                          "PRODUCT_CATEGORY, " +
                          "PRODUCT_SUB_CATEGORY  " +
                          " From VEW_RPT_CURRENT_STOCK_SUMMARY WHERE 1=1  ";

                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and ((lower(ITEM_NAME) like lower('" + objStockSummaryReport.StyleName.Trim() + " %'))  or upper(ITEM_NAME)like upper('" + objStockSummaryReport.StyleName.Trim() + " %')) ";
                    }
                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public async Task<DataSet> StockDetailsSummaryByStyleNameWithoutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " '  Style Wise Stock DetailsReport  ' RPT_TITLE, " +
                          "SHOP_ID,  " +
                          "SHOP_NAME,  " +
                          "WARE_HOUSE_NAME, " +
                          "WARE_HOUSE_ADDRESS, " +
                          "PRODUCT_ID, " +
                          "ITEM_ID, " +
                          "ITEM_NAME, " +
                          "BARCODE, " +
                          "DELIVERY_QTY, " +
                          "SHOP_SHOP_EX_QTY ," +
                          "SALE_QTY, " +
                          "SHOP_TRANSFAR_QTY, " +
                          "SHOP_TO_SHOP_RECEIVE_QTY, " +
                          "DAMAGE_QTY, " +
                          "CURRENT_STOCK, " +
                          "PURCHASE_PRICE, " +
                          "SALE_PRICE, " +
                          "COST_VALUE, " +
                          "SALE_VALUE, " +
                          "PRODUCT_CATEGORY, " +
                          "PRODUCT_SUB_CATEGORY  " +
                          " From VEW_RPT_CURRENT_STOCK_SUMMARY WHERE CURRENT_STOCK> 0  ";

                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and ((lower(ITEM_NAME) like lower('" + objStockSummaryReport.StyleName.Trim() + " %'))  or upper(ITEM_NAME)like upper('" + objStockSummaryReport.StyleName.Trim() + " %')) ";
                    }
                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SubCategorywiseSymmary(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Sub Category Style Summary Report  ' RPT_TITLE, " +
                            "PRODUCT_CATEGORY," +
                           "PRODUCT_SUB_CATEGORY," +
                           "DELIVERY_QTY," +
                           "SALE_QTY," +
                           "SHOP_TRANSFAR_QTY," +
                           "SHOP_TO_SHOP_RECEIVE_QTY," +
                           "DAMAGE_QTY," +
                           "CURRENT_STOCK" +
                          " From VEW_RPT_STOCK_SUM_BY_CATAGORY WHERE 1=1  ";

                    //if (objStockSummaryReport.ShopId > 0)
                    //{
                    //    sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    //}
                    if (objStockSummaryReport.CategoryId  != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    //}
                    //if (objStockSummaryReport.Barcode != null)
                    //{
                    //    sql = sql + " and BARCODE = '" + objStockSummaryReport.Barcode + "' ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_SUM_BY_CATAGORY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> SubCategorywiseSymmaryWithoutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Sub Category Style Summary Report  ' RPT_TITLE, " +
                            "PRODUCT_CATEGORY," +
                           "PRODUCT_SUB_CATEGORY," +
                           "DELIVERY_QTY," +
                           "SALE_QTY," +
                           "SHOP_TRANSFAR_QTY," +
                           "SHOP_TO_SHOP_RECEIVE_QTY," +
                           "DAMAGE_QTY," +
                           "CURRENT_STOCK" +
                          " From VEW_RPT_STOCK_SUM_BY_CATAGORY WHERE CURRENT_STOCK > 0 ";

                    //if (objStockSummaryReport.ShopId > 0)
                    //{
                    //    sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    //}
                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    //}
                    //if (objStockSummaryReport.Barcode != null)
                    //{
                    //    sql = sql + " and BARCODE = '" + objStockSummaryReport.Barcode + "' ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_SUM_BY_CATAGORY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StockCategorySubcategoryStyleWiseSummary(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Category Sub Category Style Summary Report  ' RPT_TITLE, " +
                            "SHOP_ID," +
                            "SHOP_NAME," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "PRODUCT_STYLE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY," +
                            "DELIVERY_QTY," +
                            "SALE_QTY," +
                            "SHOP_TRANSFAR_QTY," +
                            "SHOP_TO_SHOP_RECEIVE_QTY," +
                            "DAMAGE_QTY," +
                            "CURRENT_STOCK " +
                          " From VEW_RPT_STOCK_STY_SUMMARY WHERE 1=1  ";

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }
                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    }
                    if (objStockSummaryReport.Barcode != null)
                    {
                        sql = sql + " and BARCODE = '" + objStockSummaryReport.Barcode + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_STY_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StockCategorySubcategoryStyleWiseSummaryWithoutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Category Sub Category Style Summary Report  ' RPT_TITLE, " +
                            "SHOP_ID," +
                            "SHOP_NAME," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "PRODUCT_STYLE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY," +
                            "DELIVERY_QTY," +
                            "SALE_QTY," +
                            "SHOP_TRANSFAR_QTY," +
                            "SHOP_TO_SHOP_RECEIVE_QTY," +
                            "DAMAGE_QTY," +
                            "CURRENT_STOCK " +
                          " From VEW_RPT_STOCK_STY_SUMMARY WHERE CURRENT_STOCK > 0  ";

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }
                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    }
                    if (objStockSummaryReport.Barcode != null)
                    {
                        sql = sql + " and BARCODE = '" + objStockSummaryReport.Barcode + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_STY_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockDetailsSummaryByCategory(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                sql = "SELECT " +
                      " ' Category Wise Stock Report ' RPT_TITLE, " +
                      "SHOP_ID," +
                      "SHOP_NAME," +
                      "WARE_HOUSE_NAME," +
                      "WARE_HOUSE_ADDRESS," +
                      "DELIVERY_QTY," +
                      "SHOP_SHOP_EX_QTY, "+
                      "SALE_QTY," +
                      "SHOP_TRANSFAR_QTY," +
                      "SHOP_TO_SHOP_RECEIVE_QTY," +
                      "DAMAGE_QTY," +
                      "CURRENT_STOCK," +
                      "COST_VALUE," +
                      "SALE_VALUE," +
                      "PRODUCT_CATEGORY " +
                      " From VEW_RPT_CURRENT_STOCK_CATEGORY WHERE 1 = 1 ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + "and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_CATEGORY"));
                            //objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_CATEGORY");
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StockDetailsSummaryByCategoryWithOutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Category Wise Stock Report ' RPT_TITLE, " +
                          "SHOP_ID," +
                          "SHOP_NAME," +
                          "WARE_HOUSE_NAME," +
                          "WARE_HOUSE_ADDRESS," +
                          "DELIVERY_QTY," +
                          "SHOP_SHOP_EX_QTY, " +
                          "SALE_QTY," +
                          "SHOP_TRANSFAR_QTY," +
                          "SHOP_TO_SHOP_RECEIVE_QTY," +
                          "DAMAGE_QTY," +
                          "CURRENT_STOCK," +
                          "COST_VALUE," +
                          "SALE_VALUE," +
                          "PRODUCT_CATEGORY " +
                          " From VEW_RPT_CURRENT_STOCK_CATEGORY WHERE CURRENT_STOCK > 0  ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + "and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_CATEGORY"));
                            //objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_CATEGORY");
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StockDetailsSummaryBySubCategory(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Sub Category Wise Stock Report ' RPT_TITLE, " +
                          "SHOP_ID," +
                          "SHOP_NAME," +
                          "WARE_HOUSE_NAME," +
                          "WARE_HOUSE_ADDRESS," +
                          "DELIVERY_QTY," +
                          "SHOP_SHOP_EX_QTY," +
                          "SALE_QTY," +
                          "SHOP_TRANSFAR_QTY," +
                          "SHOP_TO_SHOP_RECEIVE_QTY," +
                          "DAMAGE_QTY," +
                          "CURRENT_STOCK," +
                          "COST_VALUE," +
                          "SALE_VALUE," +
                          "PRODUCT_SUB_CATEGORY " +
                          " From VEW_RPT_CURRENT_STOCK_SUB_CTGY WHERE 1 = 1 ";

                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + "and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUB_CTGY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StockDetailsSummaryBySubCategoryWithOutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Sub Category Wise Stock Report ' RPT_TITLE, " +
                          "SHOP_ID," +
                          "SHOP_NAME," +
                          "WARE_HOUSE_NAME," +
                          "WARE_HOUSE_ADDRESS," +
                          "DELIVERY_QTY," +
                          "SHOP_SHOP_EX_QTY," +
                          "SALE_QTY," +
                          "SHOP_TRANSFAR_QTY," +
                          "SHOP_TO_SHOP_RECEIVE_QTY," +
                          "DAMAGE_QTY," +
                          "CURRENT_STOCK," +
                          "COST_VALUE," +
                          "SALE_VALUE," +
                          "PRODUCT_SUB_CATEGORY " +
                          " From VEW_RPT_CURRENT_STOCK_SUB_CTGY WHERE CURRENT_STOCK > 0 ";

                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + "and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }

                    if (objStockSummaryReport.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    }

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CURRENT_STOCK_SUB_CTGY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> DatewiseStoreDeliveryChallanDetails(StoreDeliveryReport storeDeliveryReport)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' DateWise Delivery Challan Details Report  between ' || to_date( '" + storeDeliveryReport.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + storeDeliveryReport.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                               "RPT_TITLE," +
                                "DELIVERY_SHOP_ID," +
                                "DELIVERY_SHOP_NAME," +
                                "WARE_HOUSE_ID," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "CENTRAL_BIN_NO," +
                                "STORE_DELIVERY_ID," +
                                "DELIVERY_DATE," +
                                "STORE_DELIVERY_NUMBER," +
                                "DELIVERY_QUANTITY  " +
                                " From VEW_RPT_SHOP_DELIVERY_DATEWISE where DELIVERY_DATE BETWEEN  to_date('" + storeDeliveryReport.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + storeDeliveryReport.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (!string.IsNullOrEmpty(storeDeliveryReport.DeliveryNumber))
                    {
                        sql = sql + "and STORE_DELIVERY_NUMBER = '" + storeDeliveryReport.DeliveryNumber + "' ";
                    }
                    if (storeDeliveryReport.ShopId > 0)
                    {
                        sql = sql + "and DELIVERY_SHOP_ID = '" + storeDeliveryReport.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_PURCHASE_ORDER_DETAILS");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SHOP_DELIVERY_DATEWISE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StylewiseStoreDeliveryChallanDetails(StoreDeliveryReport storeDeliveryReport)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                               " ' Style Wise Shop Delivery'  RPT_TITLE, " +
                                "WARE_HOUSE_ID," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "STORE_DELIVERY_NUMBER," +
                                "PRODUCT_ID," +
                                "PRODUCT_STYLE," +
                                "DELIVERY_QUANTITY," +
                                "DELIVERY_SHOP_ID," +
                                "DELIVERY_SHOP_NAME," +
                                "DELIVERY_DATE," +
                                "REQUISTION_NO," +
                                "SEASON_ID," +
                                "SEASON_NAME  " +
                                " From VEW_RPT_STYL_BARCODE_DELIVERY where  1= 1 ";

                    if (!string.IsNullOrEmpty(storeDeliveryReport.DeliveryNumber))
                    {
                        sql = sql + "and STORE_DELIVERY_NUMBER = '" + storeDeliveryReport.DeliveryNumber + "' ";
                    }
                    if (!string.IsNullOrEmpty(storeDeliveryReport.ProductStyle))
                    {
                        sql = sql + "and PRODUCT_STYLE = '" + storeDeliveryReport.ProductStyle + "' ";
                    }
                    if (storeDeliveryReport.ShopId > 0)
                    {
                        sql = sql + "and DELIVERY_SHOP_ID = '" + storeDeliveryReport.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STYL_BARCODE_DELIVERY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> SaleSummaryCategorySubCategoryWiseWVat(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {


                        sql = "SELECT " +
                              " '  Category And SubCategory Sale Summary Report between ' || to_date( '" +
                              saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" +
                              saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_NAME," +
                              "WAREHOUSE_ADDRESS," +
                              "SHOP_ID, " +
                              "SHOP_NAME," +
                              "SHOP_ADDRESS," +
                              "CATEGORY_ID," +
                              "CATEGORY_NAME," +
                              "SUB_CATEGORY_ID," +
                              "SUB_CATEGORY_NAME," +
                              "INVOICE_DATE," +
                              "QUANTITY," +
                              "SUB_TOTAL," +
                              "DISCOUNT_AMOUNT," +
                              "NET_AMOUNT " +
                             

                              " From VEW_RPT_SALE_CTGY_SUBCTGY_WVAT where INVOICE_DATE BETWEEN  to_date('" +
                              saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" +
                              saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                        if (saleDetailsSummary.ShopId > 0)
                        {
                            sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                        }

                        if (saleDetailsSummary.CategoryId != null)
                        {
                            sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                        }

                        if (saleDetailsSummary.SubCategoryId != null)
                        {
                            sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                        }

                  

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_CTGY_SUBCTGY_WVAT"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> SaleSummaryCategorySubCategoryWise(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                 sql = "SELECT " +
                          " '  Category And SubCategory Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                          "WAREHOUSE_NAME," +
                          "WAREHOUSE_ADDRESS," +
                          "SHOP_ID, " +
                          "SHOP_NAME," +
                          "SHOP_ADDRESS," +
                          "CATEGORY_ID," +
                          "CATEGORY_NAME," +
                          "SUB_CATEGORY_ID," +
                          "SUB_CATEGORY_NAME," +
                          "INVOICE_DATE," +
                          "QUANTITY," +
                          "SUB_TOTAL," +
                          "DISCOUNT_AMOUNT," +
                          "NET_AMOUNT," +
                          "VAT_AMOUNT " +
                          " From VEW_RPT_SALE_CTGY_SUBCTGY where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    if (saleDetailsSummary.CategoryId  != null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                           await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_CTGY_SUBCTGY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> AllShopSaleSummaryCategorySubCategoryWise(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                             " ' All Shop  Category And SubCategory Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                             "CATEGORY_ID," +
                            "CATEGORY_NAME," +
                            "SUB_CATEGORY_ID," +
                            "SUB_CATEGORY_NAME," +
                            "QUANTITY," +
                            "SUB_TOTAL," +
                            "DISCOUNT_AMOUNT," +
                            "NET_AMOUNT," +
                            "VAT_AMOUNT " +
                             " From vew_rpt_sale_sum_cs where 1=1 ";

                   
                    if (saleDetailsSummary.CategoryId != null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "vew_rpt_sale_sum_cs"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SaleCategoryWiseDetails(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    if (saleDetailsSummary.ShopId == 0)
                    {
                        sql = "SELECT " +
                              " '  Category Wise Details Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_ID," +
                              "WAREHOUSE_NAME," +
                              "WAREHOUSE_ADDRESS," +
                              "SHOP_ID," +
                              "SHOP_NAME," +
                              "SHOP_ADDRESS," +
                              "'ALL SHOP 'SHOP_NAME_HEADER," +
                              "INVOICE_DATE," +
                              "PRODUCT_ID,"+
                              "ORGINAL_PRODUCT_STYLE," +
                              "BARCODE," +
                              "STYLE_NAME," +
                              "CATEGORY_ID," +
                              "CATEGORY_NAME," +
                              "PURCHASE_PRICE," +
                              "MATERIAL_COST, " +
                              "CM, " +
                              "PRICE," +
                              "QUANTITY," +
                              "TOTAL," +
                              "DISCOUNT_P," +
                              "DISCOUNT_AMOUNT," +
                              "DIS_PRICE," +
                              "NET_AMOUNT," +
                              "VAT_AMOUNT " +
                              " From VEW_RPT_SALE_CTGY_DETAILS where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    }
                    else
                    {
                        sql = "SELECT " +
                              " '  Category Wise Details Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_ID," +
                              "WAREHOUSE_NAME," +
                              "WAREHOUSE_ADDRESS," +
                              "SHOP_ID," +
                              "SHOP_NAME," +
                              "SHOP_ADDRESS," +
                              "SHOP_NAME_HEADER," +
                              "SHOP_ADDRESS_HEADER," +
                              "INVOICE_DATE," +
                              "PRODUCT_ID," +
                              "ORGINAL_PRODUCT_STYLE," +
                              "BARCODE," +
                              "STYLE_NAME," +
                              "CATEGORY_ID," +
                              "CATEGORY_NAME," +
                              "PURCHASE_PRICE," +
                              "MATERIAL_COST, " +
                              "CM, " +
                              "PRICE," +
                              "QUANTITY," +
                              "TOTAL," +
                              "DISCOUNT_P," +
                              "DISCOUNT_AMOUNT," +
                              "DIS_PRICE," +
                              "NET_AMOUNT," +
                              "VAT_AMOUNT " +
                              " From VEW_RPT_SALE_CTGY_DETAILS where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    }

                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    if (saleDetailsSummary.CategoryId != null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }
                    if (saleDetailsSummary.ProductId > 0)
                    {
                        sql = sql + "and PRODUCT_ID = '" + saleDetailsSummary.ProductId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            //objDataAdapter.Fill(ds, "VEW_RPT_SALE_SUMMARY_DATE");
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_CTGY_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StyleWiseWarehouseStockSummary(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Warehouse Stock Summary Report  ' RPT_TITLE, " +
                            " WARE_HOUSE_ID," +
                            " WARE_HOUSE_NAME," +
                            " WARE_HOUSE_ADDRESS," +
                            " PRODUCT_ID," +
                            " RECEIVE_QUANTITY," +
                            " RECEIVE_FROM_SHOP," +
                            " SHOP_DELIVERY," +
                            " DAMAGED_QUANTITY," +
                            " QUANTITY," +
                            " PRODUCT_STYLE," +
                            " PRODUCT_CATEGORY," +
                            " PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_STY_SUMMARY WHERE 1=1  ";
                   
                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.ProductId > 0 )
                    {
                        sql = sql + " and PRODUCT_ID = '" + objStockSummaryReport.ProductId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_STY_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StyleWiseWarehouseStockSummaryWithoutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Warehouse Stock Summary Report  ' RPT_TITLE, " +
                            " WARE_HOUSE_ID," +
                            " WARE_HOUSE_NAME," +
                            " WARE_HOUSE_ADDRESS," +
                            " PRODUCT_ID," +
                            " RECEIVE_QUANTITY," +
                            " RECEIVE_FROM_SHOP," +
                            " SHOP_DELIVERY," +
                            " DAMAGED_QUANTITY," +
                            " QUANTITY," +
                            " PRODUCT_STYLE," +
                            " PRODUCT_CATEGORY," +
                            " PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_STY_SUMMARY WHERE QUANTITY > 0  ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.ProductId > 0)
                    {
                        sql = sql + " and PRODUCT_ID = '" + objStockSummaryReport.ProductId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_STY_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StyleWiseWarehouseStockDetails(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Warehouse Stock Details Report  ' RPT_TITLE, " +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "RECEIVE_QUANTITY," +
                            "RECEIVE_FROM_SHOP," +
                            "SHOP_DELIVERY," +
                            "DAMAGED_QUANTITY," +
                            "QUANTITY," +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "BARCODE," +
                            "ITEM_NAME," +
                            "PURCHASE_PRICE," +
                            "SALE_PRICE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_DETAILS WHERE 1=1  ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.ProductId > 0 )
                    {
                        sql = sql + " and PRODUCT_ID = '" + objStockSummaryReport.ProductId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ORDER BY ITEM_ID ASC  ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DataSet> StyleWiseWarehouseStockDetailsWithoutZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Warehouse Stock Details Report  ' RPT_TITLE, " +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "RECEIVE_QUANTITY," +
                            "RECEIVE_FROM_SHOP," +
                            "SHOP_DELIVERY," +
                            "DAMAGED_QUANTITY," +
                            "QUANTITY," +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "BARCODE," +
                            "ITEM_NAME," +
                            "PURCHASE_PRICE," +
                            "SALE_PRICE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_DETAILS WHERE QUANTITY >0   ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.ProductId > 0)
                    {
                        sql = sql + " and PRODUCT_ID = '" + objStockSummaryReport.ProductId + "' ";
                    }
                    //if (objStockSummaryReport.StyleName != null)
                    //{
                    //    sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ORDER BY ITEM_ID ASC  ";
                    //}
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> ShopWiseLowStock(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Shop Low Stock Report  ' RPT_TITLE, " +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "ITEM_NAME," +
                            "BARCODE," +
                            "CURRENT_STOCK," +
                            "SALE_PRICE," +
                            "SHOP_ID," +
                            "SHOP_NAME," +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WAREHOUSE_ADDRESS  " +
                            " From VEW_RPT_SHOP_LOWSTOCK WHERE SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SHOP_LOWSTOCK"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> ShopStyleWiseLowStock(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Shop Low Stock Report  ' RPT_TITLE, " +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "ITEM_NAME," +
                            "BARCODE," +
                            "CURRENT_STOCK," +
                            "SALE_PRICE," +
                            "SHOP_ID," +
                            "SHOP_NAME," +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WAREHOUSE_ADDRESS  " +
                            " From VEW_RPT_SHOP_LOWSTOCK sm  WHERE SHOP_ID = '" + objStockSummaryReport.ShopId + "' AND  TRUNC (SYSDATE) = " +
                            " (SELECT invoice_date FROM vew_date_search_for_stock WHERE PRODUCT_ID = sm.product_id AND ROWNUM = 1)  ";

                    //if (objStockSummaryReport.ShopId > 0)
                    //{
                    //    sql = sql + " and SHOP_ID = '" + objStockSummaryReport.ShopId + "' ";
                    //}
                    if (objStockSummaryReport.ProductId > 0)
                    {
                        sql = sql + " and PRODUCT_ID = '" + objStockSummaryReport.ProductId + "' ";
                    }
                    
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SHOP_LOWSTOCK"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StyleWiseWarehouseHistory(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Style History Report  ' RPT_TITLE, " +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "RECEIVE_QUANTITY," +
                            "RECEIVE_FROM_SHOP," +
                            "SHOP_DELIVERY," +
                            "DAMAGED_QUANTITY," +
                            "QUANTITY," +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "BARCODE," +
                            "ITEM_NAME," +
                            "PURCHASE_PRICE," +
                            "SALE_PRICE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_DETAILS WHERE 1=1  ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ORDER BY ITEM_ID ASC  ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StyleWiseWarehouseHistoryWithZero(StockSummaryReport objStockSummaryReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Style History Report  ' RPT_TITLE, " +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "RECEIVE_QUANTITY," +
                            "RECEIVE_FROM_SHOP," +
                            "SHOP_DELIVERY," +
                            "DAMAGED_QUANTITY," +
                            "QUANTITY," +
                            "PRODUCT_ID," +
                            "ITEM_ID," +
                            "PRODUCT_STYLE," +
                            "BARCODE," +
                            "ITEM_NAME," +
                            "PURCHASE_PRICE," +
                            "SALE_PRICE," +
                            "PRODUCT_CATEGORY," +
                            "PRODUCT_SUB_CATEGORY " +
                            " From VEW_RPT_DC_STOCK_DETAILS WHERE QUANTITY > 0  ";

                    if (objStockSummaryReport.CategoryId != null)
                    {
                        sql = sql + " and PRODUCT_CATEGORY = '" + objStockSummaryReport.CategoryId + "' ";
                    }
                    if (objStockSummaryReport.SubCategoryId != null)
                    {
                        sql = sql + " and PRODUCT_SUB_CATEGORY = '" + objStockSummaryReport.SubCategoryId + "' ";
                    }
                    if (objStockSummaryReport.StyleName != null)
                    {
                        sql = sql + " and PRODUCT_STYLE = '" + objStockSummaryReport.StyleName + "' ORDER BY ITEM_ID ASC  ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_STOCK_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> PrivilegeCustomerDetails(CustomerReport customerReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Privilege Customer Details  ' RPT_TITLE, " +
                            "WARE_HOUSE_ID," +
                            "WARE_HOUSE_NAME," +
                            "WARE_HOUSE_ADDRESS," +
                            "SHOP_ID," +
                            "SHOP_NAME," +
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
                            "DATE_OF_BIRTH," +
                            "DATE_OF_ENROLLMENT," +
                            "DATE_OF_EXPIRE," +
                            "WHOLE_SALE_CUSTOMER," +
                            "STORE_CUSTOMER," +
                            "STATUS," +
                            "UPDATE_BY," +
                            "UPDATE_DATE," +
                            "CREATE_BY," +
                            "CREATE_DATE  " +
                            " From VEW_RPT_Privilege_CUSTOMER WHERE 1=1  ";

                   
                    if (customerReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + customerReport.ShopId + "' ORDER BY CUSTOMER_ID ASC  ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_Privilege_CUSTOMER"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SaleCustomerDetails(CustomerReport customerReport)
        {
            try
            {
                DataSet ds = null;
                var sql = "";
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                            " ' Sale Customer Details  ' RPT_TITLE, " +
                                "WARE_HOUSE_NAME," +
                                "WARE_HOUSE_ADDRESS," +
                                "SHOP_ID," +
                                "SHOP_NAME," +
                                "CUSTOMER_ID," +
                                "CUSTOMER_CODE," +
                                "CUSTOMER_NAME," +
                                "CONTACT_NO," +
                                "EMAIL," +
                                "ADDRESS," +
                                "DISCOUNT," +
                                "ENROLMENT_DATE," +
                                "CUSTOMER_INFO_ID," +
                                "CUSTOMER_AUTO_ID  " +
                            " From VEW_RPT_SALE_CUSTOMER_DETAILS WHERE 1=1  ";
                    
                    if (customerReport.ShopId > 0)
                    {
                        sql = sql + " and SHOP_ID = '" + customerReport.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALE_CUSTOMER_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> SaleSummarySaveForRpt(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_rpt_sale_summary")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_FROM_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleDetailsSummary.FromDate;
            objOracleCommand.Parameters.Add("P_TO_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleDetailsSummary.ToDate;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleDetailsSummary.ShopId > 0 ? objSaleDetailsSummary.ShopId.ToString() : null;
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
        public async Task<string> AllShopSaleSummaryCategorySubCategorySaveForRpt(SaleDetailsSummary objSaleDetailsSummary)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_sale_sumary_by_cs")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("p_from_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleDetailsSummary.FromDate;
            objOracleCommand.Parameters.Add("p_to_date", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSaleDetailsSummary.ToDate;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = 0;
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
        public async Task<DataSet> InvoiceWiseDetails(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                        sql = "SELECT " +
                              " '  Invoice Wise Details Report between ' RPT_TITLE, " +
                                 "WAREHOUSE_ID," +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "SHOP_ID," +
                                "SHOP_NAME," +
                                "SALE_INFO_ID," +
                                "CUSTOMER_ID," +
                                "CUSTOMER_NAME," +
                                "ADDRESS," +
                                "CONTACT_NO," +
                                "SALE_INFO_AUTO_ID," +
                                "SALE_ITEM_ID," +
                                "PRODUCT_ID," +
                                "ITEM_ID," +
                                "INVOICE_NUMBER," +
                                "INVOICE_DATE," +
                                "BARCODE," +
                                "STYLE_NAME," +
                                "PRICE," +
                                "DIS_PRICE," +
                                "QUANTITY," +
                                "TOTAL," +
                                "DISCOUNT_P," +
                                "DISCOUNT_AMOUNT," +
                                "NET_AMOUNT," +
                                "VAT_AMOUNT " +
                              " From VEW_RPT_INVOICE_WISE_DETAILS where INVOICE_NUMBER = '" + saleDetailsSummary.InvoiceNumber + "' ";

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_INVOICE_WISE_DETAILS"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> StyleWiseFristLaunchDateSaveRpt(StoreDeliveryReport storeDeliveryReport)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_rpt_style_lunching_date")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_FROM_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = storeDeliveryReport.FromDate;
            objOracleCommand.Parameters.Add("P_TO_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = storeDeliveryReport.ToDate;
            objOracleCommand.Parameters.Add("p_product_style", OracleDbType.Varchar2, ParameterDirection.Input).Value = storeDeliveryReport.ProductStyle;
            objOracleCommand.Parameters.Add("p_shop_id", OracleDbType.Varchar2, ParameterDirection.Input).Value = storeDeliveryReport.ShopId > 0 ? storeDeliveryReport.ShopId.ToString() : null;
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


        public async Task<DataSet> StyleWiseFristLaunchDate(StoreDeliveryReport storeDeliveryReport)
        {
            try
            {
                //storeDeliveryReport.ProductStyle = storeDeliveryReport.ProductStyle.ToLower().Trim();
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' Style Wise Frist Launch Date Report  between ' || to_date( '" + storeDeliveryReport.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + storeDeliveryReport.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "STORE_DELIVERY_NUMBER," +
                                "PRODUCT_ID," +
                                "PRODUCT_STYLE," +
                                "DELIVERY_DATE," +
                                "DELIVERY_QUANTITY," +
                                "DELIVERY_SHOP_ID," +
                                "SHOP_NAME," +
                                "CATEGORY_ID," +
                                "CATEGORY_NAME," +
                                "SUB_CATEGORY_ID," +
                                "SUB_CATEGORY_NAME," +
                                "DESIGNER_ID," +
                                "DESIGNER_NAME," +
                                "MERCHANDISER_ID," +
                                "MERCHANDISER_NAME," +
                                "DC_STOCK " +
                                " From vew_rpt_lunching_date where DELIVERY_DATE BETWEEN  to_date('" + storeDeliveryReport.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + storeDeliveryReport.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (storeDeliveryReport.CategoryId > 0)
                    {
                        sql = sql + "and CATEGORY_ID = '" + storeDeliveryReport.CategoryId + "' ";
                    }
                    if (storeDeliveryReport.SubCategoryId > 0)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + storeDeliveryReport.SubCategoryId + "' ";
                    }
                    if (!string.IsNullOrEmpty(storeDeliveryReport.ProductStyle))
                    {
                        sql = sql + "and lower(PRODUCT_STYLE) = '" + storeDeliveryReport.ProductStyle.ToLower().Trim() + "' ";
                    }
                    if (!string.IsNullOrEmpty(storeDeliveryReport.DesignerId))
                    {
                        sql = sql + "and DESIGNER_ID = '" + storeDeliveryReport.DesignerId + "' ";
                    }
                    if (!string.IsNullOrEmpty(storeDeliveryReport.MerchandiserId))
                    {
                        sql = sql + "and MERCHANDISER_ID = '" + storeDeliveryReport.MerchandiserId + "' ";
                    }
                    if (storeDeliveryReport.ShopId > 0)
                    {
                        sql = sql + "and DELIVERY_SHOP_ID = '" + storeDeliveryReport.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "vew_rpt_lunching_date"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> DesignerWiseSaleSummary(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                var sql = "";
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    sql = "SELECT " +
                          " ' Designer Wise Sale between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_ID," +
                            "WAREHOUSE_NAME," +
                            "WAREHOUSE_ADDRESS," +
                            "SHOP_ID," +
                            "SHOP_NAME," +
                            "DESIGNER_ID," +
                            "DESIGNER_NAME," +
                            "ORGINAL_PRODUCT_STYLE," +
                            "CATEGORY_ID," +
                            "CATEGORY_NAME," +
                            "SUB_CATEGORY_ID," +
                            "SUB_CATEGORY_NAME," +
                            "INVOICE_DATE," +
                            "DIS_PRICE," +
                            "QUANTITY," +
                            "TOTAL," +
                            "DISCOUNT_AMOUNT," +
                            "NET_AMOUNT," +
                            "VAT_AMOUNT " +
                          " From VEW_RPT_DISIGNER_WISE_SALE where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (!string.IsNullOrEmpty(saleDetailsSummary.DesignerId))
                    {
                        sql = sql + "and DESIGNER_ID = '" + saleDetailsSummary.DesignerId + "' ";
                    }
                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);
                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DISIGNER_WISE_SALE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StockTransferDetails(StockTransferDetails stockTransferDetails)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' Stock Transfer Details between ' || to_date( '" + stockTransferDetails.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + stockTransferDetails.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "STOCK_TRANSFER_ITEM_ID," +
                                "STOCK_TRANSFER_ID," +
                                "STOCK_TRANSFER_CHALLAN_NUM," +
                                "REQUISITION_NUM," +
                                "ITEM_ID," +
                                "ITEM_NAME," +
                                "PRODUCT_ID," +
                                "PRODUCT_STYLE," +
                                "BARCODE," +
                                "TRANSFER_QUANTITY," +
                                "SALE_PRICE," +
                                "VAT," +
                                "TRANSFER_SHOPID_TO," +
                                "TRANSFER_SHOPID_FROM," +
                                "SHOP_NAME," +
                                "TRANSFERED_BY," +
                                "TRANSFER_DATE," +
                                "RECEIVE_YN," +
                                "RECEIVED_BY," +
                                "RECEIVED_DATE  " +
                                " From VEW_RPT_STOCK_TRANS_DATE_WISE where TRANSFER_DATE BETWEEN  to_date('" + stockTransferDetails.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + stockTransferDetails.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (stockTransferDetails.ShopId > 0)
                    {
                        sql = sql + "and TRANSFER_SHOPID_FROM = '" + stockTransferDetails.ShopId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STOCK_TRANS_DATE_WISE"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> StyleWiseSaleSummaryRpt(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' Style Wise Sale Summary between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                              "WAREHOUSE_ID,WAREHOUSE_NAME,WAREHOUSE_ADDRESS,SHOP_ID,SHOP_NAME,SHOP_ADDRESS,INVOICE_DATE,PRODUCT_ID,ORGINAL_PRODUCT_STYLE,CATEGORY_ID,CATEGORY_NAME," +
                              "SUB_CATEGORY_ID,SUB_CATEGORY_NAME,TOTAL,QUANTITY,DISCOUNT_AMOUNT,NET_AMOUNT,VAT_AMOUNT,BAG_PRICE"+
                              " From VEW_RPT_STYLE_SALE_SUMMARY where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    if (saleDetailsSummary.CategoryId !=null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }
                    if (saleDetailsSummary.ProductId >0 )
                    {
                        sql = sql + "and PRODUCT_ID = '" + saleDetailsSummary.ProductId + "' ";
                    }
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_STYLE_SALE_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SaleManWiseSaleSummary(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' Sales Man Sale Summary between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "SALES_MAN_ID," +
                                "EMPLOYEE_NAME," +
                                "SUM(SALE_QUANTITY)SALE_QUANTITY," +
                                "SUM(TOTAL_AMOUNT)TOTAL_AMOUNT," +
                                "SUM(DISCOUNT_AMOUNT)DISCOUNT_AMOUNT," +
                                "SUM(NET_AMOUNT)NET_AMOUNT," +
                                "SUM(VAT_AMOUNT)VAT_AMOUNT," +
                                "SHOP_ID," +
                                "SHOP_NAME " +
                                " From VEW_RPT_SALEMAN_SALE_SUMMARY where  SHOP_ID = '" + saleDetailsSummary.ShopId + "' AND INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";


                    sql += " GROUP BY RPT_TITLE," +
                             "WAREHOUSE_NAME, " +
                             "WAREHOUSE_ADDRESS," +
                             "WAREHOUSE_ADDRESS," +
                             "SHOP_ID," +
                             "SALES_MAN_ID, "+
                             "SHOP_NAME," +
                             "EMPLOYEE_NAME ";
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SALEMAN_SALE_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> SizeWiseSaleSummaryRpt(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " ' Size Wise Sale Summary between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "SHOP_ID," +
                                "SHOP_NAME," +
                                "ITEM_SIZE," +
                                "CATEGORY_ID," +
                                "CATEGORY_NAME," +
                                "SUB_CATEGORY_ID," +
                                "SUB_CATEGORY_NAME," +
                              "SUM(SALE_QUANTITY)SALE_QUANTITY  " +
                              " From VEW_RPT_SIZE_SALE_SUMMARY where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (saleDetailsSummary.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
                    }
                    if (saleDetailsSummary.CategoryId != null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }
                    sql += " GROUP BY RPT_TITLE," +
                             "WAREHOUSE_NAME, " +
                             "WAREHOUSE_ADDRESS," +
                             "ITEM_SIZE," +
                             "CATEGORY_ID," +
                             "CATEGORY_NAME," +
                             "SUB_CATEGORY_ID," +
                             "SUB_CATEGORY_NAME," +
                             "SHOP_ID," +
                             "SHOP_NAME ";
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_SIZE_SALE_SUMMARY"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> ShopCPUCalculation(SaleDetailsSummary saleDetailsSummary)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                                 " ' Shop CPU Calculation between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                                "WAREHOUSE_NAME," +
                                "WAREHOUSE_ADDRESS," +
                                "SHOP_ID," +
                                "SHOP_NAME," +
                                "PRODUCT_STYLE," +
                                "BARCODE," +
                                "ITEM_NAME," +
                                "CM," +
                                "MATERIAL_COST," +
                                "PURCHASE_PRICE," +
                                "SALE_PRICE," +
                                "SUM(TOTAL_VALUE)TOTAL_VALUE," +
                                 "SUM(TOTAL_CM)TOTAL_CM," +
                                 "SUM(TOTAL_MATERIAL_COST)TOTAL_MATERIAL_COST," +
                                "CATEGORY_ID," +
                                "CATEGORY_NAME," +
                                "SUB_CATEGORY_ID," +
                                "SUB_CATEGORY_NAME," +
                                "SUM(SALE_QUANTITY)SALE_QUANTITY" +

                                " From VEW_RPT_CPU_CALCULATION where  SHOP_ID = '" + saleDetailsSummary.ShopId + "' AND INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

                    if (saleDetailsSummary.CategoryId != null)
                    {
                        sql = sql + "and CATEGORY_ID = '" + saleDetailsSummary.CategoryId + "' ";
                    }
                    if (saleDetailsSummary.SubCategoryId != null)
                    {
                        sql = sql + "and SUB_CATEGORY_ID = '" + saleDetailsSummary.SubCategoryId + "' ";
                    }

                    sql += " GROUP BY RPT_TITLE," +
                                         "WAREHOUSE_NAME, " +
                                         "WAREHOUSE_ADDRESS," +
                                         "SHOP_ID," +
                                         "SHOP_NAME," +
                                         "PRODUCT_STYLE, " +
                                         "BARCODE," +
                                         "CM," +
                                         "MATERIAL_COST," +
                                         "PURCHASE_PRICE," +
                                         "SALE_PRICE," +
                                         "ITEM_NAME, "+
                                         "CATEGORY_ID," +
                                         "CATEGORY_NAME," +
                                         "SUB_CATEGORY_ID," +
                                         "SUB_CATEGORY_NAME" ;

                  
                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_CPU_CALCULATION"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region GetAndSaveSection

        public async Task<IEnumerable<SalesManWiseSaleReportModel>> SalesManWiseSaleSummaryForRpt(SaleDetailsSummary objSaleDetailsSummary,string shopUrl)
        {
            IEnumerable<SalesManWiseSaleReportModel> model = new List<SalesManWiseSaleReportModel>();
            if (!string.IsNullOrWhiteSpace(shopUrl))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(shopUrl);

                    var responseTask = client.GetAsync("SalesManWiseSaleForDCRpt?fromDate=" + objSaleDetailsSummary.FromDate + "&toDate=" + objSaleDetailsSummary.ToDate);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<SalesManWiseSaleReportModel>>();
                        readTask.Wait();

                        model = readTask.Result;
                    }
                }
            }
            return model;

        }

        public async Task<string> SalesManSaleSummarySaveForRpt(SalesManWiseSaleReportModel objSalesManWiseSaleReportModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_RPT_SALE_MAN_SUMMARY")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_SALES_MAN_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.SalesManId;
            objOracleCommand.Parameters.Add("P_INVOICE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = Convert.ToDateTime(objSalesManWiseSaleReportModel.InvoiceDate).ToString("MM/dd/yyyy");
            objOracleCommand.Parameters.Add("P_SALE_QUANTITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.SaleQuantity > 0 ? objSalesManWiseSaleReportModel.SaleQuantity.ToString(): null;
            objOracleCommand.Parameters.Add("P_TOTAL_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.TotalAmount > 0 ? objSalesManWiseSaleReportModel.TotalAmount : 0.0;
            objOracleCommand.Parameters.Add("P_DISCOUNT_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.DiscountAmount > 0 ? objSalesManWiseSaleReportModel.DiscountAmount : 0.0;
            objOracleCommand.Parameters.Add("P_NET_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.NetAmount > 0 ? objSalesManWiseSaleReportModel.NetAmount : 0.0;
            objOracleCommand.Parameters.Add("P_VAT_AMOUNT", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.VatAmount > 0 ? objSalesManWiseSaleReportModel.VatAmount : 0.0;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objSalesManWiseSaleReportModel.ShopId > 0 ? objSalesManWiseSaleReportModel.ShopId.ToString() : null;
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


        public async Task<string> SalesManWiseSaleSummaryDelete(int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_DELETE_SALE_MAN_SUMMARY")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId > 0 ? shopId.ToString() : null;
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

        public async Task<string> ShopStockDataSaveForRpt(int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_RPT_SHOP_STOCK_MASTER")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId > 0 ? shopId: 0;
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

        public async Task<string> ShopStockDataSaveTrForRpt(int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_RPT_SHOP_STOCK_MISSING")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId > 0 ? shopId : 0;
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

        public async Task<string> ShopStockDataDeleteForRpt(int shopId)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_RPT_SHOP_STOCK_DELETE")
            {
                CommandType = CommandType.StoredProcedure
            };
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = shopId > 0 ? shopId : 0;
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
        #endregion

        #region Preiodical Repoet Section

        public async Task<List<DeliveredProductList>> GetAllDeliveredProduct()
        {
            var sql = "SELECT " +
                      "PRODUCT_ID," +
                      "ITEM_ID," +
                      "ITEM_NAME," +
                      "BARCODE," +
                      "STYLE_NAME," +
                      "PURCHASE_PRICE," +
                      "SALE_PRICE," +
                      "QUANTITY," +
                      "CATEGORY_ID," +
                      "PRODUCT_CATEGORY," +
                      "SUB_CATEGORY_ID," +
                      "PRODUCT_SUB_CATEGORY," +
                      "DESIGNER_ID," +
                      "DESIGNER_NAME," +
                      "MERCHANDISER_ID," +
                      "MERCHANDISER_NAME," +
                      "DELIVERY_SHOP_ID," +
                      "DELIVERY_SHOP_NAME," +
                      "DELIVERY_DATE " +
                      "FROM VEW_RPT_DELIVERED_PRODUCT_LIST ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<DeliveredProductList> productList = new List<DeliveredProductList>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                DeliveredProductList list = new DeliveredProductList();

                                list.ProductId = Convert.ToInt32(objDataReader["PRODUCT_ID"].ToString());
                                list.ItemId = Convert.ToInt32(objDataReader["ITEM_ID"].ToString());
                                list.ItemName = objDataReader["ITEM_NAME"].ToString();
                                list.Barcode = objDataReader["BARCODE"].ToString();
                                list.StyleName = objDataReader["STYLE_NAME"].ToString();

                                list.PurchasePrice = Convert.ToDouble(objDataReader["PURCHASE_PRICE"].ToString());
                                list.SalePrice = Convert.ToDouble(objDataReader["SALE_PRICE"].ToString());
                                list.Quantity = Convert.ToInt32(objDataReader["QUANTITY"].ToString());

                                list.CategoryId = Convert.ToInt32(objDataReader["CATEGORY_ID"].ToString());
                                list.CategoryName = objDataReader["PRODUCT_CATEGORY"].ToString();
                                list.SubCategoryId = Convert.ToInt32(objDataReader["SUB_CATEGORY_ID"].ToString());
                                list.SubCategoryName = objDataReader["PRODUCT_SUB_CATEGORY"].ToString();

                                list.DesignerId = objDataReader["DESIGNER_ID"].ToString();
                                list.DesignerName = objDataReader["DESIGNER_NAME"].ToString();
                                list.MerchandiserId = objDataReader["MERCHANDISER_ID"].ToString();
                                list.MerchandiserName = objDataReader["MERCHANDISER_NAME"].ToString();

                                list.ShopId = Convert.ToInt32(objDataReader["DELIVERY_SHOP_ID"].ToString());
                                list.ShopName = objDataReader["DELIVERY_SHOP_NAME"].ToString();

                                list.DeliveredDate = Convert.ToDateTime(objDataReader["DELIVERY_DATE"].ToString());

                                productList.Add(list);
                            }

                            return productList;
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

        public async Task<string> PeriodicalStockSaveForRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
           
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_rpt_periodical_stock")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_FROM_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.FromDate;
            objOracleCommand.Parameters.Add("P_TO_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.ToDate;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.ShopId;
            objOracleCommand.Parameters.Add("P_PRODUCT_STYLE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.StyleName;
            objOracleCommand.Parameters.Add("P_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.CategoryId > 0 ? objPeriodicalStockReportModel.CategoryId.ToString() : null;
            objOracleCommand.Parameters.Add("P_SUB_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.SubCategoryId > 0 ? objPeriodicalStockReportModel.SubCategoryId.ToString() : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.UpdatedBy;

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

        public async Task<string> WarehousePeriodicalStockSaveForRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {

            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("pro_rpt_dc_periodical_stock")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_FROM_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.FromDate;
            objOracleCommand.Parameters.Add("P_TO_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.ToDate;
            objOracleCommand.Parameters.Add("P_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.WarehouseId;
            objOracleCommand.Parameters.Add("P_PRODUCT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.StyleName;
            objOracleCommand.Parameters.Add("P_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.CategoryId > 0 ? objPeriodicalStockReportModel.CategoryId.ToString() : null;
            objOracleCommand.Parameters.Add("P_SUB_CATEGORY_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.SubCategoryId > 0 ? objPeriodicalStockReportModel.SubCategoryId.ToString() : null;
            objOracleCommand.Parameters.Add("p_update_by", OracleDbType.Varchar2, ParameterDirection.Input).Value = objPeriodicalStockReportModel.UpdatedBy;

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

        public async Task<DataSet> PeriodicalStockRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                             " ' Sale Stock Report  between ' || to_date( '" + objPeriodicalStockReportModel.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + objPeriodicalStockReportModel.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                             "STOCK_FROM_DATE,STOCK_TO_DATE,CATEGORY_ID,SUB_CATEGORY_ID,PRODUCT_STYLE, " +
                             "PURCAHSE_RCV_QTY,SHOP_RCV_QTY,SHOP_RETURN_QTY,RCV_QTY_FROM_OTHER_SHOP,TRANSFER_QTY_TO_OS, " +
                            "DAMAGE_QTY_SHOP,DAMAGE_QTY_WH,SALE_QUANTITY,EXCHANGE_QUANTITY,PURCAHSE_RCV_QTY_PRE,SHOP_RCV_QTY_PRE, " +
                            "SHOP_RETURN_QTY_PRE,RCV_QTY_FROM_OTHER_SHOP_PRE,TRANSFER_QTY_TO_OS_PRE,DAMAGE_QTY_SHOP_PRE, " +
                            "DAMAGE_QTY_WH_PRE,SALE_QUANTITY_PRE,EXCHANGE_QUANTITY_PRE,OPENING_STOCK_QTY,TOTAL_SALE_PRICE,MRP_VALUE, " +
                            "OPENING_VALUE,OPENING_CP_VALUE,PURCHASE_PRICE,VAT_AMOUNT,MARGIN_VALUE,MARGIN_PERCENT,DISCOUNT_MAX,DISCOUNT_PRICE, " +
                            "CLOSING_STOCK_QTY,CLOSING_VALUE,CLOSING_CP_VALUE, MATERIAL_COST,CM,OPENING_MATERIAL_COST_VAL,OPENING_CM_VAL,CLOSING_MATERIAL_VALUE,CLOSEING_CM_VALUE,UPDATE_BY,  " +
                            "UPDATE_DATE,SHOP_ID,SHOP_NAME,SHOP_ADDRESS,WAREHOUSE_ID,WARE_HOUSE_NAME " +
                            " From VEW_RPT_PERIODICAL_STOCK  " +
                             "WHERE(stock_from_date BETWEEN TO_DATE('" + objPeriodicalStockReportModel.FromDate.Trim() + "', 'dd/mm/yyyy') " +
                             "AND TO_DATE('" + objPeriodicalStockReportModel.ToDate.Trim() + "', 'dd/mm/yyyy') " +
                             "OR stock_to_date BETWEEN TO_DATE('" + objPeriodicalStockReportModel.FromDate.Trim() + "', 'dd/mm/yyyy') " +
                             "AND TO_DATE('" + objPeriodicalStockReportModel.ToDate.Trim() + "', 'dd/mm/yyyy')) ";
                    if (objPeriodicalStockReportModel.ShopId > 0)
                    {
                        sql = sql + "and SHOP_ID = '" + objPeriodicalStockReportModel.ShopId + "' ";
                    }

                    //if (storeDeliveryReport.CategoryId > 0)
                    //{
                    //    sql = sql + "and CATEGORY_ID = '" + storeDeliveryReport.CategoryId + "' ";
                    //}
                    //if (storeDeliveryReport.SubCategoryId > 0)
                    //{
                    //    sql = sql + "and SUB_CATEGORY_ID = '" + storeDeliveryReport.SubCategoryId + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.ProductStyle))
                    //{
                    //    sql = sql + "and lower(PRODUCT_STYLE) = '" + storeDeliveryReport.ProductStyle.ToLower().Trim() + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.DesignerId))
                    //{
                    //    sql = sql + "and DESIGNER_ID = '" + storeDeliveryReport.DesignerId + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.MerchandiserId))
                    //{
                    //    sql = sql + "and MERCHANDISER_ID = '" + storeDeliveryReport.MerchandiserId + "' ";
                    //}

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_PERIODICAL_STOCK"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DataSet> WarehousePeriodicalStockRpt(PeriodicalStockReportModel objPeriodicalStockReportModel)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                             " 'Warehouse Periodical Stock Report  between ' || to_date( '" + objPeriodicalStockReportModel.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + objPeriodicalStockReportModel.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                             "STOCK_FROM_DATE,STOCK_TO_DATE,PRODUCT_ID,PRODUCT_STYLE,CATEGORY_ID,SUB_CATEGORY_ID,PURCAHSE_RCV_QTY,SHOP_DELIVERY_QTY, " +
                             "SHOP_RETURN_QTY,DAMAGE_QTY_WH,PURCAHSE_RCV_QTY_PRE,SHOP_DELIVERY_QTY_PRE,SHOP_RETURN_QTY_PRE,DAMAGE_QTY_WH_PRE, "+
                            "OPENING_STOCK_QTY,TOTAL_SALE_PRICE,MRP_VALUE,OPENING_VALUE,PURCHASE_PRICE,VAT_AMOUNT,MARGIN_VALUE,MARGIN_PERCENT," +
                            "DISCOUNT_MAX,DISCOUNT_PRICE,CLOSING_STOCK_QTY,CLOSING_VALUE,CLOSING_CP_VALUE,UPDATE_BY,UPDATE_DATE,OPENING_CP_VALUE,"+
                            "MATERIAL_COST,CM,OPENING_MATERIAL_COST_VAL,OPENING_CM_VAL,CLOSING_MATERIAL_VALUE,CLOSEING_CM_VALUE,WAREHOUSE_ID, "+
                            "WARE_HOUSE_NAME,WARE_HOUSE_ADDRESS,CENTRAL_BIN  " +
                            " From VEW_RPT_DC_PERIODICAL_STOCK  " +
                             "WHERE(STOCK_FROM_DATE BETWEEN TO_DATE('" + objPeriodicalStockReportModel.FromDate.Trim() + "', 'dd/mm/yyyy') " +
                             "AND TO_DATE('" + objPeriodicalStockReportModel.ToDate.Trim() + "', 'dd/mm/yyyy') " +
                             "OR STOCK_TO_DATE BETWEEN TO_DATE('" + objPeriodicalStockReportModel.FromDate.Trim() + "', 'dd/mm/yyyy') " +
                             "AND TO_DATE('" + objPeriodicalStockReportModel.ToDate.Trim() + "', 'dd/mm/yyyy')) ";
                    //if (objPeriodicalStockReportModel.ShopId > 0)
                    //{
                    //    sql = sql + "and SHOP_ID = '" + objPeriodicalStockReportModel.ShopId + "' ";
                    //}

                    //if (storeDeliveryReport.CategoryId > 0)
                    //{
                    //    sql = sql + "and CATEGORY_ID = '" + storeDeliveryReport.CategoryId + "' ";
                    //}
                    //if (storeDeliveryReport.SubCategoryId > 0)
                    //{
                    //    sql = sql + "and SUB_CATEGORY_ID = '" + storeDeliveryReport.SubCategoryId + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.ProductStyle))
                    //{
                    //    sql = sql + "and lower(PRODUCT_STYLE) = '" + storeDeliveryReport.ProductStyle.ToLower().Trim() + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.DesignerId))
                    //{
                    //    sql = sql + "and DESIGNER_ID = '" + storeDeliveryReport.DesignerId + "' ";
                    //}
                    //if (!string.IsNullOrEmpty(storeDeliveryReport.MerchandiserId))
                    //{
                    //    sql = sql + "and MERCHANDISER_ID = '" + storeDeliveryReport.MerchandiserId + "' ";
                    //}

                    OracleCommand objOracleCommand = new OracleCommand(sql);
                    using (OracleConnection strConn = GetConnection())
                    {
                        try
                        {
                            objOracleCommand.Connection = strConn;
                            await strConn.OpenAsync();
                            var objDataAdapter = new OracleDataAdapter(objOracleCommand);

                            dt.Clear();
                            ds = new System.Data.DataSet();
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_RPT_DC_PERIODICAL_STOCK"));
                            objDataAdapter.Dispose();
                            objOracleCommand.Dispose();
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Excel Report

        public async Task<List<SaleDetailsSummaryForExcelReport>> GetSaleSummaryFroExcel(SaleDetailsSummary saleDetailsSummary)
        {
            var sql = "SELECT " +
                         " '  Sale Summary Report between ' || to_date( '" + saleDetailsSummary.FromDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + saleDetailsSummary.ToDate + "', 'dd/mm/yyyy')  RPT_TITLE, " +
                         "SHOP_ID," +
                         "SHOP_NAME," +
                         "SHOP_ADDRESS," +
                         "WAREHOUSE_ID," +
                         "WAREHOUSE_NAME," +
                         "WAREHOUSE_ADDRESS," +
                         "INVOICE_NUMBER," +
                         "to_char(INVOICE_DATE,'dd/mm/yyyy') INVOICE_DATE ," +
                         "TOTAL_ITEM," +
                         "TOTAL_AMOUNT," +
                         "EXG_RTN_AMOUNT," +
                         "ACT_AMOUNT, " +
                         "VAT," +
                         "DISCOUNT_P, " +
                         "DISCOUNT_A, " +
                         "PRE_DISCOUNT, " +
                         "ACT_DISCOUNT, " +
                         "SUB_TOTAL," +
                         "NET_AMOUNT," +
                         "PAYMENT_TYPE," +
                         "CASHAMOUNT, " +
                         "BKASHAMOUNT, " +
                         "BRACKBANKAMOUNT, " +
                         "ROCKETAMOUNT, " +
                         "DBBLAMOUNT, " +
                         "SCBAMOUNT, " +
                         "SIBLAMOUNT, " +
                         "CITYAMOUNT, " +
                         "EBLAMOUNT, " +
                         "GIFTVOUCHER, " +
                         "TOTAL_PAYMENT_TYPE_AMOUNT, " +
                         "PAYMENT_AMOUNT_ALL  " +
                         " From VEW_RPT_SALE_SUMMARY s where INVOICE_DATE BETWEEN  to_date('" + saleDetailsSummary.FromDate.Trim() + "', 'dd/mm/yyyy') AND  to_date('" + saleDetailsSummary.ToDate.Trim() + "' , 'dd/mm/yyyy')  ";

            if (saleDetailsSummary.ShopId > 0)
            {
                sql = sql + "and SHOP_ID = '" + saleDetailsSummary.ShopId + "' ";
            }
            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) )
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<SaleDetailsSummaryForExcelReport> objSaleDetailsSummaryForExcelReport = new List<SaleDetailsSummaryForExcelReport>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                SaleDetailsSummaryForExcelReport model = new SaleDetailsSummaryForExcelReport();
                                model.InvoiceNumber = objDataReader["INVOICE_NUMBER"].ToString();
                                model.InvoiceDate = objDataReader["INVOICE_DATE"].ToString();
                                model.TotalAmount = objDataReader["TOTAL_AMOUNT"].ToString();
                                model.ExReturnAmount = !string.IsNullOrWhiteSpace(objDataReader["EXG_RTN_AMOUNT"].ToString())? Convert.ToDouble(objDataReader["EXG_RTN_AMOUNT"].ToString()) : 0 ;
                                model.ActualAmount = !string.IsNullOrWhiteSpace(objDataReader["ACT_AMOUNT"].ToString()) ? Convert.ToDouble(objDataReader["ACT_AMOUNT"].ToString()) : 0 ;
                                model.PreDiscount = !string.IsNullOrWhiteSpace(objDataReader["PRE_DISCOUNT"].ToString()) ? Convert.ToDouble(objDataReader["PRE_DISCOUNT"].ToString()) : 0;
                                model.DiscountAmount = objDataReader["DISCOUNT_A"].ToString();
                                model.ActualDiscount = !string.IsNullOrWhiteSpace(objDataReader["ACT_DISCOUNT"].ToString()) ? Convert.ToDouble(objDataReader["ACT_DISCOUNT"].ToString()): 0;
                                model.DiscountPercent = objDataReader["DISCOUNT_P"].ToString();
                                model.NetAmount = objDataReader["NET_AMOUNT"].ToString();
                                model.Vat = objDataReader["VAT"].ToString();
                                model.PaymentType = objDataReader["PAYMENT_TYPE"].ToString();
                                model.ShopId = Convert.ToInt32(objDataReader["SHOP_ID"].ToString());
                                model.CashAmount = objDataReader["CASHAMOUNT"].ToString();
                                model.BkashAmount = objDataReader["BKASHAMOUNT"].ToString();
                                model.BracBankAmount = objDataReader["BRACKBANKAMOUNT"].ToString();
                                model.RocketAmount = objDataReader["ROCKETAMOUNT"].ToString();
                                model.DBBLAmount = objDataReader["DBBLAMOUNT"].ToString();
                                model.SCBAmount = objDataReader["SCBAMOUNT"].ToString();
                                model.SIBLAmount = objDataReader["SIBLAMOUNT"].ToString();
                                model.CITYAmount = objDataReader["CITYAMOUNT"].ToString();
                                model.EBLAmount = objDataReader["EBLAMOUNT"].ToString();
                                model.GIFTVoucherAmount = objDataReader["GIFTVOUCHER"].ToString();

                                objSaleDetailsSummaryForExcelReport.Add(model);
                            }

                            return objSaleDetailsSummaryForExcelReport;
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