using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewLogicTest : LogoViewBase
{
    public override void Enter()
    {
        print("Enter");
    }

    public override void Exit()
    { 
        print("Exit");
    }

    public override void Stay()
    {
        print("Stay");
    }
      
}
