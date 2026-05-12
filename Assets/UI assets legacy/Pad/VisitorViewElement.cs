using UnityEngine;
using TMPro;
using System;

namespace Assets.UI.Pad
{
    public abstract class VisitorViewElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameText;
        public ulong Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Name { get; private set; }

        public void SetId(ulong id)
        {
            Id = id;
        }

        public void SetGuid(Guid guid)
        {
            Guid = guid;
        }

        public void SetName(string userName)
        {
            Name = userName;
        }

        public void SetText(string text)
        {
            _nameText.text = text;
        }
    }
}