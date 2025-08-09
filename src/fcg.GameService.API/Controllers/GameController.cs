using fcg.GameService.API.DTOs.Requests;
using fcg.GameService.API.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameUseCase _gameUseCase;

        public GameController(IGameUseCase gameUseCase)
        {
            _gameUseCase = gameUseCase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameUseCase.GetAllAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var game = await _gameUseCase.GetByIdAsync(id);

            return game is null ? NotFound() : Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameDTO game)
        {
            var createdGame = await _gameUseCase.CreateAsync(game);
            return Ok(createdGame);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateGameDTO game)
        {
            var response = await _gameUseCase.UpdateAsync(id, game);

            return response ? NoContent() : NotFound();
        }

        [HttpPatch("/tags/{id}")]
        public async Task<IActionResult> UpdateTags(string id, [FromBody] string[] tags)
        {
            var response = await _gameUseCase.UpdateTagsAsync(id, tags);
            return response ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _gameUseCase.DeleteAsync(id);

            return response ? NoContent() : NotFound();
        }
    }
}
