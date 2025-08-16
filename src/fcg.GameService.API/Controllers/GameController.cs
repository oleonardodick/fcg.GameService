using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.ProblemsDefinitions;
using fcg.GameService.API.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class GameController : BaseApiController
    {
        private readonly IGameUseCase _gameUseCase;

        public GameController(IGameUseCase gameUseCase)
        {
            _gameUseCase = gameUseCase;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<ResponseGameDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<ResponseGameDTO>>> GetAll()
        {
            var games = await _gameUseCase.GetAllAsync();
            return Success(games);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseGameDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseGameDTO>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id), "O ID deve ser informado.");

            var game = await _gameUseCase.GetByIdAsync(id);

            return game is null ? NotFound("Jogo não encontrado.") : Success(game);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateGameDTO game)
        {
            var createdGame = await _gameUseCase.CreateAsync(game);
            return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(string id, [FromBody] UpdateGameDTO game)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id), "O ID deve ser enviado");

            var success = await _gameUseCase.UpdateAsync(id, game);

            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }

        [HttpPatch("/tags/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateTags(string id, [FromBody] List<string> tags)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id), "O ID deve ser informado.");

            var success = await _gameUseCase.UpdateTagsAsync(id, tags);
            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id), "O ID deve ser informado.");

            var success = await _gameUseCase.DeleteAsync(id);

            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }
    }
}
