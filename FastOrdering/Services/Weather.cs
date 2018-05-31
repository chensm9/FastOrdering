using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace FastOrdering.Services
{
    public class Weather
    {
        public async static Task<RootObject> GetWeather(String cityName)
        {
            var http = new HttpClient();
            String parameter1 = "http://v.juhe.cn/weather/index?format=2&cityname=";
            String parameter2 = "&key=e355b245946c176eee57f7bbc5dfbec8";
            var response = await http.GetAsync(parameter1 + cityName + parameter2);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));

            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (RootObject)serializer.ReadObject(ms);

            return data;
        }
    }
    [DataContract]
    public class Sk
    {
        [DataMember]
        public string temp { get; set; }

        [DataMember]
        public string wind_direction { get; set; }

        [DataMember]
        public string wind_strength { get; set; }

        [DataMember]
        public string humidity { get; set; }

        [DataMember]
        public string time { get; set; }
    }

    [DataContract]
    public class WeatherId
    {
        [DataMember]
        public string fa { get; set; }

        [DataMember]
        public string fb { get; set; }
    }

    [DataContract]
    public class Today
    {
        [DataMember]
        public string temperature { get; set; }

        [DataMember]
        public string weather { get; set; }

        [DataMember]
        public WeatherId weather_id { get; set; }

        [DataMember]
        public string wind { get; set; }

        [DataMember]
        public string week { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string date_y { get; set; }

        [DataMember]
        public string dressing_index { get; set; }

        [DataMember]
        public string dressing_advice { get; set; }

        [DataMember]
        public string uv_index { get; set; }

        [DataMember]
        public string comfort_index { get; set; }

        [DataMember]
        public string wash_index { get; set; }

        [DataMember]
        public string travel_index { get; set; }

        [DataMember]
        public string exercise_index { get; set; }

        [DataMember]
        public string drying_index { get; set; }
    }


    [DataContract]
    public class WeatherId2
    {
        [DataMember]
        public string fa { get; set; }

        [DataMember]
        public string fb { get; set; }
    }

    [DataContract]
    public class Future
    {
        [DataMember]
        public string temperature { get; set; }

        [DataMember]
        public string weather { get; set; }

        [DataMember]
        public WeatherId2 weather_id { get; set; }

        [DataMember]
        public string wind { get; set; }

        [DataMember]
        public string week { get; set; }

        [DataMember]
        public string date { get; set; }
    }

    [DataContract]
    public class Result
    {
        [DataMember]
        public Sk sk { get; set; }

        [DataMember]
        public Today today { get; set; }

        [DataMember]
        public List<Future> future { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public string resultcode { get; set; }

        [DataMember]
        public string reason { get; set; }

        [DataMember]
        public Result result { get; set; }

        [DataMember]
        public int error_code { get; set; }
    }
}
