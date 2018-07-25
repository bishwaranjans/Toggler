# Toggler API Documentation

The Toggler API has been designed inline with SOA and Domain Driven Design(DDD) to provide a toggling feature which can be used by multiple services simultaneously in real-time and dynamic environment.

## API Solution Architecture

DDD approch has been used for designing the architecture of the solution by clearly segregating the each responsibility with clear structure.
 - **Toggler.Common** : Responsible for holding all the common functionality/constants of the solution. 
 - **Toggler.Domain** : Responsible for representing concepts of the business, information about the business situation, and business rules. State that reflects the business situation is controlled and used here, even though the technical details of storing it are delegated to the infrastructure. This layer is the heart of our solution.
 - **Toggler.Infrastructure** : Responsible for how the data that is initially held in domain entities (in memory) is persisted in databases or another persistent store. Entity Framework Core code first has been implemented with the Repository pattern classes that use a DBContext to persist data in a Sqlite DB.
 - **Toggler.UnitTests** : Responsible for mirroring the structure of the code under test. For Toggler API, xUnit has been used for unit testing framework.
 - **Toggler.IdentityServer** : Responsible for authenticating each request to the WebApi with the use of OAuth 2.0 resource owner password grant. This allows a client to send a user's name and password to identityserver to request a token representing that user and with the received token can access the API. **_TODO_:** *More customization needs to be done for hosting.*
 - **Toggler.WebApi** : Responsible for defining the jobs the software is supposed to do and directs the expressive domain toggle objects to work out problems. It is responsible for interaction with the layers of other systems. It coordinates tasks of creating toggles and mapping these toggles with service which requests them.
 
 ![alt text](https://github.com/bishwaranjans/Toggler/blob/master/TogglerSolutionArchitecture.png)
 
## Toggler Persistence Mechanism
 
Entity Framework Core with Sqlite has been used for persisting the domain entities values along with repository pattern. In order to  facilitate the developmnet requirement of Toggler API, the toggles have been categoried into 3 major type depending upon how they should behave as per the requirement.

#### Well Known Toggle Type
     public enum WellKnownToggleType
        {
            /// <summary>
            /// Toggle type BLUE with TRUE can be used by all services.
            /// Toggle type BLUE with FALSE can only be used by the service making it exclusive which has a mapping with Toggle(type BLUE with TRUE) by updating the value to FALSE
            /// No other service can use the toggle if it made exclusive
            /// </summary>
            Blue,

            /// <summary>
            /// Toggle type GREEN with TRUE can be used by a service and once used the toggle become exclusive.
            /// </summary>
            Green,

            /// <summary>
            /// Toggle type RED with TRUE can be used by all services but can be restricted to any specific service
            /// </summary>
            Red
        };
    
#### Domain Entity : Toggle
    public class Toggle
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Constants.WellKnownToggleType Type { get; set; }
    }
    
#### Domain Entity : Service
    public class Service
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }
#### Domain Entities Mapper : ServiceToggle
    public class ServiceToggle
    {
        public string UniqueId { get; set; }
        public bool IsEnabled { get; set; }
        public Service Service { get; set; }
        public Toggle Toggle { get; set; }
        public bool IsServiceExcluded { get; set; } = false;
    }
## API Layer Breakthrough
The API provides two major functionalities i.e. managing toggles and mapping toggles with services whenever they request. All the functionalities can be tested through a swagger UI.

 **_SWAGGER UI_:** : *Swagger UI for testing out this feature can be accessed via this :[Url](http://localhost::44378)*. In otherwords, for example swagger UI for this API application can be access like *http://localhost:44378/swagger* where **_44378_** is the port of the Toggler.WebApi project. **_Examples along with description of end points can be found in the swagger UI for respective end point._**
 
   - **TogglesController** :     
   This controller API provides end points for doing CRUD opeartions related to toggle domain entity. User must have to create toggles before using them in services. Before requesting a toggle, the service must have to make sure that the toggle exists. Sample access Url : *https://localhost:44378/api/toggles*

        *Sample POST Request to URL _**/api/Toggles/Post**_*

        ```
        {
          "name": "T1",
          "description": "Type Blue Toggle",
          "type": 0
        }
        ```
   - **ServiceTogglesController** :  
   This controller API provides end points for requesting toggle by a service, returning all the toggles used a specific service with it's version and also allows to delete an existing mapping between service nad toggle by specifying the mapping unique id. Sample access Url : *https://localhost:44378/api/ServiceToggles?serviceName=s&version=1*
    
      *Sample POST Request to URL _**/api/ServiceToggles/Post**_*

        ```
        {
            "uniqueId": "1", // Make sure to provide a unique Id for mapping. Otherwise it will throw a bad request
            "isEnabled": true, // Toggle value can be of True/False
            "service": {
              "name": "Service1",
              "version": "1.0",
              "description": ""
            },
            "toggle": {  // Make sure this toggle exists in the application. Toggle can be created by TogglesController/POST end point
              "name": "T1",
              "description": "",
              "type": 0 // 0-Blue, 1-Green and 2-Red : Predefined well known toggle type.
            },
            "isServiceExcluded": false // Explicitly used by Red type toggle only. Other type toggle can not set the value of it to TRUE
         }
        ```
        
       *Sample Expected behaviour for BLUE type in sequence*
        ```
          Always isServiceExcuded = false for BLUE type toggle
          ToggleName   ToggleValue  ServiceName    Version : Result
          T1           True         S1             1.0     : CREATE
          T1           False        S1             1.0     : UPDATE and make it exclusive now for S1
          T1           False        S2             1.0     : Prevent as now T1 with false can be used by S1 only
          T1           True         S2             1.0     : CREATE
                     
          T2           True         S1             1.0     : CREATE
          T2           False        S1             1.0     : UPDATE and exclusive now for S1
          T2           False        S2             1.0     : Prevent as now T2 with false can be used by S1 only
          T2           True         S2             1.0     : CREATE
                     
          T2           True         S1             1.0     : Alreday exists Skip
          T2           False        S1             1.0     : T2 is now already exclusive for S1, Skip
          T2           False        S2             1.0     : T2 is now already exclusive for S1, Skip
          T2           True         S2             1.0     : Already exists
        ```
        
        *Sample Expected behaviour for GREEN type in sequence*
        ```
         Always isServiceExcuded = false for GREEN type toggle
         ToggleName   ToggleValue  ServiceName    Version : Result
         T1           True         S1             1.0     : CREATE
         T1           True         S2             1.0     : Prevent, exclusive for S1
         T1           False        S1             1.0     : CREATE
         T1           False        S2             1.0     : CREATE
                    
         T2           True         S1             1.0     : CREATE
         T2           True         S2             1.0     : Prevent, Exclusive for S1
         T2           False        S1             1.0     : CREATE
         T2           False        S1             1.0     : Already exists

        ```
        *Sample Expected behaviour for RED type in sequence*
        
        For RED type toggle, user has to explictly mentioned that the toggle is red type and should be excluded
        ```
        {
            "uniqueId": "1", 
            "isEnabled": true, 
            "service": {
              "name": "Service1",
              "version": "1.0",
              "description": ""
            },
            "toggle": {  
              "name": "T1",
              "description": "",
              "type": 2 // 0-Blue, 1-Green and 2-Red : Predefined well known toggle type.
            },
            "isServiceExcluded": _**true**_ // It means Service1 will be excluded to use toggle T1
         }
        ```
