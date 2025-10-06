using Microservice_HabitLogs.Controllers;
using Microservice_HabitLogs.Data;
using Microservice_HabitLogs.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace Microservice_HabitLogs.Tests
{
    public class ControllerTest
    {
        private LogsController _controller;
        private Mock<DbSet<Habit>> _mockSet;
        private Mock<AppDbContext> _mockContext;


        [SetUp]
        public void Setup()
        {
            _mockSet = new Mock<DbSet<Habit>>();
            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _controller = new LogsController(_mockContext.Object);
        }

        [Test]
        public async Task Logs_returnsCreated_WhenLogIsCreated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            // adicionar um hábito no contexto
            var habit = new Habit
            {
                name = "Beber água",
                goalType = GoalType.Count,
                goal = 1,
                clientId = "carlo"
            };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateLog(habit.Id, "carlo");

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual(201, createdResult.StatusCode);
        }

        [Test]
        public async Task Claim_returnsNoContent_WhenGetAnBadge()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // adicionar um hábito no contexto
            var badge = new Badge
            {
                name = "Gym",
                habitid = 2,
                clientId = "carlo",
                starter = 1,
                consistency = 3,
                date = hoje.AddDays(-1)
            };
            context.Badges.Add(badge);
            await context.SaveChangesAsync();

            context.HabitsLogs.AddRange(
                new Logs { Id = 1, HabitId = 1, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 1, clientId = "carlo" },
                new Logs { Id = 2, HabitId = 2, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 5, clientId = "carlo" },
                new Logs { Id = 3, HabitId = 3, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 10, clientId = "carlo" },
                new Logs { Id = 4, HabitId = 4, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 6, clientId = "carlo" },
                new Logs { Id = 5, HabitId = 5, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 7, clientId = "carlo" },
                new Logs { Id = 6, HabitId = 6, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 3, clientId = "carlo" },
                new Logs { Id = 7, HabitId = 7, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 2, clientId = "carlo" },
                new Logs { Id = 8, HabitId = 8, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 4, clientId = "carlo" },
                new Logs { Id = 9, HabitId = 9, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 8, clientId = "carlo" },
                new Logs { Id = 10, HabitId = 10, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 11, clientId = "carlo" }
            );
            await context.SaveChangesAsync();

            var habit = new Habit
            {
                name = "Gym",
                goalType = GoalType.Count,
                goal = 5,
                clientId = "carlo"
            };
            context.Habits.Add(habit);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateBadge(2, "carlo");

            // Assert
            var NoContentResult = result as NoContentResult;
            Assert.IsNotNull(NoContentResult);
            Assert.AreEqual(204, NoContentResult.StatusCode);
        }

        [Test]
        public async Task Claim_returnsCreated_WhenIsCreated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // adicionar um hábito no contexto
            var badge = new Badge
            {
                name = "Gym",
                habitid = 2,
                clientId = "carlo",
                starter = 1,
                consistency = 3,
                date = hoje.AddDays(-1)
            };
            context.Badges.Add(badge);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateBadge(2, "carlo");

            // Assert
            var CreatedResult = result as CreatedAtActionResult;
            Assert.IsNotNull(CreatedResult);
            Assert.AreEqual(201, CreatedResult.StatusCode);
        }

        [Test]
        public async Task Claim_returnsBadRequest_WhenDateIsLong()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // adicionar um hábito no contexto
            var badge = new Badge
            {
                name = "Gym",
                habitid = 2,
                clientId = "carlo",
                starter = 1,
                consistency = 3,
                date = hoje
            };
            context.Badges.Add(badge);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateBadge(2, "carlo");

            // Assert
            var BadResult = result as BadRequestObjectResult;
            Assert.IsNotNull(BadResult);
            Assert.AreEqual(400, BadResult.StatusCode);
            Assert.AreEqual(BadResult.Value, "Você já garantiu sua constancia por hoje");
        }

        [Test]
        public async Task Claim_returnsBadRequest_WhenDateIsSmall()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // adicionar um hábito no contexto
            var badge = new Badge
            {
                name = "Gym",
                habitid = 2,
                clientId = "carlo",
                starter = 1,
                consistency = 3,
                date = hoje.AddDays(-10)
            };
            context.Badges.Add(badge);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateBadge(2, "carlo");

            // Assert
            var BadResult = result as BadRequestObjectResult;
            Assert.IsNotNull(BadResult);
            Assert.AreEqual(400, BadResult.StatusCode);
            Assert.AreEqual(BadResult.Value, "Você não conseguiu manter sua consistencia");
        }

        [Test]
        public async Task Claim_returnsNotFound_WhenIsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            var controller = new LogsController(context);

            // Act
            var result = await controller.CreateBadge(2, "carlo");

            // Assert
            var NotFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(NotFoundResult);
            Assert.AreEqual(404, NotFoundResult.StatusCode);
            Assert.AreEqual(NotFoundResult.Value, "Você não possui tal tarefa");
        }

        [Test]
        public async Task Weekly_returnsOk_WheIsFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            context.HabitsLogs.AddRange(
                new Logs { Id = 1, HabitId = 1, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 1, clientId = "carlo" },
                new Logs { Id = 2, HabitId = 2, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 5, clientId = "carlo" },
                new Logs { Id = 3, HabitId = 3, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 10, clientId = "carlo" },
                new Logs { Id = 4, HabitId = 4, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 6, clientId = "carlo" },
                new Logs { Id = 5, HabitId = 5, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 7, clientId = "carlo" },
                new Logs { Id = 6, HabitId = 6, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 3, clientId = "carlo" },
                new Logs { Id = 7, HabitId = 7, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 2, clientId = "carlo" },
                new Logs { Id = 8, HabitId = 8, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 4, clientId = "carlo" },
                new Logs { Id = 9, HabitId = 9, name = "Gym", date = hoje, goalType = GoalType.Count, amount = 8, clientId = "carlo" },
                new Logs { Id = 10, HabitId = 10, name = "Gym", date = hoje, goalType = GoalType.Bool, amount = 11, clientId = "carlo" }
            );
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            //act
            var result = await controller.GetByWeekly("carlo");

            //assert
            // Assert
            var OkResult = result as OkObjectResult;
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);
        }

        [Test]
        public async Task Weekly_returnsNotFound_WheNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var controller = new LogsController(context);

            //act
            var result = await controller.GetByWeekly("carlo");

            //assert
            // Assert
            var NotFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(NotFoundResult);
            Assert.AreEqual(404, NotFoundResult.StatusCode);
        }

        [Test]
        public async Task GetBadge_returnsBronze_WhenIsFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            // adicionar um hábito no contexto
            context.Badges.AddRange(
                new Badge{name = "Gym",habitid = 2,clientId = "carlo",starter = 1,consistency = 3,date = hoje.AddDays(-1), badge = Badg3.Bronze},
                new Badge { name = "Gym", habitid = 2, clientId = "carlo", starter = 1, consistency = 3, date = hoje.AddDays(-1), badge = Badg3.Silver },
                new Badge { name = "Gym", habitid = 2, clientId = "carlo", starter = 1, consistency = 3, date = hoje.AddDays(-1), badge = Badg3.Gold }
                );
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            //act
            var resultBronze = await controller.GetByBadge("bronze", "carlo");
            var resultSilver = await controller.GetByBadge("prata", "carlo");
            var resultGold = await controller.GetByBadge("ouro", "carlo");

            //assret
            var OkResult = resultBronze as OkObjectResult;
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);

            OkResult = resultSilver as OkObjectResult;
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);

            OkResult = resultGold as OkObjectResult;
            Assert.IsNotNull(OkResult);
            Assert.AreEqual(200, OkResult.StatusCode);



        }

        [Test]
        public async Task VerifyAmount_returnsNoContent_WhenUpdated()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var hoje = DateOnly.FromDateTime(DateTime.Now);

            var log = new Logs
            {
                name = "Gym",
                HabitId = 2,
                clientId = "carlo",
                goalType = GoalType.Bool,
                amount = 10,
                date = hoje.AddDays(-7)
            };
            context.HabitsLogs.Add(log);
            await context.SaveChangesAsync();

            var controller = new LogsController(context);

            //act
            var result = await controller.VerifyAmountsFromLogs("carlo");

            //assert
            var NoContentResult = result as NoContentResult;
            Assert.IsNotNull(NoContentResult);
            Assert.AreEqual(204, NoContentResult.StatusCode);

        }

        [Test]
        public async Task VerifyAmount_returnsNotFound_WhenNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_CreateLog_Success")
                .Options;

            using var context = new AppDbContext(options);

            var controller = new LogsController(context);

            //act
            var result = await controller.VerifyAmountsFromLogs("carlo");

            //assert
            var NotFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(NotFoundResult);
            Assert.AreEqual(404, NotFoundResult.StatusCode);
            Assert.AreEqual(NotFoundResult.Value, "nenhum log realizado nesta semana");

        }

    }
}
