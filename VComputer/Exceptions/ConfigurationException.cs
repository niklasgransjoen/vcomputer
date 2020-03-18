using System;

namespace VComputer
{
    public sealed class ConfigurationException : Exception
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string message) : base(message)
        {
        }
    }
}