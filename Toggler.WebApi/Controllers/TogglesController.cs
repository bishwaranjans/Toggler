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
    /// <summary>
    /// Toggle API controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class TogglesController : ControllerBase
    {
        private readonly IRepository<Toggle> _toggleRepository;
        private readonly IRepository<ServiceToggle> _serviceToggleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TogglesController"/> class.
        /// </summary>
        /// <param name="toggleRepository">The toggle repository.</param>
        /// <param name="serviceToggleRepository">The service toggle repository.</param>
        public TogglesController(IRepository<Toggle> toggleRepository, IRepository<ServiceToggle> serviceToggleRepository)
        {
            _toggleRepository = toggleRepository;
            _serviceToggleRepository = serviceToggleRepository;
        }

        // GET: api/Toggles
        /// <summary>
        /// Gets all the toggles.
        /// </summary>
        /// <returns>Returns the list of toggles.</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        public async Task<IEnumerable<Toggle>> Get()

        {
            return await _toggleRepository.GetAllAsync();
        }

        /// <summary>
        /// Gets the toggle with a specified unique name.
        /// </summary>
        /// <param name="name">The unique name of toggle.</param>
        /// <returns>Returns the found toggle.</returns>
        /// <exception cref="HttpResourceNotFoundException"></exception>
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

        /// <summary>
        /// Updates the specified toggle
        /// </summary>
        /// <param name="name">The unique name of the toggle.</param>
        /// <param name="toggle">The toggle.</param>
        /// <returns>Returns the updated toggle.</returns>
        [HttpPut("{name}")]
        public Task<Toggle> Put(string name, [FromBody] Toggle toggle)
        {
            ValidateToggleInfo(toggle);
            return _toggleRepository.UpdateAsync(name, toggle);
        }

        /// <summary>
        /// Creates the toggle
        /// </summary>
        /// <param name="toggle">The toggle.</param>
        /// <returns>Returns the created toggle.</returns>
        [HttpPost]
        public Task Post([FromBody] Toggle toggle)
        {
            ValidateToggleInfo(toggle);
            CheckExistence(toggle);
            return _toggleRepository.CreateAsync(toggle);
        }

        /// <summary>
        /// Deletes the specified toggle with it' unique name.
        /// </summary>
        /// <param name="name">The unique toggle name.</param>
        /// <returns></returns>
        /// <exception cref="HttpResourceNotFoundException"></exception>
        [HttpDelete("{name}")]
        public async Task Delete([FromRoute] string name)
        {
            ValidateDeleteToggleInfo(name);
            var result = await _toggleRepository.DeleteAsync(name);
            if (!result)
            {
                throw new HttpResourceNotFoundException($"Toggle '{name}' doesn't exist");
            }
        }

        /// <summary>
        /// Validates the toggle before deletion.
        /// </summary>
        /// <param name="name">The toggle name.</param>
        /// <exception cref="HttpBadRequestException"></exception>
        private async void ValidateDeleteToggleInfo(string name)
        {
            var serviceToggleMapping = await _serviceToggleRepository.GetAllAsync();
            if (serviceToggleMapping.Any(s => s.Toggle.Name.Equals(name)))
            {
                throw new HttpBadRequestException($"Toggle {name} is being used by services. So can not delete it.");
            }
        }

        /// <summary>
        /// Validates the toggle information.
        /// </summary>
        /// <param name="toggle">The toggle.</param>
        /// <exception cref="HttpBadRequestException">
        /// Toggle name can\'t be empty or whitespace.
        /// or
        /// </exception>
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

        /// <summary>
        /// Checks the existence of toggle.
        /// </summary>
        /// <param name="toggle">The toggle.</param>
        /// <exception cref="HttpBadRequestException">Toggle already exists.</exception>
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