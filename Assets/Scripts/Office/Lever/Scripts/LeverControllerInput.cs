using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public enum LeverControllerType
{
	Controller_None,
	Controller_Left,
	Controller_Right
};

public enum LeverInputButton 
{
	Button_None = -1,
	Button_A  = 0,
	Button_B,
	Button_System,
	Button_Menu,
	Button_Thumbstick_Press,
	Button_Trigger,
	Button_Grip,
	Button_Thumbstick_Left,
	Button_Thumbstick_Right,
	Button_Thumbstick_Down,
	Button_Thumbstick_Up
};

public class LeverControllerInput : MonoBehaviour 
{
    public LeverInputButton gripButton = LeverInputButton.Button_Trigger;


	[HideInInspector]
	public LeverControllerType activeController;


	
	private Dictionary<int, bool>buttonStateLeft;
	private Dictionary<int, bool>buttonStateRight;
	private OVRCameraRig rig;

	private void Start() 
	{		
		buttonStateLeft = new Dictionary<int, bool>();
		buttonStateRight = new Dictionary<int, bool>();
		rig = FindObjectOfType<OVRCameraRig>();
	}

	public bool LeftControllerIsConnected 
	{
		get 
		{
#if !UNITY_SERVER
			if (rig == null) rig = FindObjectOfType<OVRCameraRig>();
#endif
			return ((OVRInput.GetConnectedControllers() & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch) ||
					((OVRInput.GetConnectedControllers() & OVRInput.Controller.LHand) == OVRInput.Controller.LHand);
		}
	}

	public bool RightControllerIsConnected 
	{
		get
		{
#if !UNITY_SERVER
			if (rig == null) rig = FindObjectOfType<OVRCameraRig>();
#endif
			return ((OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch) ||
					((OVRInput.GetConnectedControllers() & OVRInput.Controller.RHand) == OVRInput.Controller.RHand);
		}
	}

	public Vector3 LeftControllerPosition 
	{
		get
		{
            Vector3 leftHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
#if !UNITY_SERVER
			if (rig == null) rig = FindObjectOfType<OVRCameraRig>();
#endif
			if (rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformPoint(leftHandPosition);
				}
			}
            return leftHandPosition;
		}
	}

	public Vector3 RightControllerPosition
	{
		get 
		{
#if !UNITY_SERVER
			if (rig == null) rig = FindObjectOfType<OVRCameraRig>();
#endif
			Vector3 rightHandPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
			if(rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformPoint(rightHandPosition);
				}
			}
            return rightHandPosition;
		}
	}

	public Quaternion LeftControllerRotation 
	{
		get 
		{
			if (LeftControllerIsConnected)
			{

				Quaternion leftHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
				if(rig != null)
				{
					Transform trackingSpace = rig.leftControllerAnchor;
					if (trackingSpace != null)
					{
						return trackingSpace.rotation;// * leftHandRotation;
					}
				}
                return leftHandRotation;
            }

			return Quaternion.identity;
		}
	}

	public Quaternion RightControllerRotation
	{
		get
		{
			if (RightControllerIsConnected)
			{
				Quaternion rightHandRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

                if (rig != null)
                {
                    Transform trackingSpace = rig.rightControllerAnchor;
                    if (trackingSpace != null)
                    {
						return trackingSpace.rotation;// * rightHandRotation;
                    }
                }
                return rightHandRotation;
			}

			return Quaternion.identity;
		}
	}

