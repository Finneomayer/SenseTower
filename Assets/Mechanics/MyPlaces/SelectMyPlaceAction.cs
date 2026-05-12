using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMyPlaceAction : UIAction
{
    public override void Invoke()
    {
        base.Invoke();
        GetComponent<PlaceItemUI>().OnSelect.Invoke();
    }
}
