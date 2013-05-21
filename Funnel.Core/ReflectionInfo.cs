using System;
using System.Collections.Generic;

namespace Funnel
{
    /// <summary>
    /// Set of Key/Value pairs
    /// </summary>
    public class ReflectionInfo : Dictionary<object, object>
    {
        #region Fields

        /// <summary>
        /// List of explicitly mapped columns
        /// </summary>
        public Dictionary<object, List<MappedColumn>> MappedColumns = new Dictionary<object, List<MappedColumn>>();

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the source type.
        /// </summary>
        public Type SourceType { get; set; }

        #endregion
    }
}