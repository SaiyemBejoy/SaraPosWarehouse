using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PosWarehouse.DAL;
using PosWarehouse.Utility;
using PosWarehouse.ViewModel;
using PosWarehouse.ViewModel.ApiModel;
using Quartz;

namespace PosWarehouse
{
    public class JobClass : IJob
    {
        private readonly DataExchangeDal _objDataExchangeDal = new DataExchangeDal();

        public Task Execute(IJobExecutionContext context)
        {
            var value1 = GetSaleCustomerListAsync();
            var value2 = GetAllSaleInfoAsync();
            var value3 = GetAllWarehouseRequisitionAsync();
            var value4 = GetAllShopToShopExReceiveDataAsync();
            return null;
        }

        private async Task GetSaleCustomerListAsync()
        {
            //string[] shopUrl = UtilityClass.ShopUrl;
            var shopUrl = await _objDataExchangeDal.GetAllShopUrlWithId();

            using (var client = new HttpClient())
            {
                foreach (var url in shopUrl)
                {
                    if (url.ShopUrl != "")
                    {
                        client.BaseAddress = new Uri(url.ShopUrl);
                        var responseTask = client.GetAsync("SaleCustomerInfo");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<IList<CustomerSaleModel>>();
                            readTask.Wait();

                            IEnumerable<CustomerSaleModel> customer = readTask.Result;
                            foreach (var value in customer)
                            {
                                var SaleCustomerModel = new
                                {
                                    CustomerId = value.CustomerId,
                                    ShopId = value.ShopId
                                };
                                HttpResponseMessage response = await client.PostAsJsonAsync("SaleCustomerInfo", SaleCustomerModel);
                                response.EnsureSuccessStatusCode();
                                if (response.IsSuccessStatusCode)
                                {
                                    value.ShopId = Convert.ToString(url.ShopId);
                                    var message = await _objDataExchangeDal.SaveAndUpdateCustomerSale(value);
                                }
                            }

                        }
                    }
                }

            }
        }

        private async Task<int> SaveSaleInfoData(List<SaleInfoModel> saleInfo, string url)
        {
            int isSaved = 0;
            if (saleInfo.Any())
            {
                foreach (var value in saleInfo)
                {
                    var saleInfoAutoId = await _objDataExchangeDal.SaveSaleInfo(value);

                    if (saleInfoAutoId != null)
                    {
                        foreach (var saleItem in value.SaleItemList)
                        {
                            saleItem.SaleInfoAutoId = Convert.ToInt32(saleInfoAutoId);
                            await _objDataExchangeDal.SaveSaleInfoItem(saleItem);
                        }

                        foreach (var paymentInfo in value.SalePaymentInfoList)
                        {
                            paymentInfo.SaleInfoAutoId = Convert.ToInt32(saleInfoAutoId);
                            await _objDataExchangeDal.SaveSalePaymentInfo(paymentInfo);
                        }

                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);

                            var saleInfoModel = new
                            {
                                SaleInfoId = value.SaleInfoId,
                                InvoiceNumber = value.InvoiceNumber,
                                ShopId = value.ShopId
                            };

                            HttpResponseMessage postResponse = await client.PostAsJsonAsync("SaleInfo", saleInfoModel);
                            postResponse.EnsureSuccessStatusCode();

                            if (postResponse.IsSuccessStatusCode)
                                isSaved = 1;
                        }
                    }
                }
            }
            return isSaved;
        }

        private async Task GetAllSaleInfoAsync()
        {
            var shopUrl = await _objDataExchangeDal.GetAllShopUrl();

            foreach (var url in shopUrl)
            {
                if (url != "")
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);

                            using (var response = await client.GetAsync("SaleInfo"))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    using (var content = response.Content)
                                    {
                                        var result = await content.ReadAsAsync<List<SaleInfoModel>>();
                                        var serviceResult = await SaveSaleInfoData(result, url);

                                        if (serviceResult > 0)
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }

                }
            }


        }

        private async Task GetAllWarehouseRequisitionAsync()
        {
            //string[] shopUrl = UtilityClass.ShopUrl;
            var shopUrl = await _objDataExchangeDal.GetAllShopUrl();
            using (var client = new HttpClient())
            {
                foreach (var url in shopUrl)
                {
                    if (url != "")
                    {

                        client.BaseAddress = new Uri(url);
                        var responseTask = client.GetAsync("WarehouseRequisition");
                        responseTask.Wait();

                        var result = responseTask.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            var readTask = result.Content.ReadAsAsync<IList<RequisitionMainModel>>();
                            readTask.Wait();

                            IEnumerable<RequisitionMainModel> requisition = readTask.Result;
                            foreach (var value in requisition)
                            {
                                var message = await _objDataExchangeDal.SaveRequisitionMain(value);
                                if (message != null)
                                {
                                    foreach (var itemValue in value.RequisitionMainItemList)
                                    {
                                        await _objDataExchangeDal.SaveRequisitionMainItem(itemValue);
                                    }
                                }

                            }
                        }
                    }
                }

            }
        }

        private async Task GetAllShopToShopExReceiveDataAsync()
        {
            //string[] shopUrl = UtilityClass.ShopUrl;
            //var shopUrl = await _objDataExchangeDal.GetAllShopUrlWithId();
            var shopUrl = await _objDataExchangeDal.GetAllShopUrl();
            foreach (var url in shopUrl)
            {
                if (url != "")
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(url);
                            var responseTask = client.GetAsync("ShopToShopEXProduct");
                            responseTask.Wait();
                            var result = responseTask.Result;
                            if (result.IsSuccessStatusCode)
                            {
                                var readTask = result.Content.ReadAsAsync<IList<ShopToShopExchangeMain>>();
                                readTask.Wait();

                                IEnumerable<ShopToShopExchangeMain> exReceiveData = readTask.Result;
                                foreach (var value in exReceiveData)
                                {
                                    var shopToShopExchangeMain = new
                                    {
                                        StoreReceiveChallanNo = value.StoreReceiveChallanNo
                                    };
                                    HttpResponseMessage response =
                                        await client.PostAsJsonAsync("ShopToShopEXProduct", shopToShopExchangeMain);
                                    response.EnsureSuccessStatusCode();
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var message = await _objDataExchangeDal.SaveShopToShopExchangeMain(value);
                                        if (message != null)
                                        {
                                            foreach (var receiveItem in value.ShopToShopExItemList)
                                            {
                                                receiveItem.StoreReceiveAutoId = Convert.ToInt32(message);
                                                await _objDataExchangeDal.SaveShopToShopExchangeMainItem(receiveItem);
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                   
                }
            }


        }

    }
}