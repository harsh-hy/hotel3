using System.Text.Json;
using HotelManagement.Shared.Exceptions;
using HotelManagement.Shared.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Shared.Tests;

[TestFixture]
public class GlobalExceptionMiddlewareTests
{
    private Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
    }

    [Test]
    public async Task InvokeAsync_WhenNotFoundExceptionThrown_Returns404WithProblemDetails()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new NotFoundException("Room not found");
        var middleware = new GlobalExceptionMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status404NotFound));
        Assert.That(context.Response.ContentType, Is.EqualTo("application/problem+json"));

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(responseText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.That(problemDetails, Is.Not.Null);
        Assert.That(problemDetails!.Status, Is.EqualTo(404));
        Assert.That(problemDetails.Title, Is.EqualTo("Not Found"));
        Assert.That(problemDetails.Detail, Is.EqualTo("Room not found"));
    }

    [Test]
    public async Task InvokeAsync_WhenBadRequestExceptionThrown_Returns400WithProblemDetails()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new BadRequestException("Invalid date range");
        var middleware = new GlobalExceptionMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.That(responseText, Does.Contain("Invalid date range"));
    }

    [Test]
    public async Task InvokeAsync_WhenConflictExceptionThrown_Returns409WithProblemDetails()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new ConflictException("Room already booked");
        var middleware = new GlobalExceptionMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.That(responseText, Does.Contain("Room already booked"));
    }

    [Test]
    public async Task InvokeAsync_WhenUnauthorizedExceptionThrown_Returns401WithProblemDetails()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new UnauthorizedException("Invalid credentials");
        var middleware = new GlobalExceptionMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status401Unauthorized));
    }

    [Test]
    public async Task InvokeAsync_WhenGenericExceptionThrown_Returns500WithoutLeakingDetails()
    {
        // Arrange
        RequestDelegate next = (ctx) => throw new Exception("Database connection failure at secret server IP 10.0.0.1");
        var middleware = new GlobalExceptionMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.That(context.Response.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        Assert.That(responseText, Does.Not.Contain("10.0.0.1"));
        Assert.That(responseText, Does.Contain("An unexpected error occurred. Please try again later."));
    }
}
