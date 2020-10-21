using System.Threading.Tasks;
using Api.Domain.Constants;
using Api.Domain.Models;
using Api.Services.Services;
using Api.Web.Common;
using Api.Web.Handlers;
using Api.Web.Middlewares;
using Api.Web.Models;
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
        private readonly string _collection = "stations";
        private readonly IStationManager _stationManager;
        private readonly IStationRepository _stationRepository;
        private readonly IOperationHandler _operationHandler;

        public StationController(IStationManager stationManager, IStationRepository stationRepository, IOperationHandler operationHandler)
            => (_stationManager, _stationRepository, _operationHandler) = (stationManager, stationRepository, operationHandler);

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
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Create,
                Collection = _collection,
                Id = string.Empty,
                Model = station
            });
            
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
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Update,
                Collection = _collection,
                Id = id,
                Model = station
            });

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
            Emitter.EmitMessage(_operationHandler, new CollectionEventReceived
            {
                Type = EventType.Delete,
                Collection = _collection,
                Id = id,
                Model = null
            });
            
            return NoContent();
        }

        #endregion
    }
}
