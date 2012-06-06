using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FluentAssertions.Execution;

#if WINRT
using System.Reflection;
#endif

namespace FluentAssertions.Types
{
    /// <summary>
    /// Contains a number of methods to assert that a <see cref="Type"/> meets certain expectations.
    /// </summary>
    [DebuggerNonUserCode]
    public class TypeAssertions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        protected internal TypeAssertions(Type type)
        {
            Subject = type;
        }

        /// <summary>
        /// Gets the object which value is being asserted.
        /// </summary>
        public Type Subject { get; private set; }

        /// <summary>
        /// Asserts that the current type is equal to the specified <typeparamref name="TExpected"/> type.
        /// </summary>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> Be<TExpected>(string reason = "", params object[] reasonArgs)
        {
            return Be(typeof(TExpected), reason, reasonArgs);
        }

        /// <summary>
        /// Asserts that the current type is equal to the specified <paramref name="expected"/> type.
        /// </summary>
        /// <param name="expected">The expected type</param>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> Be(Type expected, string reason = "", params object[] reasonArgs)
        {
            Execute.Verification
                .ForCondition(Subject == expected)
                .BecauseOf(reason, reasonArgs)
                .FailWith(GetFailureMessageIfTypesAreDifferent(Subject, expected));

            return new AndConstraint<TypeAssertions>(this);
        }

        /// <summary>
        /// Creates an error message in case the specifed <paramref name="actual"/> type differs from the 
        /// <paramref name="expected"/> type.
        /// </summary>
        /// <returns>
        /// An empty <see cref="string"/> if the two specified types are the same, or an error message that describes that
        /// the two specified types are not the same.
        /// </returns>
        private static string GetFailureMessageIfTypesAreDifferent(Type actual, Type expected)
        {
            if (actual.Equals(expected))
            {
                return "";
            }

            string expectedType = expected.FullName;
            string actualType = actual.FullName;

            if (expectedType == actualType)
            {
                expectedType = "[" + expected.AssemblyQualifiedName + "]";
                actualType = "[" + actual.AssemblyQualifiedName + "]";
            }

            return string.Format("Expected type to be {0}{{reason}}, but found {1}.", expectedType, actualType);
        }

        /// <summary>
        /// Asserts that the current type is not equal to the specified <typeparamref name="TUnexpected"/> type.
        /// </summary>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> NotBe<TUnexpected>(string reason = "", params object[] reasonArgs)
        {
            return NotBe(typeof(TUnexpected), reason, reasonArgs);
        }

        /// <summary>
        /// Asserts that the current type is not equal to the specified <paramref name="unexpected"/> type.
        /// </summary>
        /// <param name="unexpected">The unexpected type</param>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> NotBe(Type unexpected, string reason = "", params object[] reasonArgs)
        {
            Execute.Verification
                .ForCondition(Subject != unexpected)
                .BecauseOf(reason, reasonArgs)
                .FailWith("Expected type not to be [" + unexpected.AssemblyQualifiedName + "]{reason}.");

            return new AndConstraint<TypeAssertions>(this);
        }

        /// <summary>
        /// Asserts that the <see cref="Type"/> is decorated with the specified <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> BeDecoratedWith<TAttribute>(string reason = "", params object[] reasonArgs)
        {
            Execute.Verification
                .ForCondition(IsDecoratedWith<TAttribute>(Subject))
                .BecauseOf(reason, reasonArgs)
                .FailWith("Expected type {0} to be decorated with {1}{reason}, but the attribute was not found.",
                    Subject, typeof (TAttribute));

            return new AndConstraint<TypeAssertions>(this);
        }

        /// <summary>
        /// Asserts that the <see cref="Type"/> is decorated with an attribute that matches the
        /// specified <paramref name="attributeConstraints"/>.
        /// </summary>
        /// <param name="attributeConstraints">
        /// Defines the constraints on the expected <typeparamref name="TAttribute"/>.
        /// </param>
        /// <param name="reason">
        /// A formatted phrase as is supported by <see cref="string.Format(string,object[])" /> explaining why the assertion 
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="reasonArgs">
        /// Zero or more objects to format using the placeholders in <see cref="reason" />.
        /// </param>
        public AndConstraint<TypeAssertions> BeDecoratedWith<TAttribute>(AttributeConstraints<TAttribute> attributeConstraints,
            string reason = "", params object[] reasonArgs)
        {
            BeDecoratedWith<TAttribute>(reason, reasonArgs);

            VerifyAttributeConstraints(attributeConstraints, reason, reasonArgs);

            return new AndConstraint<TypeAssertions>(this);
        }

        private void VerifyAttributeConstraints<TAttribute>(AttributeConstraints<TAttribute> attributeConstraints, string reason, object[] reasonArgs)
        {
            foreach (KeyValuePair<PropertyInfo, object> expectedProperty in attributeConstraints.ExpectedProperties)
            {
                object actualValue;
                PropertyInfo property = expectedProperty.Key;
                object expectedValue = expectedProperty.Value;
                bool hasMatchingProperty = HasMatch<TAttribute>(property, expectedValue, out actualValue);

                Execute.Verification
                    .ForCondition(hasMatchingProperty)
                    .BecauseOf(reason, reasonArgs)
                    .FailWith("Expected type {0} to be decorated with {1} " +
                        "(" + property.Name + " = {2}){reason}, but found " +
                            "(" + property.Name + " = {3}).",
                        Subject, typeof(TAttribute), expectedProperty.Value, actualValue);
            }
        }

        private bool HasMatch<TAttribute>(PropertyInfo propertyInfo, object expectedValue, out object actualValue)
        {
            actualValue = null;

            bool hasMatch = false;
            IEnumerable<TAttribute> attributes = GetCustomAttributes<TAttribute>(Subject);

            foreach (TAttribute attribute in attributes)
            {
                actualValue = propertyInfo.GetValue(attribute, null);
                hasMatch = actualValue.Equals(expectedValue);
                if(hasMatch)
                {
                    break;
                }
            }

            return hasMatch;
        }

        private static bool IsDecoratedWith<TAttribute>(Type type)
        {
            return GetCustomAttributes<TAttribute>(type).Any();
        }

        private static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(Type type)
        {
#if !WINRT
            return type.GetCustomAttributes(false).OfType<TAttribute>();
#else
            return type.GetTypeInfo().GetCustomAttributes(false).OfType<TAttribute>();
#endif
        }
    }
}