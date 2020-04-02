using System.Runtime.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid
{
    public class EnumHelperTest
    {
        private readonly ITestOutputHelper _output;

        public EnumHelperTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReadSerializedNames()
        {
            _output?.WriteLine($"Name (Generic): {EnumHelper.GetSerializationName(TestEnum.One)}, {EnumHelper.GetSerializationName(TestEnum.Two)}, {EnumHelper.GetSerializationName(TestEnum.Three)}.");
            _output?.WriteLine($"Name (Reflection): {EnumHelper.GetSerializationName(typeof(TestEnum), TestEnum.One)}, {EnumHelper.GetSerializationName(typeof(TestEnum), TestEnum.Two)}, {EnumHelper.GetSerializationName(typeof(TestEnum), TestEnum.Three)}.");
        }

        private enum TestEnum
        {
            [EnumMember(Value = "ITEM_1")]
            One,
            [EnumMember(Value = "2")]
            Two,
            Three,
        }
    }
}
