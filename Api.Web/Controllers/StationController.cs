using System.Threading.Tasks;
using Api.Domain.Models;
using Api.Services.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers
{
    [Route("api/v1/stations")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IStationManager _stationManager;
        private readonly IStationRepository _stationRepository;

        public StationController(IStationManager stationManager, IStationRepository stationRepository)
            => (_stationManager, _stationRepository) = (stationManager, stationRepository);

        /// <summary>
        /// GET
        /// </summary>

        #region snippet_GetAll

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var stations = await _stationRepository.GetAllAsync();
            return Ok(stations);
        }

        #endregion

        #region snippet_GetById

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var station = await _stationRepository.GetByIdAsync(id);
            return Ok(station);
        }

        #endregion

        /// <summary
        /// POST
        /// </summary>

        #region snippet_Create

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Station station)
        {
            await _stationManager.CreateAsync(station);
            return Created("", station);
        }

        #endregion

        /// <summary>
        /// PATCH
        /// </summary>

        #region snippet_UpdatePartial

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(string id, [FromBody] JsonPatchDocument<Station> replaceStation)
        {
            var station = await _stationRepository.GetByIdAsync(id);
            await _stationManager.UpdateByIdAsync(id, station, replaceStation);
            return Created("", station);
        }

        #endregion

        /// <summary>
        /// DELETE
        /// </summary>

        #region snippet_Delete

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteByIdAsync(string id)
        {
            await _stationManager.DeleteByIdAsync(id);
            return NoContent();
        }

        #endregion
    }
}
