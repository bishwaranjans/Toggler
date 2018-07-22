using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Toggler.Common;
using Toggler.Domain.Entities;
using Toggler.Domain.SeedWork.Interfaces;

namespace Toggler.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTogglesController : ControllerBase
    {
        private readonly IRepository<ServiceToggle> _serviceToggleRepository;

        public ServiceTogglesController(IRepository<ServiceToggle> serviceToggleRepository)
        {
            _serviceToggleRepository = serviceToggleRepository;

        }

        // GET: api/Toggles
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IEnumerable<ServiceToggle>> Get(string serviceName, string version)
        {
            return await _serviceToggleRepository.GetAllAsync();
        }

        // POST: api/Toggles
        [HttpPost]
        public async Task<Task<ServiceToggle>> Post([FromBody] ServiceToggle serviceToggle)
        {
            await ValidateToggleInfo(serviceToggle);
            return _serviceToggleRepository.CreateAsync(serviceToggle);
        }

        private async Task ValidateToggleInfo(ServiceToggle serviceToggle)
        {
            var allServiceToggleRecords = await _serviceToggleRepository.GetAllAsync();
            var serviceToggleRecords = allServiceToggleRecords as IList<ServiceToggle> ?? allServiceToggleRecords.ToList();
            var currentServiceToggleRecords = serviceToggleRecords.Where(s => s.Service.Name.Equals(serviceToggle.Service.Name) && s.Toggle.ToggleType == serviceToggle.Toggle.ToggleType).ToList();

            // Scenario 1 for Blue well known type: 
            if (serviceToggle.Toggle.ToggleType == Constants.WellKnownToggleType.Blue)
            {
                if (serviceToggle.IsEnabled)
                {
                    var isAlreadyExclusive = currentServiceToggleRecords.Any(s => !s.IsEnabled);
                    if(!isAlreadyExclusive)
                    {
                        await _serviceToggleRepository.CreateAsync(serviceToggle);
                    }
                }
                else
                {
                    await _serviceToggleRepository.UpdateAsync(serviceToggle.UniqueId, serviceToggle);
                }
            }

            // Scenario 2 for Green well known type


        }
    }
}