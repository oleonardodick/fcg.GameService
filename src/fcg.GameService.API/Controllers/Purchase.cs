using fcg.GameService.Application.Helpers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[Consumes("application/json")]
public class Purchase(
    IPurchaseUseCase purchaseUseCase,
    IValidator<PurchaseGameDTO> validatorPurchase) : ControllerBase
{
    private readonly IPurchaseUseCase _purchaseUseCase = purchaseUseCase;
    private readonly IValidator<PurchaseGameDTO> _validatorPurchase = validatorPurchase;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseGameDTO purchase)
    {
        List<ErrorDetails> errors = ValidationHelper.Validate(_validatorPurchase, purchase);

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        return Ok(await _purchaseUseCase.PublishAsync(purchase));
    }
}
