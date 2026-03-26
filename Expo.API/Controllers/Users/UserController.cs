using Asp.Versioning;
using Expo.API.Utils;
using Expo.Domain.Constants;
using Expo.Domain.DTO.User;
using Expo.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Expo.API.Controllers.Users;

internal static class UserEndpoints
{
    internal const string Promote = "promote";
    internal const string Demote = "demote";
}

/// <summary>
/// Controller for Administrator user to manage users
/// </summary>
/// <param name="logger">Logger</param>
/// <param name="userService">Users Service</param>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion(ApiConstants.V1)]
public class UserController
    (ILogger<UserController> logger,
    IUserService userService) : ControllerBase
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly IUserService _userService = userService;


    /// <summary>
    /// Get all registered users
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Users.CanReadUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all result");

        var result = await _userService.GetAllUsersAsync();


        var message = $"Retrieved {(result.IsSuccess ? result.Value.Count() : 0)} results";

        _logger.LogInformation(message);

        return Ok(result.Value.Select(u => new { u.Id, u.Email }));
    }

    /// <summary>
    /// Get an user by id
    /// </summary>
    /// <param name="id">user id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Users.CanReadUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(string id)
    {
        _logger.LogInformation($"Fetching result with id: {id}");
        var result = await _userService.GetUserByIdAsync(id);

        if (result.IsFailed)
        {
            var message = $"User with id {id} not found.";
            _logger.LogWarning(message);
            return NotFound(message);
        }

        var roles = await _userService.GetUserRolesAsync(id);
        _logger.LogInformation($"User {result.Value.Email} retrieved successfully with roles: {string.Join(", ", roles)}");
        return Ok(new { result.Value.Id, result.Value.Email, Roles = roles });
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    /// <param name="dto">new user data</param>
    /// <returns></returns>
    [HttpPost]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Users.CanCreateUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] RegisterUserDto dto)
    {
        _logger.LogInformation($"Attempting to create result with email: {dto.Email}");

        try
        {
            var role = RoleHierarchy.GetRoleByName(dto.Role);

            var result = await _userService.CreateUserAsync(dto.Email, dto.Password, dto.Role);

            if (result.IsFailed)
            {
                var message = $"User with email {dto.Email} already exists.";
                _logger.LogWarning(message);
                return BadRequest(message);
            }

            var successMessage = $"User {dto.Email} created successfully.";

            _logger.LogInformation(successMessage);

            return Ok(new { message = successMessage, result.Value.Id, result.Value.Email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Promote an existing user with Administrator cliams
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("{id}/" + UserEndpoints.Promote)]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Users.CanPromoteUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Promote(string id, [FromBody] ChangeRoleDto dto)
    {
        _logger.LogInformation($"Attempting to promote result with id: {id}");

        var result = await _userService.PromoteUserAsync(id, dto.NewRole);

        if (result.IsFailed)
        {
            var message = $"User with id {id} not found or cannot be promoted.";
            _logger.LogWarning(message);
            return NotFound(message);
        }

        var successMessage = $"User with id {id} promoted successfully.";
        _logger.LogInformation(successMessage);
        return Ok(new { message = successMessage });
    }

    /// <summary>
    /// Remove Administrator claims from an existing user
    /// </summary>
    /// <param name="id">user id</param>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("{id}/" + UserEndpoints.Demote)]
    [MapToApiVersion(ApiConstants.V1)]
    [Authorize(Policy = Policy.Users.CanDemoteUser)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Demote(string id, [FromBody] ChangeRoleDto dto)
    {
        _logger.LogInformation($"Attempting to demote result with id: {id}");

        var result = await _userService.DemoteUserAsync(id, dto.NewRole);

        if (result.IsFailed)
        {
            var message = $"User with id {id} not found or cannot be demoted.";
            _logger.LogWarning(message);
            return NotFound(message);
        }

        var successMessage = $"User with id {id} demoted successfully.";
        _logger.LogInformation(successMessage);
        return Ok(new { message = successMessage });
    }
}
