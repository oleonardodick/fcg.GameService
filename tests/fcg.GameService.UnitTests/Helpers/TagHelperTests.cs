using fcg.GameService.API.Helpers;
using Shouldly;

namespace fcg.GameService.UnitTests.Helpers;

public class TagHelperTests
{
    [Trait("Module", "TagHelper")]
    [Theory(DisplayName = "NormalizeTags_ShouldReturnNormalizedTags")]

    // 1. Casos nulos ou vazios
    [InlineData(null, new string[0])]
    [InlineData(new string[0], new string[0])]
    [InlineData(new[] { "", "   ", "\t" }, new string[0])]

    // 2. Remoção de espaços extras
    [InlineData(new[] { "  RPG  " }, new[] { "rpg" })]
    [InlineData(new[] { "Jogo   de   Ação" }, new[] { "jogo-de-acao" })]
    [InlineData(new[] { "   Tiro     em   Primeira   Pessoa " }, new[] { "tiro-em-primeira-pessoa" })]


    // 3. Acentuação / Normalização
    [InlineData(new[] { "Simulação" }, new[] { "simulacao" })]
    [InlineData(new[] { "Estratégia em Tempo Real" }, new[] { "estrategia-em-tempo-real" })]
    [InlineData(new[] { "Ação Incrível" }, new[] { "acao-incrivel" })]

    // 4. Case insensitive
    [InlineData(new[] { "MOBA", "moba", "MOBA" }, new[] { "moba" })]

    // 5. Duplicados com espaço/maiúscula
    [InlineData(new[] { "  Corrida ", "corrida", "CORRIDA " }, new[] { "corrida" })]

    // 6. Somente caracteres especiais
    [InlineData(new[] { "---", "___", "!!!" }, new[] { "---", "___", "!!!" })]

    // 7. Mistura de tudo
    [InlineData(new[] { "  Jogo   de   Luta ", "jogo-de-luta", "Platafórma", "plataforma" }, new[] { "jogo-de-luta", "plataforma" })]

    public void NormalizeTags_ShouldReturnNormalizedTags(string[] input, string[] expected)
    {
        //Arrange
        var dataToNormalize = input?.ToList();
        //Act
        var result = TagHelper.NormalizeTags(dataToNormalize);

        //Assert
        result.ShouldBe(expected.ToList());
    }

}
