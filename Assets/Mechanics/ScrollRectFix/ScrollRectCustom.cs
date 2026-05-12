using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Unity Bug fix.
///The problem is that in ScrollRect.cs, the RectTransform position is sometimes set to NaN during scrolling. 
///The cause of the problem is that Time.unscaledDeltaTime in the LateUpdate() method is sometimes 0(which in turn causes a division - by - zero in the calculation of
///velocity - see here for another mention of the same problem).A solution is to make sure that deltaTime is never 0, which I did by replacing
///https://forum.unity.com/threads/scroll-view-rect-breaks-on-android.1242772/
/// </summary>
public class ScrollRectCustom : ScrollRect
{
    protected override void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;
        if (deltaTime == 0.0f) return;
        deltaTime = Mathf.Max(Time.unscaledDeltaTime, 0.015f);
        base.LateUpdate();
    }

    protected override void SetContentAnchoredPosition(Vector2 position)
    {
        if (float.IsNaN(position.x) || float.IsNaN(position.y))
        {
            return;
        }
        base.SetContentAnchoredPosition(position);
    }
}
