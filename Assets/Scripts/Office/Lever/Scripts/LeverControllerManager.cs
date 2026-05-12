//ToDo Sally Lever может и не стоило отдельным файлом выносить
public class LeverControllerManager 
{
	public static bool leftControllerActive;
	public static bool rightControllerActive;

	public static InterectObjectGrabbable nearestGrabbableToRightController = null;
	public static InterectObjectGrabbable nearestGrabbableToLeftController = null;

	public static float distanceToRightController = 10000f;
	public static float distanceToLeftController = 10000f;
}
