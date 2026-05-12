using Cysharp.Threading.Tasks;
using Mechanics.UserWallet.Model;
using System;

namespace Mechanics.UserWallet.Service
{
    public interface IWalletService
    {
        public TwrWallet TwrWallet { get; }
        public UniTask<TwrWallet> GetMyWalletAsync();
        public event Action WalletChanged;
    }
}