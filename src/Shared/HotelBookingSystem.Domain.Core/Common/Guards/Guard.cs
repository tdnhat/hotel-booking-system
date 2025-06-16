namespace HotelBookingSystem.Domain.Core.Common.Guards
{
    public static class Guard
    {
        public static class Against
        {
            public static string NullOrWhiteSpace(string input, string parameterName)
            {
                if (string.IsNullOrWhiteSpace(input))
                    throw new ArgumentException($"Parameter {parameterName} cannot be null or whitespace.", parameterName);
                return input;
            }

            public static T Null<T>(T input, string parameterName) where T : class
            {
                return input ?? throw new ArgumentNullException(parameterName);
            }

            public static int NegativeOrZero(int input, string parameterName)
            {
                if (input <= 0)
                    throw new ArgumentException($"Parameter {parameterName} must be positive.", parameterName);
                return input;
            }

            public static decimal Negative(decimal input, string parameterName)
            {
                if (input < 0)
                    throw new ArgumentException($"Parameter {parameterName} cannot be negative.", parameterName);
                return input;
            }

            public static int OutOfRange(int input, int min, int max, string parameterName)
            {
                if (input < min || input > max)
                    throw new ArgumentOutOfRangeException(parameterName, $"Parameter {parameterName} must be between {min} and {max}.");
                return input;
            }
        }
    }
}
