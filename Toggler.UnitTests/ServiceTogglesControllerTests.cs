using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task Get_Toggles_Of_A_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlue = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            var toggleTypeGreen = new Toggle
            {
                Name = "T2",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };
            var toggleTypeRed = new Toggle
            {
                Name = "T3",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Red
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlue);
            toggleRepository.Setup(r => r.GetAsync("T2")).ReturnsAsync(toggleTypeGreen);
            toggleRepository.Setup(r => r.GetAsync("T3")).ReturnsAsync(toggleTypeRed);

            var serviceS1 = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };
            var serviceS2 = new Service()
            {
                Name = "S2",
                Version = "1.0",
                Description = string.Empty
            };

            #region Toggle Type BLUE 

            var serviceToggleWithBlueTypeTrueS1 = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = serviceS1,
                Toggle = toggleTypeBlue,
                IsServiceExcluded = false
            };

            var serviceToggleWithBlueTypeFalseForS1 = new ServiceToggle
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = serviceS1,
                Toggle = toggleTypeBlue,
                IsServiceExcluded = false
            };

            var serviceToggleWithBlueTypeFalseForS2 = new ServiceToggle
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = serviceS2,
                Toggle = toggleTypeBlue,
                IsServiceExcluded = false
            };

            #endregion

            #region Toggle Type Green 

            var serviceToggleWithGreenTypeTrueS1 = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = serviceS1,
                Toggle = toggleTypeGreen,
                IsServiceExcluded = false
            };

            var serviceToggleWithGreenTypeFalseForS1 = new ServiceToggle
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = serviceS2,
                Toggle = toggleTypeGreen,
                IsServiceExcluded = false
            };

            var serviceToggleWithGreenTypeFalseForS2 = new ServiceToggle
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = serviceS1,
                Toggle = toggleTypeGreen,
                IsServiceExcluded = false
            };

            #endregion

            #region Toggle Type Red

            var serviceToggleWithRedTypeTrueRedExcludeS1 = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = serviceS1,
                Toggle = toggleTypeRed,
                IsServiceExcluded = true
            };

            #endregion

            var list = new List<ServiceToggle>
            {
                serviceToggleWithBlueTypeTrueS1,
                serviceToggleWithBlueTypeFalseForS1,
                serviceToggleWithBlueTypeFalseForS2,
                serviceToggleWithGreenTypeTrueS1,
                serviceToggleWithGreenTypeFalseForS1,
                serviceToggleWithGreenTypeFalseForS2,
                serviceToggleWithRedTypeTrueRedExcludeS1
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var result = (await sut.Get(serviceS1.Name, serviceS1.Version)).ToList();
            Assert.Equal(4, result.Count);
        }

        #region Mapping BLUE type toggle with services

        [Fact]
        public async Task WhenServiceToggle_Has_InValid_Toggle__PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggle1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            var toggle2 = new Toggle
            {
                Name = "T2",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggle1);

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = true,
                Service = service,
                Toggle = toggle2,
                IsServiceExcluded = false
            };

            var exception1 = await Assert.ThrowsAsync<HttpResourceNotFoundException>(
                async () => await sut.Post(serviceToggle));

            Assert.Equal("Toggle 'T2' doesn't exist", exception1.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_BLUE_Or_GREEN_And_IsServiceExcluded_True_PostThrows_HttpBadRequestException()
        {
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var toggleRepository = new Mock<IRepository<Toggle>>();

            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);
            var toggleTypeBlue = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            var toggleTypeGreen = new Toggle
            {
                Name = "T2",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };
            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty,
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlue);
            toggleRepository.Setup(r => r.GetAsync("T2")).ReturnsAsync(toggleTypeGreen);

            var serviceToggleTypeBlue = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Toggle = toggleTypeBlue,
                IsServiceExcluded = true
            };
            var exception1 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlue));

            Assert.Equal("For BLUE Toggle type IsServiceExcluded value should be FALSE as it is not applicable.",
                exception1.Message);

            var serviceToggleTypeGreen = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = service,
                Toggle = toggleTypeGreen,
                IsServiceExcluded = true
            };

            var exception2 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeGreen));

            Assert.Equal("For GREEN Toggle type IsServiceExcluded value should be FALSE as it is not applicable.",
                exception2.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_True_Is_All_Ready_Registered_Or_Duplicate_For_A_Service__PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggle = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggle);

            var list = new List<ServiceToggle>
            {
                new ServiceToggle()
                {
                    UniqueId = "1",
                    IsEnabled = true,
                    Service = service,
                    Toggle = toggle,
                    IsServiceExcluded = false
                }
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var serviceToggleTypeBlueDuplicate = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = true,
                Service = service,
                Toggle = toggle,
                IsServiceExcluded = false
            };

            var exception1 = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlueDuplicate));

            Assert.Equal("The requested BLUE type service toggle with TRUE value is already registered.",
                exception1.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_True_Allow_Mapping_To__Requested_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = true,
                Service = service,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };
            serviceToggleRepository.Setup(r => r.CreateAsync(serviceToggle)).ReturnsAsync(serviceToggle);

            var result = await sut.Post(serviceToggle);
            Assert.Same(serviceToggle, result);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_False_Make_Exclusive_For_Requested_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = service,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };

            var serviceToggleWithFalse = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = service,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };

            var list = new List<ServiceToggle>
            {
                serviceToggle
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);
            serviceToggleRepository.Setup(r => r.UpdateAsync(serviceToggle.UniqueId, serviceToggle)).ReturnsAsync(serviceToggleWithFalse);

            var result = await sut.Post(serviceToggleWithFalse);
            Assert.Equal(serviceToggleWithFalse.IsEnabled, result.IsEnabled);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_False_And_Already_Exclusive_For_A_Service_PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service1 = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = service1,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };

            var list = new List<ServiceToggle>
            {
                serviceToggle
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var exception = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggle));

            Assert.Equal("The requested BLUE type service toggle T1 is exclusive for S1.", exception.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Blue_With_False__Make_Exclusive_On_First_Request_For_A_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Blue
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service1 = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = false,
                Service = service1,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };
            serviceToggleRepository.Setup(r => r.CreateAsync(serviceToggle)).ReturnsAsync(serviceToggle);

            var result = await sut.Post(serviceToggle);
            Assert.Same(serviceToggle, result);
        }

        #endregion

        #region Mapping GREEN type toggle with services

        [Fact]
        public async Task WhenServiceToggle_Type_Green_With_True__Make_Exclusive_On_First_Request_For_A_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service1 = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = service1,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };
            serviceToggleRepository.Setup(r => r.CreateAsync(serviceToggle)).ReturnsAsync(serviceToggle);

            var result = await sut.Post(serviceToggle);
            Assert.Same(serviceToggle, result);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Green_With_True_And_Already_Exclusive_For_A_Service_PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service1 = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "1",
                IsEnabled = true,
                Service = service1,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };

            var list = new List<ServiceToggle>
            {
                serviceToggle
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var exception = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggle));

            Assert.Equal("The requested GREEN type service toggle T1 is exclusive for S1.", exception.Message);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Green_With_False_Allow_Mapping_To__Requested_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = false,
                Service = service,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = false
            };
            serviceToggleRepository.Setup(r => r.CreateAsync(serviceToggle)).ReturnsAsync(serviceToggle);

            var result = await sut.Post(serviceToggle);
            Assert.Same(serviceToggle, result);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Green_With_False_Is_All_Ready_Registered_Or_Duplicate_For_A_Service__PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggle = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Green
            };

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggle);

            var list = new List<ServiceToggle>
            {
                new ServiceToggle()
                {
                    UniqueId = "1",
                    IsEnabled = false,
                    Service = service,
                    Toggle = toggle,
                    IsServiceExcluded = false
                }
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var serviceToggleTypeBlueDuplicate = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = false,
                Service = service,
                Toggle = toggle,
                IsServiceExcluded = false
            };

            var exception = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlueDuplicate));

            Assert.Equal("The requested GREEN type service toggle with FALSE value is already registered.", exception.Message);
        }

        #endregion

        #region Mapping RED type toggle with services

        [Fact]
        public async Task WhenServiceToggle_Type_Red_With_True_And_IsExclusive_True_Allow_Mapping_To__Requested_Service()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggleTypeBlueNamedT1 = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Red
            };
            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggleTypeBlueNamedT1);

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            var serviceToggle = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = false,
                Service = service,
                Toggle = toggleTypeBlueNamedT1,
                IsServiceExcluded = true
            };
            serviceToggleRepository.Setup(r => r.CreateAsync(serviceToggle)).ReturnsAsync(serviceToggle);

            var result = await sut.Post(serviceToggle);
            Assert.Same(serviceToggle, result);
        }

        [Fact]
        public async Task WhenServiceToggle_Type_Red_Registered_Or_Duplicate_For_A_Service__PostThrows_HttpBadRequestException()
        {
            var toggleRepository = new Mock<IRepository<Toggle>>();
            var serviceToggleRepository = new Mock<IRepository<ServiceToggle>>();
            var sut = new ServiceTogglesController(toggleRepository.Object, serviceToggleRepository.Object);

            var toggle = new Toggle
            {
                Name = "T1",
                Description = string.Empty,
                Type = Constants.WellKnownToggleType.Red
            };

            var service = new Service()
            {
                Name = "S1",
                Version = "1.0",
                Description = string.Empty
            };

            toggleRepository.Setup(r => r.GetAsync("T1")).ReturnsAsync(toggle);

            var list = new List<ServiceToggle>
            {
                new ServiceToggle()
                {
                    UniqueId = "1",
                    IsEnabled = false,
                    Service = service,
                    Toggle = toggle,
                    IsServiceExcluded = false
                }
            };
            serviceToggleRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var serviceToggleTypeBlueDuplicate = new ServiceToggle()
            {
                UniqueId = "2",
                IsEnabled = false,
                Service = service,
                Toggle = toggle,
                IsServiceExcluded = false
            };

            var exception = await Assert.ThrowsAsync<HttpBadRequestException>(
                async () => await sut.Post(serviceToggleTypeBlueDuplicate));

            Assert.Equal("The requested RED type service toggle is already registered.", exception.Message);
        }

        #endregion
    }
}
