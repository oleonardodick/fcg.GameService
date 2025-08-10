using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.Entities;
using fcg.GameService.API.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameLibraryController : ControllerBase
    {
        private readonly IGameLibraryUseCase _gameLibraryUseCase;

        public GameLibraryController(IGameLibraryUseCase gameLibraryUseCase)
        {
            _gameLibraryUseCase = gameLibraryUseCase;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var result = await _gameLibraryUseCase.GetByUserIdAsync(userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            var createdGameLibrary = await _gameLibraryUseCase.CreateAsync(gameLibrary);
            return Ok(createdGameLibrary);
        }

        [HttpPost("adquireGame")]
        public async Task<IActionResult> AddGame([FromBody] AddGameToLibraryDTO game)
        {
            var gameAdded = await _gameLibraryUseCase.AddGameToLibraryAsync(game);
            return NoContent();
        }

        [HttpPost("removeGame")]
        public async Task<IActionResult> RemoveGame([FromBody] RemoveGameFromLibraryDTO game)
        {
            var gameRemoved = await _gameLibraryUseCase.RemoveGameFromLibraryAsync(game);
            return NoContent();
        }
    }
}
