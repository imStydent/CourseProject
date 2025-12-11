using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ScheduleApp.Data;
using ScheduleApp.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Google.Protobuf.WellKnownTypes.Field.Types;

namespace ScheduleApp.Controllers
{
    [Authorize]
    public class ScheduleController : Controller
    {
        private readonly Data.Context _context;

        public ScheduleController(Data.Context context)
        {
            _context = context;
        }


        // GET: ScheduleController
        public async Task<IActionResult> Index(int page = 1)
        {
            var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleId");
            if (roleIdClaim == null)
                return View("~/Views/Account/Login.cshtml");

            int roleId = int.Parse(roleIdClaim.Value);

            TempData.Clear();
            int pageSize = 7;
            var context = _context.LoadUnloadOperations.Include(o => o.Orders.OrderBy(o => o.LoadTime)).ThenInclude(o => o.ProductsHasOrders).OrderBy(o => o.Date).Skip((page - 1) * pageSize).Take(pageSize);

            ViewBag.TotalCount = _context.LoadUnloadOperations.Count();
            ViewBag.CurrentPage = page;

            TempData["roleId"] = roleId;
            return View(await context.ToListAsync());
        }

        [HttpPost, ActionName("ExportAsync")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExportAsync(int id)
        {
            var operation = await _context.LoadUnloadOperations.Include(l => l.Orders).ThenInclude(o => o.ProductsHasOrders)
                .ThenInclude(p => p.Products).FirstOrDefaultAsync(l => l.Id == id);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Лист1");
                int row = 1;
                foreach (var order in operation.Orders)
                {
                    var headerStyle = worksheet.Cell(1, 1).Style;
                    headerStyle.Font.Bold = true;
                    headerStyle.Fill.BackgroundColor = XLColor.AshGrey;
                    headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 1).Style = headerStyle;
                    worksheet.Cell(row, 2).Style = headerStyle;
                    worksheet.Cell(row, 3).Style = headerStyle;
                    worksheet.Columns().AdjustToContents();

                    worksheet.Cell(row, 1).Value = order.Name;
                    worksheet.Cell(row , 2).Value = order.LoadTime;
                    worksheet.Cell(row , 3).Value = order.UnloadTime;

                    foreach (var product in order.ProductsHasOrders)
                    {
                        worksheet.Cell(row + 1, 1).Value = product.Products.Name;
                        worksheet.Cell(row + 1, 2).Value = product.Products.Kind;
                        worksheet.Cell(row + 1, 3).Value = product.Amount;
                        row++;
                    }
                    row++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var fileName = $"{operation.Date}.xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        // GET: ScheduleController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Orders
                .Include(l => l.ProductsHasOrders).ThenInclude(o => o.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleId");
            if (roleIdClaim == null)
                return View("~/Views/Account/Login.cshtml");

            int roleId = int.Parse(roleIdClaim.Value);
            TempData["roleId"] = roleId;

            return View(worker);
        }

        [HttpPost, ActionName("EditOrderAsync")]
        public async Task<ActionResult> EditOrderAsync(EditOrderViewModel editOrderViewModel)
        {
            var order = await _context.Orders.Include(o => o.ProductsHasOrders).ThenInclude(o => o.Products).FirstOrDefaultAsync(o => o.Id == editOrderViewModel.Order.Id);

            order.Name = editOrderViewModel.Order.Name;
            order.LoadTime = editOrderViewModel.Order.LoadTime;
            order.UnloadTime = editOrderViewModel.Order.UnloadTime;
            _context.Orders.Update(order);

            _context.SaveChanges();
            var viewModel = new EditOrderViewModel();
            viewModel.Order = order;
            viewModel.Products = _context.Products.ToList();

            return View("Edit", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> AddProductToOrder(int productId, int orderId, short amount)
        {
            var order = await _context.Orders
                    .Include(o => o.ProductsHasOrders).ThenInclude(o => o.Products) // Предполагаем, что есть навигационное свойство
                    .FirstOrDefaultAsync(o => o.Id == orderId);

            var productHasOrder = await _context.ProductsHasOrders
            .FirstOrDefaultAsync(ph => ph.OrdersId == orderId && ph.ProductsId == productId);

            if (productHasOrder != null)
            {
                // Если запись существует, обновляем количество
                productHasOrder.Amount += amount;
            }
            else
            {
                // Если записи нет, создаем новую
                var newProductHasOrder = new ProductsHasOrder
                {
                    OrdersId = orderId,
                    ProductsId = productId,
                    Amount = amount
                };
                await _context.ProductsHasOrders.AddAsync(newProductHasOrder);
            }

            await _context.SaveChangesAsync(); // Сохранение изменений в БД

            var viewModel = new EditOrderViewModel();
            viewModel.Order = order;
            viewModel.Products = _context.Products.ToList();

            return View("Edit", viewModel);
        }

        [HttpPost]
        public ActionResult AddProduct(Models.AddProduct addProduct, DateTime date, TimeSpan loadTime, TimeSpan unloadTime, int selectedProductId, string name, short amount /*DateTime date, TimeSpan loadTime, TimeSpan unloadTime, int productId*/)
        {
            var jsonData = TempData["OrderCreateViewModel"] as string;
            var viewModel = JsonConvert.DeserializeObject<OrderCreateViewModel>(jsonData);
            if(name == null) 
            {
                ViewBag.ErrorMessage = "Название заказа обязательно для заполнения";
                TempData["OrderCreateViewModel"] = JsonConvert.SerializeObject(viewModel);
                return View("Create", viewModel);
            }
            Product product = _context.Products.Where(p => p.Id == selectedProductId).FirstOrDefault();
            if (viewModel.AddProducts == null)
            {
                viewModel.AddProducts = new List<AddProduct>();
            }
            viewModel.Name = name;
            viewModel.Date = date;
            viewModel.UnloadTime = unloadTime;
            viewModel.LoadTime = loadTime;
            var addedProduct = new Models.AddProduct { Id = selectedProductId, Amount = amount, Kind = product.Kind, Name = product.Name, PiecesPerBox = product.PiecesPerBox };
            if (viewModel.AddProducts.Any(p => p.Id == addedProduct.Id))
            {
                var existProduct = viewModel.AddProducts.FirstOrDefault(p => p.Id == addedProduct.Id);
                existProduct.Amount += amount;
            }
            else
                viewModel.AddProducts.Add(new Models.AddProduct { Id = selectedProductId, Amount = amount, Kind = product.Kind, Name = product.Name, PiecesPerBox = product.PiecesPerBox });

            TempData["OrderCreateViewModel"] = JsonConvert.SerializeObject(viewModel);
            return View("Create", viewModel);
        }

        public ActionResult AddOrder()
        {
            var jsonData = TempData["OrderCreateViewModel"] as string;
            var viewModel = JsonConvert.DeserializeObject<OrderCreateViewModel>(jsonData);

            if(name == null) 
            {
                ViewBag.ErrorMessage = "Название заказа обязательно для заполнения";
                TempData["OrderCreateViewModel"] = JsonConvert.SerializeObject(viewModel);
                return View("Create", viewModel);
            }
            int lastId = _context.LoadUnloadOperations.Max(p => p.Id);
            var loadUnlodOperation = new LoadUnloadOperation { Date = viewModel.Date, Id = lastId + 1 };
            var order = new Order { Name = viewModel.Name, LoadTime = viewModel.LoadTime, UnloadTime = viewModel.UnloadTime };
            foreach (var product in viewModel.AddProducts)
            {
                order.ProductsHasOrders.Add(new ProductsHasOrder { ProductsId = product.Id, Amount = product.Amount });
            }
            
            if (!_context.LoadUnloadOperations.Any(p => p.Date == viewModel.Date))
            {
                loadUnlodOperation.Orders.Add(order);
                _context.LoadUnloadOperations.Add(loadUnlodOperation);
            }
            else
            {
                var existLoad = _context.LoadUnloadOperations.FirstOrDefault(p => p.Date == viewModel.Date);
                existLoad.Orders.Add(order);
                _context.SaveChanges();
            }
            _context.SaveChanges();
            TempData.Clear();
            return RedirectToAction("Index");
        }
        // GET: ScheduleController/Create
        public ActionResult Create()
        {
            //var model = TempData["OrderCreateViewModel"] as OrderCreateViewModel ?? new OrderCreateViewModel();
            var jsonData = TempData["OrderCreateViewModel"] as string;
            var viewModel = new OrderCreateViewModel();
            if (!string.IsNullOrEmpty(jsonData))
            {
                viewModel = JsonConvert.DeserializeObject<OrderCreateViewModel>(jsonData);
            }
            viewModel.Products = _context.Products.ToList();
            TempData["OrderCreateViewModel"] = JsonConvert.SerializeObject(viewModel);
            //TempData["OrderCreateViewModel"] = model;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ClearOrder()
        {
            TempData.Clear();
            return RedirectToAction("Create");
        }

        // GET: ScheduleController/Edit/5
        public ActionResult Edit(int id)
        {
            var viewModel = new EditOrderViewModel();

            var order = _context.Orders.Include(o => o.ProductsHasOrders).ThenInclude(o => o.Products).FirstOrDefault(o => o.Id == id); // Поиск заказа
            
            if(order == null)// Проверка существования заказа
                return NotFound();

            //Формирорвание модели отображения для представления
            viewModel.Order = order;
            viewModel.Products = _context.Products.ToList();
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var loadUnload = await _context.LoadUnloadOperations.Include(l => l.Orders).ThenInclude(o => o.ProductsHasOrders).FirstOrDefaultAsync(l => l.Id == id);
            if (loadUnload != null)
            {
                _context.RemoveRange(loadUnload.Orders);
                _context.LoadUnloadOperations.Remove(loadUnload);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("DeleteOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders.Include(o => o.ProductsHasOrders).ThenInclude(o => o.Products).FirstOrDefaultAsync(l => l.Id == id);
            if (order != null)
            {
                _context.RemoveRange(order.ProductsHasOrders);
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("DeleteProduct")]
        public async Task<ActionResult> DeleteProduct(int productId, int orderId, bool fromCreate = true)
        {
            if (fromCreate)
            {
                var jsonData = TempData["OrderCreateViewModel"] as string;
                var viewModel = JsonConvert.DeserializeObject<OrderCreateViewModel>(jsonData);

                var productForDelete = viewModel.AddProducts.FirstOrDefault(ad => ad.Id == productId);
                viewModel.AddProducts.Remove(productForDelete);

                TempData["OrderCreateViewModel"] = JsonConvert.SerializeObject(viewModel);
                return View("Create", viewModel);
            }
            else
            {
                // Получение заказа с продуктами
                var order = await _context.Orders
                    .Include(o => o.ProductsHasOrders).ThenInclude(o => o.Products) // Предполагаем, что есть навигационное свойство
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order != null)
                {
                    // Поиск продукта, который нужно удалить
                    var productHasOrder = await _context.ProductsHasOrders
                        .FirstOrDefaultAsync(ph => ph.OrdersId == orderId && ph.ProductsId == productId);
                    if (productHasOrder != null)
                    {
                        order.ProductsHasOrders.Remove(productHasOrder);
                        await _context.SaveChangesAsync(); // Сохранение изменений в БД
                    }
                }

                var viewModel = new EditOrderViewModel();
                viewModel.Order = order;
                viewModel.Products = _context.Products.ToList();
                return View("Edit", viewModel);
            }
        }
    }
}
