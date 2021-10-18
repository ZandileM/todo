using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using Todo.API.Controllers;
using Todo.API.Data.Entities;
using Todo.API.Models.Auth;
using Todo.API.Services;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
namespace Todo.API.UnitTests.AdminTest
{
   
    public class AdminTests
    {
        private const string MOCK_TOKEN_SIG = "rNZQndjf1Bf-11c9A7qELIKEiDPTTFBonflNgBN-cCk";
        private static string MOCK_ADMIN_TOKEN =
            $"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAdGVzdC5jb20iLCJuYW1laWQiOiI5OSIsImp0aSI6IjRkNmI4MTEwLTMxODYtNDc3ZC1iMDc5LTVlOWIzMGJkNjNkOCIsInJvbGUiOiJBZG1pbiIsImV4cCI6MTYwNTk2NTAwNn0.{MOCK_TOKEN_SIG}";
        private DefaultHttpContext _mockHttpContext;


        //Begin Print Set-Up;
        private  ITestOutputHelper _testOutputHelper;
        private  AdminTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        public AdminTests()
        {
            this._mockHttpContext = new DefaultHttpContext();
        }
        //Close Print Set-up


        private Mock<IUserService> CreateMockUserService()
        {
            var mockUserService = new Mock<IUserService>();

            mockUserService.Setup(m => m.GetUserById(It.IsAny<long>())).ReturnsAsync((long id) => {
                if (id != -1)
                {
                    return new User()
                    {
                        Id = id,
                        Email = "test@test.com",
                        UserName = "test@test.com"
                    };
                }
                return null;
            });
            mockUserService.Setup(m => m.GetUsers()).Returns((new List<User>() {
                new User() { Id = 1, Email = "test1@test.com", UserName = "test1@test.com" },
                new User() { Id = 1, Email = "test2@test.com", UserName = "test2@test.com" },
                new User() { Id = 1, Email = "test3@test.com", UserName = "test3@test.com" }
            }).AsQueryable());

            return mockUserService;
        }

        private UserController CreateTestController(IUserService userService, bool forAdmin = true)
        {
            var testController = new UserController(CreateMockUserService().Object);
            var mockControllerContext = new ControllerContext() { HttpContext = _mockHttpContext };

            _mockHttpContext.Request.Headers["Authorization"] = $"bearer {MOCK_ADMIN_TOKEN}";
            testController.ControllerContext = mockControllerContext;

            return testController;
        }
        [Fact]
       public void GivenUserAdminRole_WhenGettingUsers_ReturnAllUsers()
        {
            
            //Arrange
            var testController = CreateTestController(CreateMockUserService().Object);
            var mockUserService = new Mock<IUserService>();

            //Act
            var result = testController.GetUsers();
            var users = ((OkObjectResult)result).Value as List<UserDto>;
            var expected = 3;

            //Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(users.Count, expected);
            Assert.Equal(users.OfType<UserDto>().Count(), expected);
           
            
           //_testOutputHelper.WriteLine("List of All users", users.Count);

        }


    } 
}
