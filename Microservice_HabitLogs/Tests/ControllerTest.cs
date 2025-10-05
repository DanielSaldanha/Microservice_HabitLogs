using Microservice_HabitLogs.Controllers;
using Microservice_HabitLogs.Data;
using Microservice_HabitLogs.Model;
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

    }
}
