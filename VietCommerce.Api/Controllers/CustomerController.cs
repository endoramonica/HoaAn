using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietCommerce.Api.Services.Interfaces;
using VietCommerce.Core.DTOs.Customers;

namespace VietCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetCustomers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
    {
        var response = await _customerService.GetCustomersAsync(pageNumber, pageSize, search);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var response = await _customerService.GetCustomerByIdAsync(id);
        if (!response.Success || response.Data == null)
            return NotFound(response);
        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerCreateDTO dto)
    {
        var response = await _customerService.CreateCustomerAsync(dto);
        if (!response.Success)
            return BadRequest(response);
        return CreatedAtAction(nameof(GetCustomerById), new { id = response.Data.Id }, response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager,Staff")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerUpdateDTO dto)
    {
        var response = await _customerService.UpdateCustomerAsync(id, dto);
        if (!response.Success)
            return BadRequest(response);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        var response = await _customerService.DeleteCustomerAsync(id);
        return Ok(response);
    }
}
