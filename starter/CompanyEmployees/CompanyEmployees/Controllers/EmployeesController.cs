using System;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers
{
	[Route("api/companies/{companyId}/employees")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly IRepositoryManager _repository;
		private readonly ILoggerManager _logger;
		private readonly IMapper _mapper;

		public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
		{
			_repository = repository;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpPatch("{id}")]
		public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
		{
			if(patchDoc == null)
			{
				_logger.LogError("patchDoc object sent from client is null.");
				return BadRequest("patchDoc object is null");
			}

			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeEntity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);
			if (employeeEntity == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

			patchDoc.ApplyTo(employeeToPatch);

			_mapper.Map(employeeToPatch, employeeEntity);

			_repository.Save();

			return NoContent();
		}
	}
}