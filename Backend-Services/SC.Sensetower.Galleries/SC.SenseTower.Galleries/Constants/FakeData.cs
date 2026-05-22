using SC.SenseTower.Galleries.Dto.Galleries;
using SC.SenseTower.Galleries.Dto.Images;
using SC.SenseTower.Galleries.Dto.Spaces;
using SC.SenseTower.Common.Enums;

namespace SC.SenseTower.Galleries.Constants
{
    public static class FakeData
    {
        public static GalleryDto[] Galleries = new GalleryDto[]
        {
            new()
            {
                Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                Name = "Галерея 1",
                GalleryInfoTable = new InfoTableDto
                {
                    Description = "Описание для галереи N 1: все про автомобили и не только",
                    ShowInformation = true,
                    Image = new ImageInfoDto
                    {
                        Id = Guid.Parse("20b9846d-8d1e-4d4b-8402-2a26edaa3d52"),
                        Name = "BMWhybrid1",
                        FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=false",
                        PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=true"
                    }
                },
                Space = new SpaceDto
                {
                        SpaceConnectionInfo = new ConnectionInfoDto
                        {
                            Ip = "51.250.89.118",
                            Port = 7843
                        },
                        Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                        SpaceType = SpaceType.MyGallery,
                        SceneName = "NewGallery",
                },
                PicturesLocation = new Dictionary<int, GalleryImageDto>
                {
                    {
                        0,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 1",
                            Name = "BMWhybrid1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("20b9846d-8d1e-4d4b-8402-2a26edaa3d52"),
                                Name = "BMWhybrid1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=true"
                            }
                        }
                    },
                    {
                        1,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 2",
                            Name = "BMWhybrid2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("93ef8801-d154-421c-a312-50ad6139ea63"),
                                Name = "BMWhybrid2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=true"
                            }
                        }
                    },
                    {
                        2,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 3",
                            Name = "BMWhybrid3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1337a2c9-9d67-4c9e-ae0f-cc901e4de132"),
                                Name = "BMWhybrid3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=true"
                            }
                        }
                    },
                    {
                        3,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 4",
                            Name = "BMWhybrid4",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("af08c909-90a2-4dd5-8136-78cbe26d9e21"),
                                Name = "BMWhybrid4.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=true"
                            }
                        }
                    },
                    {
                        4,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 5",
                            Name = "lamborghini countach2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("4d92456c-9296-44f6-8c1a-2c7d0e27301c"),
                                Name = "lamborghini countach2.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=true"
                            }
                        }
                    },
                    {
                        5,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 6",
                            Name = "mclaren mp4-12c 1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4"),
                                Name = "mclaren mp4-12c 1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=true"
                            }
                        }
                    },
                    {
                        6,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 7",
                            Name = "mclaren mp4-12c 3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("007ea36f-c0b7-4c8d-b7f8-bd50295dd95b"),
                                Name = "mclaren mp4-12c 3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=true"
                            }
                        }
                    },
                    {
                        7,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 8",
                            Name = "морган1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("c78a5230-74bf-4ad8-9847-86ffca1b3ec5"),
                                Name = "морган1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=true"
                            }
                        }
                    },
                    {
                        8,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 9",
                            Name = "морган2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1d3fbea8-be68-4d84-ab77-02ebce96815c"),
                                Name = "морган2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=true"
                            }
                        }
                    },
                    {
                        9,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 10",
                            Name = "морган3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("122c64d1-ad7c-497f-8527-3114bc043a83"),
                                Name = "морган3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=true"
                            }
                        }
                    },
                    {
                        10,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 11",
                            Name = "BMWhybrid1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("20b9846d-8d1e-4d4b-8402-2a26edaa3d52"),
                                Name = "BMWhybrid1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=true"
                            }
                        }
                    },
                    {
                        11,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 12",
                            Name = "BMWhybrid2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("93ef8801-d154-421c-a312-50ad6139ea63"),
                                Name = "BMWhybrid2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=true"
                            }
                        }
                    },
                    {
                        12,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 13",
                            Name = "BMWhybrid3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1337a2c9-9d67-4c9e-ae0f-cc901e4de132"),
                                Name = "BMWhybrid3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=true"
                            }
                        }
                    },
                    {
                        13,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 14",
                            Name = "BMWhybrid4",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("af08c909-90a2-4dd5-8136-78cbe26d9e21"),
                                Name = "BMWhybrid4.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=true"
                            }
                        }
                    },
                    {
                        14,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 15",
                            Name = "lamborghini countach2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("4d92456c-9296-44f6-8c1a-2c7d0e27301c"),
                                Name = "lamborghini countach2.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=true"
                            }
                        }
                    },
                    {
                        15,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 16",
                            Name = "mclaren mp4-12c 1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4"),
                                Name = "mclaren mp4-12c 1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=true"
                            }
                        }
                    },
                    {
                        16,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 17",
                            Name = "mclaren mp4-12c 3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("007ea36f-c0b7-4c8d-b7f8-bd50295dd95b"),
                                Name = "mclaren mp4-12c 3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=true"
                            }
                        }
                    },
                    {
                        17,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 18",
                            Name = "морган1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("c78a5230-74bf-4ad8-9847-86ffca1b3ec5"),
                                Name = "морган1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=true"
                            }
                        }
                    },
                    {
                        18,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 19",
                            Name = "морган2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1d3fbea8-be68-4d84-ab77-02ebce96815c"),
                                Name = "морган2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=true"
                            }
                        }
                    },
                    {
                        19,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 20",
                            Name = "морган3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("122c64d1-ad7c-497f-8527-3114bc043a83"),
                                Name = "морган3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=true"
                            }
                        }
                    },
                    {
                        20,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 21",
                            Name = "BMWhybrid1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("20b9846d-8d1e-4d4b-8402-2a26edaa3d52"),
                                Name = "BMWhybrid1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=true"
                            }
                        }
                    },
                    {
                        21,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 22",
                            Name = "BMWhybrid2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("93ef8801-d154-421c-a312-50ad6139ea63"),
                                Name = "BMWhybrid2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=true"
                            }
                        }
                    },
                    {
                        22,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 23",
                            Name = "BMWhybrid3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1337a2c9-9d67-4c9e-ae0f-cc901e4de132"),
                                Name = "BMWhybrid3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=true"
                            }
                        }
                    },
                    {
                        23,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 24",
                            Name = "BMWhybrid4",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("af08c909-90a2-4dd5-8136-78cbe26d9e21"),
                                Name = "BMWhybrid4.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=true"
                            }
                        }
                    },
                    {
                        24,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 25",
                            Name = "lamborghini countach2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("4d92456c-9296-44f6-8c1a-2c7d0e27301c"),
                                Name = "lamborghini countach2.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=true"
                            }
                        }
                    },
                    {
                        25,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 26",
                            Name = "mclaren mp4-12c 1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4"),
                                Name = "mclaren mp4-12c 1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=true"
                            }
                        }
                    },
                    {
                        26,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 27",
                            Name = "mclaren mp4-12c 3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("007ea36f-c0b7-4c8d-b7f8-bd50295dd95b"),
                                Name = "mclaren mp4-12c 3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=true"
                            }
                        }
                    },
                    {
                        27,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 28",
                            Name = "морган1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("c78a5230-74bf-4ad8-9847-86ffca1b3ec5"),
                                Name = "морган1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=true"
                            }
                        }
                    },
                    {
                        28,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 29",
                            Name = "морган2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1d3fbea8-be68-4d84-ab77-02ebce96815c"),
                                Name = "морган2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=true"
                            }
                        }
                    },
                    {
                        29,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 30",
                            Name = "морган3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("122c64d1-ad7c-497f-8527-3114bc043a83"),
                                Name = "морган3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=true"
                            }
                        }
                    },
                    {
                        30,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 31",
                            Name = "BMWhybrid1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("20b9846d-8d1e-4d4b-8402-2a26edaa3d52"),
                                Name = "BMWhybrid1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/20b9846d-8d1e-4d4b-8402-2a26edaa3d52?preview=true"
                            }
                        }
                    },
                    {
                        31,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 32",
                            Name = "BMWhybrid2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("93ef8801-d154-421c-a312-50ad6139ea63"),
                                Name = "BMWhybrid2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/93ef8801-d154-421c-a312-50ad6139ea63?preview=true"
                            }
                        }
                    },
                    {
                        32,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 33",
                            Name = "BMWhybrid3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1337a2c9-9d67-4c9e-ae0f-cc901e4de132"),
                                Name = "BMWhybrid3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1337a2c9-9d67-4c9e-ae0f-cc901e4de132?preview=true"
                            }
                        }
                    },
                    {
                        33,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 34",
                            Name = "BMWhybrid4",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("af08c909-90a2-4dd5-8136-78cbe26d9e21"),
                                Name = "BMWhybrid4.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/af08c909-90a2-4dd5-8136-78cbe26d9e21?preview=true"
                            }
                        }
                    },
                    {
                        34,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 35",
                            Name = "lamborghini countach2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("4d92456c-9296-44f6-8c1a-2c7d0e27301c"),
                                Name = "lamborghini countach2.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/4d92456c-9296-44f6-8c1a-2c7d0e27301c?preview=true"
                            }
                        }
                    },
                    {
                        35,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 36",
                            Name = "mclaren mp4-12c 1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4"),
                                Name = "mclaren mp4-12c 1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/ef3890b1-448a-4b70-9ab7-a0dd7ebf43c4?preview=true"
                            }
                        }
                    },
                    {
                        36,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 37",
                            Name = "mclaren mp4-12c 3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("007ea36f-c0b7-4c8d-b7f8-bd50295dd95b"),
                                Name = "mclaren mp4-12c 3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/007ea36f-c0b7-4c8d-b7f8-bd50295dd95b?preview=true"
                            }
                        }
                    },
                    {
                        37,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 38",
                            Name = "морган1",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("c78a5230-74bf-4ad8-9847-86ffca1b3ec5"),
                                Name = "морган1.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/c78a5230-74bf-4ad8-9847-86ffca1b3ec5?preview=true"
                            }
                        }
                    },
                    {
                        38,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 39",
                            Name = "морган2",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("1d3fbea8-be68-4d84-ab77-02ebce96815c"),
                                Name = "морган2.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/1d3fbea8-be68-4d84-ab77-02ebce96815c?preview=true"
                            }
                        }
                    },
                    {
                        39,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 40",
                            Name = "морган3",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("122c64d1-ad7c-497f-8527-3114bc043a83"),
                                Name = "морган3.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/122c64d1-ad7c-497f-8527-3114bc043a83?preview=true"
                            }
                        }
                    },
                    {
                        40,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Авария Кубицы в 2007, Канада",
                            Name = "Авария",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("68aa0506-5c67-4094-9012-2218922d262d"),
                                Name = "Авария Кубицы в 2007, Канада.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/68aa0506-5c67-4094-9012-2218922d262d?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/68aa0506-5c67-4094-9012-2218922d262d?preview=true"
                            }
                        }
                    },
                    {
                        41,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 42",
                            Name = "Русский Хаммер",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("e21f4836-310a-49c8-880d-1f059d61c50a"),
                                Name = "Русский Хаммер.jpeg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/e21f4836-310a-49c8-880d-1f059d61c50a?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/e21f4836-310a-49c8-880d-1f059d61c50a?preview=true"
                            }
                        }
                    },
                    {
                        42,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 43",
                            Name = "грузовик",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("34c0d2cb-80aa-4648-aa6e-2ee473aa2c07"),
                                Name = "грузовик.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/34c0d2cb-80aa-4648-aa6e-2ee473aa2c07?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/34c0d2cb-80aa-4648-aa6e-2ee473aa2c07?preview=true"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = Guid.Parse("466C4F30-6B7D-4155-878E-08F80E9B7330"),
                Name = "Галерея 2",
                GalleryInfoTable = new InfoTableDto
                {
                    Description = "Описание для галереи N 1: не наша история в современной интерпретации",
                    ShowInformation = true,
                    Image = new ImageInfoDto
                    {
                        Id = Guid.Parse("32121418-0d52-4323-86de-6c2fc2a011b5"),
                        Name = "knights",
                        FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/32121418-0d52-4323-86de-6c2fc2a011b5?preview=false",
                        PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/32121418-0d52-4323-86de-6c2fc2a011b5?preview=true"
                    }
                },
                Space = new SpaceDto
                {
                        SpaceConnectionInfo = new ConnectionInfoDto
                        {
                            Ip = "51.250.89.118",
                            Port = 7830
                        },
                        Id = Guid.Parse("9FD08C9C-621C-4F36-BC73-6BEBEB7D6FF6"),
                        SpaceType = SpaceType.MyGallery,
                        SceneName = "NewGallery",
                },
                PicturesLocation = new Dictionary<int, GalleryImageDto>
                {
                    {
                        0,
                        new GalleryImageDto
                        {
                            Author = "Unknown",
                            Description = "Картинка номер 1",
                            Name = "iwakuni_music",
                            PassepartoutWidthInMeters = 1,
                            PictureWidthInMeters = 1,
                            Image = new ImageInfoDto
                            {
                                Id = Guid.Parse("7c2ed60a-43d8-4d47-9429-e6fd9e107ad2"),
                                Name = "iwakuni_music.jpg",
                                FileUrl = "https://dev.SenseTower.io/images/api/v1/images/download/7c2ed60a-43d8-4d47-9429-e6fd9e107ad2?preview=false",
                                PreviewUrl = "https://dev.SenseTower.io/images/api/v1/images/download/7c2ed60a-43d8-4d47-9429-e6fd9e107ad2?preview=true"
                            }
                        }
                    }
                }
            }
        };
    }
}
