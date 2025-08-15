
using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using smartquote.api.Controllers;
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
}
