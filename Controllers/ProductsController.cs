using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using veebipoe.Models;

namespace veebipoe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new()
        {
        new Product(1,"Koola", 1.5, "Koola.png", true, 5, 1),
        new Product(2,"Fanta", 1.0, "Fanta.png", false, 1, 1),
        new Product(3,"Sprite", 1.7, "Sprite.png",true, 7, 1),
        new Product(4,"Vichy", 2.0, "Vichy.png", true, 14, 1),
        new Product(5,"Vitamin well", 2.5, "Vitamin_well.png", true , 3, 1)
        };

        // https://localhost:4444/tooted
        [HttpGet]
        public List<Product> Get()
        {
            return _products;
        }

        // https://localhost:4444/tooted/kustuta/0
        [HttpDelete("kustuta/{index}")]
        public List<Product> Delete(int index)
        {
            _products.RemoveAt(index);
            return _products;
        }

        [HttpPost("lisa/{id}/{name}/{image}/{price}/{active}/{stock}/{catId}")]
        public List<Product> Add(int id, string name, string image, double price, bool active, int stock,int catId)
        {
            Product product = new Product(id, name, price, image, active, stock, catId);
            _products.Add(product);
            return _products;
        }

        /*[HttpGet("lisa")] // GET /tooted/lisa?id=1&nimi=Koola&price=1.5&aktiivne=true
        public List<Product> Add2([FromQuery] int id, [FromQuery] string name, [FromQuery] string image, [FromQuery] double price, [FromQuery] bool active, [FromQuery] int stock, [FromQuery] int catId)
        {
            Product product = new Product(id, name, price, image, active, stock, catId);
            _products.Add(product);
            return _products;
        }*/

        [HttpPatch("hind-dollaritesse/{kurss}")] 
        public List<Product> Dollaritesse(double kurss)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                _products[i].Price = _products[i].Price * kurss;
            }
            return _products;
        }

        [HttpPatch("hind-eurosse/{kurss}")]
        public List<Product> Eurosse(double kurss)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                _products[i].Price = _products[i].Price * kurss;
            }
            return _products;
        }

        [HttpGet("pay/{sum}/{id}")]
        public async Task<IActionResult> MakePayment(string sum, int id)
        {
            var paymentData = new
            {
                api_username = "e36eb40f5ec87fa2",
                account_name = "EUR3D1",
                amount = sum,
                order_reference = Math.Ceiling(new Random().NextDouble() * 999999),
                nonce = $"a9b7f7e7as{DateTime.Now}{new Random().NextDouble() * 999999}",
                timestamp = DateTime.Now,
                customer_url = "https://maksmine.web.app/makse"
            };

            var json = JsonSerializer.Serialize(paymentData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "ZTM2ZWI0MGY1ZWM4N2ZhMjo3YjkxYTNiOWUxYjc0NTI0YzJlOWZjMjgyZjhhYzhjZA==");

            var response = await client.PostAsync("https://igw-demo.every-pay.com/api/v4/payments/oneoff", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseContent);
                var paymentLink = jsonDoc.RootElement.GetProperty("payment_link");
                _products[id].Stock = _products[id].Stock - 1;
                if (_products[id].Stock==0)
                {
                    _products[id].Active = false;
                }
                return Ok(paymentLink);
            }
            else
            {
                return BadRequest("Payment failed.");
            }
        }
    }
}
