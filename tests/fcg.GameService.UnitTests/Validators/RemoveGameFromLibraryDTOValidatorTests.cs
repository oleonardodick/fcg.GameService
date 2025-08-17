using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.Validators;
using FluentValidation.TestHelper;
using MongoDB.Bson;

namespace fcg.GameService.UnitTests.Validators;

public class RemoveGameFromLibraryDTOValidatorTests
{
    private RemoveGameFromLibraryDTOValidator _validator;

    public RemoveGameFromLibraryDTOValidatorTests()
    {
        _validator = new RemoveGameFromLibraryDTOValidator();
    }

    [Trait("Module", "RemoveGameFromLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldId")]
    public void Validate_ShouldReturnRequiredFieldId()
    {
        //Arrange
        var request = new RemoveGameFromLibraryDTO
        {
            Id = string.Empty,
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Id).WithErrorMessage("O ID do jogo deve ser informado.");
    }

    [Trait("Module", "RemoveGameFromLibraryDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        var request = new RemoveGameFromLibraryDTO
        {
            Id = ObjectId.GenerateNewId().ToString(),
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
