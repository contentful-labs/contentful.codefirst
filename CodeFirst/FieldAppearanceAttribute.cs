using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.CodeFirst
{
    /// <summary>
    /// Attribute specifying the appearance of a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class FieldAppearanceAttribute : Attribute
    {
        /// <summary>
        /// The id of the UI extension to use in the Contentful WebApp for this field.
        /// </summary>
        public string ExtensionId { get; set; }

        /// <summary>
        /// The helptext to accompany this field.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Creates a new FieldAppearanceAttribute.
        /// </summary>
        /// <param name="extensionId">The id of the UI extension to use in the Contentful WebApp for this field.</param>
        public FieldAppearanceAttribute(string extensionId) : this(extensionId, "") { }

        /// <summary>
        /// Creates a new FieldAppearanceAttribute.
        /// </summary>
        /// <param name="extensionId">The id of the UI extension to use in the Contentful WebApp for this field.</param>
        /// <param name="helpText">The helptext to accompany this field.</param>
        public FieldAppearanceAttribute(string extensionId, string helpText)
        {
            ExtensionId = extensionId;
            HelpText = helpText;
        }

        /// <summary>
        /// Returns the EditorInterfaceControl for this attribute.
        /// </summary>
        public virtual EditorInterfaceControl EditorInterfaceControl => new EditorInterfaceControl() { WidgetId = ExtensionId, Settings = new EditorInterfaceControlSettings { HelpText = HelpText } };

        /// <summary>
        /// Attribute specifying a field should have the boolean appearance.
        /// </summary>
        public class BooleanAppearanceAttribute : FieldAppearanceAttribute
        {

            /// <summary>
            /// The label for the true alternative.
            /// </summary>
            public string TrueLabel { get; set; }

            /// <summary>
            /// The label for the false alternative.
            /// </summary>
            public string FalseLabel { get; set; }

            /// <summary>
            /// Creates a new BooleanAppearance attribute.
            /// </summary>
            /// <param name="trueLabel">The label for the true alternative.</param>
            /// <param name="falseLabel">The label for the false alternative.</param>
            /// <param name="helpText">The helptext to accompany this field.</param>
            public BooleanAppearanceAttribute(string trueLabel, string falseLabel, string helpText) : base(SystemWidgetIds.Boolean, helpText)
            {
                TrueLabel = trueLabel;
                FalseLabel = falseLabel;
            }

            /// <summary>
            /// Returns the EditorInterfaceControl for this attribute.
            /// </summary>
            public override EditorInterfaceControl EditorInterfaceControl => new EditorInterfaceControl() { WidgetId = ExtensionId, Settings = new BooleanEditorInterfaceControlSettings { TrueLabel = TrueLabel, FalseLabel = FalseLabel, HelpText = HelpText } };
        }

        /// <summary>
        /// Attribute specifying a field should have the rating appearance.
        /// </summary>
        public class RatingAppearanceAttribute : FieldAppearanceAttribute
        {
            /// <summary>
            /// The number of stars in the rating.
            /// </summary>
            public int NumberOfStars { get; set; }

            /// <summary>
            /// Creates a new RatingAppearanceAttribute.
            /// </summary>
            /// <param name="numberOfStars">The number of stars in the rating.</param>
            /// <param name="helpText">The helptext to accompany this field.</param>
            public RatingAppearanceAttribute(int numberOfStars, string helpText) : base(SystemWidgetIds.Rating, helpText)
            {
                NumberOfStars = numberOfStars;
            }

            /// <summary>
            /// Returns the EditorInterfaceControl for this attribute.
            /// </summary>
            public override EditorInterfaceControl EditorInterfaceControl => new EditorInterfaceControl() { WidgetId = ExtensionId, Settings = new RatingEditorInterfaceControlSettings { NumberOfStars = NumberOfStars, HelpText = HelpText } };
        }

        /// <summary>
        /// Attribute specifying that a field should have the datepicker appearance.
        /// </summary>
        public class DatePickerAppearanceAttribute : FieldAppearanceAttribute
        {
            /// <summary>
            /// The date format of the datepicker.
            /// </summary>
            public EditorInterfaceDateFormat DateFormat { get; set; }

            /// <summary>
            /// The clock format of the datepicker.
            /// </summary>
            public string ClockFormat { get; set; }

            /// <summary>
            /// Creates a new DatePickerAppearanceAttribute.
            /// </summary>
            /// <param name="dateFormat">The date format of the date picker.</param>
            /// <param name="clockFormat">The clock format of the date picker.</param>
            /// <param name="helpText">The helptext to accompany this field.</param>
            public DatePickerAppearanceAttribute(EditorInterfaceDateFormat dateFormat, string clockFormat, string helpText) : base(SystemWidgetIds.DatePicker, helpText)
            {
                DateFormat = dateFormat;
                ClockFormat = clockFormat;
            }

            /// <summary>
            /// Returns the EditorInterfaceControl for this attribute.
            /// </summary>
            public override EditorInterfaceControl EditorInterfaceControl => new EditorInterfaceControl() { WidgetId = ExtensionId, Settings = new DatePickerEditorInterfaceControlSettings { DateFormat = DateFormat, ClockFormat = ClockFormat, HelpText = HelpText } };
        }
    }
}
