namespace SC.SenseTower.Auth.Services.EmailSender.Interfaces
{
    public interface IBaseRequest
    {
        string format { get; set; }

        string api_key { get; set; }

        int error_checking { get; set; }
    }
}
