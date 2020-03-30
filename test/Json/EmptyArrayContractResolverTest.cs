using System.Collections.Generic;
using KodeAid.Json.Serialization;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid.Json
{
    public class EmptyArrayContractResolverTest
    {
        private readonly ITestOutputHelper _output;

        public EmptyArrayContractResolverTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AllPropertiesShouldBeSerializedWithNoResolver()
        {
            var settings = new JsonSerializerSettings()
            {
            };

            var json = JsonConvert.SerializeObject(new D(), settings);

            _output?.WriteLine(json);
            Assert.NotEqual("boo", json);
        }

        [Fact]
        public void AllEmptyPropertiesShouldNotBeSerializedWithResolver()
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new EmptyArrayContractResolver()
            };

            var json = JsonConvert.SerializeObject(new D(), settings);

            _output?.WriteLine(json);
            Assert.NotEqual("boo", json);
        }
    }

    internal class D
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public List<string> NullList { get; set; } = null;
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public List<string> EmptyList { get; set; } = new List<string>();
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public List<string> List { get; set; } = new List<string>() { "item1" };

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] NullArray { get; set; } = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] EmptyArray { get; set; } = new string[0];
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] Array { get; set; } = new[] { "item1" };

        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] NullArray1 { get; set; } = null;
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] EmptyArray1 { get; set; } = new string[0];
        [JsonEmptyArray(EmptyArrayHandling = EmptyArrayHandling.Ignore)]
        public string[] Array1 { get; set; } = new[] { "item1" };
    }
}
