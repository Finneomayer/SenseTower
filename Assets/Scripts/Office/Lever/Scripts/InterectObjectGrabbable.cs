using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Management;

[RequireComponent(typeof(LeverControllerInput))]
[RequireComponent(typeof(Rigidbody))]
public class InterectObjectGrabbable : MonoBehaviour
{
    private const float kGrabResetTime = 0.5f;

    [Space(15)]   
    public bool canGrab = true;   
    public bool shouldFly = true;   
    public float grabFlyDistance = .35f;  
    public float grabFlyTime = .1f;

    public bool IsFixHandGrab = false;
    public Vector3 RightRotateOffset = new Vector3(90, 0, 0);
    public Vector3 LeftRotateOffset = new Vector3(90, 0, 0);

    public bool isFixRotateTarget = false;

    [Space(15)]   
    public bool mirrorXOffsetInLeftHand = true;   
  //  [Range(0.0f, 1.0f)]
    public float inHandLerpSpeed = 0.4f;   
    public float objectDropDistance = 0.3f;


    [HideInInspector]
    public bool inHand = false;
    public bool isUpdate = false;

    private Indicator gripIndicatorComponent;
    private LeverControllerInput input;

    private struct GrabData
    {
        public float grabStartTime;
        public float grabEndTime;
        public bool recentlyReleased;
        public bool recentlyDropped;
        public Vector3 grabStartPosition;
        public Quaternion grabStartLocalRotation;
        public Quaternion grabStartWorldRotation;
        public bool wasKinematic;
        public bool didHaveGravity;
        public bool wasKnockable;
        public bool hasJoint;
    };

    private GrabData grabData;
    private Rigidbody rb;

    protected virtual void Start()
    {
        if (gameObject.GetComponent<Indicator>())
        {
            gripIndicatorComponent = gameObject.GetComponent<Indicator>();
        }

        input = gameObject.GetComponent<LeverControllerInput>();
        rb = GetComponent<Rigidbody>();
    }

#if !UNITY_SERVER
    private void FixedUpdate()
    {
        DoGrabbedUpdate();
    }
#endif

    #region Grabbe
    private void DoGrabbedUpdate()
    {
        if (!XRGeneralSettings.Instance.Manager.isInitializationComplete) return;

        if (input.activeController == LeverControllerType.Controller_None)
        {
            UngrabbedUpdate();
        }
        else
        {
            if (canGrab && !isUpdate)
            {
                GrabbedUpdate();
            }
        }
    }
  
    private void UngrabbedUpdate()
    {
        inHand = false;

        
        if (grabData.recentlyReleased && (Time.time - grabData.grabEndTime) > kGrabResetTime)
        {
            grabData.recentlyReleased = false;
            grabData.recentlyDropped = false;
        }

      
        if (grabData.recentlyDropped)
        {
            return;
        }

        float distanceToLeftHand = 1000;
        if (input.LeftControllerIsConnected) {
            distanceToLeftHand = (this.transform.position - input.LeftControllerPosition).magnitude;
        }

        float distanceToRightHand = 1000;
        if (input.RightControllerIsConnected) {
            distanceToRightHand = (transform.position - input.RightControllerPosition).magnitude;
        }

        if (grabFlyDistance > distanceToLeftHand ||
            grabFlyDistance > distanceToRightHand)
        {
            float distance = Mathf.Min(distanceToLeftHand, distanceToRightHand);
            if (gripIndicatorComponent != null)
            {
                float distanceForHighlight = grabFlyDistance / 4f;
                float highlight = Mathf.Max(0, Mathf.Min(1, (grabFlyDistance - distance) / distanceForHighlight));
                gripIndicatorComponent.IndicatorActive = highlight;
            }

          
            LeverControllerType firstController = LeverControllerType.Controller_None;
            LeverControllerType secondController = LeverControllerType.Controller_None;

            if (distanceToLeftHand < distanceToRightHand) {
                if (LeverControllerManager.nearestGrabbableToLeftController == this)
                    firstController = LeverControllerType.Controller_Left;

                if (LeverControllerManager.nearestGrabbableToRightController == this)
                    secondController = LeverControllerType.Controller_Right;
            } else {
                if (LeverControllerManager.nearestGrabbableToRightController == this)
                    firstController = LeverControllerType.Controller_Right;

                if (LeverControllerManager.nearestGrabbableToLeftController == this)
                    secondController = LeverControllerType.Controller_Left;
            }

            TrySetActiveController(firstController);
            TrySetActiveController(secondController);

          
            if (distanceToLeftHand < LeverControllerManager.distanceToLeftController ||
                LeverControllerManager.nearestGrabbableToLeftController == null ||
                LeverControllerManager.nearestGrabbableToLeftController == this) {
                LeverControllerManager.nearestGrabbableToLeftController = this;
                LeverControllerManager.distanceToLeftController = distanceToLeftHand;
            }

            if (distanceToRightHand < LeverControllerManager.distanceToRightController ||
                LeverControllerManager.nearestGrabbableToRightController == null ||
                LeverControllerManager.nearestGrabbableToRightController == this) {
                LeverControllerManager.nearestGrabbableToRightController = this;
                LeverControllerManager.distanceToRightController = distanceToRightHand;
            }

        }
        else
        {
            if (gripIndicatorComponent != null) 
            {
                gripIndicatorComponent.IndicatorActive = 0;
            }
            
            if (LeverControllerManager.nearestGrabbableToRightController == this)
            {
                LeverControllerManager.nearestGrabbableToRightController = null;
            }
            if (LeverControllerManager.nearestGrabbableToLeftController == this)
            {
                LeverControllerManager.nearestGrabbableToLeftController = null;
            }
        }
    }

