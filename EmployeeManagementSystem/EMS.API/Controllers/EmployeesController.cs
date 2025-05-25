using AutoMapper;
using EMS.API.DTOs;
using EMS.API.DTOs.EmployeeDTOs;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, IMapper mapper, IUnitOfWork unitOfWork, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
   
    
    [HttpGet("department/{departmentId}")]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetByDepartment(int departmentId)
    {
        var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
        return Ok(_mapper.Map<List<EmployeeResponseDTO>>(employees));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDTO>>> GetAll()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            var result = _mapper.Map<List<EmployeeResponseDTO>>(employees);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeResponseDTO>> GetById(int id)
    {
        try
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);

            if (employee == null)
                return NotFound();

            return _mapper.Map<EmployeeResponseDTO>(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting employee with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDTO>> Create([FromBody] EmployeeCreateDTO dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var departmentExists = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId) != null;
            if (!departmentExists)
            {
                return BadRequest("The specified department does not exist");
            }

            var employee = _mapper.Map<Employee>(dto);

            if (employee.BaseSalary <= 0)
            {
                return BadRequest("The base salary must be greater than zero");
            }

            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CompleteAsync();

            var responseDto = _mapper.Map<EmployeeResponseDTO>(employee);

            responseDto.CalculatedSalary = await _employeeService.CalculateSalaryAsync(employee.Id);

            return CreatedAtAction(
                nameof(GetSalary),
                new { id = employee.Id },
                responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "An internal error occurred while processing the request");
        }
    }

    [HttpGet("{id}/salary")]
    public async Task<ActionResult<decimal>> GetSalary(int id)
    {
        try
        {
            return await _employeeService.CalculateSalaryAsync(id);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDTO dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest("ID does not match");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _mapper.Map<Employee>(dto);

            await _employeeService.UpdateEmployeeAsync(employee);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating employee with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting employee with ID {id}");
            return StatusCode(500, "Internal Server Error");
        }
    }
}