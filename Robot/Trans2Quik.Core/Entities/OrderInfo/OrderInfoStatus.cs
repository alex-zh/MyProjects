namespace Robot.Trans2Quik.Entities.OrderInfo
{
    /// <summary>
    /// Состояние исполнения заявки
    /// </summary>
    public enum OrderInfoStatus
    {
        /// <summary>
        /// Заявка исполнена
        /// </summary>
        Completed = 0,

        /// <summary>
        /// Активная заявка
        /// </summary>
        Active = 1,

        /// <summary>
        /// Заявка снята
        /// </summary>
        Withdrawn = 2
    }
}
