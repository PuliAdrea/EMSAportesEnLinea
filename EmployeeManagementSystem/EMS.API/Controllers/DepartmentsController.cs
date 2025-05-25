using AutoMapper;
using EMS.API.DTOs.DepartmentDTOs;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<DepartmentsController> _logger;
    private readonly IMapper _mapper;

    public DepartmentsController(
        IDepartmentService departmentService,
        IEmployeeService employeeService,
        IMapper mapper,
        ILogger<DepartmentsController> logger)
    {
        _departmentService = departmentService;
        _employeeService = employeeService;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentResponseDTO>>> GetAll()
    {
        try
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var result = _mapper.Map<List<DepartmentResponseDTO>>(departments);

            foreach (var department in result)
            {
                department.TotalSalaryExpenditure = await _employeeService.CalculateDepartmentSalaryAsync(department.Id);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all departments");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<DepartmentResponseDTO>> Create([FromBody] DepartmentCreateDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = _mapper.Map<Department>(dto);
            var createdDepartment = await _departmentService.CreateDepartmentAsync(department);
            var result = _mapper.Map<DepartmentResponseDTO>(createdDepartment);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating department");
            return StatusCode(500, "Internal Server Error");
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<DepartmentResponseDTO>> GetById(int id)
    {
        try
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);

            if (department == null)
                return NotFound();

            var result = _mapper.Map<DepartmentResponseDTO>(department);
            result.TotalSalaryExpenditure = await _employeeService.CalculateDepartmentSalaryAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting department with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentUpdateDTO dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest("ID does not match");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = _mapper.Map<Department>(dto);
            await _departmentService.UpdateDepartmentAsync(department);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating department with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _departmentService.DeleteDepartmentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting department with ID {id}");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}/salary-total")]
    public async Task<ActionResult<decimal>> GetDepartmentSalaryTotal(int id)
    {
        try
        {
            var total = await _employeeService.CalculateDepartmentSalaryAsync(id);
            return Ok(total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error calculating total salary for department with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}