using UnityEngine;

public class PlayerMenuService : MenuService
{
    public void AnimateButton(ButtonUI button) 
    {
        button.InteractElement.gameObject.transform.localScale = Vector3.one * 1.15f;
        button.Background.color = Color.green;
    }

    public void SetPreviousState(ButtonUI button) 
    {
        button.Background.color = Color.white;
        button.InteractElement.gameObject.transform.localScale = Vector3.one;
    }
}
