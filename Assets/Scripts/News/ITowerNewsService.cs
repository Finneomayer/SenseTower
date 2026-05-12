using Cysharp.Threading.Tasks;

namespace Assets.Scripts.News
{
    public interface ITowerNewsService
    {
        /// <summary>
        /// Получаем новости, начиная с самых новых
        /// </summary>
        /// <param name="getCount"></param>
        /// <returns></returns>
        UniTask<TowerNews[]> GetNews(int? getCount);
    }
}