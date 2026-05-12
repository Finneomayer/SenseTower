using UnityEngine;
using UnityEngine.UI;

public class UIAction : MonoBehaviour
{
    #region PrivateVariables
    private Button _button;
    #endregion

    private void Start()
    {
        _button = GetComponent<Button>();
        if(_button != null)
            _button.onClick.AddListener(delegate { Invoke(); });    
    }
    private void OnDestroy()
    {
        if(_button != null)
            _button.onClick.RemoveAllListeners();
    }

    public virtual void Invoke() 
    {
    }

}
