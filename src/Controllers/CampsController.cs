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
    [Route("api/[controller]"), ApiController]
    public class CampsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICampRepository _repository;
        private readonly LinkGenerator linkGenerator;

        public CampsController(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._repository = repository;
            this._mapper = mapper;
            this.linkGenerator = linkGenerator;
        }

        // GET
        public async Task<ActionResult<CampModel[]>> Get(bool includeTalks = false)
        {
            try
            {
                Camp[] result = await this._repository.GetAllCampsAsync(includeTalks);

                return this._mapper.Map<CampModel[]>(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                Camp result = await this._repository.GetCampAsync(moniker);

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
                Camp[] result = await this._repository.GetAllCampsByEventDate(theDate, includeTalks);
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
                if (await this._repository.GetCampAsync(campModel.Moniker) != null)
                {
                    return BadRequest("Moniker in use");
                }

                string location = this.linkGenerator.GetPathByAction("Get", "Camps", new {moniker = campModel.Moniker});

                if (string.IsNullOrEmpty(location))
                {
                    return BadRequest("Could not use current moniker");
                }

                this._repository.Add(this._mapper.Map<Camp>(campModel));

                if (await this._repository.SaveChangesAsync())
                {
                    return Created(location, this._mapper.Map<CampModel>(await this._repository.GetCampAsync(campModel.Moniker)));
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
                Camp oldCamp = await this._repository.GetCampAsync(moniker);

                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker {moniker}");
                }

                this._mapper.Map(campModel, oldCamp);

                if (await this._repository.SaveChangesAsync())
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
    }
}