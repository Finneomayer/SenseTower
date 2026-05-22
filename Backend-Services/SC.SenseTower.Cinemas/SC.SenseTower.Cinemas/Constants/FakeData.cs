using SC.SenseTower.Cinemas.Data.Models;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Cinemas.Constants
{
    public static class FakeData
    {
        public static Cinema[] Cinemas =
        {
            new()
            {
                Id = Guid.Parse("CADF3657-90AA-4451-99DC-C15E2BCA4A84"),
                Name = "Кинотеатр \"Sense Movie Source\"",
                Space = new()
                {
                    Id = Guid.Parse("17585806-0be3-4bb2-8f0e-0dfff489843c"),
                    Name = "Кинотеатр",
                    RemoteSceneName = null,
                    SceneName = "Cinema",
                    SpaceConnectionInfo = new()
                    {
                        Ip = "51.250.89.118",
                        Port = 7792
                    },
                    SpaceType = SpaceType.CinemaScene
                },
                Administrators = new UserInfo[]
                {
                    new()
                    {
                        Id = Guid.Parse("22c85b81-8489-4fc1-85fc-f26bf3f42b3c"),
                        Login = "test_app",
                        Role = "vr_admin"
                    }
                }
            },
            new()
            {
                Id = Guid.Parse("3CBE0735-72BC-493D-971B-060B4BC24E08"),
                Name = "Запасной кинотеатр",
                Space = new()
                {
                    Id = Guid.Parse("17585806-0be3-4bb2-8f0e-0dfff489843c"),
                    Name = "Кинотеатр",
                    RemoteSceneName = null,
                    SceneName = "Cinema",
                    SpaceConnectionInfo = new()
                    {
                        Ip = "51.250.89.118",
                        Port = 7792
                    },
                    SpaceType = SpaceType.CinemaScene
                },
                Administrators = new UserInfo[]
                {
                    new()
                    {
                        Id = Guid.Parse("22c85b81-8489-4fc1-85fc-f26bf3f42b3c"),
                        Login = "test_app",
                        Role = "vr_admin"
                    }
                }
            }
        };
    }
}
