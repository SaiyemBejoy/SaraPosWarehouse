using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel.ApiModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PosWarehouse.DAL
{
    public class MaterialPurchaseReportDal
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


        public async Task<DataSet> MaterialPurchaseReport(PurchaseReportModelApi purchaseReportModelApi)
        {
            try
            {
                DataSet ds = null;
                DataTable dt = new DataTable();
                try
                {
                    var sql = "SELECT " +
                              " '  From ' || to_date( '" + purchaseReportModelApi.StartDate + "', 'dd/mm/yyyy') || ' to '|| to_date( '" + purchaseReportModelApi.EndDate + "', 'dd/mm/yyyy')  PERIOD, " +
                     "PURCHASE_ID, " +
                     "REQUISITION_ID, " +
                     "CREATE_DATE," +
                     "STORE_ID," +
                     "PURCHASE_CODE," +
                     "INVOICE_NUMBER," +
                     "INVOICE_DATE," +
                     "SUPPLIER_NAME," +
                     "SUPPLIER_ADDRESS," +
                     "BIN," +
                     "MATERIAL_TYPE_ID," +
                     "MATERIAL_TYPE_NAME," +
                     "MATERIAL_SUB_TYPE_ID," +
                     "MATERIAL_SUB_TYPE_NAME," +
                     "UNIT_PRICE," +
                     "TOTAL_AMOUNT," +
                     "QUANTITY," +
                     "BULK_USED," +
                     "BULK_UTILIZED_DATE," +
                     "SAMPLE_USED," +
                     "SAMPLE_UTILIZED_DATE," +
                     "OPENING_STOCK," +
                     "OPENING_AMOUNT," +
                     "PURCHASE_QUANTITY," +
                     "PURCHASE_AMOUNT," +
                     "UTILIZED_QUANTITY," +
                     "UTILIZED_AMOUNT," +
                     "CLOSING_STOCK," +
                     "CLOSING_AMOUNT," +
                     "TOTAL_STOCK,"+
                     "TOTAL_STOCK_AMOUNT " +
                     " From VEW_MMS_REPORT_PURCHASE_ERP WHERE 1= 1  ";


                    if (purchaseReportModelApi.MaterialTypeId !=null)
                    {
                        sql = sql + " and MATERIAL_TYPE_ID = '" + purchaseReportModelApi.MaterialTypeId  + "'  ";
                    }

                    if (purchaseReportModelApi.MaterialSubTypeId != null)
                    {
                        sql = sql + "  and MATERIAL_SUB_TYPE_ID = '" + purchaseReportModelApi.MaterialSubTypeId + "' ";
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
                            await Task.Run(() => objDataAdapter.Fill(ds, "VEW_MMS_REPORT_PURCHASE_ERP"));
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


    }
}