using API.Models;
using API.Models.Registration;
using Cysharp.Threading.Tasks;

namespace Assets.Scripts.API.RegistrationService
{
    public interface IRegistrationService
    {
        public UniTask<RegisterResult> Register(string login,string password, string email);
        public UniTask<RegisterResult> MakeGuestResident(string login, string password, string email);
        public UniTask<bool> RegisterAsGuest(string deviceId);
    }
}