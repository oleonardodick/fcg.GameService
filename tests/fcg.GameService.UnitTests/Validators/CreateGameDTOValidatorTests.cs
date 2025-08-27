using Bogus;
using fcg.GameService.Application.Validators;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation.TestHelper;
using Shouldly;

namespace fcg.GameService.UnitTests.Validators;

public class CreateGameDTOValidatorTests
{
    private readonly CreateGameDTOValidator _validator;

    public CreateGameDTOValidatorTests()
    {
        _validator = new CreateGameDTOValidator();
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnRequiredFieldForName")]
    public void Validate_ShouldReturnRequiredFieldForName()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Price = 10,
            ReleasedDate = DateTime.UtcNow,
            Tags = ["tag"]
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome é obrigatório.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMaxSizeForName")]
    public void Validate_ShouldReturnMaxSizeForName()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Name = "Lorem ipsum dolor sit amet, consectetur adipiscing.",
            Price = 10,
            ReleasedDate = DateTime.UtcNow,
            Tags = ["tag"]
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Name).WithErrorMessage("O nome deve possuir no máximo 50 caracteres.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
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

        CreateGameDTO request = new()
        {
            Name = "Game test",
            Description = description,
            Tags = ["tag"],
            Price = 10
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

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
        CreateGameDTO request = new()
        {
            Name = "Game test",
            Tags = ["tag"],
        };
        if (price != null) request.Price = (double)price;

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Price).WithErrorMessage("O preço deve ser maior que 0.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorWhenNoDataIsProvided")]
    public void Validate_ShouldReturnErrorWhenNoDataIsProvided()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Name = "Game test",
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = default
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.ReleasedDate).WithErrorMessage("A data de lançamento deve ser informada.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorWhenFutureDataIsProvided")]
    public void Validate_ShouldReturnErrorWhenFutureDataIsProvided()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Name = "Game test",
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = DateTime.Now.AddDays(1)
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.ReleasedDate).WithErrorMessage("Não é possível cadastrar um jogo com lançamento futuro.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnErrorFromTagEmpty")]
    public void Validate_ShouldReturnErrorWhenNoTagsWereProvided()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Name = "Game test",
            Price = 10,
            Tags = []
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

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
        CreateGameDTO request = new()
        {
            Name = "Game test",
            Price = 10,
            Tags = [.. tags]
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveValidationErrorFor(g => g.Tags).WithErrorMessage("A tag não pode ser nula, vazia ou com espaço em branco.");
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnMoreThanOneError")]
    public void Validate_ShouldReturnMoreThanOneError()
    {
        //Arrange
        CreateGameDTO request = new()
        {
            Price = 10,
            Tags = []
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldHaveAnyValidationError();
        result.Errors.Count.ShouldBeGreaterThan(1);
    }

    [Trait("Module", "CreateGameDTOValidator")]
    [Fact(DisplayName = "Validate_ShouldReturnValidData")]
    public void Validate_ShouldReturnValidData()
    {
        //Arrange
        Faker faker = new();

        string nome = faker.Commerce.ProductName();
        string description = faker.Commerce.ProductDescription();
        if (description.Length > 500)
            description = description[..500];

        CreateGameDTO request = new()
        {
            Name = nome,
            Description = description,
            Price = 10,
            Tags = ["tag"],
            ReleasedDate = new DateTime(2023, 1, 1)
        };

        //Act
        TestValidationResult<CreateGameDTO> result = _validator.TestValidate(request);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
