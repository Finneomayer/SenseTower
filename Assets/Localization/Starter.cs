using Assets.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Starter : MonoBehaviour
{
    [SerializeField] private TMP_Text _language;
    void Start()
    {
        
        Debug.LogWarning($"{Application.systemLanguage}");
        _language.text = Application.systemLanguage.ToString();
        
    }

    
}
