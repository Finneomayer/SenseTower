using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CustomContinuousMove : MonoBehaviour
{
    public bool isEnabled;
    [SerializeField, Range(0.1f, 2f)] private float speed;
    [SerializeField] private float additionalHeight = 0.1f;
    [SerializeField] private InputActionReference inputSource;
    [SerializeField] private CharacterController character;
    [SerializeField] private XROrigin rig;
    [SerializeField] private Canvas vignette;
    public bool isVignetteEnabled;
    private Vector2 inputAxis;

    private void Update()
    {
        if (isEnabled) inputAxis = inputSource.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        if (inputAxis.magnitude > 0)
        {
            Quaternion headYaw = Quaternion.Euler(0, rig.Camera.transform.eulerAngles.y, 0);
            Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
            character.Move(direction * Time.fixedDeltaTime * speed);
            if (isVignetteEnabled) vignette.enabled = true;
        }
        else vignette.enabled = false;
    }

    private void CapsuleFollowHeadset()
    {
        character.height = rig.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);

        character.center = new Vector3(capsuleCenter.x, character.height / 2 + character.skinWidth, capsuleCenter.z);
    }
}
