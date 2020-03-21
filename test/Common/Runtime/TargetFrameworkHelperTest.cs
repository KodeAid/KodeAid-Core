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
            _output?.WriteLine($"Name: {TargetFrameworkHelper.Name}");
            _output?.WriteLine($"Version: {TargetFrameworkHelper.Version}");
            _output?.WriteLine($"DisplayName: {TargetFrameworkHelper.DisplayName}");
            _output?.WriteLine($"IsNetCoreApp: {TargetFrameworkHelper.IsNetCoreApp}");
            _output?.WriteLine($"IsNetFramework: {TargetFrameworkHelper.IsNetFramework}");
        }
    }
}
