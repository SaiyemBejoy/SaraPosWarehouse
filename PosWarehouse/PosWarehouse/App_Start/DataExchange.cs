using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using PosWarehouse.ViewModel;
using Quartz;

namespace PosWarehouse
{
    public class DataExchange
    {
        public static void timer_Elapsed()
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60;
            timer.Elapsed += GetSetOrderedProduct;
            timer.Start();
        }

        private static void GetSetOrderedProduct(object sender, System.Timers.ElapsedEventArgs e)
        {
            IEnumerable<DeliveredProduct> product = null;
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:62672/Api/");
                    //HTTP GET
                    var responseTask = client.GetAsync("DeliveredProduct");
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<DeliveredProduct>>();
                        readTask.Wait();

                        product = readTask.Result;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}