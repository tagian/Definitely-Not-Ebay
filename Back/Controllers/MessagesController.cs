using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Conversation;
using DefNotEbay_API.DTOs.Message;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

//we need to post message, update isRead


namespace DefNotEbay_API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;
        public MessagesController(IMapper mapper, IMessageService messageService)
        {
            _mapper = mapper;
            _messageService = messageService;
        }

        [HttpGet("id")]
        [Authorize]
        public async Task<ActionResult<MessageResponse>> GetMessage (int id)
        {
            var message = await _messageService.GetMessageAsync(id);
            return _mapper.Map<MessageResponse>(message);

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateMessage(CreateMessageRequest req)
        {
            var message = _mapper.Map<Message>(req);
            var success = await _messageService.CreateMessageAsync(message);
            if (success)
                return CreatedAtAction(nameof(GetMessage), new { id = message.MessageId}, message);
            return BadRequest();
        }

        [HttpGet("GetMessageByConvo/{convoid}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetMessageByConvo(int convoid)
        {
            var messages = await _messageService.GetMessageByConvo(convoid);
            
            return Ok(_mapper.Map<IEnumerable<MessageResponse>>(messages));
        }
    }
}
