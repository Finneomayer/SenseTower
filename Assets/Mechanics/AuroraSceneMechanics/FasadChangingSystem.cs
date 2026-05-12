using System.Net.Mime;
using Meta.WitAi;
using Oculus.Interaction;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Mechanics.AuroraSceneMechanics
{
    public class FasadChangingSystem : NetworkBehaviour
    {
        public NetworkVariable<int> FasadNumber;
        public GameObject Fasad1;
        public GameObject Fasad2;
        public GameObject Phone;

        private XRSimpleInteractable phoneInteractor;

        public void Init()
        {
            var Fasad = GameObject.Find("Fasads");
            if (Fasad != null)
            {
                Fasad1 = Fasad.transform.GetChild(0).gameObject;
                Fasad2 = Fasad.transform.GetChild(1).gameObject;
                Phone = Fasad.transform.GetChild(2).gameObject;

                phoneInteractor = Phone.AddComponent<XRSimpleInteractable>();
                phoneInteractor.interactionLayers = InteractionLayerMask.GetMask("Default", "Grab");

                phoneInteractor.selectEntered.AddListener(OnPressPhone);

                SetFasadType(FasadNumber.Value);
            }
        }

        private void OnPressPhone(SelectEnterEventArgs arg0)
        {
            ChangeFasadVariantServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChangeFasadVariantServerRpc()
        {
            if (FasadNumber.Value == 0) FasadNumber.Value = 1;
            else FasadNumber.Value = 0;

            ChangeFasadVariantClientRpc(FasadNumber.Value);
        }

        [ClientRpc]
        private void ChangeFasadVariantClientRpc(int num)
        {
            SetFasadType(num);
        }

        private void SetFasadType(int num)
        {
            Fasad1.SetActive(num == 0);
            Fasad2.SetActive(num != 0);
        }

        public override void OnNetworkDespawn()
        {
            if (phoneInteractor != null) phoneInteractor.selectEntered.RemoveListener(OnPressPhone);
            base.OnNetworkDespawn();
        }
    }
}
