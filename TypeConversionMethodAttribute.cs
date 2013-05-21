using System;

namespace Funnel
{
    /// <summary>
    /// The type conversion method attribute.
    /// </summary>
    public class TypeConversionMethodAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// The converter.
        /// </summary>
        private readonly IMappingConverter _converter;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeConversionMethodAttribute"/> class.
        /// </summary>
        /// <param name="t">
        /// The t.
        /// </param>
        /// <exception cref="Exception">
        /// Throws exception if the type is not an IMappingConverter
        /// </exception>
        public TypeConversionMethodAttribute(Type t)
        {
            if (typeof(IMappingConverter).IsAssignableFrom(t))
                _converter = Activator.CreateInstance(t) as IMappingConverter;
            else
                throw new Exception("Invalid IMappingConverter");
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The System.Object.
        /// </returns>
        public object Parse(object item)
        {
            return _converter.ConversionMethod(item);
        }

        #endregion
    }
}