namespace SC.SenseTower.Common.Enums
{
    /// <summary>
    /// Уровень доступа в помещение.
    /// </summary>
    public enum AccessType : byte
    {
        /// <summary>
        /// Все могут заходить в помещение.
        /// </summary>
        Public,

        /// <summary>
        /// Все могут заходить, если собственник в помещении.
        /// </summary>
        Live,

        /// <summary>
        /// Может заходить только собственник.
        /// </summary>
        Private
    }
}
