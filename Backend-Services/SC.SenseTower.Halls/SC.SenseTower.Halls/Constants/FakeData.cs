using SC.SenseTower.Common.Enums;
using SC.SenseTower.Halls.Data.Models;

namespace SC.SenseTower.Halls.Constants
{
    public static class FakeData
    {
        public static Hall[] Halls = {
            new()
            {
                Id = Guid.Parse("577A7307-3826-4451-AA3A-1E2CF9A1A5F5"),
                Name = "Hall 1",
                Space = null,
                PublicPlaces = new Space[]
                {
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7790
                        },
                        Id = Guid.Parse("e81e71b9-e685-40c7-9d4e-49f9a328c1db"),
                        SpaceType = SpaceType.HallScene,
                        SceneName = "TheHallScene",
                        Name = "Лобби"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7791
                        },
                        Id = Guid.Parse("24b84aa8-909e-4d61-b942-4c2a63ba9af9"),
                        SpaceType = SpaceType.LectureScene,
                        SceneName = "TheLectureHall",
                        Name = "Лекторий"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7792
                        },
                        Id = Guid.Parse("17585806-0be3-4bb2-8f0e-0dfff489843c"),
                        SpaceType = SpaceType.CinemaScene,
                        SceneName = "Cinema",
                        Name = "Кинотеатр"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7793
                        },
                        Id = Guid.Parse("51aaab1a-4f93-452d-95b2-b9b55f981f4c"),
                        SpaceType = SpaceType.StandupScene,
                        SceneName = "Standup",
                        Name = "Стендап"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7794
                        },
                        Id = Guid.Parse("1ab3c0cd-3ead-4d57-8374-d9c207f18941"),
                        SpaceType = SpaceType.MeetingScene,
                        SceneName = "Meeting_small",
                        Name = "Переговорная"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7795
                        },
                        Id = Guid.Parse("a3db6e1f-4da8-4feb-8ca2-691137d32af5"),
                        SpaceType = SpaceType.ShowroomScene,
                        SceneName = "showroom",
                        Name = "Магазин"
                    },
                    new()
                    {
                        SpaceConnectionInfo = new SpaceConnectionInfo
                        {
                            Ip = "51.250.89.118",
                            Port = 7796
                        },
                        Id = Guid.Parse("ba7c8cdb-4cae-4c60-8725-e494a6f997ed"),
                        SpaceType = SpaceType.ArtGallery,
                        SceneName = "ArtGallery",
                        Name = "Галерея"
                    }
                },
                UserPlaces = new Place[]
                {
                    new()
                    {
                        Id = Guid.Parse("423A299C-7E8A-46B3-B672-70DCC40E32EA"),
                        PlaceName = "Sense Tower Space #1",
                        Number = 1,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "Miron",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("9502c0b8-e65e-4786-baf6-546afe642da1"),
                            Name = "01.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/9502c0b8-e65e-4786-baf6-546afe642da1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/9502c0b8-e65e-4786-baf6-546afe642da1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("423A299C-7E8A-46B3-B672-70DCC40E32EA"),
                            Name = "Пространство #1",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7801
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B73EF"),
                        PlaceName = "Sense Tower Space #2",
                        Number = 2,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "Miron",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("6ab34fa5-9b29-4ab9-99c2-416107f1f236"),
                            Name = "02.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/6ab34fa5-9b29-4ab9-99c2-416107f1f236?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/6ab34fa5-9b29-4ab9-99c2-416107f1f236?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B73EF"),
                            Name = "Пространство #2",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7802
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7303"),
                        PlaceName = "Sense Tower Space #3",
                        Number = 3,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("56369458-6ff1-425a-82d9-c7475a632288"),
                            Name = "03.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/56369458-6ff1-425a-82d9-c7475a632288?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/56369458-6ff1-425a-82d9-c7475a632288?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7303"),
                            Name = "Пространство #3",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7803
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("791AAF13-6692-493C-97FE-0BD4F4388BCF"),
                        PlaceName = "Sense Tower Space #4",
                        Number = 4,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("cb936c60-fd81-4fd5-92de-03f56ca9d0a3"),
                            Name = "04.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/cb936c60-fd81-4fd5-92de-03f56ca9d0a3?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/cb936c60-fd81-4fd5-92de-03f56ca9d0a3?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("791AAF13-6692-493C-97FE-0BD4F4388BCF"),
                            Name = "Пространство #4",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7804
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7305"),
                        PlaceName = "Sense Tower Space #5",
                        Number = 5,
                        OwnerId = Guid.Parse("91b48eac-4257-4649-9fad-3d028b365419"),
                        OwnerName = "kovalevsky",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("c611f19d-32c3-4f1d-a766-f62eda136db9"),
                            Name = "05.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/c611f19d-32c3-4f1d-a766-f62eda136db9?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/c611f19d-32c3-4f1d-a766-f62eda136db9?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7305"),
                            Name = "Пространство #5",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7805
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7306"),
                        PlaceName = "Sense Tower Space #6",
                        Number = 6,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("a3d48326-782a-4169-83a5-715d43ebde8e"),
                            Name = "06.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/a3d48326-782a-4169-83a5-715d43ebde8e?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/a3d48326-782a-4169-83a5-715d43ebde8e?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7306"),
                            Name = "Пространство #6",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7806
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7307"),
                        PlaceName = "Sense Tower Space #7",
                        Number = 7,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("0ab1e50f-8b2e-4891-a44e-74ff6fd6017b"),
                            Name = "07.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/0ab1e50f-8b2e-4891-a44e-74ff6fd6017b?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/0ab1e50f-8b2e-4891-a44e-74ff6fd6017b?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7307"),
                            Name = "Пространство #7",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7807
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7308"),
                        PlaceName = "Sense Tower Space #8",
                        Number = 8,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("b6d3d6a5-6327-4332-8253-9a67dbde1b6d"),
                            Name = "08.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/b6d3d6a5-6327-4332-8253-9a67dbde1b6d?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/b6d3d6a5-6327-4332-8253-9a67dbde1b6d?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7308"),
                            Name = "Пространство #8",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7808
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7309"),
                        PlaceName = "Sense Tower Space #9",
                        Number = 9,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("0679b11a-0c7e-4e02-83ad-d0a163db8f10"),
                            Name = "09.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/0679b11a-0c7e-4e02-83ad-d0a163db8f10?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/0679b11a-0c7e-4e02-83ad-d0a163db8f10?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7309"),
                            Name = "Пространство #9",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7809
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7310"),
                        PlaceName = "Sense Tower Space #10",
                        Number = 10,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("766a136b-3357-475e-afee-6a7be9a5837a"),
                            Name = "10.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/766a136b-3357-475e-afee-6a7be9a5837a?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/766a136b-3357-475e-afee-6a7be9a5837a?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7310"),
                            Name = "Пространство #10",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7810
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7311"),
                        PlaceName = "Sense Tower Space #11",
                        Number = 11,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("5db44b41-f6cb-4aee-ab00-d473de25b4c0"),
                            Name = "11.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/5db44b41-f6cb-4aee-ab00-d473de25b4c0?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/5db44b41-f6cb-4aee-ab00-d473de25b4c0?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7311"),
                            Name = "Пространство #11",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7811
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7312"),
                        PlaceName = "Sense Tower Space #12",
                        Number = 12,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("b3bccd2a-27ca-471d-9524-14326a7f99ca"),
                            Name = "12.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/b3bccd2a-27ca-471d-9524-14326a7f99ca?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/b3bccd2a-27ca-471d-9524-14326a7f99ca?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7312"),
                            Name = "Пространство #12",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7812
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("41EDF986-1EF4-4968-BFAC-844D2410DD0B"),
                        PlaceName = "Sense Tower Space #14",
                        Number = 14,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("02c80ebc-6e2c-4a60-b79b-a74e08847ec5"),
                            Name = "14.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/02c80ebc-6e2c-4a60-b79b-a74e08847ec5?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/02c80ebc-6e2c-4a60-b79b-a74e08847ec5?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("41EDF986-1EF4-4968-BFAC-844D2410DD0B"),
                            Name = "Пространство #14",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7814
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7315"),
                        PlaceName = "Sense Tower Space #15",
                        Number = 15,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1f4557af-6089-4d73-a122-96ed9c1309ba"),
                            Name = "15.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1f4557af-6089-4d73-a122-96ed9c1309ba?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1f4557af-6089-4d73-a122-96ed9c1309ba?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7315"),
                            Name = "Пространство #15",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7815
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("8C0F906D-E916-41FC-878F-1D0E81D7FD03"),
                        PlaceName = "Sense Tower Space #16",
                        Number = 16,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("ceeab599-87ad-4511-aa4c-32365e8776f1"),
                            Name = "16.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/ceeab599-87ad-4511-aa4c-32365e8776f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/ceeab599-87ad-4511-aa4c-32365e8776f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("8C0F906D-E916-41FC-878F-1D0E81D7FD03"),
                            Name = "Пространство #16",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7816
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7317"),
                        PlaceName = "Sense Tower Space #17",
                        Number = 17,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("755a9cd3-8dc7-498f-ab91-85eefd655d6b"),
                            Name = "17.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/755a9cd3-8dc7-498f-ab91-85eefd655d6b?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/755a9cd3-8dc7-498f-ab91-85eefd655d6b?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7317"),
                            Name = "Пространство #17",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7817
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("72FAB701-5C2A-42D7-A7D6-9E04FFE98EED"),
                        PlaceName = "Sense Tower Space #18",
                        Number = 18,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("5bc293a5-a93e-46a8-8600-496a3012dc3e"),
                            Name = "18.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/5bc293a5-a93e-46a8-8600-496a3012dc3e?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/5bc293a5-a93e-46a8-8600-496a3012dc3e?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("72FAB701-5C2A-42D7-A7D6-9E04FFE98EED"),
                            Name = "Пространство #18",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7818
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("2A4973D8-728A-415D-A12F-E09F070CFE3F"),
                        PlaceName = "Sense Tower Space #19",
                        Number = 19,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("826be669-0d63-4a3e-ac01-3326ebceb5c4"),
                            Name = "19.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/826be669-0d63-4a3e-ac01-3326ebceb5c4?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/826be669-0d63-4a3e-ac01-3326ebceb5c4?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("2A4973D8-728A-415D-A12F-E09F070CFE3F"),
                            Name = "Пространство #19",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7819
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7320"),
                        PlaceName = "Sense Tower Space #20",
                        Number = 20,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("2779ff67-5b0b-4688-8a0e-229f9e5d0a90"),
                            Name = "20.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/2779ff67-5b0b-4688-8a0e-229f9e5d0a90?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/2779ff67-5b0b-4688-8a0e-229f9e5d0a90?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7320"),
                            Name = "Пространство #20",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7820
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7321"),
                        PlaceName = "Sense Tower Space #21",
                        Number = 21,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("5276eb99-5486-4180-ae62-c3e79bcb4f74"),
                            Name = "21.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/5276eb99-5486-4180-ae62-c3e79bcb4f74?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/5276eb99-5486-4180-ae62-c3e79bcb4f74?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7321"),
                            Name = "Пространство #21",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7821
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7322"),
                        PlaceName = "Sense Tower Space #22",
                        Number = 22,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("267bcac1-7f71-4490-a6d5-2d97e13457e0"),
                            Name = "22.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/267bcac1-7f71-4490-a6d5-2d97e13457e0?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/267bcac1-7f71-4490-a6d5-2d97e13457e0?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7322"),
                            Name = "Пространство #22",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7822
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7323"),
                        PlaceName = "Sense Tower Space #23",
                        Number = 23,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("d0bf5af7-e6bb-465e-8740-44ffe0cd4691"),
                            Name = "23.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/d0bf5af7-e6bb-465e-8740-44ffe0cd4691?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/d0bf5af7-e6bb-465e-8740-44ffe0cd4691?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7323"),
                            Name = "Пространство #23",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7823
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #24",
                        Number = 24,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "31.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #24",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7824
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7325"),
                        PlaceName = "Sense Tower Space #25",
                        Number = 25,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("0976978a-dac0-4b31-8fe8-4914d08c0697"),
                            Name = "25.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/0976978a-dac0-4b31-8fe8-4914d08c0697?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/0976978a-dac0-4b31-8fe8-4914d08c0697?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7325"),
                            Name = "Пространство #25",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7825
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7326"),
                        PlaceName = "Sense Tower Space #26",
                        Number = 26,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("53fbfb10-6eca-4bf7-93e7-58952eafa0f4"),
                            Name = "26.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/53fbfb10-6eca-4bf7-93e7-58952eafa0f4?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/53fbfb10-6eca-4bf7-93e7-58952eafa0f4?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7326"),
                            Name = "Пространство #26",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7826
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7327"),
                        PlaceName = "Sense Tower Space #27",
                        Number = 27,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("3a90fe5a-15c3-4435-90fa-40a758cc255a"),
                            Name = "27.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/3a90fe5a-15c3-4435-90fa-40a758cc255a?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/3a90fe5a-15c3-4435-90fa-40a758cc255a?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7327"),
                            Name = "Пространство #27",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7827
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7328"),
                        PlaceName = "Sense Tower Space #28",
                        Number = 28,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("0781c1f8-658f-46d9-9d35-a35d44cc9a72"),
                            Name = "28.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/0781c1f8-658f-46d9-9d35-a35d44cc9a72?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/0781c1f8-658f-46d9-9d35-a35d44cc9a72?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7328"),
                            Name = "Пространство #28",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7828
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7329"),
                        PlaceName = "Sense Tower Space #29",
                        Number = 29,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1ac30207-6244-40fb-9de2-d84b915c7132"),
                            Name = "29.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1ac30207-6244-40fb-9de2-d84b915c7132?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1ac30207-6244-40fb-9de2-d84b915c7132?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7329"),
                            Name = "Пространство #29",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7829
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7330"),
                        PlaceName = "Sense Tower Space #30",
                        Number = 30,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("9b5b902a-dfae-4ed9-98f1-85949363c583"),
                            Name = "30.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/9b5b902a-dfae-4ed9-98f1-85949363c583?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/9b5b902a-dfae-4ed9-98f1-85949363c583?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7330"),
                            Name = "Пространство #30",
                            RemoteSceneName = null,
                            SceneName = "NewGallery",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7830
                            },
                            SpaceType = SpaceType.MyGallery
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("AB544DCA-7FEB-48CD-A495-C09277D3F7C4"),
                        PlaceName = "Sense Tower Space #31",
                        Number = 31,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("851eaff5-65fd-45fb-9384-62bfbb2d8b28"),
                            Name = "31.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/851eaff5-65fd-45fb-9384-62bfbb2d8b28?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/851eaff5-65fd-45fb-9384-62bfbb2d8b28?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("AB544DCA-7FEB-48CD-A495-C09277D3F7C4"),
                            Name = "Пространство #31",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7831
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("73E38519-1F50-43B5-A126-C5F160927A9B"),
                        PlaceName = "Sense Tower Space #32",
                        Number = 32,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("18920ca0-4eb2-4a49-8021-0b8d03158143"),
                            Name = "32.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/18920ca0-4eb2-4a49-8021-0b8d03158143?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/18920ca0-4eb2-4a49-8021-0b8d03158143?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("73E38519-1F50-43B5-A126-C5F160927A9B"),
                            Name = "Пространство #32",
                            RemoteSceneName = "Level",
                            SceneName = "InfrastructureScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7832
                            },
                            SpaceType = SpaceType.InfrastructureScene
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("A2D194C3-77F6-430B-A1B2-5F18AF89240B"),
                        PlaceName = "Sense Tower Space #33",
                        Number = 33,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("f7af096c-970c-4d84-9275-f59c57fb3507"),
                            Name = "33.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/f7af096c-970c-4d84-9275-f59c57fb3507?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/f7af096c-970c-4d84-9275-f59c57fb3507?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("A2D194C3-77F6-430B-A1B2-5F18AF89240B"),
                            Name = "Пространство #33",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7833
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #34",
                        Number = 34,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "34.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #34",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7834
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("4CDD5DA8-99C8-4DC1-9C40-21605B52AC24"),
                        PlaceName = "Sense Tower Space #35",
                        Number = 35,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("cfd7116e-7ec5-4642-8361-5328d00e75c1"),
                            Name = "35.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/cfd7116e-7ec5-4642-8361-5328d00e75c1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/cfd7116e-7ec5-4642-8361-5328d00e75c1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("4CDD5DA8-99C8-4DC1-9C40-21605B52AC24"),
                            Name = "Пространство #35",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7835
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #36",
                        Number = 36,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "36.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #36",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7836
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #37",
                        Number = 37,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "37.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #37",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7837
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("233DE0F6-E9B2-44A5-ADE6-29AEF80A38C2"),
                        PlaceName = "Sense Tower Space #38",
                        Number = 38,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("46f61427-9c3d-4f1b-8b4e-a8f3548adcf0"),
                            Name = "38.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/46f61427-9c3d-4f1b-8b4e-a8f3548adcf0?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/46f61427-9c3d-4f1b-8b4e-a8f3548adcf0?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("233DE0F6-E9B2-44A5-ADE6-29AEF80A38C2"),
                            Name = "Пространство #38",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7838
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("754688FD-F141-481E-8A1C-E78989CCE7EB"),
                        PlaceName = "Sense Tower Space #39",
                        Number = 39,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("dabf1cc7-fbe5-4fea-b86a-4a22fd511ccb"),
                            Name = "39.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/dabf1cc7-fbe5-4fea-b86a-4a22fd511ccb?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/dabf1cc7-fbe5-4fea-b86a-4a22fd511ccb?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("754688FD-F141-481E-8A1C-E78989CCE7EB"),
                            Name = "Пространство #39",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7839
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #40",
                        Number = 40,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "40.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #40",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7840
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Empty,
                        PlaceName = "Sense Tower Space #41",
                        Number = 41,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("1d3acdf3-60e5-4628-96ba-61db464728f1"),
                            Name = "41.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/1d3acdf3-60e5-4628-96ba-61db464728f1?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Empty,
                            Name = "Пространство #41",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7841
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("51716EDF-2023-44C9-BDBD-7841D8E5C755"),
                        PlaceName = "Sense Tower Space #42",
                        Number = 42,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("7ed4f9c5-3a36-4272-aef0-a5ef06b17b60"),
                            Name = "42.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/7ed4f9c5-3a36-4272-aef0-a5ef06b17b60?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/7ed4f9c5-3a36-4272-aef0-a5ef06b17b60?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("51716EDF-2023-44C9-BDBD-7841D8E5C755"),
                            Name = "Пространство #42",
                            RemoteSceneName = null,
                            SceneName = "TheOfficeScene",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7842
                            },
                            SpaceType = SpaceType.MySpace
                        },
                        Images = Array.Empty<Picture>()
                    },
                    new()
                    {
                        Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                        PlaceName = "Sense Tower Space #43",
                        Number = 43,
                        OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                        OwnerName = "",
                        DoorImage = new ImageInfo
                        {
                            Id = Guid.Parse("6c1becdb-5895-41ce-8c58-ebd4987aeed8"),
                            Name = "43.png",
                            FileUrl = "https://dev.sensetower.io/images/api/v1/images/download/6c1becdb-5895-41ce-8c58-ebd4987aeed8?preview=false",
                            PreviewUrl = "https://dev.sensetower.io/images/api/v1/images/download/6c1becdb-5895-41ce-8c58-ebd4987aeed8?preview=true"
                        },
                        PublicAccessType = 0,
                        LocalSpace = new Space
                        {
                            Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                            Name = "Пространство #43",
                            RemoteSceneName = null,
                            SceneName = "NewGallery",
                            SpaceConnectionInfo = new SpaceConnectionInfo
                            {
                                Ip = "51.250.89.118",
                                Port = 7843
                            },
                            SpaceType = SpaceType.MyGallery
                        },
                        Images = Array.Empty<Picture>()
                    }                
                }
            }
        };
    }
}
