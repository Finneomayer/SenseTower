using Assets.Scripts.API;
using UnityEngine;

public class ResetPasswordAction : UIAction
{
    #region Inspector
    [SerializeField] private string URL;
    #endregion
    

    public override void Invoke()
    {
        base.Invoke();
        Application.OpenURL(APIService.ResetPasswordPageUrl);
    }
}
