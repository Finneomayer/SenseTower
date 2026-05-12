using System;
using System.Collections.Generic;
using Assets.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Mechanics.Mafia
{
    [Serializable]
    public class MafiaRoleViewElement
    {
        public MafiaPlayerRole Role;
        public Sprite RoleImage;
    }

    public class MafiaNightView : MonoBehaviour
    {
        [SerializeField] private List<MafiaRoleViewElement> _rolesImages;
        [SerializeField] private Canvas _nightCanvas;
        [SerializeField] private Image _roleImage;
        [SerializeField] private TMP_Text _roleName;
        [SerializeField] private TMP_Text _roleDescription;
        [SerializeField] private TMP_Text _gameState;
        [SerializeField] private TMP_Text _advice;
        [SerializeField] private LocalizationVariant _yourRoleLV;
        [SerializeField] private LocalizationVariant _adviceLV;
        [SerializeField] private LocalizationVariant _roleDescriptionLV;
        [SerializeField] private LocalizationVariant _gameStateLV;

        private MafiaLocalizationResultDto _mafiaLocalizationData;
        public  Canvas NightCanvas => _nightCanvas;

        public void Init(MafiaLocalizationResultDto mafiaLocalizationData)
        {
            _mafiaLocalizationData = mafiaLocalizationData;
        }

        public void SetData(PlayerState playerState, GameState gameState)
        {
            GetRoleTextForSleepingScreen(playerState, out string roleName, out string roleDescription);

            _roleName.text = $"{_yourRoleLV.Localize()} {roleName}";
            _roleDescription.text = $"{_roleDescriptionLV.Localize()} {roleDescription}";
            _gameState.text = $"{_gameStateLV.Localize()} {_mafiaLocalizationData.StageNames[gameState.GameStage]}";

            int randomRole = Random.Range(2, _mafiaLocalizationData.RoleDescriptions.Count); //random starts from mafia

            _advice.text = (_adviceLV.Localize().
                Replace("{0}", _mafiaLocalizationData.RoleNames[(MafiaPlayerRole)randomRole]).
                Replace("{1}", _mafiaLocalizationData.RoleDescriptions[(MafiaPlayerRole)randomRole]));

            
            foreach (var image in _rolesImages)
            {
                if (image.Role == playerState.Role)
                {
                    _roleImage.sprite = image.RoleImage;
                }
            }
        }

        private bool GetRoleTextForSleepingScreen(PlayerState playerState, out string roleName, out string roleDescription)
        {
            roleName = string.Empty;
            roleDescription = string.Empty;
            if (playerState == null)
            {
                return false;
            }

            return _mafiaLocalizationData.RoleNames.TryGetValue(playerState.Role, out roleName)
                   && _mafiaLocalizationData.RoleDescriptions.TryGetValue(playerState.Role, out roleDescription);
        }
    }
}
