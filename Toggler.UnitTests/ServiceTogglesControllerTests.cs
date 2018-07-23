using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Toggler.Common;
using Toggler.Domain.Entities;
using Toggler.Domain.SeedWork.Interfaces;
using Toggler.WebApi;
using Toggler.WebApi.Controllers;
using Xunit;

namespace Toggler.UnitTests
{
    public class ServiceTogglesControllerTests
    {
        [Fact]
        public async Task WhenServiceToggle_Has_Type_BLUE_Or_GREEN_And_IsServiceExcluded_True_PostThrows_HttpBadRequestException()
        {
            var _serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            _serviceToggleRepository.Setup(r => r.GetAsync(It.IsAny<string>())).ReturnsAsync((ServiceToggle)null);

            var sut = new ServiceTogglesController(_serviceToggleRepository.Object);

            var serviceToggleTypeBlue = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                ServiceName = "S1",
                ServiceVersion = "1.0",
                ServiceDescription = string.Empty,
                ToggleName = "T1",
                ToggleDescription = string.Empty,
                ToggleType = Constants.WellKnownToggleType.Blue,
                IsServiceExcluded = true
            };
            var exception1 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlue));

            Assert.Equal("For BLUE Toggle type IsServiceExcluded value should be FALSE as it is not applicable.", exception1.Message);

            var serviceToggleTypeGreen = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                ServiceName = "S1",
                ServiceVersion = "1.0",
                ServiceDescription = string.Empty,
                ToggleName = "T1",
                ToggleDescription = string.Empty,
                ToggleType = Constants.WellKnownToggleType.Green,
                IsServiceExcluded = true
            };

            var exception2 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeGreen));

            Assert.Equal("For GREEN Toggle type IsServiceExcluded value should be FALSE as it is not applicable.", exception2.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_TRUE_value_Is_All_Ready_Registered_For_A_Service__PostThrows_HttpBadRequestException()
        {
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(serviceToggleRepository.Object);

            var list = new List<ServiceToggle>
            {
                new ServiceToggle()
                {
                    UniqueId = "1",
                    IsEnabled = true,
                    ServiceName = "S1",
                    ServiceVersion = "1.0",
                    ServiceDescription = string.Empty,
                    ToggleName = "T1",
                    ToggleDescription = string.Empty,
                    ToggleType = Constants.WellKnownToggleType.Blue,
                    IsServiceExcluded = false
                }
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync((List<ServiceToggle>)list);

            var serviceToggleTypeBlueDuplicate = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = true,
                ServiceName = "S1",
                ServiceVersion = "1.0",
                ServiceDescription = string.Empty,
                ToggleName = "T1",
                ToggleDescription = string.Empty,
                ToggleType = Constants.WellKnownToggleType.Blue,
                IsServiceExcluded = false
            };

            var exception1 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlueDuplicate));

            Assert.Equal("The requested BLUE type service toggle with TRUE value is already registered.", exception1.Message);

        }
    }
}
