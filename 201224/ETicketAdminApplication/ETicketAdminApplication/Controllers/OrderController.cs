using ETicketAdminApplication.Models;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ETicketAdminApplication.Controllers
{
    public class OrderController : Controller
    {


        public OrderController()
        {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

        }
        public IActionResult Index()
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44331/API/Admin/getOrders";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var data = response.Content.ReadAsAsync<List<Order>>().Result;

            return View(data);
        }




        public IActionResult Details(Guid orderId)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44331/API/Admin/getOrderDetails";

            var model = new
            {
                Id = orderId
            };

            HttpContent content =new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;

            return View();

        }


        public FileContentResult CreateInvoice(Guid orderId)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:44331/API/Admin/getOrderDetails";

            var model = new
            {
                Id = orderId
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            var result = response.Content.ReadAsAsync<Order>().Result;

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");

            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", result.Id.ToString());
            document.Content.Replace("{{UserName}}", result.UserId.ToString());
            StringBuilder stringBuilder = new StringBuilder();
            var total = 0.0;
            foreach (var item in result.Tickets) 
            {
                total += item.SelectedTicket.TicketPrice;
                stringBuilder.AppendLine(item.SelectedTicket.TicketName + ",");
            }

            document.Content.Replace("{{TicketsList}}", stringBuilder.ToString());
            document.Content.Replace("{{TicketsPrice}}", total.ToString());

            var stream = new MemoryStream();


            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(),new PdfSaveOptions().ContentType,"ExportInvoice.pdf");

        }

        

        


    }
}
