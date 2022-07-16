using API.Models;
using Common;
using Common.Exception;
using Data.Repositories.UserRepositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApiResult<IQueryable<UserSelectDto>> Get()
        {
            var users = _userRepository.GetAll().Select(x => new UserSelectDto
            {
                Id = x.Id,
                FirstName = x.UserName,
                LastName = x.LastName,
                UserName = x.UserName,
                IsActive = x.IsActive
            });
            return Ok(users);
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ApiResult<UserSelectDto> Get(int id)
        {
            var user = _userRepository.GetAll(x => x.Id == id).Select(x => new UserSelectDto
            {
                Id = x.Id,
                FirstName = x.UserName,
                LastName = x.LastName,
                UserName = x.UserName,
                IsActive = x.IsActive
            }).FirstOrDefault();

            if (user == null)
                return NotFound();
            return Ok(user);
        }


        /// <summary>
        /// Create new User
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<User>> Post([FromBody] UserDto userDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Password = SecurityHelper.GetSha256Hash(userDto.Password),
                IsActive = userDto.IsActive
            };
            await _userRepository.AddAsync(user, cancellationToken);
            return user;
        }

        /// <summary>
        /// Update exist User
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpPut]
        public virtual async Task<ApiResult<User>> Update(UserDto userDto, CancellationToken cancellationToken)
        {
            var userDb = await _userRepository.GetFirstAsync(x => x.Id == userDto.Id, cancellationToken);
            if (userDb == null)
                throw new NotFoundException("User not found");

            userDb.UserName = userDto.UserName;
            userDb.FirstName = userDto.FirstName;
            userDb.LastName = userDto.LastName;
            userDb.Password = SecurityHelper.GetSha256Hash(userDto.Password);
            userDb.IsActive = userDto.IsActive;

            await _userRepository.UpdateAsync(userDb, cancellationToken);
            return userDb;
        }

        /// <summary>
        /// Delete exist User
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public virtual async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
        {
            var model = await _userRepository.GetFirstAsync(x => x.Id.Equals(id), cancellationToken);
            await _userRepository.DeleteAsync(model, cancellationToken);
            return Ok();
        }


    }
}
