using System.Xml.Serialization;
using Verse;

namespace AnotherTweaks
{
    public class LogMessageHided : LogMessage
    {
        [XmlIgnore]
        public bool show;

        public LogMessageHided() : base(null)
        {
        }

        public LogMessageHided(string text) : base(text)
        {
        }

        public LogMessageHided(LogMessageType type, string text) : base(text)
        {
            this.type = type;
            this.show = true;
        }

        public LogMessageHided(LogMessageType type, string text, string stackTrace) : base(type, text, stackTrace)
        {
        }

        public override int GetHashCode()
        {
            return text.GetHashCode() * 2 + type.GetHashCode() * 3;
        }

        public override bool Equals(object obj)
        {
            if (obj is LogMessageHided o)
            {
                return text.Equals(o.text) && type.Equals(o.type);
            }
            return false;
        }

        public static LogMessageHided FromLogMessage(LogMessage logMessage) => new LogMessageHided(logMessage.type, logMessage.text);
    }
}