    private void GrabbedUpdate() 
    {
        if (!input.GetGripButtonDown(input.activeController))
        {
            ClearActiveController();
            return;
        }

       
        Quaternion targetRotation;
        Quaternion controllerRotation = input.RotationForController(input.activeController);
      
        targetRotation = controllerRotation * grabData.grabStartLocalRotation;
        


      
        Vector3 targetOffset;
        Vector3 finalPosition = transform.position;
        Quaternion finalRotation = transform.rotation;
        if (input.activeController == LeverControllerType.Controller_Left && mirrorXOffsetInLeftHand) 
        {
            Matrix4x4 mirrorMatrix = Matrix4x4.Scale(new Vector3(-1, 1, 1));
            Matrix4x4 offsetAndRotation = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
            Matrix4x4 finalOffsetAndRotation = mirrorMatrix * offsetAndRotation;

            targetRotation = controllerRotation * Quaternion.LookRotation(finalOffsetAndRotation.GetColumn(2), finalOffsetAndRotation.GetColumn(1));
            targetOffset = targetRotation * finalOffsetAndRotation.GetColumn(3);
            targetRotation *= Quaternion.AngleAxis(180, Vector3.up);
        } 
        else
        {
            targetOffset = targetRotation * Vector3.zero;
            targetRotation *= Quaternion.AngleAxis(180, Vector3.up);
        }

        float percComplete = (Time.time - grabData.grabStartTime) / grabFlyTime;
        if (percComplete < 1 && shouldFly && !grabData.hasJoint) 
        {
            inHand = false;
            finalPosition = Vector3.Lerp(grabData.grabStartPosition, input.PositionForController(input.activeController) + targetOffset, percComplete);
            finalRotation = Quaternion.Lerp(grabData.grabStartWorldRotation, targetRotation, percComplete);
        } 
        else
        {
            inHand = true;
            Vector3 targetPosition = input.PositionForController(input.activeController);

          
            if ((transform.position - targetPosition).magnitude >= objectDropDistance) 
            {
                grabData.recentlyDropped = true;
                ClearActiveController();
                return;
            }

            if (grabData.hasJoint)
            {
                finalPosition = targetPosition + targetOffset;
                if (IsFixHandGrab)
                {
                    finalRotation = (input.activeController == LeverControllerType.Controller_Right) ? controllerRotation * Quaternion.Euler(RightRotateOffset) : controllerRotation * Quaternion.Euler(LeftRotateOffset);
                }
            }
            else
            {
                if (IsFixHandGrab)
                {
                    targetRotation =  (input.activeController == LeverControllerType.Controller_Right) ? controllerRotation * Quaternion.Euler(RightRotateOffset) : controllerRotation * Quaternion.Euler(LeftRotateOffset);

                }

                finalPosition = Vector3.Lerp(transform.position, targetPosition + targetOffset, inHandLerpSpeed);
                  finalRotation = Quaternion.Lerp(transform.rotation, targetRotation, inHandLerpSpeed);
               //   finalRotation =  controllerRotation * Quaternion.Euler(RightRotateOffset);
                rb.velocity = input.ActiveControllerVelocity();
                rb.angularVelocity = input.ActiveControllerAngularVelocity();

            }
        }

        if (isFixRotateTarget)
            finalRotation = transform.rotation;

        SetPositionAndRotation(finalPosition, finalRotation);


    }

    protected virtual void SetPositionAndRotation(Vector3 finalPosition, Quaternion finalRotation)
    {
        transform.position = finalPosition;
        transform.rotation = finalRotation;
    }
    #endregion

    #region Active Controller
    private void TrySetActiveController(LeverControllerType controller)
    {
        if (input.activeController != LeverControllerType.Controller_None ||
            controller == LeverControllerType.Controller_None)
            return;

        
            if (!input.GetGripButtonDown(controller)) 
            {
                return;
            }
        

        if (input.SetActiveController(controller))
        {
            grabData.grabStartTime = Time.time;
            grabData.grabStartPosition = gameObject.transform.position;
            grabData.grabStartWorldRotation = gameObject.transform.rotation;
            grabData.grabStartLocalRotation = Quaternion.Inverse(input.RotationForController(controller)) * grabData.grabStartWorldRotation;
            grabData.hasJoint = (gameObject.GetComponent<Joint>() != null);
            if (gripIndicatorComponent != null) {
                gripIndicatorComponent.IndicatorActive = 0;
            }

           
            grabData.wasKinematic = rb.isKinematic;
            grabData.didHaveGravity = rb.useGravity;
           
                rb.useGravity = false;
           
        }
    }

    private void ClearActiveController() 
    {
        grabData.grabEndTime = Time.time;
        grabData.recentlyReleased = true;

        rb.isKinematic = grabData.wasKinematic;
        rb.useGravity = grabData.didHaveGravity;

       

        if (!grabData.recentlyDropped) 
        {
            rb.velocity = input.ActiveControllerVelocity();
            rb.angularVelocity = input.ActiveControllerAngularVelocity();
        }

        input.ClearActiveController();
    }
    #endregion
}
