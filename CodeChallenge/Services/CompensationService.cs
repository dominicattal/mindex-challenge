using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class CompensationService : ICompensationService
    {
        private readonly ICompensationRepository _compensationRepository;
        private readonly ILogger<CompensationService> _logger;

        public CompensationService(ILogger<CompensationService> logger, ICompensationRepository compensationRepository)
        {
            _compensationRepository = compensationRepository;
            _logger = logger;
        }

        public Compensation CreateOrReplace(Compensation compensation)
        {
            if (compensation == null)
                return compensation;
            
            var existingCompensation = _compensationRepository.GetById(compensation.EmployeeId);
            if (existingCompensation == null) {
                _compensationRepository.Add(compensation);
            } else {
                _compensationRepository.Remove(existingCompensation);
            }
            _compensationRepository.Add(compensation);

            return compensation;
        }

        public Compensation GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _compensationRepository.GetById(id);
            }

            return null;
        }
    }
}
