Unity Bug fix.
The problem is that in ScrollRect.cs, the RectTransform position is sometimes set to NaN during scrolling. 
The cause of the problem is that Time.unscaledDeltaTime in the LateUpdate() method is sometimes 0 (which in turn causes a division-by-zero in the calculation of 
velocity - see here for another mention of the same problem). A solution is to make sure that deltaTime is never 0, which I did by replacing

https://forum.unity.com/threads/scroll-view-rect-breaks-on-android.1242772/