    public Vector3 LeftControllerVelocity 
	{
        get 
		{
            Vector3 leftHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
			if(rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformDirection(leftHandVelocity);
				}
			}
            return leftHandVelocity;
        }
    }

    public Vector3 RightControllerVelocity 
	{
        get
		{
            Vector3 rightHandVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
			if(rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformDirection(rightHandVelocity);
				}
			}
            return rightHandVelocity;
        }
    }

    public Vector3 LeftControllerAngularVelocity 
	{
        get 
		{
            Vector3 leftHandAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
			if(rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformDirection(leftHandAngularVelocity);
				}
			}
            return leftHandAngularVelocity;
        }
    }

    public Vector3 RightControllerAngularVelocity 
	{
        get
		{

            Vector3 rightHandAngularVelocity = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
			if(rig != null)
			{
				Transform trackingSpace = rig.trackingSpace;
				if (trackingSpace != null)
				{
					return trackingSpace.TransformDirection(rightHandAngularVelocity);
				}
			}
            return rightHandAngularVelocity;
        }
    }


    public Vector3 PositionForController(LeverControllerType controller) 
	{
		if (controller == LeverControllerType.Controller_Left) 
		{
			return LeftControllerPosition;
		} 
		else if (controller == LeverControllerType.Controller_Right)
		{
			return RightControllerPosition;
		}
		return Vector3.zero;
	}

	public Quaternion RotationForController(LeverControllerType controller) 
	{
		if (controller == LeverControllerType.Controller_Left)
		{
			return LeftControllerRotation;
		} 
		else if (controller == LeverControllerType.Controller_Right)
		{
			return RightControllerRotation;
		}

		return Quaternion.identity;
	}

	public bool ControllerIsConnected(LeverControllerType controller)
	{
		if (controller == LeverControllerType.Controller_Left) 
		{
			return LeftControllerIsConnected;
		}
		else if (controller == LeverControllerType.Controller_Right) 
		{
			return RightControllerIsConnected;
		}

		return false;
	}



	public bool GetGripButtonDown(LeverControllerType controller) 
	{
		return this.GetButtonDown (controller, this.gripButton);
	}

	public bool GetGripButtonPressed(LeverControllerType controller) 
	{
		return this.GetButtonPressDown (controller, this.gripButton);
	}


	public bool GetButtonDown(LeverControllerType controller, LeverInputButton button)
	{
		if (button == LeverInputButton.Button_None || !ControllerIsConnected(controller))
			return false;		

		return GetOVRButtonDown(controller, button);

	}

	public bool GetButtonPressDown(LeverControllerType controller, LeverInputButton button) 
	{
		if (button == LeverInputButton.Button_None || !ControllerIsConnected(controller))
			return false;		

		return GetOVRButtonPressDown(controller, button);
	}

	public bool SetActiveController(LeverControllerType activeController)
	{
		
		if (activeController == LeverControllerType.Controller_Left &&
		    LeverControllerManager.leftControllerActive) 
		{
			return false;
		}

		if (activeController == LeverControllerType.Controller_Right &&
			LeverControllerManager.rightControllerActive) 
		{
			return false;
		}

		this.activeController = activeController;


		
		if (this.activeController == LeverControllerType.Controller_Right)
		{
			LeverControllerManager.rightControllerActive = true;
		} 
		else
		{
			LeverControllerManager.leftControllerActive = true;
		}
			
		return true;
	}

	public void ClearActiveController()
	{
		if (this.activeController == LeverControllerType.Controller_Right)
		{
			LeverControllerManager.rightControllerActive = false;
		} 
		else 
		{
			LeverControllerManager.leftControllerActive = false;
		}

		this.activeController = LeverControllerType.Controller_None;
	}



	public Vector3 ActiveControllerVelocity() 
	{
        if (activeController == LeverControllerType.Controller_Left)
		{
            return LeftControllerVelocity;
        } 
		else if (activeController == LeverControllerType.Controller_Right) 
		{
            return RightControllerVelocity;
        } 
		else
		{
            return Vector3.zero;
        }
	}

	public Vector3 ActiveControllerAngularVelocity()
	{
        if (activeController == LeverControllerType.Controller_Left)
		{
            return LeftControllerAngularVelocity;
        }
		else if (activeController == LeverControllerType.Controller_Right)
		{
            return RightControllerAngularVelocity;
        }
		else
		{
            return Vector3.zero;
        }
    }



	private OVRInput.Button GetOVRButtonMapping(LeverInputButton button) 
	{
		switch (button) {
		case LeverInputButton.Button_A:
			return OVRInput.Button.One;
		case LeverInputButton.Button_B:
			return OVRInput.Button.Two;
		case LeverInputButton.Button_System:
			return OVRInput.Button.Start;
		case LeverInputButton.Button_Thumbstick_Press:
			return OVRInput.Button.PrimaryThumbstick;
		}

		return 0;
	}

	private bool GetOVRButtonPressDown(LeverControllerType controller, LeverInputButton button) {
		bool isRight = (controller == LeverControllerType.Controller_Right);
		Dictionary<int, bool> buttonState = isRight ? this.buttonStateRight : this.buttonStateLeft;

		bool isDown = GetOVRButtonDown (controller, button);
		bool inputIsDown = buttonState.ContainsKey ((int)button) && (bool)buttonState [(int)button];
		bool isPressDown = (!inputIsDown && isDown);
		buttonState [(int)button] = isDown;
		return isPressDown;
	}

	private bool GetOVRButtonDown(LeverControllerType controller, LeverInputButton button)
	{
		bool isRight = (controller == LeverControllerType.Controller_Right);
		OVRInput.Controller ovrController = OVRInput.Controller.None;
		if (OVRInput.GetConnectedControllers() == OVRInput.Controller.Touch)
			 ovrController =  (isRight ? OVRInput.Controller.RTouch  : OVRInput.Controller.LTouch);
		else 
			 ovrController = (isRight ? OVRInput.Controller.RHand : OVRInput.Controller.LHand);
		switch (button)
		{
		// Buttons
		case LeverInputButton.Button_A:
		case LeverInputButton.Button_B:
		case LeverInputButton.Button_System:
		case LeverInputButton.Button_Thumbstick_Press:
			return OVRInput.Get (GetOVRButtonMapping(button), ovrController);

		// 2D Axis
		case LeverInputButton.Button_Thumbstick_Down:
		case LeverInputButton.Button_Thumbstick_Left:
		case LeverInputButton.Button_Thumbstick_Right:
		case LeverInputButton.Button_Thumbstick_Up:
			{
				OVRInput.Axis2D axis2D = OVRInput.Axis2D.PrimaryThumbstick;

				Vector2 vec = OVRInput.Get (axis2D, ovrController);

				if (button == LeverInputButton.Button_Thumbstick_Down) {
					return vec.y < -0.75;
				} else if (button == LeverInputButton.Button_Thumbstick_Up) {
					return vec.y > 0.75;
				} else if (button == LeverInputButton.Button_Thumbstick_Left) {
					return vec.x < -0.75;
				} else if (button == LeverInputButton.Button_Thumbstick_Right) {
					return vec.x > 0.75;
				}
				return false;
			}


		case LeverInputButton.Button_Trigger:
		case LeverInputButton.Button_Grip:
				{

					OVRInput.Axis1D axis = OVRInput.Axis1D.PrimaryIndexTrigger;
					
					if (button == LeverInputButton.Button_Trigger)
					{
						
						axis = OVRInput.Axis1D.PrimaryIndexTrigger;
					}
					else if (button == LeverInputButton.Button_Grip)
					{
					
						axis = OVRInput.Axis1D.PrimaryHandTrigger;
					}

					float value = 0;
					if (OVRInput.GetActiveController() == OVRInput.Controller.Touch ||
					    OVRInput.GetActiveController() == OVRInput.Controller.Hands)
					{
						value= OVRInput.Get(axis, ovrController);
					}
					else
					{
						value= OVRInput.Get(axis);
					}

					return (value> 0.75f);
				}

		default:
			return false;	
		}
			
	}
}
