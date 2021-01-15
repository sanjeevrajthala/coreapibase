using AutoMapper;
using CoreBase.Data;
using CoreBase.Dtos;
using CoreBase.Models;
using CoreBase.Options;
using CoreBase.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommandsController : ControllerBase
    {
        #region Declared Variables
        private readonly ICommanderRepo _repository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public CommandsController(ICommanderRepo repository, IMapper mapper, IEmailService emailService)
        {
            _repository = repository;
            _mapper = mapper;
            _emailService = emailService;
        }
        #endregion

        #region Get All Commands
        //GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commands = _repository.GetAllCommands();
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }
        #endregion

        #region Get Command By Id
        //GET api/commands/{id}
        [HttpGet("{id}", Name = "GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var command = _repository.GetCommandById(id);

            if (command != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(command));

            }
            return NotFound();
        }
        #endregion

        #region CreateCommand
        //POST api/commands
        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var command = _mapper.Map<Command>(commandCreateDto);

            _repository.CreateCommand(command);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandById), new { Id = commandReadDto.Id }, commandReadDto);
        }
        #endregion

        #region Update Command
        //PUT api/commands/1
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandFromRepo = _repository.GetCommandById(id);

            if (commandFromRepo == null) return NotFound();

            _mapper.Map(commandUpdateDto, commandFromRepo);

            _repository.UpdateCommand(commandFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }
        #endregion

        #region Patch Command
        //PATCH api/commands/1
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandFromRepo = _repository.GetCommandById(id);

            if (commandFromRepo == null) return NotFound();

            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch)) return ValidationProblem(ModelState);

            _mapper.Map(commandToPatch, commandFromRepo);

            _repository.UpdateCommand(commandFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }
        #endregion

        #region Delete Command
        //DELETE api/commands/1
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandFromRepo = _repository.GetCommandById(id);

            if (commandFromRepo == null) return NotFound();

            _repository.DeleteCommand(commandFromRepo);
            _repository.SaveChanges();

            return NoContent();
        }
        #endregion

        #region Send Email
        [HttpGet("SendEmail")]
        public async Task<IActionResult> SendEmail() {
            try
            {
                EmailOption options = new EmailOption
                {
                    ToEmails = new List<string>() { "test@test.com", "sanjeev@hello.com" },
                    PlaceHolders = new List<KeyValuePair<string, string>>() {
                    new KeyValuePair<string, string> ("{{UserName}}", "JOhn"),
                    new KeyValuePair<string, string> ("{{Link}}", "https://code-maze.com/send-email-with-attachments-aspnetcore-2/")
                }
                };

                await _emailService.SendTestEmail(options)
                    ; return NoContent();
            }
            catch(Exception e)
            {
                return Ok(e.Message);
            }
        }
        #endregion
    }
}
