using SC.SenseTower.Common.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace SC.SenseTower.Images.Services
{
    public class ImageProcessingService
    {
        private readonly ILogger<ImageProcessingService> logger;

        public ImageProcessingService(ILogger<ImageProcessingService> logger)
        {
            this.logger = logger;
        }

        public async Task<Stream> Resize(Stream imageStream, int width, int height, string resampler, CancellationToken cancellationToken)
        {
            logger.LogTrace($"Изменение размера изображения до {width}x{height}");
            using var image = Image.Load(imageStream, out IImageFormat format);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Sampler = GetResampler(resampler),
                Size = new Size(width, height)
            }));
            var outStream = new MemoryStream();
            await image.SaveAsync(outStream, format, cancellationToken);
            outStream.Position = 0;
            imageStream.Position = 0;
            logger.LogTrace($"Изменение размера изображения завершено");
            return outStream;
        }

        public async Task<Point> GetImageSize(MemoryStream st, CancellationToken cancellationToken)
        {
            var image = await Image.LoadAsync(st, cancellationToken);
            st.Position = 0;
            var result = new Point(image.Width, image.Height);
            return result;
        }

        private IResampler GetResampler(string name)
        {
            return name switch
            {
                "Bicubic" => KnownResamplers.Bicubic,
                "Box" => KnownResamplers.Box,
                "CatmullRom" => KnownResamplers.CatmullRom,
                "Hermite" => KnownResamplers.Hermite,
                "Lanczos2" => KnownResamplers.Lanczos2,
                "Lanczos3" => KnownResamplers.Lanczos3,
                "Lanczos5" => KnownResamplers.Lanczos5,
                "Lanczos8" => KnownResamplers.Lanczos8,
                "MitchellNetravali" => KnownResamplers.MitchellNetravali,
                "NearestNeighbor" => KnownResamplers.NearestNeighbor,
                "Robidoux" => KnownResamplers.Robidoux,
                "RobidouxSharp" => KnownResamplers.RobidouxSharp,
                "Spline" => KnownResamplers.Spline,
                "Triangle" => KnownResamplers.Triangle,
                "Welch" => KnownResamplers.Welch,
                _ => throw new ScException("Неизвестный ресэмплер")
            };
        }
    }
}
