using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Oracle.ManagedDataAccess.Client;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;

namespace PosWarehouse.DAL
{
    public class OtherCompanyOfferDal
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

        public async Task<List<OtherCompanyOfferModel>> GetOtherCompanyOfferList()
        {
            var sql = "SELECT * FROM OTHER_COMPANY_OFFER  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<OtherCompanyOfferModel> objOtherCompanyOfferModel = new List<OtherCompanyOfferModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                OtherCompanyOfferModel model = new OtherCompanyOfferModel
                                {
                                    OtherCompanyOfferId = Convert.ToInt32(objDataReader["COMPANY_OFFER_ID"].ToString()),
                                    CompanyName = objDataReader["COMPANY_NAME"].ToString(),
                                    Offer = objDataReader["OFFER"].ToString(),
                                    OfferValidity = objDataReader["OFFER_VALIDITY"].ToString(),
                                    EligibleForOffer = objDataReader["ELIGIBLE_FOR_OFFER"].ToString(),
                                    CreatedBy = objDataReader["CREATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WAREHOUSE_ID"].ToString()

                                };
                                objOtherCompanyOfferModel.Add(model);
                            }
                            return objOtherCompanyOfferModel;
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

        public async Task<string> SaveData(OtherCompanyOfferDisModel objOtherCompanyOfferDisModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_OTHER_COMPANY_OFFER_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_COMPANY_OFFER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objOtherCompanyOfferDisModel.OtherCompanyOfferId;
            objOracleCommand.Parameters.Add("P_COMPANY_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.CompanyName) ? objOtherCompanyOfferDisModel.CompanyName.Trim() : null;
            objOracleCommand.Parameters.Add("P_OFFER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.Offer) ? objOtherCompanyOfferDisModel.Offer.Trim() : null;
            objOracleCommand.Parameters.Add("P_OFFER_VALIDITY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.OfferValidity) ? objOtherCompanyOfferDisModel.OfferValidity : null;
            objOracleCommand.Parameters.Add("P_ELIGIBLE_FOR_OFFER", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.EligibleForOffer) ? objOtherCompanyOfferDisModel.EligibleForOffer : null;
            objOracleCommand.Parameters.Add("P_CREATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.CreatedBy) ? objOtherCompanyOfferDisModel.CreatedBy : null;
            objOracleCommand.Parameters.Add("P_ACTIVE_YN", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.Active_YN) ? objOtherCompanyOfferDisModel.Active_YN : null;
            objOracleCommand.Parameters.Add("P_WAREHOUSE_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objOtherCompanyOfferDisModel.WareHouseId) ? objOtherCompanyOfferDisModel.WareHouseId : null;
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

        public async Task<List<OtherCompanyOfferDisModel>> GetOtherCompanyOfferListFroDisplay()
        {
            var sql = "SELECT COMPANY_OFFER_ID,COMPANY_NAME,OFFER,TO_CHAR(OFFER_VALIDITY,'DD/MM/YYYY')OFFER_VALIDITY ,ELIGIBLE_FOR_OFFER ,CREATE_BY ,CREATE_DATE ,WAREHOUSE_ID,ACTIVE_YN  FROM OTHER_COMPANY_OFFER  ";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection))
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<OtherCompanyOfferDisModel> objOtherCompanyOfferDisModel = new List<OtherCompanyOfferDisModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                OtherCompanyOfferDisModel model = new OtherCompanyOfferDisModel
                                {
                                    OtherCompanyOfferId = Convert.ToInt32(objDataReader["COMPANY_OFFER_ID"].ToString()),
                                    CompanyName = objDataReader["COMPANY_NAME"].ToString(),
                                    Offer = objDataReader["OFFER"].ToString(),
                                    OfferValidity = objDataReader["OFFER_VALIDITY"].ToString(),
                                    EligibleForOffer = objDataReader["ELIGIBLE_FOR_OFFER"].ToString(),
                                    CreatedBy = objDataReader["CREATE_BY"].ToString(),
                                    WareHouseId = objDataReader["WAREHOUSE_ID"].ToString(),
                                    Active_YN = objDataReader["ACTIVE_YN"].ToString()

                                };
                                objOtherCompanyOfferDisModel.Add(model);
                            }
                            return objOtherCompanyOfferDisModel;
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

        public async Task<OtherCompanyOfferDisModel> GetAOtherCompanyOffer(int companyOfferId)
        {

            var sql = "SELECT COMPANY_OFFER_ID,COMPANY_NAME,OFFER,TO_CHAR(OFFER_VALIDITY,'DD/MM/YYYY')OFFER_VALIDITY ,ELIGIBLE_FOR_OFFER ,CREATE_BY ,CREATE_DATE ,WAREHOUSE_ID,ACTIVE_YN  FROM OTHER_COMPANY_OFFER WHERE COMPANY_OFFER_ID =:COMPANY_OFFER_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":COMPANY_OFFER_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = companyOfferId; 
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        OtherCompanyOfferDisModel objOtherCompanyOfferDisModel = new OtherCompanyOfferDisModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objOtherCompanyOfferDisModel.OtherCompanyOfferId =Convert.ToInt32(objDataReader["COMPANY_OFFER_ID"].ToString());
                                objOtherCompanyOfferDisModel.CompanyName = objDataReader["COMPANY_NAME"].ToString();
                                objOtherCompanyOfferDisModel.Offer = objDataReader["OFFER"].ToString();
                                objOtherCompanyOfferDisModel.OfferValidity = objDataReader["OFFER_VALIDITY"].ToString();
                                objOtherCompanyOfferDisModel.EligibleForOffer = objDataReader["ELIGIBLE_FOR_OFFER"].ToString();
                                objOtherCompanyOfferDisModel.Active_YN = objDataReader["ACTIVE_YN"].ToString();
                                objOtherCompanyOfferDisModel.ActiveStatus = objOtherCompanyOfferDisModel.Active_YN == "Y" ? true : false;
                                objOtherCompanyOfferDisModel.CreatedBy = objDataReader["CREATE_BY"].ToString();
                                objOtherCompanyOfferDisModel.WareHouseId = objDataReader["WAREHOUSE_ID"].ToString();
                            }

                            return objOtherCompanyOfferDisModel;
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
    }
}