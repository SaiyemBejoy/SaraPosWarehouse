using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PosWarehouse.Controllers.Report;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;

namespace PosWarehouse.Controllers
{
    [LogAction]
    public class PurchaseOrderController : Controller
    {

        //private readonly PromotionDAL _objPromotionDAL = new PromotionDAL();

        private readonly DropdownDAL _objDropdownDal = new DropdownDAL();
        private readonly PurchaseOrderDal _objPurchaseOrderDal = new PurchaseOrderDal();

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

        #region "PurchaseOrder Section"
        [RoleFilter]
        public async Task<ActionResult> PurchaseOrderList()
        {
            ModelState.Clear();
            Session["PurchaseItem"] = null;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }

            LoadSession();
            PurchaseOrderModel model = new PurchaseOrderModel();
            var objPurchaseOrderModel = await _objPurchaseOrderDal.PurchaseOrderList(_strWareHouseId, _strShopId);
            ViewBag.PurchaseOrderList = objPurchaseOrderModel;

            if (TempData.ContainsKey("OrderNumber"))
            {
                //PurchaseOrderReport report = new PurchaseOrderReport();
                //report.PurchaseOrderNumber = TempData["OrderNumber"] as string;
                //await Task.Run(() => GeneratePoNumberWiseDetailsForDireectAction(report));
                model.PurchaseOrderNumber = TempData["OrderNumber"] as string;
            }

