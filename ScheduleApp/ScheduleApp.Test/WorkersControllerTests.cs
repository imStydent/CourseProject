using ClosedXML.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using ScheduleApp.Controllers;
using ScheduleApp.Data;
using ScheduleApp.Models;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ScheduleApp.Test
{
    public class WorkersControllerTests
    {
        private readonly Context _context;

        public WorkersControllerTests()
        {
            var options = new DbContextOptionsBuilder<Context>()
            .UseMySQL("server=localhost;database=test_database;user=root;password=root")
            .Options;
            
            _context = new Context(options);

            // Удаление данных из тестовой базы данных перед каждым тестом
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            SeedDatabase();
        }

           private void SeedDatabase()
            {
                _context.Roles.AddRange(new List<Role>
                {
                    new Role { Name = "Администратор" },
                    new Role { Name = "Работник" }
                });
            _context.SaveChanges();


            _context.Workers.AddRange(new List<Worker>
                {
                    new Worker { Id = 1, Name = "Алиса", Surname = "Попова", Patronymic = "Вячеславовна", Password = "12345", PhoneNumber = "89600045055", Role = _context.Roles.First(r => r.Name == "Администратор") },
                    new Worker { Id = 2, Name = "Витя", Surname = "Газов", Patronymic = "Вячеславович", Password = "54321", PhoneNumber = "89600045550", Role = _context.Roles.First(r => r.Name == "Работник") },
                    new Worker { Id = 3, Name = "Чарли", Surname = "Пыжиков", Patronymic = "Артёмович", Password = "45321", PhoneNumber = "89600045555",Role = _context.Roles.First(r => r.Name == "Администратор") },
                    new Worker { Id = 4, Name = "Дима",Surname = "Кузькин", Patronymic = "Дмитриевич", Password = "12543", PhoneNumber = "89600040500", Role = _context.Roles.First(r => r.Name == "Работник") }
                });

                _context.SaveChanges();
            }

        [Fact]
        public async Task Index_ReturnsWithFilteredSearchedAndSortedWorkers()
        {
            // Arrange
            // Создаем экземпляр контроллера WorkersController, передавая контекст базы данных
            var controller = new WorkersController(_context);

            // Устанавливаем начальные параметры для фильтрации, поиска и сортировки
            string filter = "Администратор";       // Фильтр для поиска работников с ролью "Администратор"
            string searchString = "Алиса";          // Строка для поиска работника по имени "Алиса"
            string sortOrder = "name_desc";        // Параметр сортировки по убыванию имени

            // Act
            // Получаем результат метода Index с фильтром по роли
            var filteredResult = await controller.Index(sortOrder, null, filter) as ViewResult;
            var filteredModel = filteredResult.Model as List<Worker>;

            // Получаем результат метода Index с поиском по имени
            var searchedResult = await controller.Index(null, searchString, null) as ViewResult;
            var searchedModel = searchedResult.Model as List<Worker>;

            // Получаем результат метода Index с сортировкой по имени
            var sortedResult = await controller.Index(sortOrder, null, null) as ViewResult;
            var sortedModel = sortedResult.Model as List<Worker>;

            // Assert
            // Проверяем, что результат фильтрации корректен
            Assert.NotNull(filteredResult);
            Assert.Equal(2, filteredModel.Count); // Ожидаем 2 работников с ролью "Администратор"
            Assert.All(filteredModel, w => Assert.Equal("Администратор", w.Role.Name)); // Проверяем, что все работники имеют роль "Администратор"

            // Проверяем, что результат поиска корректен
            Assert.NotNull(searchedResult);
            Assert.Single(searchedModel); // Ожидаем, что найдётся только один работник по имени "Алиса"
            Assert.Equal("Алиса", searchedModel[0].Name); // Проверяем, что найденный работник – именно "Алиса"

            // Проверяем, что результат сортировки корректен
            Assert.NotNull(sortedResult);
            Assert.Equal("Чарли", sortedModel[0].Name); // Ожидаем, что первым будет "Чарли" после сортировки по убыванию
            Assert.Equal("Дима", sortedModel[1].Name);
            Assert.Equal("Витя", sortedModel[2].Name);
            Assert.Equal("Алиса", sortedModel[3].Name); // Ожидаем, что последним будет "Алиса"
        }
    }
}