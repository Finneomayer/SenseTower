using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IMyImageService
{
    public UniTask<MyImage[]> GetAllImages();

    //public UniTask<bool> Auth(WWWForm data);

    //public bool CheckUserAuthentication();

    //public void OpenRegisterSite();
}
