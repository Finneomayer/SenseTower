using System;

namespace Assets.Blackboard
{
    public sealed class BlackboardEventMediator
    {
        public event Action<string> LoadBlackboardFileRequested;
        public event Action<string> DeleteBlackboardFileRequested;

        public void RequestLoadBlackboardFile(string filename)
        {
            LoadBlackboardFileRequested?.Invoke(filename);
        }

        public void RequestDeleteBlackboardFile(string filename)
        {
            DeleteBlackboardFileRequested?.Invoke(filename);
        }
    }
}


