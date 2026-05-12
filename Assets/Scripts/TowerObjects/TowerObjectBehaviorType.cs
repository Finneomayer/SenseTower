namespace Assets.Scripts.TowerObjects
{
    /// <summary>
    /// Как мы используем объект на сцене
    /// </summary>
    public enum TowerObjectBehaviorType
    {
        /// <summary>
        /// Размещаем статически
        /// </summary>
        Static = 0,

        /// <summary>
        /// Достаем с полки пользователя и взаимодействуем с ним
        /// </summary>
        Movable = 1,

        /// <summary>
        /// Носим на Аватаре
        /// </summary>
        Wearable = 2
    }
}