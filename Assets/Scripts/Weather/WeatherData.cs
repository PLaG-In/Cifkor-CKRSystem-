using System;
using System.Collections.Generic;

namespace Weather
{
    [Serializable]
    public class WeatherResponse
    {
        public WeatherProperties properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        public List<WeatherPeriod> periods;
    }

    [Serializable]
    public class WeatherPeriod
    {
        public int temperature;
        public string temperatureUnit;
        public string icon;
    }
}
