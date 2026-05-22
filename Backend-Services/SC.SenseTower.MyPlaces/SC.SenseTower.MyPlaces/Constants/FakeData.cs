using SC.SenseTower.Common.Enums;
using SC.SenseTower.MyPlaces.Data.Models;

namespace SC.SenseTower.MyPlaces.Constants
{
    public static class FakeData
    {
        public static Place[] Places =
        {
            new()
            {
                Id = Guid.Parse("423A299C-7E8A-46B3-B672-70DCC40E32EA"),
                PlaceName = "Sense Tower Space #1",
                PlaceNumber = 1,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("9502c0b8-e65e-4786-baf6-546afe642da1"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 2,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("6ab34fa5-9b29-4ab9-99c2-416107f1f236"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 3,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("56369458-6ff1-425a-82d9-c7475a632288"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 4,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("cb936c60-fd81-4fd5-92de-03f56ca9d0a3"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 5,
                OwnerId = Guid.Parse("91b48eac-4257-4649-9fad-3d028b365419"),
                DoorImageId = Guid.Parse("c611f19d-32c3-4f1d-a766-f62eda136db9"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 6,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("a3d48326-782a-4169-83a5-715d43ebde8e"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 7,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("0ab1e50f-8b2e-4891-a44e-74ff6fd6017b"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 8,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("b6d3d6a5-6327-4332-8253-9a67dbde1b6d"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 9,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("0679b11a-0c7e-4e02-83ad-d0a163db8f10"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 10,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("766a136b-3357-475e-afee-6a7be9a5837a"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 11,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("5db44b41-f6cb-4aee-ab00-d473de25b4c0"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 12,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("b3bccd2a-27ca-471d-9524-14326a7f99ca"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 14,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("02c80ebc-6e2c-4a60-b79b-a74e08847ec5"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 15,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("1f4557af-6089-4d73-a122-96ed9c1309ba"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 16,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("ceeab599-87ad-4511-aa4c-32365e8776f1"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 17,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("755a9cd3-8dc7-498f-ab91-85eefd655d6b"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 18,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("5bc293a5-a93e-46a8-8600-496a3012dc3e"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 19,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("826be669-0d63-4a3e-ac01-3326ebceb5c4"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 20,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("2779ff67-5b0b-4688-8a0e-229f9e5d0a90"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 21,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("5276eb99-5486-4180-ae62-c3e79bcb4f74"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 22,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("267bcac1-7f71-4490-a6d5-2d97e13457e0"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 23,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("d0bf5af7-e6bb-465e-8740-44ffe0cd4691"),
                PublicAccessType = 0,
                Space = new Space
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
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7325"),
                PlaceName = "Sense Tower Space #25",
                PlaceNumber = 25,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("0976978a-dac0-4b31-8fe8-4914d08c0697"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 26,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("53fbfb10-6eca-4bf7-93e7-58952eafa0f4"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 27,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("3a90fe5a-15c3-4435-90fa-40a758cc255a"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 28,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("0781c1f8-658f-46d9-9d35-a35d44cc9a72"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 29,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("1ac30207-6244-40fb-9de2-d84b915c7132"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 30,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("9b5b902a-dfae-4ed9-98f1-85949363c583"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 31,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("851eaff5-65fd-45fb-9384-62bfbb2d8b28"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 32,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("18920ca0-4eb2-4a49-8021-0b8d03158143"),
                PublicAccessType = 0,
                Space = new Space
                {
                    Id = Guid.Parse("73E38519-1F50-43B5-A126-C5F160927A9B"),
                    Name = "Пространство #32",
                    RemoteSceneName = null,
                    SceneName = "TheOfficeScene",
                    SpaceConnectionInfo = new SpaceConnectionInfo
                    {
                        Ip = "51.250.89.118",
                        Port = 7832
                    },
                    SpaceType = SpaceType.MySpace
                },
                Images = Array.Empty<Picture>()
            },
            new()
            {
                Id = Guid.Parse("A2D194C3-77F6-430B-A1B2-5F18AF89240B"),
                PlaceName = "Sense Tower Space #33",
                PlaceNumber = 33,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("f7af096c-970c-4d84-9275-f59c57fb3507"),
                PublicAccessType = 0,
                Space = new Space
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
                Id = Guid.Parse("4CDD5DA8-99C8-4DC1-9C40-21605B52AC24"),
                PlaceName = "Sense Tower Space #35",
                PlaceNumber = 35,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("cfd7116e-7ec5-4642-8361-5328d00e75c1"),
                PublicAccessType = 0,
                Space = new Space
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
                Id = Guid.Parse("233DE0F6-E9B2-44A5-ADE6-29AEF80A38C2"),
                PlaceName = "Sense Tower Space #38",
                PlaceNumber = 38,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("46f61427-9c3d-4f1b-8b4e-a8f3548adcf0"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 39,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("dabf1cc7-fbe5-4fea-b86a-4a22fd511ccb"),
                PublicAccessType = 0,
                Space = new Space
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
                Id = Guid.Parse("51716EDF-2023-44C9-BDBD-7841D8E5C755"),
                PlaceName = "Sense Tower Space #42",
                PlaceNumber = 42,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("7ed4f9c5-3a36-4272-aef0-a5ef06b17b60"),
                PublicAccessType = 0,
                Space = new Space
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
                PlaceNumber = 43,
                OwnerId = Guid.Parse("0D7EEB34-32C6-48A3-8C6E-976B12290912"),
                DoorImageId = Guid.Parse("6c1becdb-5895-41ce-8c58-ebd4987aeed8"),
                PublicAccessType = 0,
                Space = new Space
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
        };
    }
}
