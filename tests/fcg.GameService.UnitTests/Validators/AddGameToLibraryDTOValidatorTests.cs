using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using FluentValidation.TestHelper;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Validators;

public class AddGameToLibraryDTOValidatorTests
{
    private AddGameToLibraryDTOValidator _validator;

    public AddGameToLibraryDTOValidatorTests()
    {
        _validator = new AddGameToLibraryDTOValidator();
    }

    [Trait("Module", "AddGameToLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldId")]
    public void Validate_ShouldReturnRequiredFieldId()
    {
        //Arrange
        var request = new AddGameToLibraryDTO
        {
            Id = string.Empty,
            Name = "game name"
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Id).WithErrorMessage("O ID do jogo deve ser informado.");
    }

    [Trait("Module", "AddGameToLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldName")]
    public void Validate_ShouldReturnRequiredFieldName()
    {
        //Arrange
        var request = new AddGameToLibraryDTO
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = string.Empty
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome do jogo deve ser informado.");
    }

    [Trait("Module", "AddGameToLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        var request = new AddGameToLibraryDTO
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = "game name"
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
