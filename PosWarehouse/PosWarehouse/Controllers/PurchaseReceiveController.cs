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
    public class PurchaseReceiveController : Controller
    {
        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly PurchaseReceiveDal _objPurchaseReceiveDal = new PurchaseReceiveDal();
        private readonly ProductDal _objProductDalDal = new ProductDal();

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

        #region "PurchaseReceive Section"
        [RoleFilter]
        public async Task<ActionResult> PurchaseReceiveList()
        {
            ModelState.Clear();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            PurchaseReceiveModel model = new PurchaseReceiveModel();
            var objPurchaseReceiveModel = await _objPurchaseReceiveDal.PurchaseReceiveList(_strWareHouseId, _strShopId);
            ViewBag.PurchaseReceiveList = objPurchaseReceiveModel;

            return View(model);
        }

        public async Task<JsonResult> GetPurchaseReceiveItemList(string purchaseReceiveId)
        {

            var objPurChaseReceiveItemModel = await _objPurchaseReceiveDal.GetPurchaseReceiveItemList(purchaseReceiveId);

            return Json(objPurChaseReceiveItemModel, JsonRequestBehavior.AllowGet);
        }
        [RoleFilter]
        public async Task<ActionResult> PurchaseReceive()
        {
            PurchaseReceiveModel model = new PurchaseReceiveModel();
            ViewBag.VendorList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetVendorListDropdown(), "VENDOR_ID", "VENDOR_NAME");
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.PurchaseOrderList = GetSelectListByDataTableForAddStaticValue(await _objDropdownDal.GetPurchaseOrderNumberDropdown(), "PURCHASE_ORDER_ID", "PURCHASE_ORDER_NUMBER");
            ViewBag.OtherPurchaseReceiveList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetOtherPurchaseReceiveNumberDropdown(), "O_PURCHASE_RECEIVE_NUMBER", "O_PURCHASE_RECEIVE_NUMBER");
            //var objDirectPurchaseReceiveProduct = await _objProductDalDal.GetAllProductItemForDirectPurchaseReceive(_strWareHouseId, _strShopId);
            //ViewBag.DirectPurchaseReceiveProduct = objDirectPurchaseReceiveProduct;
            return View(model);
        }

        public async Task<JsonResult> GetSearchHintsList(string query)
        {
            List<string> list = await _objPurchaseReceiveDal.GetProductGrids(query);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        //This dropdownlist Use for Get Daynamic Value shathe Static Value Add Korar jonno .

        private static SelectList GetSelectListByDataTableForAddStaticValue(DataTable objDataTable, string pValueField, string pTextField)
        {
            List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--Select Item--"},
                new SelectListItem() {Value = "0", Text = "Direct"}
            };
            objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                                        select new SelectListItem()
                                        {
                                            Value = dataRow[pValueField].ToString(),
                                            Text = dataRow[pTextField].ToString()
                                        });

            return new SelectList(objSelectListItems, "Value", "Text");
        }
        //end

        public async Task<ActionResult> SearchProductByOrderNumber(string purchaseOrderNumber)
        {
            var list = await _objPurchaseReceiveDal.SearchProductByOrderNumber(purchaseOrderNumber);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetInfoByotherPurchaseReceiveNumber(string OtherPurchaseReceiveNumber)
        {
            var list = await _objPurchaseReceiveDal.GetProductInfoForSOLScanReceive(OtherPurchaseReceiveNumber);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetAllProductItemForDirectPurchaseReceive(DataTableAjaxPostModel model)
        {
            var data = await _objPurchaseReceiveDal.GetAllProductItemForDirectPurchaseReceive(model);

            //var data = await _objPurchaseReceiveDal.GetAllProductItemForDirectPurchaseReceive(model);

            var recordsFiltered = data.Count > 0 ? data[0].TotalItem : data.Count;

            int recordsTotal = data.Count;

            if (recordsTotal < model.length)
                recordsTotal = model.length;

            //data.ToList();
            //data = model.length == -1 ? data.ToList() : data.Skip(model.start).Take(model.length).ToList();
            //ViewBag.DirectPurchaseReceiveProduct = objDirectPurchaseReceiveProduct;
            //return Json(objDirectPurchaseReceiveProduct, JsonRequestBehavior.AllowGet);
            return Json(new { model.draw, recordsTotal, recordsFiltered, data }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SaveAllPurchaseReceiveItem(PurchaseReceiveModel objPurchaseReceiveModel)
        {
            LoadSession();
            string returnMessage = "";
            string purchaseReceiveNumber = "";
            //if (ModelState.IsValid)
            //{
            objPurchaseReceiveModel.UpdateBy = _strEmployeeId;
            objPurchaseReceiveModel.WareHouseId = _strWareHouseId;
            objPurchaseReceiveModel.ShopId = _strShopId;

            try
            {
                foreach (var poNumber in objPurchaseReceiveModel.PoNumberList)
                {
                    if (poNumber.ReceiveItemsList == null)
                        poNumber.ReceiveItemsList = new List<ReceiveItem>();
                    foreach (var receiveItem in objPurchaseReceiveModel.ReceiveItemsList)
                    {
                        if (poNumber.PurchaseOrderNumber == receiveItem.PurchaseOrderNumber)
                        {
                            poNumber.ReceiveItemsList.Add(receiveItem);
                        }
                    }

                }

                if (objPurchaseReceiveModel.PoNumberList != null)
                {
                    foreach (var asd in objPurchaseReceiveModel.PoNumberList)
                    {
                        objPurchaseReceiveModel.PurchaseOrderNumber = asd.PurchaseOrderNumber;
                        //Direct Receive Korar jonno And Sol Er Challan Receive Korar jonno
                        if (asd.PurchaseOrderNumber =="Direct" || objPurchaseReceiveModel.SOLReceiveYN ==true)
                        {
                            asd.DeliveryShopId = objPurchaseReceiveModel.DeliveryShopId;
                        }
                        else
                        {
                           objPurchaseReceiveModel.DeliveryShopId = asd.DeliveryShopId;
                        }
                        //End
                        objPurchaseReceiveModel.PurchaseOrderId = asd.PurchaseOrderId;
                         var strMessageData = await _objPurchaseReceiveDal.SavePurchaseReceive(objPurchaseReceiveModel);
                         purchaseReceiveNumber = strMessageData.Item2;
                        if (strMessageData.Item1 != null && strMessageData.Item2 != null)
                        {
                            foreach (var tableData in asd.ReceiveItemsList)
                            {
                                tableData.WareHouseId = _strWareHouseId;
                                tableData.ShopId = _strShopId;
                                tableData.PurchaseReceiveNumber = strMessageData.Item2;
                                tableData.PurchaseReceiveId = Convert.ToInt32(strMessageData.Item1);
                                returnMessage = await _objPurchaseReceiveDal.SavePurchaseReceiveItemList(tableData);
                            }
                            if (strMessageData.Item1 != null)
                            {
                                objPurchaseReceiveModel.PurchaseReceiveNumber = strMessageData.Item2;
                                //Sol er Challan Update Korar jonno
                                var updateOtherPurchaseReceive = await _objPurchaseReceiveDal.UpdateOtherPurchaseReceive(objPurchaseReceiveModel.OtherPurchaseReceiveNumber, objPurchaseReceiveModel.UpdateBy);
                            }
                          
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error : " + ex.Message);
            }

            // }
            var messageAndReload = new
            {
                purchaseReceiveNumber = purchaseReceiveNumber,
                m = returnMessage,
                isRedirect = true,
                redirectUrl = Url.Action("PurchaseReceiveList")
            };
            return Json(messageAndReload, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> SelectProductItemByCheckBox(int itemId)
        {
            //var list = itemId;
            var list = await _objPurchaseReceiveDal.SelectProductItemByCheckBox(itemId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetMaxChallanNumber()
        {
            var maxChallanNumber = await _objPurchaseReceiveDal.GetMaxChallanNumber();
            return Json(maxChallanNumber, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetProductInfoForScanReceive(string barcode)
        {
            var list = await _objPurchaseReceiveDal.GetProductInfoForScanReceive(barcode);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public async Task<ActionResult> ShowReport(string purchaseReceiveNumber)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/POReceive/PurchaseReceive.rpt"));
            _objReportDocument.Load(strPath);

            DataSet objDataSet = (await _objReportDal.PurchaseReceiveNumberWiseDetails(purchaseReceiveNumber));

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

            string pFileDownloadName = "PurReceiveReport.pdf";

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