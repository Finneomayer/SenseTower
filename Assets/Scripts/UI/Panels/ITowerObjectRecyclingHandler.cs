namespace UI
{
    public interface ITowerObjectRecyclingHandler<T>
    {
        public bool CanBeRecycled(T towerObject);
        public bool ProcessRecycling(T towerObject);
    }
}
