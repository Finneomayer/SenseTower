namespace Assets.Mechanics.InAppPurchase.Models
{
    public class MetaPurchaseDTO
    {
        public string assetFileName { get; set; }
        public string assetFileID { get; set; }
        public IapStatus IapStatus { get; set; }
        public DownloadStatus downloadStatus { get; set; }
    }

    public enum DownloadStatus
    {
        installed = 0,
        available = 1,
        inprogress = 2,
    }

    public enum IapStatus
    {
        free = 0,
        entitled = 1,
        notentitled = 2
    }
}