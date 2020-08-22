using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class ReturnReceiveController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly StockTransferDal _objStockTransferDal = new StockTransferDal();
        private readonly DamageTransferDal _objDamageTransferDal = new DamageTransferDal();
        private readonly StockReturnReceiveDal _objStockReturnReceiveDal = new StockReturnReceiveDal();

        #region "Common"
        private string _strEmployeeId = "";
        private string _strWareHouseId = "";
        private string _strShopId = "";

        public void LoadSession()
        {
            var auth = Session["authentication"] as AuthModel;

            if (auth != null)
            {
                _strEmployeeId = auth.EmployeeId;
                _strWareHouseId = auth.WareHouseId;
                _strShopId = auth.ShopId;
            }
            else
            {
                string url = Url.Action("Index", "Auth");
                if (url != null) Response.Redirect(url);
            }
        }

        #endregion
        [RoleFilter]
        public async Task<ActionResult> ReceiveList()
        {
            var receiveList = await _objStockReturnReceiveDal.GetReceivelist();
            
            ViewBag.returnReceiveList = receiveList;
            return View();
        }
        [RoleFilter]
        public async Task<ActionResult> ReturnReceive()
        {
            StockReceiveModel model = new StockReceiveModel();

            ViewBag.StockTransferChallanNo = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetStockTransferChallanNo(), "STOCK_TRANSFER_CHALLAN_NUM", "STOCK_TRANSFER_CHALLAN_NUM");
            return View(model);
        }
        public async Task<ActionResult> GetAllTransferInfoByChallanNo(string stockTransferChallanNo)
        {
            var data = await _objStockTransferDal.GetAllStockTarnsferInfo(stockTransferChallanNo);
            if (data != null)
            {
                int stockTransferId = data.StockTransferId;
                data.StockTransferItemList = await _objStockTransferDal.GetAllStockTarnsferItemInfo(stockTransferId);
            }
            else
                return Json(0, JsonRequestBehavior.AllowGet);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllDamageInfoByChallanNo(string damageChallan)
        {
            var data = await _objDamageTransferDal.GetAllDamageTransferMainInfo(damageChallan);
            if (data != null)
            {
                int damageTransferId = data.DamageTransferId;
                data.DamageTransferItemList = await _objDamageTransferDal.GetAllDamageTransferItemInfo(damageTransferId);
            }
            else
                return Json(0, JsonRequestBehavior.AllowGet);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllReceiveItemByChallanNo(string challanNum)
        {
            List<StockReceiveItemModel> data = new List<StockReceiveItemModel>();
            var dataId = await _objStockReturnReceiveDal.GetAStockReceiveInfo(challanNum);
            if (dataId != "")
            {
                data = await _objStockReturnReceiveDal.GetAllReceiveItemByChallanNo(dataId);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveReceiveItem(StockReceiveModel objStockReceiveModel)
        {
            string returnMessage = "";
            LoadSession();
            objStockReceiveModel.ReceivedBy = _strEmployeeId;
            var shopUrl = await _objStockReturnReceiveDal.GetShopUrlForUpdate(objStockReceiveModel.StockTransferFromShopId);

            if (shopUrl != null)
            {
                if (objStockReceiveModel.DamageTransferChallanNo == "0")
                {
                    var challanNo = objStockReceiveModel.StockTransferChallanNo;
                    var updateShop = await _objStockReturnReceiveDal.UpdateShopReceive(shopUrl, challanNo);
                    if (updateShop != null)
                    {
                        var data = await _objStockReturnReceiveDal.SaveStockReturnReceive(objStockReceiveModel);
                        if (data != null)
                        {
                            foreach (var stockReceiveItem in objStockReceiveModel.StockReceiveItemList)
                            {
                                stockReceiveItem.StockReceiveId = Convert.ToInt32(data);
                                returnMessage = await _objStockReturnReceiveDal.SaveStockReturnReceiveItem(stockReceiveItem);
                            }

                        }
                    }
                }
                else
                {
                    var damageChallanNo = objStockReceiveModel.DamageTransferChallanNo;
                    var damageRequisitionNo = objStockReceiveModel.RequisitionNo;
                    var updateDmShop = await _objStockReturnReceiveDal.UpdateShopForDmgReceive(shopUrl, damageRequisitionNo);
                    if (updateDmShop =="OK")
                    {                    
                        string employeeId = _strEmployeeId;
                        returnMessage = await _objStockReturnReceiveDal.SaveDamageReturnReceive(damageChallanNo, employeeId);
                    }
                }
                
               
            }
            var messageAndReload = new
            {
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("ReceiveList")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllDamageChallan()
        {
            var data = await _objStockReturnReceiveDal.GetAllDamageChallan();
     
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(string challanNum)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/ReturnReceive/ReturnReceiveChallanNumWise.rpt"));
            _objReportDocument.Load(strPath);

            StockReceiveModel model = new StockReceiveModel();
            model.StockTransferChallanNo = challanNum;

            DataSet objDataSet = (await _objReportDal.ChallanNumberWiseReturnReceiveDetails(model));

            _objReportDocument.Load(strPath);
            _objReportDocument.SetDataSource(objDataSet);
            _objReportDocument.SetDatabaseLogon("POSWAREHOUSE", "POSWAREHOUSE");

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Clear();
            Response.Buffer = true;

            _objExportFormatType = ExportFormatType.PortableDocFormat;

            Stream oStream = _objReportDocument.ExportToStream(_objExportFormatType);
            byte[] byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

            Response.ContentType = "application/pdf";

            string pFileDownloadName = "poReport.pdf";

            Response.BinaryWrite(byteArray);
            Response.Flush();
            Response.Close();
            _objReportDocument.Close();
            _objReportDocument.Dispose();

            return File(oStream, Response.ContentType, pFileDownloadName);
        }

        #endregion

    }
}