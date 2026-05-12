namespace Mechanics.UserWallet.Model
{
    public class TransferTwrsCommand
    {
        public string ReceiverId;
        public WalletOwnerType ReceiverType;
        public WalletOwnerType SenderType;
        public decimal Amount;
        public bool HoldTransfer;
    }
}