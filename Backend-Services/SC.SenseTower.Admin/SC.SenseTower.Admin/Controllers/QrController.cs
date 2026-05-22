using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Microsoft.Extensions.Options;
using QRCoder;
using SC.SenseTower.Admin.Settings;

namespace SC.SenseTower.Admin.Controllers;

public class QrController : Controller
{
    private readonly ServiceEndpointsSettings settings;

    // GET
    public QrController(IOptions<ServiceEndpointsSettings> options)
    {
        settings = options.Value ?? throw new ArgumentNullException(nameof(options));
    }

    [HttpGet("invites/{id}/qr")]
    public IActionResult Qr(string id)
    {
        if (string.IsNullOrWhiteSpace(id) || !Guid.TryParse(id, out _))
        {
            return StatusCode(400, "wrong id");
        }

        var qr = GenerateQR(id);
        return File(qr, "image/png", $"sense-tower-space-{id}.png");
    }

    private byte[] GenerateQR(string id)
    {
        using QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(settings.AccountsRegisterUrl + id, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        byte[] qr = qrCode.GetGraphic(20, new byte[] { Color.Black.R, Color.Black.G, Color.Black.B },
            new[] { Color.White.R, Color.White.G, Color.White.B }, false);
        return qr;
    }
}