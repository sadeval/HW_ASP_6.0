using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Models;

namespace OnlineStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProductController> _logger;

        public ProductController(AppDbContext context, ILogger<ProductController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает список всех товаров в виде JSON.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            _logger.LogInformation("Получен список всех товаров. Количество: {Count}", products.Count);
            return Json(products);
        }

        /// <summary>
        /// Возвращает форму для добавления нового товара.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Добавляет новый товар через POST запрос.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Добавлен новый товар: {ProductName} с ID {ProductId}", product.Name, product.Id);
                return RedirectToAction(nameof(Details), new { id = product.Id });
            }
            _logger.LogWarning("Некорректные данные при добавлении товара.");
            return View(product);
        }

        /// <summary>
        /// Ищет товары по ключевому слову и возвращает их в виде JSON.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                _logger.LogWarning("Попытка поиска без ключевого слова.");
                return BadRequest("Ключевое слово для поиска обязательно.");
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword))
                .ToListAsync();

            _logger.LogInformation("Поиск товаров по ключевому слову '{Keyword}'. Найдено: {Count}", keyword, products.Count);
            return Json(products);
        }

        /// <summary>
        /// Отображает подробную информацию о товаре по его идентификатору в виде JSON.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Товар с ID {ProductId} не найден.", id);
                return NotFound($"Товар с ID {id} не найден.");
            }

            _logger.LogInformation("Получены детали для товара {ProductName} с ID {ProductId}.", product.Name, product.Id);
            return Json(product);
        }

        /// <summary>
        /// Удаляет товар по его идентификатору через POST запрос и возвращает удалённый товар в виде JSON.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                _logger.LogWarning("Попытка удалить несуществующий товар с ID {ProductId}.", id);
                return NotFound($"Товар с ID {id} не найден.");
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Товар {ProductName} с ID {ProductId} удалён.", product.Name, product.Id);
            return Json(product);
        }
    }
}
