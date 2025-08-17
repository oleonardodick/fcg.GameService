using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.Validators;
using FluentValidation.TestHelper;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Validators;

public class CreateGameLibraryDTOValidatorTests
{
    private CreateGameLibraryDTOValidator _validator;

    public CreateGameLibraryDTOValidatorTests()
    {
        _validator = new CreateGameLibraryDTOValidator();
    }

    [Trait("Module", "CreateGameLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldForUserId")]
    public void Validate_ShouldReturnRequiredFieldForUserId()
    {
        //Arrange
        var request = new CreateGameLibraryDTO
        {
            UserId = string.Empty
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.UserId).WithErrorMessage("O ID do usuário deve ser informado.");
    }

    [Trait("Module", "CreateGameLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        var request = new CreateGameLibraryDTO
        {
            UserId = ObjectId.GenerateNewId().ToString()
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
