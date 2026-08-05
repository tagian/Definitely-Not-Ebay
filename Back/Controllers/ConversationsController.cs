using AutoMapper;
using DefNotEbay_API.Data;
using DefNotEbay_API.DTOs.Category;
using DefNotEbay_API.DTOs.Conversation;
using DefNotEbay_API.Models;
using DefNotEbay_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//We need to create, get and delete convos and also readReceipt

namespace DefNotEbay_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;
        public ConversationsController (IConversationService conversationService, IMapper mapper) { 
            _conversationService = conversationService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConversationResponse>> GetConversation (int id)
        {
            var convo = await _conversationService.GetConversationAsync(id);

            if (convo == null)
                return NotFound();
            
            return _mapper.Map<ConversationResponse>(convo);

        }

        [Authorize]
        [HttpGet("getmine")]
        public async Task<ActionResult<IEnumerable<ConversationResponse>>> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("id");

            if (userId == null)
                return BadRequest();

            var convo = await _conversationService.GetUserConversations(Int32.Parse(userId));

            if (convo == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<ConversationResponse>>(convo));

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConversationResponse>> CreateConversation (CreateConversationRequest req)
        {
            var convo = _mapper.Map<Conversation>(req);
            var success = await _conversationService.CreateConversationAsync(convo);
            if (!success)
                return BadRequest("Failed to create conversation");
            return CreatedAtAction(nameof(GetConversation), new { id = convo.ConversationId }, _mapper.Map<ConversationResponse>(convo));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConversation (int id)
        {
            var convo = await _conversationService.GetConversationAsync(id);
            
            if (convo == null)
                return NotFound();

            await _conversationService.DeleteConversationAsync(id);
            return NoContent();
        }
        
    }
}
