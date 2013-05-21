using System;

namespace Funnel
{
    /// <summary>
    /// A mapped column
    /// </summary>
    public class MappedColumn
    {
        #region Public Properties
        /// <summary>
        /// The IMapping Converter to use when setting the source to the target.
        /// </summary>
        public Type Converter { get; set; }

        /// <summary>
        /// The source property/columns name.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The target property/columns name.
        /// </summary>
        public string Target { get; set; }

        #endregion
    }
}