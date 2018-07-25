using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toggler.Common;
using Toggler.Domain.Entities;
using Toggler.Domain.SeedWork.Interfaces;

namespace Toggler.WebApi.Controllers
{

    /// <summary>
    /// Service toggle mapping API controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServiceTogglesController : ControllerBase
    {
        private readonly IRepository<Toggle> _toggleRepository;
        private readonly IRepository<ServiceToggle> _serviceToggleRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceTogglesController"/> class.
        /// </summary>
        /// <param name="toggleRepository">The toggle repository.</param>
        /// <param name="serviceToggleRepository">The service toggle repository.</param>
        public ServiceTogglesController(IRepository<Toggle> toggleRepository, IRepository<ServiceToggle> serviceToggleRepository)
        {
            _serviceToggleRepository = serviceToggleRepository;
            _toggleRepository = toggleRepository;
        }

        // GET: api/ Service Toggles
        /// <summary>
        /// Gets the specified service details along with the mapped toggles with their values.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <param name="version">The version.</param>
        /// <returns>Returns list of mapped service toggles.</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IEnumerable<ServiceToggle>> Get(string serviceName, string version)
        {
            var serviceRecords = await _serviceToggleRepository.GetAllAsync();

            var requestedServiceRecords = serviceRecords.Where(s => s.Service.Name.Equals(serviceName) && s.Service.Version.Equals(version) && s.IsServiceExcluded == false);
            return requestedServiceRecords;
        }

        // POST: api/Service Toggles
        /// <summary>
        /// Posts the specified service toggle.
        /// </summary>
        /// <param name="serviceToggle">The service toggle.</param>
        /// <returns></returns>
        /// <exception cref="HttpResourceNotFoundException">
        /// </exception>
        /// <exception cref="HttpBadRequestException">
        /// For BLUE Toggle type IsServiceExcluded value should be FALSE as it is not applicable.
        /// or
        /// The requested BLUE type service toggle with TRUE value is already registered.
        /// or
        /// or
        /// For GREEN Toggle type IsServiceExcluded value should be FALSE as it is not applicable.
        /// or
        /// or
        /// The requested GREEN type service toggle with FALSE value is already registered.
        /// or
        /// The requested RED type service toggle is already registered.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [HttpPost]
        public async Task<ServiceToggle> Post([FromBody] ServiceToggle serviceToggle)
        {
            var allServiceToggleRecords = (await _serviceToggleRepository.GetAllAsync()).ToList();

            // Validate Toggle existence
            var toggle = await _toggleRepository.GetAsync(serviceToggle.Toggle.Name);

            if (toggle == null)
            {
                throw new HttpResourceNotFoundException($"Toggle '{serviceToggle.Toggle.Name}' doesn't exist");
            }

            // Validate uniqueId for mapping
            if (allServiceToggleRecords.Any(s => s.UniqueId.Equals(serviceToggle.UniqueId)))
            {
                throw new HttpResourceNotFoundException($"Service Toggle with unique id '{serviceToggle.UniqueId}' already exists");
            }

            switch (toggle.Type)
            {
                case Constants.WellKnownToggleType.Blue: // TODO: Make it more modular

                    #region " BLUE Type Toggle " 

                    // Always isServiceExcuded = false for BLUE type toggle
                    // ToggleName   ToggleValue  ServiceName    Version : Result
                    // T1           True         S1             1.0     : CREATE
                    // T1           False        S1             1.0     : UPDATE and make it exclusive now for S1
                    // T1           False        S2             1.0     : Prevent as now T1 with false can be used by S1 only
                    // T1           True         S2             1.0     : CREATE
                    // 
                    // T2           True         S1             1.0     : CREATE
                    // T2           False        S1             1.0     : UPDATE and exclusive now for S1
                    // T2           False        S2             1.0     : Prevent as now T2 with false can be used by S1 only
                    // T2           True         S2             1.0     : CREATE
                    // 
                    // T2           True         S1             1.0     : Alreday exists Skip
                    // T2           False        S1             1.0     : T2 is now already exclusive for S1, Skip
                    // T2           False        S2             1.0     : T2 is now already exclusive for S1, Skip
                    // T2           True         S2             1.0     : Already exists

                    // Check the well know type toggle
                    if (serviceToggle.IsServiceExcluded)
                    {
                        throw new HttpBadRequestException("For BLUE Toggle type IsServiceExcluded value should be FALSE as it is not applicable.");
                    }

                    var serviceToggleTypeBlueRecords = allServiceToggleRecords.Where(s => s.Toggle.Type == Constants.WellKnownToggleType.Blue).ToList();

                    if (serviceToggle.IsEnabled)
                    {
                        var existingBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s =>
                            s.Service.Name.Equals(serviceToggle.Service.Name) &&
                            s.Toggle.Name.Equals(serviceToggle.Toggle.Name) && s.IsEnabled);

                        if (existingBlueRecord == null)
                        {
                            return await _serviceToggleRepository.CreateAsync(serviceToggle);
                        }
                        else
                        {
                            throw new HttpBadRequestException("The requested BLUE type service toggle with TRUE value is already registered.");
                        }
                    }
                    else
                    {
                        var exclusiveBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s =>
                            s.Service.Name.Equals(serviceToggle.Service.Name) &&
                            s.Service.Version.Equals(serviceToggle.Service.Version) &&
                            s.Toggle.Name.Equals(serviceToggle.Toggle.Name) &&
                            s.IsEnabled);

                        if (exclusiveBlueRecord != null)
                        {
                            exclusiveBlueRecord.IsEnabled = false;
                            return await _serviceToggleRepository.UpdateAsync(exclusiveBlueRecord.UniqueId, exclusiveBlueRecord);
                        }
                        else
                        {
                            var existingBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s => s.Toggle.Name.Equals(serviceToggle.Toggle.Name) && s.IsEnabled == false);

                            // Make toggle exclusive on first request itself for a service
                            if (existingBlueRecord == null)
                            {
                                return await _serviceToggleRepository.CreateAsync(serviceToggle);
                            }
                            else
                            {
                                throw new HttpBadRequestException($"The requested BLUE type service toggle {serviceToggle.Toggle.Name} is exclusive for {existingBlueRecord.Service.Name}.");
                            }
                        }
                    }
                #endregion

