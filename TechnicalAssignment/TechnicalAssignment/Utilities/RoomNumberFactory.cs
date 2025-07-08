using System;

namespace TechnicalAssignment.Utilities
{
    /// <summary>
    /// Provides methods for generating test data, specifically room numbers.
    /// This centralizes test data creation logic, separating it from test execution flow.
    /// </summary>
    public static class RoomNumberFactory
    {
        private static readonly Random _random = new();

        /// <summary>
        /// Generates a random room number within the valid range for creation.
        /// The application's validator for room creation is set to 1-999.
        /// However, since some tests calculate the price as (room number + 100) and the maximum allowed price is 1000,
        /// this method restricts the range to 1-899 to ensure test data remains valid.
        /// </summary>
        /// <returns>A valid random room number for creation tests.</returns>
        public static int GetRandomRoomNumber()
        {
            return _random.Next(1, 900); // 1 to 899
        }

        /// <summary>
        /// Generates a room number outside the standard validation range (e.g., > 1000).
        /// This is used specifically for testing scenarios where the edit functionality
        /// has a validation gap and does not correctly enforce the maximum room number limit,
        /// unlike the creation form.
        /// </summary>
        /// <returns>A random room number intended to bypass weak validation.</returns>
        public static int GetExoticRoomNumber()
        {
            return _random.Next(1001, 9999);
        }
    }
} 