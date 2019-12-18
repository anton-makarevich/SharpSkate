using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using Sanet.SmartSkating.Dto.Models;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Tracking;
using Xunit;

namespace Sanet.SmartSkating.Tests.Services.Tracking
{
    public class TrackServiceTests
    {
        public const string TracksData = @"[
            {
            ""Name"": ""Eindhoven"",
            ""Start"": 
            {
                ""Latitude"":51.4157028,
                ""Longitude"":5.4724154
            },
            ""Finish"": 
            {
                ""Latitude"":51.4148027,
                ""Longitude"":5.4724154
            }
            },
            {
            ""Name"": ""Grefrath"",
            ""Start"": 
            {
                ""Latitude"":51.347566,
                ""Longitude"":6.340406
            },
            ""Finish"": 
            {
                ""Latitude"":51.348305,
                ""Longitude"":6.339573
            }
            }
            ]";

        private readonly TrackService _sut;

        public TrackServiceTests()
        {
            var tracks = JsonConvert.DeserializeObject<List<TrackDto>>(TracksData);
            var tracksProviderMock = Substitute.For<ITrackProvider>();
            tracksProviderMock.GetAllTracksAsync().Returns(Task.FromResult(tracks));
            _sut = new TrackService(tracksProviderMock);
        }

        [Fact]
        public async Task LoadsTracksFromTheProvider()
        {
            await _sut.LoadTracksAsync();
            
            Assert.NotEmpty(_sut.Tracks);
        }

        [Fact]
        public async Task ReturnsRinkByNameIfFound()
        {
            var name = "Eindhoven";
            await _sut.LoadTracksAsync();

            _sut.SelectRinkByName(name);

            Assert.NotNull(_sut.SelectedRink);
        }
        
        [Fact]
        public async Task ReturnsNullIfCannotFindRinkByName()
        {
            var name = "AntekGura";
            await _sut.LoadTracksAsync();

            _sut.SelectRinkByName(name);

            Assert.Null(_sut.SelectedRink);
        }
    }
}