namespace Toggler.Common
{
    /// <summary>
    /// Common constant file
    /// </summary>
    public static class Constants
    {
        // TODO: Please add more toggle type if needed
        /// <summary>
        /// Well known toggle type
        /// </summary>
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
    }
}
