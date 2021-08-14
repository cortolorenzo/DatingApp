using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            this._mapper = mapper;
            this._userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
        {
            //Przed wprowadzeniem interfejsu repository
            //var users = await _context.Users.ToListAsync();

            var users = await _userRepository.GetMembersAsync();

           
            return Ok(users);
        }


        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUser(string username)
        { 
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByNameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Upadte(user);

            if (await _userRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Failed to update user");
        }

    }
}
