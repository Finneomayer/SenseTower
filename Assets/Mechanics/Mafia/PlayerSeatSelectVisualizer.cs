using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Mafia
{
    public class PlayerSeatSelectVisualizer : MonoBehaviour
    {
        [SerializeField]
        private Image SelectingArrow;
        [SerializeField]
        private GameObject SelectingRampContainer;
        //[SerializeField]
        //private GameObject SelectedElementsContainer;
        [SerializeField]
        private MeshRenderer MeshRenderer;
        [SerializeField]
        private Color HoveredColor;
        [SerializeField]
        private Color SelectedColor;

        private Material _material;

        public void Awake()
        {
            _material = MeshRenderer.material;
            SelectingArrow.color = SelectedColor;
            HideVisual();
        }

        public Color GetSelectedColor()
        {
            return SelectedColor;
        }

        public void HoverVisual()
        {
            SetSelected(false);
            SetColor(HoveredColor);
            SelectingRampContainer.SetActive(true);
        }

        public void SelectVisual()
        {
            SetSelected(true);
        }

        public void HideVisual()
        {
            SetSelected(false);
        }

        private void SetColor(Color color)
        {
            if (_material != null)
            {
                _material.SetColor("Color_cce786789ad1477dafce72d1048b98c1", color);
            }
        }

        private void SetSelected(bool selected)
        {
            SelectingArrow.gameObject.SetActive(selected);
            SelectingRampContainer.SetActive(selected);
            //SelectedElementsContainer.SetActive(selected);
            if (selected)
            {
                SetColor(SelectedColor);
            }
            else
            {
                SetColor(HoveredColor);
            }
        }
    }
}
