using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using ProductAPIVS.Controllers;
using ProductAPIVS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ProductAPIVS.Handler;
 
namespace ProductAPIVS.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        private Learn_DBContext _dbContext;
        private JwtSettings _jwtSettings;
        private IRefereshTokenGenerator _refreshTokenGenerator;
 
        [SetUp]
        public void Setup()
        {
            // Mocking dependencies
            var dbContextOptions = new DbContextOptionsBuilder<Learn_DBContext>().UseInMemoryDatabase(databaseName: "InMemoryDatabase").Options;
            _dbContext = new Learn_DBContext(dbContextOptions);
 
            _jwtSettings = new JwtSettings
            {
                securitykey = "A_Strong_Secret_Key_With_At_Least_32_Bytes"
            };
 
            var refreshTokenGeneratorMock = new Mock<IRefereshTokenGenerator>();
            refreshTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<string>())).ReturnsAsync("mocked_refresh_token");
 
            _refreshTokenGenerator = refreshTokenGeneratorMock.Object;
        }
 
//         [Test]
// public async Task Authenticate_ValidCredentials_ReturnsOk()
// {
//     // Arrange
//     var userController = new UserController(_dbContext, Options.Create(_jwtSettings), _refreshTokenGenerator);
//     var userCred = new UserCred { username = "sssssssssssssbuchy", password = "buchsssssssssssssssssyma" };
 
//     // Setup the in-memory database with a user that matches the credentials
//     _dbContext.TblUsers.Add(new TblUser { Userid = userCred.username, Password = userCred.password, Role = "admin" });
//     await _dbContext.SaveChangesAsync();
 
//     // Act
//     var result = await userController.Authenticate(userCred) as OkObjectResult;
 
//     // Assert
//     // Assert.IsNotNull(result, "Result is expected to be not null");
//     // Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode, "Expected HTTP 200 OK status");
//        Assert.NotNull(result);
//        Assert.AreEqual(200, result.StatusCode);
//     // Add more assertions as needed based on the expected response content
//     // For example, you might want to check the content of the result object.
// }
[Test]
public async Task Authenticate_ValidCredentials_ReturnsOk()
{
    // Arrange
    var userController = new UserController(_dbContext, Options.Create(_jwtSettings), _refreshTokenGenerator);
 
    // Use incorrect credentials intentionally
    var validUserCred = new UserCred { username = "buchy", password = "buchy" };
 
    // Setup the in-memory database with a valid user
    _dbContext.TblUsers.Add(new TblUser { Userid = "buchy", Password = "buchy", Role = "admin" });
    await _dbContext.SaveChangesAsync();
 
    // Act
    var resultValid = await userController.Authenticate(validUserCred) as OkObjectResult;
 
    // Assert
    Assert.NotNull(resultValid);
    Assert.AreEqual(200, resultValid.StatusCode);
}
 
[Test]
public async Task Authenticate_InvalidCredentials_ReturnsUnauthorized()
{
    // Arrange
    var userController = new UserController(_dbContext, Options.Create(_jwtSettings), _refreshTokenGenerator);
    var invalidUserCred = new UserCred { username = "invalid_username", password = "invalid_password" };
 
    // Act
    var resultInvalid = await userController.Authenticate(invalidUserCred) as UnauthorizedResult;
 
    // Assert
    Assert.NotNull(resultInvalid);
    Assert.AreEqual(StatusCodes.Status401Unauthorized, resultInvalid.StatusCode);
}
 
    }
}
