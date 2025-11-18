using Lab10_Lazarinos.Application.DTOs.Common;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public RolesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtener todos los roles
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<Role>>>> GetAllRoles()
    {
        try
        {
            var roles = await _unitOfWork.Roles.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<Role>>.SuccessResponse(roles));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<Role>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener un rol por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Role>>> GetRoleById(Guid id)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            
            if (role == null)
                return NotFound(ApiResponse<Role>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<Role>.SuccessResponse(role));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Role>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener un rol por nombre
    /// </summary>
    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<ApiResponse<Role>>> GetRoleByName(string name)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByNameAsync(name);
            
            if (role == null)
                return NotFound(ApiResponse<Role>.ErrorResponse("Role not found"));

            return Ok(ApiResponse<Role>.SuccessResponse(role));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Role>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Crear un nuevo rol
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Role>>> CreateRole([FromBody] string roleName)
    {
        try
        {
            // Verificar si ya existe
            var existingRole = await _unitOfWork.Roles.GetByNameAsync(roleName);
            if (existingRole != null)
                return BadRequest(ApiResponse<Role>.ErrorResponse("Role already exists"));

            var role = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleName
            };

            await _unitOfWork.Roles.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRoleById), new { id = role.RoleId },
                ApiResponse<Role>.SuccessResponse(role, "Role created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Role>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Actualizar un rol
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<Role>>> UpdateRole(Guid id, [FromBody] string newRoleName)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            
            if (role == null)
                return NotFound(ApiResponse<Role>.ErrorResponse("Role not found"));

            // Verificar si el nuevo nombre ya existe
            var existingRole = await _unitOfWork.Roles.GetByNameAsync(newRoleName);
            if (existingRole != null && existingRole.RoleId != id)
                return BadRequest(ApiResponse<Role>.ErrorResponse("Role name already exists"));

            role.RoleName = newRoleName;
            
            _unitOfWork.Roles.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<Role>.SuccessResponse(role, "Role updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<Role>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Eliminar un rol
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteRole(Guid id)
    {
        try
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            
            if (role == null)
                return NotFound(ApiResponse<bool>.ErrorResponse("Role not found"));

            _unitOfWork.Roles.Remove(role);
            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Role deleted successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }
}