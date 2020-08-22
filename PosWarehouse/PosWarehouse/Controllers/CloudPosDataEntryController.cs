using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;
using static System.StringSplitOptions;

namespace PosWarehouse.Controllers
{
    public class CloudPosDataEntryController : Controller
    {
        CloudPosDataEntryDal _dal;

        public CloudPosDataEntryController()
        {
            _dal = new CloudPosDataEntryDal();
        }

        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> PostDataFile(CloudPosDataEntryModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["message"] = "File is required";
                return RedirectToAction("Index");
            }

            var databaseItem = await _dal.GetItemFromDatabase();
            var fileItem = new List<WareHouseBarcodeUpdate>();

            var fileExtension = Path.GetExtension(model.File.FileName);

            if (fileExtension != null)
            {
                fileExtension = fileExtension.ToUpper();

                if (fileExtension == ".CSV")
                {
                    using (var reader = new StreamReader(model.File.InputStream))
                    {
                        while (!reader.EndOfStream)
                        {
                            var dataFromFile = new WareHouseBarcodeUpdate();
                            var line = reader.ReadLine();
                            if (line != null)
                            {
                                char[] separator = {'-'};
                                int count = 3;
                                var values = line.Split(',');
                                dataFromFile.BarCode = values[0];
                                dataFromFile.Style = values[1];
                                dataFromFile.Attribute = values[5].Replace("\"", "").Split(separator, count).Select(p => p.Trim()).Take(2).ToArray();

                                fileItem.Add(dataFromFile);
                            }
                        }
                    }
                }
                else
                {
                    TempData["message"] = "File must be .CSV Format";
                    return RedirectToAction("Index");
                }
            }

            if (databaseItem != null)
            {
                if (fileItem.Count > 0)
                {
                    fileItem = fileItem.OrderBy(f => f.Style).ToList();

                    foreach (var databaseItemData in databaseItem)
                    {
                        if (databaseItemData.Attribute[0] != null && databaseItemData.Attribute[1] == null)
                        {
                            string databaseAttribute = databaseItemData.Attribute[0].Trim().ToLower();

                            foreach (var fileItemData in fileItem)
                            {
                                if (databaseItemData.Style.Trim().ToLower() == fileItemData.Style.Trim().ToLower())
                                {
                                    string fileAttribute = fileItemData.Attribute[0].Trim().ToLower();

                                    if (databaseAttribute == fileAttribute)
                                    {
                                        databaseItemData.BarCode = fileItemData.BarCode;
                                    }
                                }
                            }
                        }
                        else if(databaseItemData.Attribute[0] != null && databaseItemData.Attribute[1] != null)
                        {
                            string databaseAttribute = databaseItemData.Attribute[0].Trim().ToLower() + "," + databaseItemData.Attribute[1].Trim().ToLower();

                            foreach (var fileItemData in fileItem)
                            {
                                if (databaseItemData.Style.Trim().ToLower() == fileItemData.Style.Trim().ToLower())
                                {
                                    if (fileItemData.Attribute.Length == 2)
                                    {
                                        string fileAttribute = fileItemData.Attribute[0].Trim().ToLower() + "," + fileItemData.Attribute[1].Trim().ToLower();
                                        if (databaseAttribute == fileAttribute)
                                        {
                                            databaseItemData.BarCode = fileItemData.BarCode;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (databaseItem != null)
            {
                var message = "";
                foreach (var wareHouseBarcodeUpdate in databaseItem)
                {
                    message = await _dal.UpdateBarCodeInProductItem(wareHouseBarcodeUpdate);
                }
                TempData["message"] = message;
            }
                


            return RedirectToAction("Index");
        }
    }
}