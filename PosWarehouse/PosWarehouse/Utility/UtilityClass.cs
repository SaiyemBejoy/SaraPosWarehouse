using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PosWarehouse.DAL;
using PosWarehouse.ViewModel;
using ZXing;

namespace PosWarehouse.Utility
{
    public class UtilityClass
    {
        private static readonly DataExchangeDal _objDataExchangeDal = new DataExchangeDal();
        //all shop Url
        public static string[] ShopUrl = {
            "http://localhost:63780/Api/"
            //"http://192.168.2.76:40/Api/"//publish shop Api
        };

        //For dropdown list
        public static SelectList GetSelectListByDataTable(DataTable objDataTable, string pValueField, string pTextField)
    {
        List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--Select Item--"},
            };


        objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                                    select new SelectListItem()
                                    {
                                        Value = dataRow[pValueField].ToString(),
                                        Text = dataRow[pTextField].ToString()
                                    });

        return new SelectList(objSelectListItems, "Value", "Text");
    }
        //For dropdown list
        public static SelectList GetSelectListForShop(DataTable objDataTable, string pValueField, string pTextField)
        {
            List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--All Shop--"},
            };


            objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                select new SelectListItem()
                {
                    Value = dataRow[pValueField].ToString(),
                    Text = dataRow[pTextField].ToString()
                });

            return new SelectList(objSelectListItems, "Value", "Text");
        }
        public static SelectList GetSelectListForWarehouseName(DataTable objDataTable, string pValueField, string pTextField)
        {
            List<SelectListItem> objSelectListItems = new List<SelectListItem>
            {
                new SelectListItem() {Value = "", Text = "--Select Warehouse--"},
            };


            objSelectListItems.AddRange(from DataRow dataRow in objDataTable.Rows
                select new SelectListItem()
                {
                    Value = dataRow[pValueField].ToString(),
                    Text = dataRow[pTextField].ToString()
                });

            return new SelectList(objSelectListItems, "Value", "Text");
        }

        //For save an image
        public static string SaveBase64Image(HttpPostedFileBase image)
    {
        var imageExtension = Path.GetExtension(image.FileName);
        string base64Image = null;

        if (imageExtension != null)
        {
            imageExtension = imageExtension.ToUpper();

            if (imageExtension == ".JPG" || imageExtension == ".JPEG" || imageExtension == ".PNG")
            {
                Stream str = image.InputStream;
                BinaryReader br = new BinaryReader(str);
                Byte[] fileDet = br.ReadBytes((Int32)str.Length);
                base64Image = Convert.ToBase64String(fileDet);
            }
        }
        return base64Image;
    }

    //For display an image Compressed
    public static string GetBase64ImageCompressed(byte[] image)
    {
        string base64Image = null;

        if (image != null)
        {
            MemoryStream myMemStream = new MemoryStream(image);
            Image fullSizeImage = Image.FromStream(myMemStream);
            Image newImage = fullSizeImage.GetThumbnailImage(50, 50, null, IntPtr.Zero);
            MemoryStream myResult = new MemoryStream();
            newImage.Save(myResult, ImageFormat.Png);
            image = myResult.ToArray();

            base64Image = "data:image/png;base64," + Convert.ToBase64String(image);
        }
        return base64Image;
    }

    //For display an image
    public static string GetBase64Image(byte[] image)
    {
        string base64Image = null;

        if (image != null)
        {
            MemoryStream myMemStream = new MemoryStream(image);
            Image fullSizeImage = Image.FromStream(myMemStream);
            MemoryStream myResult = new MemoryStream();
            fullSizeImage.Save(myResult, ImageFormat.Png);
            image = myResult.ToArray();

            base64Image = "data:image/png;base64," + Convert.ToBase64String(image);
        }
        return base64Image;
    }

    //For Encrypt an Text
    public string Encrypt(string clearText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                clearText = Convert.ToBase64String(ms.ToArray());
            }
        }
        return clearText;
    }

    //For Decrypt an Text
    public string Decrypt(string cipherText)
    {
        string EncryptionKey = "MAKV2SPBNI99212";
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    //For Save an Image
    public static string SaveAnImage(HttpPostedFileBase file, string path)
    {
        string fileName = "";
        string vFileName = Guid.NewGuid().ToString();
        string fileExtension = Path.GetExtension(file.FileName)?.ToLower();
        if (fileExtension != null)
        {
            path += vFileName;
            path += fileExtension.ToLower();

            file.SaveAs(path);
            fileName = vFileName + fileExtension.ToLower();
        }
        return fileName;
    }

    //Generate All Combinations
    public static List<string> GenerateAllCombinationsOfName(List<List<string>> remaining)
    {
        if (remaining.Count() == 1) return remaining.First();
        else
        {
            var current = remaining.First();
            List<string> outputs = new List<string>();
            List<string> ids = GenerateAllCombinationsOfName(remaining.Skip(1).ToList());

            foreach (var cur in current)
                foreach (var id in ids)
                    outputs.Add(cur + " | " + id);

            return outputs;
        }
    }

    //Generate Barcode
    public static byte[] RenderBarcode(string code)
    {
        using (var ms = new MemoryStream())
        {
            var writer = new ZXing.BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
            writer.Options.Height = 40;
            writer.Options.Width = 180;
            writer.Options.PureBarcode = true;
            Image img = writer.Write(code);
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var byteArrayImage = ms.ToArray();

            return byteArrayImage;
        }
    }

}
}