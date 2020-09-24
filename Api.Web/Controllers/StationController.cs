using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Authorize]
    [Route("api/v1/stations")]
    [Produces("application/json")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationManager _stationManager;
        private readonly IStationRepository _stationRepository;

        public StationController(IStationManager stationManager, IStationRepository stationRepository)
            => (_stationManager, _stationRepository) = (stationManager, stationRepository);

        #region snippet_GetAll

        [HttpGet]
        [ProducesResponseType(typeof(Station), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync()
        {
            var stations = await _stationRepository.GetAllAsync();
            return Ok(new { Status = true, Data = stations });
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Station), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [StationExists]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var station = await _stationRepository.GetByIdAsync(id);
            return Ok(new { Status = true, Data = station });
        }

        #endregion

        #region snippet_Create

        [HttpPost]
        [ProducesResponseType(typeof(Station), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(Station station)
        {
            await _stationManager.CreateAsync(station);
            return Created("", new { Status = true, Data = station });
        }

        #endregion

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(Station), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [StationExists]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<Station> replaceStation)
        {
            var station = await _stationRepository.GetByIdAsync(id);
            await _stationManager.UpdateByIdAsync(id, station, replaceStation);
            return Created("", new { Status = true, Data = station });
        }

        #endregion

        #region snippet_Delete

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [StationExists]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _stationManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}
