using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseApiController
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
         private readonly IMapper mapper;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            this.mapper = mapper;
            this._userRepository = userRepository;
            this._messageRepository = messageRepository;
        }
       

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {

            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);

            if (await _messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage,messages.pageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }


        [HttpDelete("id")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _messageRepository.Getmessage(id);

            if(message.Sender.UserName != username && message.Recipient.UserName != username )
                return Unauthorized();
            if(message.Sender.UserName == username) message.Senderdeleted = true;
            if(message.Recipient.UserName == username) message.RecipientDeleted = true;

            if(message.Senderdeleted && message.RecipientDeleted) _messageRepository.DeleteMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok();
            
            return BadRequest("Problem deleting the message");
        }
    }
}