using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/[controller]"), ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
            this._campRepository = campRepository;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker, bool includeSpeakers = false)
        {
            try
            {
                Talk[] talks = await this._campRepository.GetTalksByMonikerAsync(moniker, includeSpeakers);

                return this._mapper.Map<TalkModel[]>(talks);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id, bool includeSpeaker = false)
        {
            try
            {
                Talk talk = await this._campRepository.GetTalkByMonikerAsync(moniker, id, includeSpeaker);

                return this._mapper.Map<TalkModel>(talk);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }
    }
}