                case Constants.WellKnownToggleType.Green: // TODO: Make it more modular

                    #region " GREEN Type toggle "
                    // Always isServiceExcuded = false for GREEN type toggle
                    // ToggleName   ToggleValue  ServiceName    Version : Result
                    // T1           True         S1             1.0     : CREATE
                    // T1           True         S2             1.0     : Prevent, exclusive for S1
                    // T1           False        S1             1.0     : CREATE
                    // T1           False        S2             1.0     : CREATE
                    //
                    // T2           True         S1             1.0     : CREATE
                    // T2           True         S2             1.0     : Prevent, Exclusive for S1
                    // T2           False        S1             1.0     : CREATE
                    // T2           False        S1             1.0     : Already exists

                    // Check the well know type toggle
                    if (serviceToggle.IsServiceExcluded)
                    {
                        throw new HttpBadRequestException("For GREEN Toggle type IsServiceExcluded value should be FALSE as it is not applicable.");
                    }

                    var serviceToggleTypeGreenRecords = allServiceToggleRecords.Where(s => s.Toggle.Type == Constants.WellKnownToggleType.Green).ToList();
                    if (serviceToggle.IsEnabled)
                    {
                        var exclusiveRecord = serviceToggleTypeGreenRecords.FirstOrDefault(s => s.Toggle.Name.Equals(serviceToggle.Toggle.Name) && s.IsEnabled);

                        if (exclusiveRecord == null)
                        {
                            return await _serviceToggleRepository.CreateAsync(serviceToggle);
                        }
                        else
                        {
                            throw new HttpBadRequestException($"The requested GREEN type service toggle {serviceToggle.Toggle.Name} is exclusive for {exclusiveRecord.Service.Name}.");
                        }
                    }
                    else
                    {
                        var existingBlueRecord = serviceToggleTypeGreenRecords.FirstOrDefault(s =>
                            s.Service.Name.Equals(serviceToggle.Service.Name) &&
                            s.Service.Version.Equals(serviceToggle.Service.Version) &&
                            s.Toggle.Name.Equals(serviceToggle.Toggle.Name) && s.IsEnabled == false);

                        if (existingBlueRecord == null)
                        {
                            return await _serviceToggleRepository.CreateAsync(serviceToggle);
                        }
                        else
                        {
                            throw new HttpBadRequestException("The requested GREEN type service toggle with FALSE value is already registered.");
                        }
                    }

                #endregion

                case Constants.WellKnownToggleType.Red: // TODO: Make it more modular

                    #region " RED Type toggle "
                    var serviceToggleTypeRedRecords = allServiceToggleRecords.Where(s => s.Toggle.Type == Constants.WellKnownToggleType.Red).ToList();

                    var existingRedRecord = serviceToggleTypeRedRecords.FirstOrDefault(s =>
                    s.Service.Name.Equals(serviceToggle.Service.Name) &&
                    s.Toggle.Name.Equals(serviceToggle.Toggle.Name) &&
                    s.IsServiceExcluded == serviceToggle.IsServiceExcluded &&
                    s.IsEnabled == serviceToggle.IsEnabled);

                    var existingAlreadyExcludedRedRecord = serviceToggleTypeRedRecords.FirstOrDefault(s =>
                        s.Service.Name.Equals(serviceToggle.Service.Name) &&
                        s.Toggle.Name.Equals(serviceToggle.Toggle.Name) &&
                        s.IsServiceExcluded);

                    // If a service is already excluded, dont do anything
                    if (existingAlreadyExcludedRedRecord != null)
                    {
                        return null;
                    }

                    if (existingRedRecord == null)
                    {
                        return await _serviceToggleRepository.CreateAsync(serviceToggle);
                    }
                    else
                    {
                        throw new HttpBadRequestException("The requested RED type service toggle is already registered.");
                    }

                #endregion 

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // DELETE: api/Toggles/5
        /// <summary>
        /// Deletes the specified service toggle mapping.
        /// </summary>
        /// <param name="uniqueId">The unique identifier.</param>
        /// <returns></returns>
        /// <exception cref="HttpResourceNotFoundException"></exception>
        [HttpDelete("{UniqueId}")]
        public async Task Delete([FromRoute] string uniqueId)
        {
            var result = await _serviceToggleRepository.DeleteAsync(uniqueId);
            if (!result)
            {
                throw new HttpResourceNotFoundException($"Toggle with unique id '{uniqueId}' doesn't exist");
            }
        }
    }
}

