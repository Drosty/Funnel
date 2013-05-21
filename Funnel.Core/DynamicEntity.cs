using System.Collections.Generic;
using System.Dynamic;

namespace Funnel
{
    /// <summary>
    /// Defines a Dynamic Object containing dictionary functionality.
    /// </summary>
    public class DynamicEntity : DynamicObject
    {
        private readonly IDictionary<string, object> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        public DynamicEntity()
        {
            values = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicEntity" /> class.
        /// </summary>
        /// <param name="values">The values to initialize the DynamicEntity with.</param>
        public DynamicEntity(IDictionary<string, object> values)
        {
            this.values = values;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object" /> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.Object" />.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>Returns the value of the dynamic object at the specified key.</returns>
        public object this[string key]
        {
            get
            {
                return values[key];
            }
            set
            {
                if (values.ContainsKey(key)) values[key] = value;
                values.Add(key, value);
            }
        }

        /// <summary>
        /// Provides the implementation for operations that get member values.
        /// Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.
        /// The binder.Name property provides the name of the member on which the dynamic operation is performed.
        /// For example, for the Console.WriteLine(sampleObject.SampleProperty) statement,
        /// where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, binder.Name returns "SampleProperty".
        /// The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result" />.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false.
        /// If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (values.ContainsKey(binder.Name))
            {
                result = values[binder.Name];
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Provides the implementation for operations that set member values.
        /// Classes derived from the <see cref="T:System.Dynamic.DynamicObject" /> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation.
        /// The binder.Name property provides the name of the member to which the value is being assigned.
        /// For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" />
        /// class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test",
        /// where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject" /> class, the <paramref name="value" /> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior.
        /// (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (values.ContainsKey(binder.Name))
                values[binder.Name] = value;
            else
                values.Add(binder.Name, value);
            return true;
        }
    }
}
