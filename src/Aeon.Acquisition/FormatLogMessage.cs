using Bonsai;
using Bonsai.Expressions;
using Bonsai.Harp;
using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Aeon.Acquisition
{
    [Combinator]
    [DefaultProperty(nameof(Timestamp))]
    [WorkflowElementCategory(ElementCategory.Transform)]
    [Description("Generates a sequence of custom formatted prioritised messages for the event log.")]
    public class FormatLogMessage : Bonsai.Expressions.FormatBuilder
    {
        [Category("Metadata")]
        [Description("The priority level of the log message.")]
        public PriorityLevel Priority { get; set; }

        [Category("Metadata")]
        [Description("The type or category of the log message.")]
        public string Type { get; set; } = LogMessage.DefaultType;

        [Category("Metadata")]
        [Description("The inner property that will be selected as the timestamp for each element of the sequence.")]
        [Editor("Bonsai.Design.MemberSelectorEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string Timestamp { get; set; }

        protected override Expression BuildSelector(Expression expression)
        {
            var message = base.BuildSelector(expression);
            var timestamp = ExpressionHelper.MemberAccess(expression, Timestamp);
            if (timestamp.Type.IsGenericType && timestamp.Type.GetGenericTypeDefinition() == typeof(Timestamped<>))
            {
                timestamp = ExpressionHelper.MemberAccess(timestamp, nameof(Timestamped<object>.Seconds));
            }

            if (timestamp.Type != typeof(double))
            {
                throw new InvalidOperationException("The timestamp selector must be a 64-bit floating point value representing the seconds timestamp.");
            }

            return Expression.Call(
                Expression.Constant(this),
                nameof(CreateLogMessage),
                null,
                message,
                timestamp);
        }

        Timestamped<LogMessage> CreateLogMessage(string message, double seconds)
        {
            var logMessage = new LogMessage(Priority, Type, message);
            return Timestamped.Create(logMessage, seconds);
        }
    }
}
