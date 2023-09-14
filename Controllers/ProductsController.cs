using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet("kustuta/{index}")]
        public List<Product> Delete(int index)
        {
            _products.RemoveAt(index);
            return _products;
        }

        [HttpGet("lisa/{id}/{name}/{image}/{price}/{active}/{stock}/{catId}")]
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

        [HttpGet("hind-dollaritesse/{kurss}")] 
        public List<Product> Dollaritesse(double kurss)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                _products[i].Price = _products[i].Price * kurss;
            }
            return _products;
        }

        [HttpGet("hind-eurosse/{kurss}")]
        public List<Product> Eurosse(double kurss)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                _products[i].Price = _products[i].Price * kurss;
            }
            return _products;
        }
    }
}
