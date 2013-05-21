namespace Funnel
{
    /// <summary>
    /// The MappingConverter interface.
    /// </summary>
    public interface IMappingConverter
    {
        #region Public Methods and Operators

        /// <summary>
        /// The conversion method.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        object ConversionMethod(object item);

        #endregion
    }
}