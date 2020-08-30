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
    public class GiftVoucherDepositDAL
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

        #region "Gift Voucher Deposit Section"

        public async Task<List<GiftVoucherDepositModel>> GetGiftVoucherDepositList(/*int giftVoucherId*/)
        {


            var sql = "SELECT " +
                         "GIFT_VOUCHER_DEPOSIT_ID," +
                         "GIFT_VOUCHER_ID," +
                         "GIFT_VOUCHER_CODE," +
                         "GIFT_VOUCHER_VALUE," +
                         "GIFT_CUSTOMER_NAME," +
                         "GIFT_CUSTOMER_PHONE," +
                         "GIFT_VOU_DEPOSIT_SHOPID," +
                         "GIFT_VOU_DEPOSIT_SHOP_NAME," +
                         "CREATE_BY," +
                         "CREATE_DATE " +
                         "FROM VEW_GIFTVOUCHER_DEPOSIT ORDER BY CREATE_DATE DESC";
            //"FROM VEW_GIFTVOUCHER_DEPOSIT where GIFT_VOUCHER_ID = :GIFT_VOUCHER_ID ORDER BY GIFT_VOUCHER_DEPOSIT_ID DESC";


            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    //objCommand.Parameters.Add(":GIFT_VOUCHER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<GiftVoucherDepositModel> objGiftVoucherDeposit = new List<GiftVoucherDepositModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                GiftVoucherDepositModel model = new GiftVoucherDepositModel
                                {
                                    GiftVoucherDepositId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_DEPOSIT_ID"].ToString()),
                                    GiftVoucherId = Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString()),
                                    GiftVoucherCode = objDataReader["GIFT_VOUCHER_CODE"].ToString(),
                                    GiftVoucherValue = objDataReader["GIFT_VOUCHER_VALUE"].ToString(),
                                    GiftVoucherCustomerName = objDataReader["GIFT_CUSTOMER_NAME"].ToString(),
                                    GiftVoucherCustomerPhone = objDataReader["GIFT_CUSTOMER_PHONE"].ToString(),
                                    DepositShopId = Convert.ToInt32(objDataReader["GIFT_VOU_DEPOSIT_SHOPID"].ToString()),
                                    DepositShopName = objDataReader["GIFT_VOU_DEPOSIT_SHOP_NAME"].ToString(),
                                    CreateBy = objDataReader["CREATE_BY"].ToString(),
                                    CreateDate = objDataReader["CREATE_DATE"].ToString(),
                                };
                                objGiftVoucherDeposit.Add(model);
                            }

                            return objGiftVoucherDeposit;
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


        //public async Task<GiftVoucherDepositModel> GetAGiftVoucherDeposit(int giftVoucherId, int giftVoucherDepositId)
        //{


        //    var sql = "SELECT " +
        //                 "GIFT_VOUCHER_DEPOSIT_ID," +
        //                 "GIFT_VOUCHER_ID," +
        //                 "GIFT_VOUCHER_CODE," +
        //                 "GIFT_VOUCHER_VALUE," +
        //                 "GIFT_CUSTOMER_NAME," +
        //                 "GIFT_CUSTOMER_PHONE," +
        //                 "GIFT_VOU_DEPOSIT_SHOPID," +
        //                 "GIFT_VOU_DEPOSIT_SHOP_NAME," +
        //                 "CREATE_BY," +
        //                 "CREATE_DATE " +
        //                 "FROM VEW_GIFTVOUCHER_DEPOSIT where GIFT_VOUCHER_ID = :GIFT_VOUCHER_ID AND GIFT_VOUCHER_DEPOSIT_ID = :GIFT_VOUCHER_DEPOSIT_ID";

        //    using (OracleConnection objConnection = GetConnection())
        //    {
        //        using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
        //        {
        //            CommandType = CommandType.Text

        //        })
        //        {

        //            objCommand.Parameters.Add(":GIFT_VOUCHER_DEPOSIT_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherDepositId;
        //            objCommand.Parameters.Add(":GIFT_VOUCHER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherId;
        //            await objConnection.OpenAsync();
        //            using (OracleDataReader objDataReader =
        //                (OracleDataReader)await objCommand.ExecuteReaderAsync())
        //            {
        //                GiftVoucherDepositModel objGiftVoucherDepositModel = new GiftVoucherDepositModel();

        //                try
        //                {
        //                    while (await objDataReader.ReadAsync())
        //                    {
        //                        objGiftVoucherDepositModel.GiftVoucherDepositId =
        //                            Convert.ToInt32(objDataReader["GIFT_VOUCHER_DEPOSIT_ID"].ToString());
        //                        objGiftVoucherDepositModel.GiftVoucherId =
        //                            Convert.ToInt32(objDataReader["GIFT_VOUCHER_ID"].ToString());
        //                        objGiftVoucherDepositModel.GiftVoucherCode = objDataReader["GIFT_VOUCHER_CODE"].ToString();
        //                        objGiftVoucherDepositModel.GiftVoucherValue = objDataReader["GIFT_VOUCHER_VALUE"].ToString();
        //                        objGiftVoucherDepositModel.GiftVoucherCustomerName = objDataReader["GIFT_CUSTOMER_NAME"].ToString();
        //                        objGiftVoucherDepositModel.GiftVoucherCustomerPhone = objDataReader["GIFT_CUSTOMER_PHONE"].ToString();
        //                        objGiftVoucherDepositModel.DepositShopId =
        //                            Convert.ToInt32(objDataReader["GIFT_VOU_DEPOSIT_SHOPID"].ToString());
        //                        objGiftVoucherDepositModel.DepositShopName = objDataReader["GIFT_VOU_DEPOSIT_SHOP_NAME"].ToString();
        //                        objGiftVoucherDepositModel.CreateBy = objDataReader["CREATE_BY"].ToString();
        //                        objGiftVoucherDepositModel.CreateDate = objDataReader["CREATE_DATE"].ToString();
        //                    }
        //                    return objGiftVoucherDepositModel;
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


        public async Task<string> SaveAndUpdateGiftVoucherDeposit(GiftVoucherDepositModel objGiftVoucherDepositModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_GIFTVOUCHER_DEPOSIT_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDepositModel.GiftVoucherCode) ? objGiftVoucherDepositModel.GiftVoucherCode.Trim() : null;
            objOracleCommand.Parameters.Add("P_GIFT_VOUCHER_VALUE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDepositModel.GiftVoucherValue) ? objGiftVoucherDepositModel.GiftVoucherValue.Trim() : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDepositModel.GiftVoucherCustomerName) ? objGiftVoucherDepositModel.GiftVoucherCustomerName.Trim() : null;
            objOracleCommand.Parameters.Add("P_CUSTOMER_PHONE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDepositModel.GiftVoucherCustomerPhone) ? objGiftVoucherDepositModel.GiftVoucherCustomerPhone.Trim() : null;
            objOracleCommand.Parameters.Add("P_DEPOSIT_SHOP_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(Convert.ToInt32(objGiftVoucherDepositModel.DepositShopId).ToString()) ? Convert.ToInt32(objGiftVoucherDepositModel.DepositShopId).ToString().Trim() : null;
            objOracleCommand.Parameters.Add("P_UPDATED_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objGiftVoucherDepositModel.CreateBy) ? objGiftVoucherDepositModel.CreateBy.Trim() : null;
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


        public async Task<string> GetValueForSpecificVoucherCode(string giftVoucherCode)
        {
            var sql = "SELECT " +
                      " GIFT_VOUCHER_VALUE AS GIFT_VOUCHER_VALUE_FOR_CODE " +
                         "FROM GIFT_VOUCHER_DELIVERY WHERE GIFT_VOUCHER_CODE = :GIFT_VOUCHER_CODE ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":GIFT_VOUCHER_CODE", OracleDbType.Varchar2, ParameterDirection.Input).Value = giftVoucherCode;


                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        string giftVoucherValue = "";
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                giftVoucherValue = objDataReader["GIFT_VOUCHER_VALUE_FOR_CODE"].ToString();
                            }

                            return giftVoucherValue;
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