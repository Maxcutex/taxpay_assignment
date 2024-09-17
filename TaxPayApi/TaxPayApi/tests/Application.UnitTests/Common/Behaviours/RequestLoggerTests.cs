using TaxPayApi.Application.Common.Behaviours;
using TaxPayApi.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TaxPayApi.Application.Accounts.Commands.CreateAccountList;
using TaxPayApi.Application.Transaction.Commands.CreateTransaction;
using TaxPayApi.Domain.Enums;

namespace TaxPayApi.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    private Mock<ILogger<CreateAccountCommand>> _logger = null!;
    private Mock<ICurrentUserService> _currentUserService = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<CreateAccountCommand>>();
        _currentUserService = new Mock<ICurrentUserService>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _currentUserService.Setup(x => x.UserId).Returns(Guid.NewGuid().ToString());

        var requestLogger =
            new LoggingBehaviour<CreateAccountCommand>(_logger.Object, _currentUserService.Object,
                _identityService.Object);

        await requestLogger.Process(new CreateAccountCommand { AccountId = "0000000000", Title = "title", Type = AccountType.Tax}, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger =
            new LoggingBehaviour<CreateAccountCommand>(_logger.Object, _currentUserService.Object,
                _identityService.Object);

        await requestLogger.Process(new CreateAccountCommand { AccountId = "0000000000", Title = "title", Type = AccountType.Tax}, new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}