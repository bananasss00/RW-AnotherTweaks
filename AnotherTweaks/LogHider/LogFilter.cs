using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using Verse;
using System.Xml.Serialization;

namespace AnotherTweaks
{
    [HarmonyPatch(typeof(Log))]
    public static class Log_Patch
    {
        [HarmonyPatch(nameof(Log.Error), new[] { typeof(string) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Error(string text)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.ErrorContainFilters.ContainFilter(text) && !cfg.ErrorHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }
        [HarmonyPatch(nameof(Log.Warning), new[] { typeof(string) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Warning(string text)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.WarningContainFilters.ContainFilter(text) && !cfg.WarningHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }
        [HarmonyPatch(nameof(Log.Message), new[] { typeof(string) })]
        [HarmonyPrefix]
        [HarmonyPriority(Priority.First)]
        public static bool Message(string text)
        {
            var cfg = Settings.Get().LogFilter;
            if (!cfg.enabled || !cfg.MessageContainFilters.ContainFilter(text) && !cfg.MessageHashFilters.Contains(text.GetHashCode())) return true;
            return false;
        }

        public static bool ContainFilter(this string[] filter, string msg)
        {
            foreach (var s in filter)
            {
                // regular expression
                if (s.StartsWith("!"))
                {
                    string pattern = s.Substring(1);
                    try
                    {
                        if (Regex.IsMatch(msg, pattern))
                            return true;
                    }
                    catch {}
                    
                    continue;
                }
                // str contain
                if (msg.Contains(s))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainFilter(this string filterWithDelimiter, string msg)
        {
            var filter = filterWithDelimiter.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            return filter.ContainFilter(msg);
        }
    }

    public class LogFilter
    {
        public static readonly string SettingsFileName = $"{GenFilePaths.FolderUnderSaveData("Config")}\\AnotherTweaks_LogFilter.xml";
        public bool enabled { get; set; }
        public string errorContain { get; set; }
        public string warningContain { get; set; }
        public string messageContain { get; set; }
        public List<LogMessageHided> logsHided { get; set; } // public for xml serializer

        [XmlIgnore]
        public int[] ErrorHashFilters { get; private set; }
        [XmlIgnore]
        public int[] WarningHashFilters { get; private set; }
        [XmlIgnore]
        public int[] MessageHashFilters { get; private set; }
        [XmlIgnore]
        public string[] ErrorContainFilters { get; private set; }
        [XmlIgnore]
        public string[] WarningContainFilters { get; private set; }
        [XmlIgnore]
        public string[] MessageContainFilters { get; private set; }
        [XmlIgnore]
        public IEnumerable<LogMessageHided> LogsHided => logsHided;

        public void SetLogsHided(List<LogMessageHided> logsHided)
        {
            this.logsHided = logsHided;
            UpdateHashes();
            Save();
        }

        public void UpdateHashes()
        {
            ErrorHashFilters = logsHided.Where(l => l.type == LogMessageType.Error).Select(l => l.text.GetHashCode()).ToArray();
            WarningHashFilters = logsHided.Where(l => l.type == LogMessageType.Warning).Select(l => l.text.GetHashCode()).ToArray();
            MessageHashFilters = logsHided.Where(l => l.type == LogMessageType.Message).Select(l => l.text.GetHashCode()).ToArray();
            
            if (!String.IsNullOrWhiteSpace(errorContain))
                ErrorContainFilters = errorContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else ErrorContainFilters = new string[]{};

            if (!String.IsNullOrWhiteSpace(warningContain))
                WarningContainFilters = warningContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else WarningContainFilters = new string[]{};
            
            if (!String.IsNullOrWhiteSpace(messageContain))
                MessageContainFilters = messageContain.Split(new[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
            else MessageContainFilters = new string[]{};
        }

        public void Save()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(LogFilter));
            using (FileStream fs = new FileStream(SettingsFileName, FileMode.Create))
            {
                xmlSerializer.Serialize(fs, this);
            }
        }

        public void Load()
        {
            if (File.Exists(SettingsFileName))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(LogFilter));
                using (FileStream fs = new FileStream(SettingsFileName, FileMode.OpenOrCreate))
                {
                    LogFilter filter = xmlSerializer.Deserialize(fs) as LogFilter;

                    if (filter == null)
                        throw new Exception("Can't deserialize LogFilter");

                    enabled = filter.enabled;
                    errorContain = filter.errorContain;
                    warningContain = filter.warningContain;
                    messageContain = filter.messageContain;
                    logsHided = filter.logsHided;
                }
            }

            if (logsHided == null)
                logsHided = new List<LogMessageHided>();
            UpdateHashes();
        }
    }
}