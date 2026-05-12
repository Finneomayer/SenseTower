using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Ledder : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] steps;
    [SerializeField] private float _delay;
    [SerializeField] private Transform _target;
    private Coroutine _stairsFlashing;
    private PlayerLogic _playerLogic;

    IEnumerator StairsFlashing()
    {
        while (true)
        {
            for (int i = 0; i < steps.Length; i++)
            {
                steps[i].enabled = false;
                if (i > 0) steps[i - 1].enabled = true;
                yield return new WaitForSeconds(_delay);
            }
            steps[steps.Length - 1].enabled = true;
        }
        
    }

    public void OnHoverEnter()
    {        
        //_stairsFlashing = StartCoroutine(StairsFlashing());
    }

    public void OnHoverExit()
    {
        //StopAllCoroutines();
        //foreach (var item in steps)
        //{
        //    item.enabled = true;
        //}
    }
    
    public void OnSelectLedder(SelectEnterEventArgs args)
    {
        OnHoverExit();
        _playerLogic = args.interactorObject.transform.gameObject.GetComponentInParent<PlayerLogic>();
        _playerLogic.TeleportTo(_target);
    }
}
