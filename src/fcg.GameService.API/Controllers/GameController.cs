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
            {
                return BadRequest("id", "O ID deve ser informado.");
            }

            var game = await _gameUseCase.GetByIdAsync(id);

            if (game == null)
            {
                return NotFound("Jogo não encontrado");
            }

            return Success(game);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateGameDTO game)
        {
            var createdGame = await _gameUseCase.CreateAsync(game);
            return Created($"/api/game/{createdGame.Id}", createdGame);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateGameDTO game)
        {
            var response = await _gameUseCase.UpdateAsync(id, game);

            return response ? NoContent() : NotFound();
        }

        [HttpPatch("/tags/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateTags(string id, [FromBody] string[] tags)
        {
            var response = await _gameUseCase.UpdateTagsAsync(id, tags);
            return response ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _gameUseCase.DeleteAsync(id);

            return response ? NoContent() : NotFound();
        }
    }
}
