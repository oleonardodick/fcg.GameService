using Bogus;
using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation.TestHelper;
using Shouldly;

namespace fcg.GameService.UnitTests.Validators;

public class CreateGameDTOValidatorTests
{
    private CreateGameDTOValidator _validator;

    public CreateGameDTOValidatorTests()
    {
        _validator = new CreateGameDTOValidator();
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldForName")]
    public void Validate_ShouldReturnRequiredFieldForName()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Price = 10,
            ReleasedDate = DateTime.UtcNow,
            Tags = ["tag"]
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome é obrigatório.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMaxSizeForName")]
    public void Validate_ShouldReturnMaxSizeForName()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Lorem ipsum dolor sit amet, consectetur adipiscing.",
            Price = 10,
            ReleasedDate = DateTime.UtcNow,
            Tags = ["tag"]
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome deve possuir no máximo 50 caracteres.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMaxSizeForDescription")]
    public void Validate_ShouldReturnMaxSizeForDescription()
    {
        //Arrange
        var faker = new Faker();

        string description = "";
        while (description.Length <= 500)
        {
            description += " " + faker.Lorem.Sentence();
        }

        var request = new CreateGameDTO
        {
            Name = "Game test",
            Description = description,
            Tags = ["tag"],
            Price = 10
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Description).WithErrorMessage("A descrição deve possuir no máximo 500 caracteres.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Theory(DisplayName = "Validate_ShouldReturnPriceGreaterThanZero")]
    [InlineData(null)]
    [InlineData(0.0)]
    public void Validate_ShouldReturnPriceGreaterThanZero(double? price)
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Game test",
            Tags = ["tag"],
        };
        if (price != null) request.Price = (double)price;

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Price).WithErrorMessage("O preço deve ser maior que 0.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorWhenNoDataIsProvided")]
    public void Validate_ShouldReturnErrorWhenNoDataIsProvided()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Game test",
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = default
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.ReleasedDate).WithErrorMessage("A data de lançamento deve ser informada.");
    }

     [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorWhenFutureDataIsProvided")]
    public void Validate_ShouldReturnErrorWhenFutureDataIsProvided()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Game test",
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = DateTime.Now.AddDays(1)
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.ReleasedDate).WithErrorMessage("Não é possível cadastrar um jogo com lançamento futuro.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorFromTagEmpty")]
    public void Validate_ShouldReturnErrorWhenNoTagsWereProvided()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Game test",
            Price = 10,
            Tags = []
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Tags).WithErrorMessage("Ao menos uma tag deve ser informada.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Theory(DisplayName = "Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat")]
    [InlineData("simulacao", "")]
    [InlineData(null, "esporte")]
    [InlineData("", " ", "rpg")]
    public void Validate_ShouldReturnErrorWhenSomeTagIsInWrongFormat(params string[] tags)
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Name = "Game test",
            Price = 10,
            Tags = tags.ToList()
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Tags).WithErrorMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMoreThanOneError")]
    public void Validate_ShouldReturnMoreThanOneError()
    {
        //Arrange
        var request = new CreateGameDTO
        {
            Price = 10,
            Tags = []
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveAnyValidationError();
        result.Errors.Count().ShouldBeGreaterThan(1);
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        var faker = new Faker();

        string nome = faker.Commerce.ProductName();
        string description = faker.Commerce.ProductDescription();
        if (description.Length > 500)
            description = description.Substring(0, 500);

        var request = new CreateGameDTO
        {
            Name = nome,
            Description = description,
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = DateTime.Now
        };

        //Act
        var result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
