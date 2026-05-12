using TMPro;
using UnityEngine;

namespace Assets.UI.Pad
{
    public class VisitorNonAdminViewElement : VisitorViewElement
    {
        [SerializeField] private TMP_Text _labelText;

        public void SetLabel(string label)
        {
            _labelText.text = label;
        }
    }
}
