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
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;

        }


        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {

            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await unitOfWork.UserRepository.GetUserByNameAsync(username);
            var recipient = await unitOfWork.UserRepository.GetUserByNameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            unitOfWork.MessageRepository.AddMessage(message);

            if (await unitOfWork.Complete()) return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.pageSize, messages.TotalCount, messages.TotalPages);

            return messages;
        }

        // [HttpGet("thread/{username}")]
        // public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        // {
        //     var currentUsername = User.GetUsername();
        //     return Ok(await unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));
        // }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await unitOfWork.MessageRepository.Getmessage(id);

            if (message.Sender.UserName != username && message.Recipient.UserName != username)
                return Unauthorized();
            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) unitOfWork.MessageRepository.DeleteMessage(message);

            if (await unitOfWork.Complete()) return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}