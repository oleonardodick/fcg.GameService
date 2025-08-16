using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class GameLibraryController : ControllerBase
    {
        private readonly IGameLibraryUseCase _gameLibraryUseCase;

        public GameLibraryController(IGameLibraryUseCase gameLibraryUseCase)
        {
            _gameLibraryUseCase = gameLibraryUseCase;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _gameLibraryUseCase.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var result = await _gameLibraryUseCase.GetByUserIdAsync(userId);

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            var createdGameLibrary = await _gameLibraryUseCase.CreateAsync(gameLibrary);
            return Ok(createdGameLibrary);
        }

        [HttpPost("addGame")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AddGame([FromBody] AddGameToLibraryDTO game)
        {
            var added = await _gameLibraryUseCase.AddGameToLibraryAsync(game);

            return added ? NoContent() : NotFound();
        }

        [HttpDelete("removeGame")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveGame([FromBody] RemoveGameFromLibraryDTO game)
        {
            var removed = await _gameLibraryUseCase.RemoveGameFromLibraryAsync(game);

            return removed ? NoContent() : NotFound();
        }
    }
}
