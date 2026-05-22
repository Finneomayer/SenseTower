using Microsoft.Extensions.Options;
using SC.SenseTower.Common.Enums;
using SC.SenseTower.Spaces.Settings;
using SC.SenseTower.Spaces.Spaces;

namespace SC.SenseTower.Spaces.Services;

public sealed class SpaceServiceStub : ISpaceService
{
    public SpaceServiceStub(IOptions<SpaceServiceSettings> spaceServiceSettings)
    {
        if (spaceServiceSettings.Value != null)
            SpaceServiceSettings = spaceServiceSettings.Value;
        else
            throw new ArgumentException(nameof(spaceServiceSettings));
    }

    private SpaceServiceSettings SpaceServiceSettings { get; }

    private string DefaultUnityServerIp => SpaceServiceSettings.DefaultUnityServerIp;
    private int DefaultPort => SpaceServiceSettings.DefaultPort;

    private Dictionary<string, TowerSpace> Spaces => new()
    {
        {
            SpaceType.HallScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("E81E71B9-E685-40C7-9D4E-49F9A328C1DB"),
                SpaceType = SpaceType.HallScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort
                },
                SceneName = "TheHallScene",
                SpaceName = "Лобби"
            }
        },
        {
            SpaceType.LectureScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("24B84AA8-909E-4D61-B942-4C2A63BA9AF9"),
                SpaceType = SpaceType.LectureScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 1
                },
                SceneName = "TheLectureHall",
                SpaceName = "Лекторий"
            }
        },
        {
            SpaceType.CinemaScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("17585806-0BE3-4BB2-8F0E-0DFFF489843C"),
                SpaceType = SpaceType.CinemaScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 2
                },
                SceneName = "Cinema",
                SpaceName = "Кинотеатр"
            }
        },
        {
            SpaceType.StandupScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("51AAAB1A-4F93-452D-95B2-B9B55F981F4C"),
                SpaceType = SpaceType.StandupScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 3
                },
                SceneName = "Standup",
                SpaceName = "Стендап"
            }
        },
        {
            SpaceType.MeetingScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("1AB3C0CD-3EAD-4D57-8374-D9C207F18941"),
                SpaceType = SpaceType.MeetingScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 4
                },
                SceneName = "Meeting_small",
                SpaceName = "Переговорная"
            }
        },
        {
            SpaceType.ShowroomScene.ToString(), new TowerSpace
            {
                Id = Guid.Parse("A3DB6E1F-4DA8-4FEB-8CA2-691137D32AF5"),
                SpaceType = SpaceType.ShowroomScene,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 5
                },
                SceneName = "showroom",
                SpaceName = "Магазин"
            }
        },
        {
            SpaceType.ArtGallery.ToString(), new TowerSpace
            {
                Id = Guid.Parse("BA7C8CDB-4CAE-4C60-8725-E494A6F997ED"),
                SpaceType = SpaceType.ArtGallery,
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 6 //was 20
                },
                SceneName = "ArtGallery",
                SpaceName = "Галерея"
            }
        },
        {
            GetKey(SpaceType.MySpace, "423A299C-7E8A-46B3-B672-70DCC40E32EA"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("423A299C-7E8A-46B3-B672-70DCC40E32EA"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 11 //1
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #1"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B73EF"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B73EF"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 12 //2
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #2"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7303"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7303"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 13 //3
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #3"
            }
        },
        {
            GetKey(SpaceType.MySpace, "791AAF13-6692-493C-97FE-0BD4F4388BCF"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("791AAF13-6692-493C-97FE-0BD4F4388BCF"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 14 //4 vasilyk
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #4"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7305"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7305"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 15 //5
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #5"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7306"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7306"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 16 //6
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #6"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7307"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7307"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 17 //7
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #7"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7308"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7308"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 18 //8
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #8"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7309"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7309"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 19 //9
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #9"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7310"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7310"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 20 //10
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #10"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7311"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7311"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 21 //11
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #11"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7312"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7312"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 22 //12
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #12"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7313"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7313"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 23 //13
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #13"
            }
        },
        {
            GetKey(SpaceType.MySpace, "41EDF986-1EF4-4968-BFAC-844D2410DD0B"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("41EDF986-1EF4-4968-BFAC-844D2410DD0B"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 24 //14
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #14"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7315"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7315"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 25 //15
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #15"
            }
        },
        {
            GetKey(SpaceType.MySpace, "8C0F906D-E916-41FC-878F-1D0E81D7FD03"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("8C0F906D-E916-41FC-878F-1D0E81D7FD03"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 26 //16
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #16"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7317"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7317"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 27 //17
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #17"
            }
        },
        {
            GetKey(SpaceType.MySpace, "72FAB701-5C2A-42D7-A7D6-9E04FFE98EED"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("72FAB701-5C2A-42D7-A7D6-9E04FFE98EED"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 28 //18
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #18"
            }
        },
        {
            GetKey(SpaceType.MySpace, "2A4973D8-728A-415D-A12F-E09F070CFE3F"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("2A4973D8-728A-415D-A12F-E09F070CFE3F"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 29 //19
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #19"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7320"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7320"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 30 //20
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #20"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7321"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7321"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 31 //21
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #21"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7322"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7322"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 32 //22
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #22"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7323"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7323"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 33 //23
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #23"
            }
        },
        //{
        //    GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7324"), new TowerSpace
        //    {
        //        SpaceType = SpaceType.MySpace,
        //        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7324"),
        //        SpaceConnectionInfo = new SpaceConnectionInfo
        //        {
        //            Ip = DefaultUnityServerIp,
        //            Port = DefaultPort + 34 //24
        //        },
        //        SceneName = "TheOfficeScene",
        //        SpaceName = "Пространство #24"
        //    }
        //},
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7325"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7325"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 35 //25
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #25"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7326"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7326"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 36 //26
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #26"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7327"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7327"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 37 //27
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #27"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7328"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7328"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 38 //28
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #28"
            }
        },
        {
            GetKey(SpaceType.MySpace, "466C4F30-6B7D-4155-878E-08F80E9B7329"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7329"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 39 //29
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #29"
            }
        },
        {
            GetKey(SpaceType.MyGallery, "466C4F30-6B7D-4155-878E-08F80E9B7330"), new TowerSpace
            {
                SpaceType = SpaceType.MyGallery,
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7330"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 40 //30
                },
                SceneName = "NewGallery",
                SpaceName = "Пространство #30"
            }
        },
        {
            GetKey(SpaceType.MySpace, "AB544DCA-7FEB-48CD-A495-C09277D3F7C4"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("AB544DCA-7FEB-48CD-A495-C09277D3F7C4"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 41 //31
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #31"
            }
        },
        {
            GetKey(SpaceType.MySpace, "73E38519-1F50-43B5-A126-C5F160927A9B"), new TowerSpace
            {
                SpaceType = SpaceType.InfrastructureScene,
                Id = Guid.Parse("73E38519-1F50-43B5-A126-C5F160927A9B"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 42 //32
                },
                RemoteSceneName = "Level",
                SceneName = "InfrastructureScene",
                SpaceName = "Пространство #32"
            }
        },
        {
            GetKey(SpaceType.MySpace, "A2D194C3-77F6-430B-A1B2-5F18AF89240B"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("A2D194C3-77F6-430B-A1B2-5F18AF89240B"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 43 //33
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #33"
            }
        },
        {
            GetKey(SpaceType.MySpace, "4CDD5DA8-99C8-4DC1-9C40-21605B52AC24"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("4CDD5DA8-99C8-4DC1-9C40-21605B52AC24"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 45 //35
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #35"
            }
        },
        {
            GetKey(SpaceType.MySpace, "233DE0F6-E9B2-44A5-ADE6-29AEF80A38C2"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("233DE0F6-E9B2-44A5-ADE6-29AEF80A38C2"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 48 //38
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #38"
            }
        },
        {
            GetKey(SpaceType.MySpace, "754688FD-F141-481E-8A1C-E78989CCE7EB"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("754688FD-F141-481E-8A1C-E78989CCE7EB"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 49 //39
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #39"
            }
        },
        {
            GetKey(SpaceType.MySpace, "51716EDF-2023-44C9-BDBD-7841D8E5C755"), new TowerSpace
            {
                SpaceType = SpaceType.MySpace,
                Id = Guid.Parse("51716EDF-2023-44C9-BDBD-7841D8E5C755"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 52 //42
                },
                SceneName = "TheOfficeScene",
                SpaceName = "Пространство #42"
            }
        },
        {
            GetKey(SpaceType.MyGallery, "9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"), new TowerSpace
            {
                SpaceType = SpaceType.MyGallery,
                Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                SpaceConnectionInfo = new SpaceConnectionInfo
                {
                    Ip = DefaultUnityServerIp,
                    Port = DefaultPort + 53 //43
                },
                SceneName = "NewGallery",
                SpaceName = "Пространство #43"
            }
        }
    };


    public Task<TowerSpace[]> GetAllSpaces()
    {
        return Task.FromResult(Spaces.Values.ToArray());
    }

    private static string GetKey(SpaceType type, string id)
    {
        return type + (string.IsNullOrWhiteSpace(id) ? string.Empty : "_" + id.ToLowerInvariant());
    }
}