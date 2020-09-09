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
    public class MaterialDetailsDal
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

        public async Task<List<MaterialDetailsModel>> GetMaterialList()
        {


            var sql = "SELECT " +
                      "MATERIAL_ID," +
                      "MATERIAL_NAME," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE " +
                      "FROM VEW_MATERIAL_DETAILS ORDER BY MATERIAL_ID DESC";



            using (OracleConnection objConnection = GetConnection())
            {

                using (OracleCommand objCommand = new OracleCommand(sql, objConnection) { CommandType = CommandType.Text })
                {
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader = (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        List<MaterialDetailsModel> objMaterialDetailsModels = new List<MaterialDetailsModel>();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                MaterialDetailsModel model = new MaterialDetailsModel
                                {
                                    MaterialId = Convert.ToInt32(objDataReader["MATERIAL_ID"].ToString()),
                                    MaterialName = objDataReader["MATERIAL_NAME"].ToString(),
                                    UpdateBy = objDataReader["UPDATE_BY"].ToString(),
                                    UpdateDate = objDataReader["UPDATE_DATE"].ToString(),
                                    CreateBy = objDataReader["CREATE_BY"].ToString(),
                                    CreateDate = objDataReader["CREATE_DATE"].ToString()
                                };
                                objMaterialDetailsModels.Add(model);
                            }
                            return objMaterialDetailsModels;
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


        public async Task<MaterialDetailsModel> GetAMaterial(int materialId)
        {

            var sql = "SELECT " +
                      "MATERIAL_ID," +
                      "MATERIAL_NAME," +
                      "UPDATE_BY," +
                      "UPDATE_DATE," +
                      "CREATE_BY," +
                      "CREATE_DATE " +
                      "FROM VEW_MATERIAL_DETAILS where MATERIAL_ID = :MATERIAL_ID";

            using (OracleConnection objConnection = GetConnection())
            {
                using (OracleCommand objCommand = new OracleCommand(sql, objConnection)
                { CommandType = CommandType.Text })
                {
                    objCommand.Parameters.Add(":MATERIAL_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = materialId;
                    await objConnection.OpenAsync();
                    using (OracleDataReader objDataReader =
                        (OracleDataReader)await objCommand.ExecuteReaderAsync())
                    {
                        MaterialDetailsModel objMaterialDetailsModels = new MaterialDetailsModel();
                        try
                        {
                            while (await objDataReader.ReadAsync())
                            {
                                objMaterialDetailsModels.MaterialId =
                                    Convert.ToInt32(objDataReader["MATERIAL_ID"].ToString());
                                objMaterialDetailsModels.MaterialName = objDataReader["MATERIAL_NAME"].ToString();
                                objMaterialDetailsModels.UpdateBy = objDataReader["UPDATE_BY"].ToString();
                                objMaterialDetailsModels.UpdateDate = objDataReader["UPDATE_DATE"].ToString();
                                objMaterialDetailsModels.CreateBy = objDataReader["CREATE_BY"].ToString();
                                objMaterialDetailsModels.CreateDate = objDataReader["CREATE_DATE"].ToString();
                            }

                            return objMaterialDetailsModels;
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

        public async Task<string> SaveAndUpdateMaterial(MaterialDetailsModel objMaterialDetailsModel)
        {
            string strMessage;

            OracleCommand objOracleCommand = new OracleCommand("PRO_MATERIAL_SAVE")
            {
                CommandType = CommandType.StoredProcedure
            };

            objOracleCommand.Parameters.Add("P_MATERIAL_ID", OracleDbType.Varchar2, ParameterDirection.Input).Value = objMaterialDetailsModel.MaterialId;
            objOracleCommand.Parameters.Add("P_MATERIAL_NAME", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMaterialDetailsModel.MaterialName) ? objMaterialDetailsModel.MaterialName.Trim() : null;
            objOracleCommand.Parameters.Add("P_UPDATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMaterialDetailsModel.UpdateBy) ? objMaterialDetailsModel.UpdateBy.Trim() : null;
            objOracleCommand.Parameters.Add("P_UPDATE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMaterialDetailsModel.UpdateDate) ? objMaterialDetailsModel.UpdateDate.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_BY", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMaterialDetailsModel.CreateBy) ? objMaterialDetailsModel.CreateBy.Trim() : null;
            objOracleCommand.Parameters.Add("P_CREATE_DATE", OracleDbType.Varchar2, ParameterDirection.Input).Value = !string.IsNullOrWhiteSpace(objMaterialDetailsModel.CreateDate) ? objMaterialDetailsModel.CreateDate.Trim() : null;
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

                    strMessage = objOracleCommand.Parameters["P_MESSAGE"].Value.ToString();
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