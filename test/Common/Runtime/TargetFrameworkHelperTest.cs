using KodeAid.Runtime;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid.Json
{
    public class TargetFrameworkHelperTest
    {
        private readonly ITestOutputHelper _output;

        public TargetFrameworkHelperTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReadValuesCorrectly()
        {
            _output?.WriteLine($"Name: {TargetFramework.Current.Name}");
            _output?.WriteLine($"Version: {TargetFramework.Current.Version}");
            _output?.WriteLine($"DisplayName: {TargetFramework.Current.DisplayName}");
            _output?.WriteLine($"IsNetCoreApp: {TargetFramework.Current.IsNetCoreApp()}");
            _output?.WriteLine($"IsNetFramework: {TargetFramework.Current.IsNetFramework()}");
            _output?.WriteLine($"IsNetStandard: {TargetFramework.Current.IsNetStandard()}");
        }
    }
}
