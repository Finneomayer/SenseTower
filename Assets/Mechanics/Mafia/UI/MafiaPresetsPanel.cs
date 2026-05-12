using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Localization;
using Assets.Mechanics.Mafia.UI;
using Assets.UI.Pad;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Mechanics.Mafia
{
    public class MafiaPresetsPanel : MonoBehaviour
    {
        public event Action<MafiaPlayerRole[]> OnUpdatePreset;
        [SerializeField] private RoleItem _currentRolePrefab;
        [SerializeField] private Transform _currentRolesParent;
        [SerializeField] private Button _defaultButton;
        [SerializeField] private Button _applyButton;
        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private TMP_Text _errorText;
        [SerializeField] private LocalizationVariant _errorLocalization;

        private MafiaPlayerRole[] _currentRoles;
        private MafiaPlayerRole[] _defaultRoles;
        private int _currentRolesCount;

        public void ShowPreset(MafiaPlayerRole[] roles)
        {
            if (_currentRolesCount == roles.Length) return; //when the player just moved and does not need to update the preset on UI

            _currentRolesCount = roles.Length;

            _currentRoles = new MafiaPlayerRole[_currentRolesCount];
            _defaultRoles = new MafiaPlayerRole[_currentRolesCount];

            for (int i = 0; i < roles.Length; i++)
            {
                _currentRoles[i] = roles[i];
                _defaultRoles[i] = roles[i];
            }

            ShowCurrentRoles(_currentRoles);

            _infoText.enabled = true;
            _errorText.enabled = false;
        }

        private void OnEnable()
        {
            _defaultButton.onClick.AddListener(SetDefaultRoles);
            _applyButton.onClick.AddListener(ApplyOnClick);
        }

        private void OnDisable()
        {
            _defaultButton.onClick.RemoveListener(SetDefaultRoles);
            _applyButton.onClick.RemoveListener(ApplyOnClick);
        }

        private void SetDefaultRoles()
        {
            _currentRoles = new MafiaPlayerRole[_defaultRoles.Length];

            for (int i = 0; i < _defaultRoles.Length; i++)
            {
                _currentRoles[i] = _defaultRoles[i];
            }

            ShowCurrentRoles(_currentRoles);

            _applyButton.interactable = _currentRoles.Length == _currentRolesCount;
            _infoText.enabled = _currentRoles.Length == _currentRolesCount;
            _errorText.enabled = _currentRoles.Length != _currentRolesCount;

            OnUpdatePreset?.Invoke(_currentRoles);
        }

        private void ApplyOnClick()
        {
            OnUpdatePreset?.Invoke(_currentRoles);
        }


        public void ShowCurrentRoles(MafiaPlayerRole[] roles)
        {
            var children = _currentRolesParent.GetComponentsInChildren<RoleItem>();
            foreach (var child in children)
            {
                Destroy(child.gameObject);
            }

            var roleDictionary = TranslateRoleArrayToDictionary(roles);
            var sortedRoleDictionary = 
                from entry in roleDictionary orderby entry.Value descending select entry;

            foreach (var role in sortedRoleDictionary)
            {
                CreateCurrentRoleItem(role.Key, role.Value, roleDictionary);
            }
        }

        private void CreateCurrentRoleItem(MafiaPlayerRole role, int count, Dictionary<MafiaPlayerRole, int> dictionary)
        {
            RoleItem item = Instantiate(_currentRolePrefab, _currentRolesParent);
            item.RoleName.text = role.ToString();
            item.UpButton.onClick.AddListener(() => ChangeRoleCountInPreset(role, 1, count, dictionary));
            item.DownButton.onClick.AddListener(() => ChangeRoleCountInPreset(role, -1, count, dictionary));
            item.Count.text = count.ToString();
        }

        private void ChangeRoleCountInPreset(MafiaPlayerRole role, int value, int count, Dictionary<MafiaPlayerRole, int> dictionary)
        {
            if (count + value < 0) return;

            dictionary[role] = count + value;

            _currentRoles = TranslateRoleDictionaryToArray(dictionary);

            ShowCurrentRoles(_currentRoles);

            _applyButton.interactable = _currentRoles.Length == _currentRolesCount;
            _infoText.enabled = _currentRoles.Length == _currentRolesCount;
            _errorText.enabled = _currentRoles.Length != _currentRolesCount;

            _errorText.text = _errorLocalization.Localize().Replace("{1}", _currentRoles.Length.ToString())
                    .Replace("{2}", _currentRolesCount.ToString());
        }

        private Dictionary<MafiaPlayerRole, int> TranslateRoleArrayToDictionary(MafiaPlayerRole[] roles)
        {
            Dictionary<MafiaPlayerRole, int> result = new Dictionary<MafiaPlayerRole, int>();

            for (int i = 0; i < roles.Length; i++)
            {
                if (result.ContainsKey(roles[i])) result[roles[i]]++;
                else result.Add(roles[i], 1);
            }

            var extraRoles = System.Enum.GetValues(typeof(MafiaPlayerRole));

            foreach (var role in extraRoles)
            {
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Unknown) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.GameMaster) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Beauty) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Executioner) continue; //с первого тз
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Fool) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Lawyer) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Poisoner) continue;
                if ((MafiaPlayerRole)role == MafiaPlayerRole.Psycho) continue;

                if (!result.ContainsKey((MafiaPlayerRole)role)) result.Add((MafiaPlayerRole)role, 0);
            }

            return result;
        }

        private MafiaPlayerRole[] TranslateRoleDictionaryToArray(Dictionary<MafiaPlayerRole, int> roles)
        {
            List<MafiaPlayerRole> result = new List<MafiaPlayerRole>();

            foreach (var role in roles)
            {
                if (role.Value == 0) continue;
                for (int i = 0; i < role.Value; i++)
                {
                    result.Add(role.Key);
                }
            }
            return result.ToArray();
        }
    }
}
