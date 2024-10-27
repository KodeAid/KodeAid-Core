using AutoMapper;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid.AutoMapper
{
    public class AutoMapperTest
    {
        private readonly ITestOutputHelper _output;

        public AutoMapperTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void OverwriteWithNonNullSourceMember()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>();
            });

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();
            var destination = mapper.Map(
                new Source()
                {
                    Property1 = "NewValue",
                },
                new Destination()
                {
                    Property1 = "OriginalValue",
                });

            Assert.Equal("NewValue", destination.Property1);
        }

        [Fact]
        public void OverwriteWithNullSourceMember()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>();
            });

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();
            var destination = mapper.Map(
                new Source()
                {
                    Property1 = null,
                },
                new Destination()
                {
                    Property1 = "OriginalValue",
                });

            Assert.Null(destination.Property1);
        }

        [Fact]
        public void OverwriteWithNonNullSourceMemberWithForMemberMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "MapFrom"));
            });

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();
            var destination = mapper.Map(
                new Source()
                {
                    Property1 = "NewValue",
                },
                new Destination()
                {
                    Property1 = "OriginalValue",
                });

            Assert.Equal("NewValueMapFrom", destination.Property1);
        }

        [Fact]
        public void OverwriteWithNullSourceMemberWithForMemberMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "MapFrom"));
            });

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();
            var destination = mapper.Map(
                new Source()
                {
                    Property1 = null,
                },
                new Destination()
                {
                    Property1 = "OriginalValue",
                });

            Assert.Equal("MapFrom", destination.Property1);
        }

        [Fact]
        public void OverridePreviousForMemberMapping()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "1"))
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "2"));
            });

            mapperConfig.AssertConfigurationIsValid();
            var mapper = mapperConfig.CreateMapper();
            var destination = mapper.Map(
                new Source()
                {
                    Property1 = "NewValue",
                },
                new Destination()
                {
                    Property1 = "OriginalValue",
                });

            Assert.Equal("NewValue2", destination.Property1);
        }

        public class Source
        {
            public string Property1 { get; set; }
        }

        public class Destination
        {
            public string Property1 { get; set; }
        }
    }
}
