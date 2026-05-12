using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assets.Mechanics.Mafia
{
    public class MafiaTicketChecker
    {
        private CancellationTokenSource _cts;
        private IMafiaGameService _mafiaService;
        private string _tableId;
        private bool _disableChecking = false;

        public bool IsTicketAvailable { get; private set; }

        public event Action Updated;

        public void Init(IMafiaGameService mafiaService, string tableId, bool disableTicketChecking)
        {
            StopChecking();
            _mafiaService = mafiaService;
            _tableId = tableId;
            IsTicketAvailable = false;
            _disableChecking = disableTicketChecking;

            //dev
            if (_disableChecking) IsTicketAvailable = true;
        }

        public void StartChecking()
        {
            StopChecking();

            _cts = new CancellationTokenSource();
            CheckingRoutine(_tableId, _cts.Token).Forget();
        }

        public void StopChecking()
        {
            if (_cts == null)
            {
                return;
            }
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        private async UniTaskVoid CheckingRoutine(string tableId, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                MafiaTicketCheckResultDto checkResultDto = await _mafiaService.CheckMafiaTicketClient(tableId);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (checkResultDto != null)
                {
                    IsTicketAvailable = checkResultDto.Result;

                    //dev
                    if (_disableChecking) IsTicketAvailable = true;

                    Updated?.Invoke();
                }
                await UniTask.Delay(5000, DelayType.Realtime, PlayerLoopTiming.Update, cancellationToken);
            }
        }
    }
}