using API.Controllers;
using API.Models;
using Common;
using Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Moq;

namespace API.Test
{
    public class UsersControllerTest
    {
        private readonly UsersController _controller;

        public UsersControllerTest()
        {
            var repository = new Mock<IUserRepository>();
            repository.Setup(x => x.GetById(1))
                .Returns(new User { Id = 1, FirstName = "Amir", LastName = "Test" });
            _controller = new UsersController(repository.Object);
        }

        [Fact]
        public void Get_WhenCalled_ReturnsOkResult()
        {
            // Act
            var result = _controller.Get();

            // Assert
            Assert.IsType<ApiResult<IQueryable<UserSelectDto>>>(result);
        }

        [Fact]
        public void GetById_UnknownIdPassed_ReturnsNotFoundResult()
        {
            // Act
            var notFoundResult = _controller.Get(-1);
            // Assert
            Assert.Equal(ApiResultStatusCode.NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public void GetById_ExistingIdPassed_ReturnsOkResult()
        {
            // Arrange
            var testId = 1;
            // Act
            var result = _controller.Get(testId);
            // Assert
            Assert.IsType<ApiResult<UserSelectDto>>(result);
        }

        [Fact]
        public async void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var missingItem = new UserDto
            {
                UserName = "Test",
                FirstName = "Test"
            };
            _controller.ModelState.AddModelError("LastName", "Required");
            // Act
            var result = await _controller.Post(missingItem, new CancellationToken());
            // Assert
            Assert.Equal(ApiResultStatusCode.BadRequest, result.StatusCode);
        }

        [Fact]
        public async void Add_ValidObjectPassed_ReturnsCreatedResponse()
        {
            // Arrange
            var item = new UserDto
            {
                UserName = "Test",
                FirstName = "Test",
                Password = "123",
                LastName = "No"
            };
            // Act
            var result = await _controller.Post(item, new CancellationToken());
            // Assert
            Assert.IsType<ApiResult<User>>(result);
        }


    }
}