using TMPro;
using UnityEngine;

namespace UI
{
    public sealed class TwrTransactionItem : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text AmountText;
        [SerializeField]
        private TMP_Text DesctiptionText;
        [SerializeField]
        private TMP_Text DateTimeText;

        [SerializeField]
        private Color IncomeColor = Color.green;
        [SerializeField]
        private Color OutcomeColor = Color.red;
        [SerializeField]
        private Color HoldColor = Color.blue;

        public void Init(TransactionDto transactionData)
        {            
            DesctiptionText.text = transactionData.Description != null ? 
                transactionData.Description.ToString() : "";

            DateTimeText.text = transactionData.Timestamp.HasValue ?
                transactionData.Timestamp.Value.ToLocalTime().ToString("HH:mm dd.MM.yyyy") : "";

            if (transactionData.Amount.HasValue)
            {
                AmountText.text = transactionData.Amount.Value.ToString("#.#");
                if (transactionData.Type == TransactionType.Income)
                {
                    AmountText.text = $"+{AmountText.text}";
                }
                else if(transactionData.Type == TransactionType.Hold)
                {
                    AmountText.text = $"{AmountText.text}";
                }
                else if(transactionData.Type == TransactionType.Outcome)
                {
                    AmountText.text = $"-{AmountText.text}";

                }
            }
            else
            {
                AmountText.text = "";
            }

            switch (transactionData.Type)
            {
                case TransactionType.Income:
                    SetTextColor(IncomeColor);
                    break;
                case TransactionType.Outcome:
                    SetTextColor(OutcomeColor);
                    break;
                case TransactionType.Hold:                   
                    SetTextColor(HoldColor);
                    break;
                default:
                    break;
            }
        }

        private void SetTextColor(Color color)
        {
            AmountText.color = color;
            DesctiptionText.color = color;
            DateTimeText.color = color;
        }
    }
}
