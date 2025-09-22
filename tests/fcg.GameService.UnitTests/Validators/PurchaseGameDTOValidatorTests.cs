using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation.TestHelper;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Validators;

public class PurchaseGameDTOValidatorTests
{
    private readonly PurchaseGameDTOValidator _validator;

    public PurchaseGameDTOValidatorTests()
    {
        _validator = new PurchaseGameDTOValidator();
    }

    [Trait("Module", "PurchaseGameDTOValidatorTests")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldForUserId")]
    public void Validate_ShouldReturnRequiredFieldForUserId()
    {
        //Arrange
        PurchaseGameDTO request = new()
        {
            UserId = string.Empty
        };

        //Act
        TestValidationResult<PurchaseGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.UserId).WithErrorMessage("O ID do usuário deve ser informado.");
    }

    [Trait("Module", "PurchaseGameDTOValidatorTests")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldForUserId")]
    public void Validate_ShouldReturnRequiredFieldForGameId()
    {
        //Arrange
        PurchaseGameDTO request = new()
        {
            UserId = ObjectId.GenerateNewId().ToString(),
            GameId = string.Empty
        };

        //Act
        TestValidationResult<PurchaseGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.GameId).WithErrorMessage("O ID do jogo deve ser informado.");
    }
}
