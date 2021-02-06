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
        public async Task<ActionResult<TalkModel[]>> Get(string moniker, bool includeSpeakers = true)
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
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id, bool includeSpeaker = true)
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

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel talkModel)
        {
            try
            {
                Camp camp = await this._campRepository.GetCampAsync(moniker);
                if (camp == null)
                {
                    return BadRequest("Camp does not exist");
                }

                Talk talk = this._mapper.Map<Talk>(talkModel);
                talk.Camp = camp;

                if (talkModel.Speaker == null)
                {
                    return BadRequest("Speaker is required for a talk");
                }

                Speaker speaker = await this._campRepository.GetSpeakerAsync(talkModel.Speaker.SpeakerId);
                if (speaker == null)
                {
                    return BadRequest("Speaker could not be found");
                }

                talk.Speaker = speaker;

                this._campRepository.Add(talk);

                if (await this._campRepository.SaveChangesAsync())
                {
                    string url = this._linkGenerator.GetPathByAction(this.HttpContext, "Get", values: new {moniker, id = talk.TalkId});

                    return Created(url, this._mapper.Map<TalkModel>(talk));
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest("Failed to save talk");
        }
    }
}