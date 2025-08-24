using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation.TestHelper;

namespace fcg.GameService.UnitTests.Validators;

public class UpdateTagsDTOValidatorTests
{
    private UpdateTagsDTOValidator _validator;

    public UpdateTagsDTOValidatorTests()
    {
        _validator = new UpdateTagsDTOValidator();
    }

    [Trait("Module", "UpdateTagsDTOValidator")]
    [Theory(DisplayName = "Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat")]
    [InlineData("simulacao", "")]
    [InlineData(null, "esporte")]
    [InlineData("", " ", "rpg")]
    public void Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat(params string[] tags)
    {
        //Arrange
        var request = new UpdateTagsDTO
        {
            Tags = tags.ToList()
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Tags).WithErrorMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
    }

    [Trait("Module", "UpdateTagsDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        var request = new UpdateTagsDTO
        {
            Tags = ["tag"]
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
