
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using smartquote.api.Controllers;
using smartquote.api.DTOs.Quotes;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;
using smartquote.api.Services.Interfaces;
using smartquote.api.Validators;

namespace smartquote.tests.Unit.Controllers;

public class QuoteControllerTests
{
    private readonly Mock<IQuoteService> _quoteService;
    private readonly IValidator<CreateQuoteRequestDto> _createQuoteValidator;

    private readonly QuotesController _quoteController;

    public QuoteControllerTests()
    {
        _quoteService = new Mock<IQuoteService>();
        _createQuoteValidator = new CreateQuoteRequestDtoValidator();

        _quoteController = new QuotesController(
            _quoteService.Object,
            _createQuoteValidator);
    }

    [Fact]
    public async Task CreateQuote_ShouldReturnOkWithCreateQuoteResponseDto_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateQuoteRequestDto
        {
            Customer = "John Doe",
            UserId = "c20f35c3-7427-4705-b4bc-642c7c68309b",
        };

        var expectedResponse = new CreateQuoteResponseDto
        {
            QuoteId = 1,
        };

        _quoteService
            .Setup(s => s.CreateQuoteAsync(request))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _quoteController.CreateQuote(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<CreatedAtActionResult>();
        (result as CreatedAtActionResult)!.Value.Should().BeEquivalentTo(expectedResponse);
        (result as CreatedAtActionResult)!.StatusCode.Should().Be(201);
        _quoteService.Verify(s => s.CreateQuoteAsync(request), Times.Once);
    }

    [Fact]
    public async Task CreateQuote_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateQuoteRequestDto
        {
            Customer = "",
            UserId = "invalid-guid",
        };
        // Act
        Func<Task> act = async () => await _quoteController.CreateQuote(request);
        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        _quoteService.Verify(s => s.CreateQuoteAsync(It.IsAny<CreateQuoteRequestDto>()), Times.Never);
    }

    [Fact]
    public async Task GetAllQuotes_ShouldReturnOkWithQuotesResponseDto_WhenPageNumberAndSizeAreValid()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 10;
        var expectedResponse = new QuotesResponseDto
        {
            Quotes = new List<QuoteDto>
            {
                new QuoteDto
                {
                    Id = 1,
                    Customer = "John Doe",
                    Total = 0,
                    UserId = "c20f35c3-7427-4705-b4bc-642c7c68309b"
                }
            },
            TotalCount = 1,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        _quoteService
            .Setup(s => s.GetQuotesAsync(pageNumber, pageSize))
            .ReturnsAsync(expectedResponse);
        // Act
        var result = await _quoteController.GetAllQuotes(pageNumber, pageSize);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedResponse);
        (result as OkObjectResult)!.StatusCode.Should().Be(200);
        _quoteService.Verify(s => s.GetQuotesAsync(pageNumber, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetAllQuotes_ShouldReturnBadRequest_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var pageNumber = 0;
        var pageSize = 10;
        // Act
        var result = await _quoteController.GetAllQuotes(pageNumber, pageSize);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Page number must be greater than or equal to 1."
        });
        (result as BadRequestObjectResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetAllQuotes_ShouldReturnBadRequest_WhenPageSizeIsLessThanOne()
    {
        // Arrange
        var pageNumber = 1;
        var pageSize = 0;
        // Act
        var result = await _quoteController.GetAllQuotes(pageNumber, pageSize);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Page size must be between 1 and 100."
        });
        (result as BadRequestObjectResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetQuoteById_ShouldReturnOkWithQuoteResponseDto_WhenIdIsValid()
    {
        // Arrange
        var id = 1;
        var expectedResponse = new QuoteResponseDto
        {
            Quote = new QuoteDto
            {
                Id = id,
                Customer = "John Doe",
                Total = 0,
                UserId = "c20f35c3-7427-4705-b4bc-642c7c68309b"
            }
        };

        _quoteService
            .Setup(s => s.GetQuoteByIdAsync(id))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _quoteController.GetQuoteById(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedResponse);
        (result as OkObjectResult)!.StatusCode.Should().Be(200);
        _quoteService.Verify(s => s.GetQuoteByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetQuoteById_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var id = 0;
        // Act
        var result = await _quoteController.GetQuoteById(id);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Quote ID must be greater than 0."
        });
        (result as BadRequestObjectResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DeleteQuote_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var id = 1;
        // Act
        var result = await _quoteController.DeleteQuote(id);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContentResult>();
        (result as NoContentResult)!.StatusCode.Should().Be(204);
        _quoteService.Verify(s => s.DeleteQuoteAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteQuote_ShouldReturnBadRequest_WhenIdIsLessThanOrEqualToZero()
    {
        // Arrange
        var id = 0;
        // Act
        var result = await _quoteController.DeleteQuote(id);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>();
        (result as BadRequestObjectResult)!.Value.Should().BeEquivalentTo(new
        {
            Success = false,
            Message = "Quote ID must be greater than 0."
        });
        (result as BadRequestObjectResult)!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task UpdateQuote_ShouldReturnOkWithUpdateQuoteResponseDto_WhenRequestIsValid()
    {
        // Arrange
        var request = new UpdateQuoteRequestDto
        {
            Id = 1,
            Customer = "Jane Doe",
            UserId = "c20f35c3-7427-4705-b4bc-642c7c68309b",
        };
        var expectedResponse = new UpdateQuoteResponseDto
        {
            QuoteId = 1,
            Success = true
        };
        _quoteService
            .Setup(s => s.UpdateQuoteAsync(request))
            .ReturnsAsync(expectedResponse);
        // Act
        var result = await _quoteController.UpdateQuote(1, request);
        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();
        (result as OkObjectResult)!.Value.Should().BeEquivalentTo(expectedResponse);
        (result as OkObjectResult)!.StatusCode.Should().Be(200);
        _quoteService.Verify(s => s.UpdateQuoteAsync(request), Times.Once);
    }

    //[Fact]
    //public async Task UpdateQuote_ShouldThrowValidationException_WhenRequestIsInvalid()
    //{
    //    // Arrange
    //    var request = new UpdateQuoteRequestDto
    //    {
    //        Id = 0, // Invalid ID
    //        Customer = "",
    //        UserId = "invalid-guid",
    //    };
    //    // Act
    //    Func<Task> act = async () => await _quoteController.UpdateQuote(0, request);
    //    // Assert
    //    await act.Should().ThrowAsync<ValidationException>();
    //    _quoteService.Verify(s => s.UpdateQuoteAsync(It.IsAny<UpdateQuoteRequestDto>()), Times.Never);
    //}
}
