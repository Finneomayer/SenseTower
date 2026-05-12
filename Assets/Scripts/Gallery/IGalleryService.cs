using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Assets.Scripts.Gallery
{
    public interface IGalleryService
    {
        UniTask<Gallery> GetById (string id);
    }
}