using Bogus;
using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation.TestHelper;

namespace fcg.GameService.UnitTests.Validators;

public class UpdateGameDTOValidatorTests
{
    private readonly UpdateGameDTOValidator _validator;

    public UpdateGameDTOValidatorTests()
    {
        _validator = new UpdateGameDTOValidator();
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMaxSizeForName")]
    public void Validate_ShouldReturnMaxSizeForName()
    {
        //Arrange
        UpdateGameDTO request = new()
        {
            //name with 51 characteres
            Name = "Lorem ipsum dolor sit amet, consectetur adipiscing.",
        };

        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome deve possuir no máximo 50 caracteres.");
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMaxSizeForDescription")]
    public void Validate_ShouldReturnMaxSizeForDescription()
    {
        //Arrange
        Faker faker = new();
        string description = "";
        while (description.Length <= 500)
        {
            description += " " + faker.Lorem.Sentence();
        }
        UpdateGameDTO request = new()
        {
            Description = description,
        };

        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Description).WithErrorMessage("A descrição deve possuir no máximo 500 caracteres.");
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnPriceGreaterThanZero")]
    public void Validate_ShouldReturnPriceGreaterThanZero()
    {
        //Arrange
        UpdateGameDTO request = new()
        {
            Price = 0
        };

        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Price).WithErrorMessage("O preço deve ser maior que 0.");
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorWhenFutureDataIsProvided")]
    public void Validate_ShouldReturnErrorWhenFutureDataIsProvided()
    {
        //Arrange
        UpdateGameDTO request = new()
        {
            ReleasedDate = DateTime.Now.AddDays(1)
        };

        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.ReleasedDate).WithErrorMessage("Não é possível cadastrar um jogo com lançamento futuro.");
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Theory(DisplayName = "Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat")]
    [InlineData("simulacao", "")]
    [InlineData(null, "esporte")]
    [InlineData("", " ", "rpg")]
    public void Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat(params string[] tags)
    {
        //Arrange
        UpdateGameDTO request = new()
        {
            Tags = [.. tags]
        };

        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Tags).WithErrorMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
    }

    [Trait("Module", "UpdateGameDTOValidator")]
    [Theory(DisplayName = "Validate_ShouldPassTheValidation")]
    [MemberData(nameof(TestData.GenerateValidData), MemberType = typeof(TestData))]
    public void Validate_ShouldPassTheValidation(UpdateGameDTO request)
    {
        //Act
        TestValidationResult<UpdateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

public static class TestData
{
    public static IEnumerable<object[]> GenerateValidData()
    {
        yield return new object[]
        {
            new UpdateGameDTO{Name = "Game test"}
        };

        yield return new object[]
        {
            new UpdateGameDTO{Description = "Game description"}
        };

        yield return new object[]
        {

            new UpdateGameDTO{Price = 10}
        };

        yield return new object[]
        {
            new UpdateGameDTO{ReleasedDate = DateTime.UtcNow.AddMinutes(-5)}
        };

        yield return new object[]
        {

            new UpdateGameDTO{Tags = ["tag"]}
        };
    }
}
