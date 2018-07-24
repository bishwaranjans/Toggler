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
    public class TogglesController : ControllerBase
    {
        private readonly IRepository<Toggle> _toggleRepository;
        private readonly IRepository<ServiceToggle> _serviceToggleRepository;

        public TogglesController(IRepository<Toggle> toggleRepository, IRepository<ServiceToggle> serviceToggleRepository)
        {
            _toggleRepository = toggleRepository;
            _serviceToggleRepository = serviceToggleRepository;
        }

        // GET: api/Toggles
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<Toggle>> Get()

        {
            return await _toggleRepository.GetAllAsync();
        }

        // GET: api/Toggles/5
        [HttpGet("{name}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Toggle>> Get([FromRoute] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var toggle = await _toggleRepository.GetAsync(name);

            if (toggle == null)
            {
                throw new HttpResourceNotFoundException($"Toggle '{name}' doesn't exist");
            }

            return toggle;
        }

        // PUT: api/Toggles/5
        [HttpPut("{name}")]
        public Task<Toggle> Put(string name, [FromBody] Toggle toggle)
        {
            ValidateToggleInfo(toggle);
            return _toggleRepository.UpdateAsync(name, toggle);
        }

        // POST: api/Toggles
        [HttpPost]
        public Task Post([FromBody] Toggle toggle)
        {
            ValidateToggleInfo(toggle);
            CheckExistence(toggle);
            return _toggleRepository.CreateAsync(toggle);
        }

        // DELETE: api/Toggles/5
        [HttpDelete("{name}")]
        public async Task Delete([FromRoute] string name)
        {
            var result = await _toggleRepository.DeleteAsync(name);
            if (!result)
            {
                throw new HttpResourceNotFoundException($"Toggle '{name}' doesn't exist");
            }
        }

        private async void ValidateDeleteToggleInfo(Toggle toggle)
        {
            var serviceToggleMapping = await _serviceToggleRepository.GetAllAsync();
            if (serviceToggleMapping.Any(s => s.Toggle.Name.Equals(toggle.Name)))
            {
                throw new HttpBadRequestException($"Toggle {toggle.Name} is being used by services. So can not delete it.");
            }
        }

        private void ValidateToggleInfo(Toggle toggle)
        {
            if (string.IsNullOrWhiteSpace(toggle.Name))
            {
                throw new HttpBadRequestException("Toggle name can\'t be empty or whitespace.");
            }

            // Check the well know type toggle
            if (!Enum.IsDefined(typeof(Constants.WellKnownToggleType), toggle.Type))
            {
                throw new HttpBadRequestException($"Toggle type is not a well defined type. The well defined types are { string.Join(",", Enum.GetNames(typeof(Constants.WellKnownToggleType)))}");
            }
        }

        private void CheckExistence(Toggle toggle)
        {
            // Check existence
            var availableToggle = _toggleRepository.GetAsync(toggle.Name).Result;
            if (availableToggle != null && availableToggle.Name.Equals(toggle.Name))
            {
                throw new HttpBadRequestException("Toggle already exists.");
            }
        }
    }
}