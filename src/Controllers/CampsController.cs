using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]"), ApiController, ApiVersion("1.0"), ApiVersion("1.1")]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                Camp[] result = await this._campRepository.GetAllCampsAsync(includeTalks);

                return this._mapper.Map<CampModel[]>(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}"), MapToApiVersion("1.0")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                Camp result = await this._campRepository.GetCampAsync(moniker);

                return result != null ? this._mapper.Map<CampModel>(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}"), MapToApiVersion("1.1")]
        public async Task<ActionResult<CampModel>> Get11(string moniker)
        {
            try
            {
                Camp result = await this._campRepository.GetCampAsync(moniker, true);

                return result != null ? this._mapper.Map<CampModel>(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                Camp[] result = await this._campRepository.GetAllCampsByEventDate(theDate, includeTalks);
                return result.Any() ? this._mapper.Map<CampModel[]>(result) : NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel campModel)
        {
            try
            {
                if (await this._campRepository.GetCampAsync(campModel.Moniker) != null)
                {
                    return BadRequest("Moniker in use");
                }

                string location = this._linkGenerator.GetPathByAction("Get", "Camps", new {moniker = campModel.Moniker});

                if (string.IsNullOrEmpty(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                this._campRepository.Add(this._mapper.Map<Camp>(campModel));

                if (await this._campRepository.SaveChangesAsync())
                {
                    return Created(location, this._mapper.Map<CampModel>(await this._campRepository.GetCampAsync(campModel.Moniker)));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel campModel)
        {
            try
            {
                Camp oldCamp = await this._campRepository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker {moniker}");
                }

                this._mapper.Map(campModel, oldCamp);

                if (await this._campRepository.SaveChangesAsync())
                {
                    return this._mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<ActionResult> Delete(string moniker)
        {
            try
            {
                Camp oldCamp = await this._campRepository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker {moniker}");
                }

                this._campRepository.Delete(oldCamp);

                if (await this._campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed to delete camp");
        }
    }
}