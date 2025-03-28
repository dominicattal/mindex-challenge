using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;
using System.Collections;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpGet("report/{id}")]
        public IActionResult GetEmployeeNumberOfReports(String id)
        {
            _logger.LogDebug($"Received employee report request for '{id}'");

            var employee = _employeeService.GetById(id);
            if (employee == null)
                return NotFound();

            // Use dfs with stack to find all employees
            Stack<String> stack = new Stack<String>();
            stack.Push(id);

            // Keep track of seen employees to check for circular references
            HashSet<String> seen = new HashSet<String>();

            int res = employee.DirectReports.Count;
            int count = 0;
            while (stack.Count > 0) {
                var empId = stack.Pop();

                // If we've seen this employee before, stop the search
                if (seen.Contains(empId))
                    return Conflict(new { message = "Circular reference detected, operation aborted"});
                seen.Add(empId);

                // check if employee has direct reports
                employee = _employeeService.GetById(empId);
                if (employee.DirectReports == null) {
                    continue;
                }

                // parse the next employee
                foreach (var emp in employee.DirectReports) {
                    stack.Push(emp.EmployeeId);
                    count += 1;
                }
            }
            
            return Ok(new ReportingStructure {
                EmployeeId = id,
                numberOfReports = count
            });
        }
    }
}
