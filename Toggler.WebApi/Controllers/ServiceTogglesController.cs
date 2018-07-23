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

        // GET: api/ Service Toggles
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IEnumerable<ServiceToggle>> Get(string serviceName, string version)
        {
            var serviceRecords = await _serviceToggleRepository.GetAllAsync();

            var requestedServiceRecords = serviceRecords.Where(s => s.ServiceName.Equals(serviceName) && s.ServiceVersion.Equals(version) && s.IsServiceExcluded == false);
            return requestedServiceRecords;
        }

        // POST: api/Service Toggles
        [HttpPost]
        public async Task<ServiceToggle> Post([FromBody] ServiceToggle serviceToggle)
        {

            var allServiceToggleRecords = _serviceToggleRepository.GetAllAsync().Result;

            #region " BLUE Type Toggle "

            // Always isServiceExcuded = false for BLUE type toggle
            // ToggleName, ToggleValue, ServiceName/Version
            // T1, True, S1 : CREATE
            // T1, False, S1 : UPDATE and make it exclusive now for S1
            // T1, False, S2 : Prevent as now T1 with false can be used by S1 only
            // T1, True, S2 : CREATE
            // 
            // T2, True, S1 : CREATE
            // T2, False, S1 : UPDATE and exclusive now for S1
            // T2, False, S2 : Prevent as now T2 with false can be used by S1 only
            // T2, True, S2 : CREATE
            // 
            // T2, True, S1 : Alreday exists Skip
            // T2, False, S1 : T2 is now already exclusive for S1, Skip
            // T2, False, S2 : T2 is now already exclusive for S1, Skip
            // T2, True, S2 : Already exists

            // Scenario 1 for Blue well known type: 
            if (serviceToggle.ToggleType == Constants.WellKnownToggleType.Blue)
            {
                // Check the well know type toggle
                if (serviceToggle.IsServiceExcluded)
                {
                    throw new HttpBadRequestException($"For BLUE Toggle type IsServiceExcluded value should be FALSE as it is not applicable.");
                }

                var serviceToggleTypeBlueRecords = allServiceToggleRecords as IList<ServiceToggle> ?? allServiceToggleRecords.Where(s => s.ToggleType == Constants.WellKnownToggleType.Blue).ToList();

                if (serviceToggle.IsEnabled)
                {
                    var existingBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s =>
                        s.ServiceName.Equals(serviceToggle.ServiceName) &&
                        s.ToggleName.Equals(serviceToggle.ToggleName) && s.IsEnabled);

                    if (existingBlueRecord == null)
                    {
                        await _serviceToggleRepository.CreateAsync(serviceToggle);
                    }
                    else
                    {
                        throw new HttpBadRequestException($"The requested BLUE type service toggle with TRUE value is already registered.");
                    }
                }
                else
                {
                    var exclusiveBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s =>
                        s.ServiceName.Equals(serviceToggle.ServiceName) &&
                        s.ServiceVersion.Equals(serviceToggle.ServiceVersion) &&
                        s.ToggleName.Equals(serviceToggle.ToggleName) &&
                        !s.IsEnabled);

                    if (exclusiveBlueRecord != null)
                    {
                        exclusiveBlueRecord.IsEnabled = false;
                        await _serviceToggleRepository.UpdateAsync(exclusiveBlueRecord.UniqueId, exclusiveBlueRecord);
                    }
                    else
                    {
                        var existingBlueRecord = serviceToggleTypeBlueRecords.FirstOrDefault(s => s.ToggleName.Equals(serviceToggle.ToggleName) && s.IsEnabled == false);

                        if (existingBlueRecord == null)
                        {
                            await _serviceToggleRepository.CreateAsync(serviceToggle);
                        }
                    }
                }
            }

            #endregion

            #region " GREEN Type toggle "
            // T1, True , S1 : CREATE
            // T1, True, S2 : Prevent, exclusive for S2
            // T1, False , S1 : CREATE
            // T1, False, S2 : CREATE
            //
            // T2, True, S1, isService Excuded=false : CREATE
            // T2, True , S2, isService Excuded=false : Exclusive for S1
            // T2, False, S1, isService Excuded=false : CREATE
            // T2, False, S1, isService Excuded=false : Already exists

            // Scenario 2 for Green well known type
            if (serviceToggle.ToggleType == Constants.WellKnownToggleType.Green)
            {
                // Check the well know type toggle
                if (serviceToggle.IsServiceExcluded)
                {
                    throw new HttpBadRequestException($"For GREEN Toggle type IsServiceExcluded value should be FALSE as it is not applicable.");
                }

                var serviceToggleTypeGreenRecords = allServiceToggleRecords as IList<ServiceToggle> ?? allServiceToggleRecords.Where(s => s.ToggleType == Constants.WellKnownToggleType.Green).ToList();
                if (serviceToggle.IsEnabled)
                {
                    var exclusiveRecord = serviceToggleTypeGreenRecords.FirstOrDefault(s =>
                        s.ToggleName.Equals(serviceToggle.ToggleName) && s.IsEnabled == true);

                    if (exclusiveRecord == null)
                    {
                        await _serviceToggleRepository.CreateAsync(serviceToggle);
                    }
                }
                else
                {
                    var existingBlueRecord = serviceToggleTypeGreenRecords.FirstOrDefault(s =>
                        s.ServiceName.Equals(serviceToggle.ServiceName) &&
                        s.ServiceVersion.Equals(serviceToggle.ServiceVersion) &&
                        s.ToggleName.Equals(serviceToggle.ToggleName) && s.IsEnabled);

                    if (existingBlueRecord == null)
                    {
                        await _serviceToggleRepository.CreateAsync(serviceToggle);
                    }
                }
            }

            #endregion
            #region " RED Type toggle "
            // T1, True, S1, IsService Excuded=true : CREATE
            // T1, True, S1, IsService Excuded=true : Duplicate skip
            // T2, True, S1, IsService Excuded=false : CREATE
            // T1, false, S1 : 

            // Scenario 3 for Red well known type
            if (serviceToggle.ToggleType == Constants.WellKnownToggleType.Red)
            {
                var serviceToggleTypeRedRecords = allServiceToggleRecords as IList<ServiceToggle> ??
                                                  allServiceToggleRecords.Where(s =>
                                                      s.ToggleType == Constants.WellKnownToggleType.Red).ToList();

                var existingBlueRecord = serviceToggleTypeRedRecords.FirstOrDefault(s =>
                    s.ServiceName.Equals(serviceToggle.ServiceName) &&
                    s.ToggleName.Equals(serviceToggle.ToggleName) && s.IsEnabled);

                if (existingBlueRecord == null)
                {
                    await _serviceToggleRepository.CreateAsync(serviceToggle);
                }
            }

            #endregion
            return serviceToggle;
        }
    }
}

