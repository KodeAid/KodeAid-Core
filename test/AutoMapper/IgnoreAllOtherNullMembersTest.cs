using AutoMapper;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid.AutoMapper
{
    public class IgnoreAllOtherNullMembersTest
    {
        private readonly ITestOutputHelper _output;

        public IgnoreAllOtherNullMembersTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void SkipNullSourceMembers()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .IgnoreAllOtherNullMembers();
            });//, NopLoggerFactory.Instance);

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

            Assert.Equal("OriginalValue", destination.Property1);
        }

        [Fact]
        public void DoNotSkipNonNullSourceMembers()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .IgnoreAllOtherNullMembers();
            });//, NopLoggerFactory.Instance);

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
        public void DoNotSkipForMemberMappingWhenSourceMemberIsNotNull()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "MapFrom"))
                    .IgnoreAllOtherNullMembers();
            });//, NopLoggerFactory.Instance);

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
        public void DoNotSkipForMemberMappingWhenSourceMemberIsNull()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => s.Property1 + "MapFrom"))
                    .IgnoreAllOtherNullMembers();
            });//, NopLoggerFactory.Instance);

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
        public void DoNotSkipForMemberMappingWhenSourceMemberResolvesToNull()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Source, Destination>()
                    .ForMember(d => d.Property1, o => o.MapFrom(s => (string)null))
                    .IgnoreAllOtherNullMembers();
            });//, NopLoggerFactory.Instance);

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

            Assert.Null(destination.Property1);
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
