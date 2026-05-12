
using Cysharp.Threading.Tasks;

namespace Mechanics.CodeCompiler
{
    public interface IBehaviourCreator
    {
        public UniTask GetBehaviourSource();
        public void CreateBehaviour();
    }
}