            return View(model);
        }
        public async Task<JsonResult> GetPurchaseOrderItemList(string purchaseOrderNumber)
        {

            var objPurChaseOrderItemModel = await _objPurchaseOrderDal.GetPurchaseOrderItemList(purchaseOrderNumber);

            return Json(objPurChaseOrderItemModel, JsonRequestBehavior.AllowGet);
        }
        [RoleFilter]
        public async Task<ActionResult> PurchaseOrder()
        {
            if (TempData.ContainsKey("message"))
                ViewBag.message = TempData["message"] as string;

            PurchaseOrderModel model = new PurchaseOrderModel();

            if (Session["PurchaseItem"] != null)
                model.PurchaseItems = Session["PurchaseItem"] as List<PurchaseItem>;
            else
                model.PurchaseItems = new List<PurchaseItem>();

            ViewBag.VendorList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetVendorListDropdown(), "VENDOR_ID", "VENDOR_NAME");
            ViewBag.ShopList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetShopListDropdown(), "SHOP_ID", "SHOP_NAME");
            ViewBag.SeasonList = UtilityClass.GetSelectListByDataTable(await _objDropdownDal.GetSeasonListDropdown(), "SEASON_ID", "SEASON_NAME");

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SearchProduct(string searchKey)
        {
            LoadSession();

            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                var purchaseItem = await _objPurchaseOrderDal.GetItemList(searchKey, _strWareHouseId, _strShopId);

                if (Session["PurchaseItem"] != null)
                {
                    List<PurchaseItem> model = Session["PurchaseItem"] as List<PurchaseItem>;
                    foreach (var item in purchaseItem)
                    {
                        bool exists = true;
                        if (model != null)
                        {
                            foreach (var listItem in model)
                            {
                                if (listItem.Barcode == item.Barcode)
                                    exists = false;
                            }

                            if (exists)
                                model.Add(item);
                            else
                                TempData["message"] = "Item already exists";


                        }
                    }
                    Session["PurchaseItem"] = model;
                }
                else
                {
                    Session["PurchaseItem"] = purchaseItem;
                }              
            }
            else
                TempData["message"] = "Search Key can not be empty";

            return RedirectToAction("PurchaseOrder", "PurchaseOrder");
        }

        public async Task<JsonResult> GetSearchHintsList(string query)
        {
            List<string> list = await _objPurchaseOrderDal.GetProductGrids(query);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SavePurchaseOrder(PurchaseOrderModel objPurchaseOrderModel)
        {
            string message = "";
            ModelState.Remove("SearchKey");
            Session["PurchaseItem"] = null;
            var removeItemList = new List<int>();
            foreach (var itemList in objPurchaseOrderModel.PurchaseItems)
            {
                if(!itemList.IsActiveItem)
                    removeItemList.Add(itemList.ItemId);
            }

            objPurchaseOrderModel.PurchaseItems.RemoveAll(c => removeItemList.Any(i => i == c.ItemId));

            if (!string.IsNullOrWhiteSpace(objPurchaseOrderModel.OrderDate) && 
                !string.IsNullOrWhiteSpace(objPurchaseOrderModel.DeliveryDate) &&
                objPurchaseOrderModel.VendorId <= 0 &&
                objPurchaseOrderModel.DeliveryShopId <= 0 &&
                objPurchaseOrderModel.SeasonId <= 0)
            {
                if (objPurchaseOrderModel.PurchaseItems.Any())
                    TempData["PurchaseItem"] = objPurchaseOrderModel.PurchaseItems;

                return RedirectToAction("PurchaseOrder", "PurchaseOrder");
            }

            objPurchaseOrderModel.FabricModels = new List<FabricModel>();
            for (int i = 0; i < objPurchaseOrderModel.RowCount.Length; i++)
            {
                var fabricModel = new FabricModel();

                fabricModel.GarmentsType = objPurchaseOrderModel.GarmentsType[i];
                fabricModel.FabricType = objPurchaseOrderModel.FabricType[i];
                fabricModel.FabricCode = objPurchaseOrderModel.FabricCode[i];
                fabricModel.Consumption = objPurchaseOrderModel.Consumption[i];
                fabricModel.FabricQuantity = objPurchaseOrderModel.FabricQuantity[i];
                
                objPurchaseOrderModel.FabricModels.Add(fabricModel);
            }
            LoadSession();
            objPurchaseOrderModel.UpdateBy = _strEmployeeId;
            objPurchaseOrderModel.WareHouseId = _strWareHouseId;
            objPurchaseOrderModel.ShopId = _strShopId;

            var orderNumber = await _objPurchaseOrderDal.PurchaseOrderSave(objPurchaseOrderModel);

            if (!string.IsNullOrWhiteSpace(orderNumber))
            {
                string deleteMessage = await _objPurchaseOrderDal.PurchaseOrderItemDelete(orderNumber, _strWareHouseId, _strShopId);

                if (!string.IsNullOrWhiteSpace(deleteMessage))
                {
                    try
                    {
                         message = await _objPurchaseOrderDal.PurchaseOrderItemSave(objPurchaseOrderModel.PurchaseItems, orderNumber, _strEmployeeId, _strWareHouseId, _strShopId);
                        TempData["message"] = message;
                    }
                    catch (Exception e)
                    {
                         message = await _objPurchaseOrderDal.PurchaseOrderDelete(orderNumber, _strWareHouseId, _strShopId);
                        TempData["message"] = e + message;
                    }

                    if (message == "SUCCESS")
                    {
                        try
                        {
                            message = await _objPurchaseOrderDal.PurchaseOrderFabricSave(objPurchaseOrderModel.FabricModels, orderNumber, _strEmployeeId, _strWareHouseId, _strShopId);
                            TempData["message"] = message;
                        }
                        catch (Exception e)
                        {
                            message = await _objPurchaseOrderDal.PurchaseOrderAndItemDelete(orderNumber, _strWareHouseId, _strShopId);
                            TempData["message"] = e + message;
                        }
                    }
                   
                }
            }
            TempData["OrderNumber"] = orderNumber;

            return RedirectToAction("PurchaseOrderList", "PurchaseOrder");
            //return RedirectToAction("PurchaseOrder", "AllReport",new { purchaseOrderReport=objPurchaseOrderModel });
        }



        #endregion

        #region Report
        private readonly ReportDocument _objReportDocument = new ReportDocument();
        private ExportFormatType _objExportFormatType = ExportFormatType.NoFormat;
        private readonly ReportDal _objReportDal = new ReportDal();

        public ActionResult ShowReport(string poNumber)
        {
            _objExportFormatType = ExportFormatType.PortableDocFormat;
            ExportOptions option = new ExportOptions();
            option.ExportFormatType = ExportFormatType.PortableDocFormat;
            string strPath = Path.Combine(Server.MapPath("~/Reports/Sale/PurchaseOrderDetails.rpt"));
            _objReportDocument.Load(strPath);

            PurchaseOrderReport model = new PurchaseOrderReport();
            model.PurchaseOrderNumber = poNumber;

            DataSet objDataSet = (_objReportDal.PoNumberWiseDetails(model));

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


        public async Task<ActionResult> ReportForHtml(string poNumber)

        {
            //PurchaserOrderModelForReport purchaseOrderModelForReport = new PurchaserOrderModelForReport();
            //purchaseOrderModelForReport = await _objPurchaseOrderDal.GetPurchaseOrderInfoForHtmlRpt(poNumber);
            //purchaseOrderModelForReport.PurchaseItemList = await _objPurchaseOrderDal.PurchaseOrderItemListForHtmlRpt(poNumber);
            //foreach (var purchaseItemRpt in purchaseOrderModelForReport.PurchaseItemList)
            //{
            //    purchaseItemRpt.PurchaseItemColorRpt = await _objPurchaseOrderDal.PurchaseOrderColorListForHtmlRpt(poNumber, purchaseItemRpt.ProductId);
            //    purchaseItemRpt.SizeModelList = await _objPurchaseOrderDal.PurchaseOrderSizeListForHtmlRpt(poNumber, purchaseItemRpt.ProductId);
            //    purchaseItemRpt.FFitModelList = await _objPurchaseOrderDal.PurchaseOrderFitListForHtmlRpt(poNumber, purchaseItemRpt.ProductId);
            //}
            //purchaseOrderModelForReport.FabricModelList = await _objPurchaseOrderDal.PurchaseOrderFabricListForHtmlRpt(poNumber);
            ////purchaseOrderModelForReport.ColorModelList = await _objPurchaseOrderDal.PurchaseOrderColorListForHtmlRpt(poNumber);
            //purchaseOrderModelForReport.SizeModelListHeading = await _objPurchaseOrderDal.PurchaseOrderSizeListForHtmlHeadingRpt(poNumber);
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //var productInfo = await _objPurchaseOrderDal.GetAllProductColorSize();
            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds,
            //    ts.Milliseconds / 10);

            PurchaserOrderModelForReport purchaseOrderModelForReport = new PurchaserOrderModelForReport();
            purchaseOrderModelForReport = await _objPurchaseOrderDal.GetPurchaseOrderInfoForHtmlRpt(poNumber);
            purchaseOrderModelForReport.PurchaseItemList = await _objPurchaseOrderDal.PurchaseOrderItemListForHtmlRpt(poNumber);

            foreach (var item in purchaseOrderModelForReport.PurchaseItemList)
            {
                var productColorSize = await _objPurchaseOrderDal.GetAllProductColorSize(item.ProductId);

                item.PurchaseItemColorRpt = productColorSize.GroupBy(d => new { d.ColorId, d.ColorName }).Select(x => new PurchaseItemColorRpt()
                {
                    ColorId = x.Key.ColorId,
                    ColorName = x.Key.ColorName,
                    PurchaseOrderNumber = purchaseOrderModelForReport.PurchaseOrderNumber,
                    SizeModelList = productColorSize.Select(y => new PurchaseItemSizeRpt()
                    {
                        PurchaseOrderNumber = purchaseOrderModelForReport.PurchaseOrderNumber,
                        SizeId = y.SizeId,
                        SizeName = y.SizeName,
                        ColorId = y.ColorId,
                        Quantity = _objPurchaseOrderDal.GetItemQuantity(y.ItemId, purchaseOrderModelForReport.PurchaseOrderNumber)
                    }).Where(c => c.ColorId == x.Key.ColorId).ToList()
                }).ToList();
            }
            purchaseOrderModelForReport.FabricModelList = await _objPurchaseOrderDal.PurchaseOrderFabricListForHtmlRpt(poNumber);
            //purchaseOrderModelForReport.ColorModelList = await _objPurchaseOrderDal.PurchaseOrderColorListForHtmlRpt(poNumber);
            purchaseOrderModelForReport.SizeModelListHeading = await _objPurchaseOrderDal.PurchaseOrderSizeListForHtmlHeadingRpt(poNumber);
            //purchaseOrderModelForReport.SizeModelList = await _objPurchaseOrderDal.PoSizeListForHtmlRpt(poNumber);
            return View(purchaseOrderModelForReport);
        }
        #endregion
    }

    #region Test

    public class ProductColorSizeList
    {
        public int ProductId { get; set; }
        public string StyleName { get; set; }
        public int ItemId { get; set; }
        public int ColorId { get; set; }
        public string ColorName { get; set; }
        public int SizeId { get; set; }
        public string SizeName { get; set; }
    }

    #endregion
}