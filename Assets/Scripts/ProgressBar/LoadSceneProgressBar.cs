using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

public class LoadSceneProgressBar : MonoBehaviour
{
    #region Inspector
    [SerializeField] private ViewPanel _loadingPanel;
    [SerializeField] private ViewPanel _failPanel;
    [SerializeField] private GameObject _progressBarParent;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Image _sliderImage;
    private static readonly int VisibleValue = Shader.PropertyToID("_DIsolveAmount");

    #endregion
    public void ShowProgressBar() 
    {
        _sliderImage.fillAmount = 0;
        _progressBarParent.SetActive(true);
        _loadingPanel.ShowPanel();
        _failPanel.HidePanel();
    }

    public void SetAsyncOperation(AsyncOperationHandle<SceneInstance> dl)
    {
        _sliderImage.fillAmount = 0;
        _progressBarParent.SetActive(true);
        _loadingPanel.ShowPanel();
        _failPanel.HidePanel();
        StartCoroutine(LoadSceneProgress(dl));
    }

    public void OnLoadProgressChanged(float progressValue)
    {
        if (progressValue > 0.9f)
            progressValue = 1;

        _sliderImage.fillAmount = progressValue;
        /*
        //its use if need change visible material
        float value = Remap(progressValue,0,1,0.44f,0.56f);
        foreach (Material material in _meshRenderer.sharedMaterials)
        {
            material.SetFloat(VisibleValue,value);
        }
        */
    }
    private float Remap(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s-a1)*(b2-b1)/(a2-a1);
    }


    public void OnSceneLoadComplete(AsyncOperationHandle<SceneInstance> asyncOperationHandle ,string name)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Failed)
            StartCoroutine(HideFailAfterTimer());
        
        foreach (Material material in _meshRenderer.sharedMaterials)
        {
            material.SetFloat(VisibleValue,1);
        }
    }

    private IEnumerator LoadSceneProgress(AsyncOperationHandle<SceneInstance> dl)
    {
        dl.Completed += (AsyncOperationHandle) =>
        {
            OnSceneLoadComplete(AsyncOperationHandle,AsyncOperationHandle.Result.Scene.name);
        };
 
        while (dl.PercentComplete < 1 && !dl.IsDone)
        {
            OnLoadProgressChanged(dl.PercentComplete);
            yield return null;
        }
    }

    private IEnumerator HideFailAfterTimer()
    {
        _loadingPanel.HidePanel();
        _failPanel.ShowPanel();
        yield return new WaitForSeconds(2f);
        _failPanel.HidePanel();
        _progressBarParent.SetActive(false);
    }
}
