using System;
using Assets.Scripts.API;
using UnityEngine;

public class UserInterfaceLogic
{
    public event Action LoginSuccessful;
    public event Action LoginFailed;
    
    private UserInterfaceView _userInterfaceView;
    private IApiService _apiService;

    public void Init(IApiService apiService, UserInterfaceView userInterfaceView)
    {
        _apiService = apiService;
        _userInterfaceView = userInterfaceView;

        if (_userInterfaceView == null) return;

        //userInterfaceView.onPressedRegister += () => _apiService.OpenRegisterPage();
        _userInterfaceView.onPressedLogin += _userInterfaceView_onPressedLogin;
    }

    private void _userInterfaceView_onPressedLogin(string login, string password)
    {
        switch (password)
        {
            /*case "пароль1": 
                LoginSuccessful?.Invoke(login, false); //не владелец - в холл
                EnterMessage?.Invoke("¬веден пароль обычного пользовател€");
                //_clientData.TypeOwner = OwnerType.Guest;
                //_clientData.AccessToken = LocalDeveloperAccessToken;
                break;
            case "пароль2":
                LoginSuccessful?.Invoke(login, true); //владелец - в комнату
                EnterMessage?.Invoke("¬веден пароль владельца");
               //_clientData.TypeOwner = OwnerType.Owner;
               //_clientData.AccessToken = LocalDeveloperAccessToken;
                break;*/
            default:
                SendAuthRequest(login, password);
                break;
        }
    }

    private async void SendAuthRequest(string login, string password)
    {
        var form = new WWWForm();
        form.AddField("login", login);
        form.AddField("password", password);

        var success  = await _apiService.Auth(form);

        if (success)
        {
            LoginSuccessful?.Invoke();
        }
        else
        {
            LoginFailed?.Invoke();
        }
    }
}
