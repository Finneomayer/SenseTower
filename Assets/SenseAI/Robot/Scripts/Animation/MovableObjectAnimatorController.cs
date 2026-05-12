using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(string))] 
public class MovableObjectAnimatorController : MonoBehaviour
{
    [SerializeReference] private string _blendKey;
    [SerializeField] Animator _animator;
    private void Start()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(UpdateAnimatorRoutine());
    }

    private IEnumerator UpdateAnimatorRoutine()
    {       
        while (true)
        {
            Vector3 currentPosition = transform.position;

            yield return null;

            float velocity = (transform.position - currentPosition).magnitude / Time.deltaTime;
            _animator.SetFloat(_blendKey, velocity);
        }
    }
}
