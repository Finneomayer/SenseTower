using Assets.Scripts.API;
using UnityEngine;

public class RegistrationAction : UIAction
{
    public override void Invoke()
    {
        base.Invoke();
        Application.OpenURL(APIService.RegistrationPageUrl);
    }
}
