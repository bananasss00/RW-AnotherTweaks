using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class LogHiderWindow : Window
    {
        private static readonly Color SelectedColor = new Color(0.5f, 1f, 0.5f, 1f);
        private LogMessageType _logMessageType;
        private Vector2 _scrollPosition = new Vector2(0, 0);
        private List<LogMessageHided> _logMessages;
        private const int controlHeight = 30;

        private string _searchBuf;

        public override Vector2 InitialSize => new Vector2(1024f, 768f);

        public LogHiderWindow()
        {
            //optionalTitle = "WealthWatcher";
            preventCameraMotion = false;
            absorbInputAroundWindow = false;
            //draggable = true;
            doCloseX = true;

            _logMessages = Settings.Get().LogFilter.LogsHided.Concat(Log.Messages.Select(LogMessageHided.FromLogMessage)).Distinct().ToList();
        }

        public override void PostClose()
        {
            Settings.Get().LogFilter.SetLogsHided(_logMessages
                .Where(x => !x.show)
                .Distinct()
                .ToList());
        }

        public override void DoWindowContents(Rect rect)
        {
            int btnWidth = 200;
            Rect chkEnabledRect = new Rect(x: 0, y: 0f, width: rect.width, height: controlHeight);
            Rect teSearchRect = new Rect(x: 0, y: chkEnabledRect.yMax, width: rect.width, height: controlHeight);
            Rect btnErrorsRect = new Rect(x: 10f, y: teSearchRect.yMax, width: btnWidth, height: controlHeight);
            Rect btnWarningsRect = new Rect(x: btnErrorsRect.xMax, y: btnErrorsRect.y, width: btnWidth, height: controlHeight);
            Rect btnMessagesRect = new Rect(x: btnWarningsRect.xMax, y: btnErrorsRect.y, width: btnWidth, height: controlHeight);
            Rect teContainFilterRect = new Rect(x: 0, y: btnMessagesRect.yMax, width: rect.width, height: controlHeight);

            Widgets.CheckboxLabeled(chkEnabledRect, "AnotherTweaks.LogFilter.Enabled".Translate(), ref Settings.Get().LogFilter.enabled);
            _searchBuf = Widgets.TextEntryLabeled(teSearchRect, "AnotherTweaks.LogFilter.Search".Translate(), _searchBuf);

            Color color = GUI.color;
            if (_logMessageType == LogMessageType.Error)
                GUI.color = SelectedColor;
            if (Widgets.ButtonText(btnErrorsRect, "AnotherTweaks.LogFilter.ErrorsTab".Translate()))
            {
                _logMessageType = LogMessageType.Error;
            }
            GUI.color = color;

            if (_logMessageType == LogMessageType.Warning)
                GUI.color = SelectedColor;
            if (Widgets.ButtonText(btnWarningsRect, "AnotherTweaks.LogFilter.WarningsTab".Translate()))
            {
                _logMessageType = LogMessageType.Warning;
            }
            GUI.color = color;

            if (_logMessageType == LogMessageType.Message)
                GUI.color = SelectedColor;
            if (Widgets.ButtonText(btnMessagesRect, "AnotherTweaks.LogFilter.MessagesTab".Translate()))
            {
                _logMessageType = LogMessageType.Message;
            }
            GUI.color = color;

            var cfg = Settings.Get().LogFilter;
            IEnumerable<LogMessageHided> containFilteredItems = null;
            switch (_logMessageType)
            {
                case LogMessageType.Error:
                    cfg.errorContain = Widgets.TextEntryLabeled(teContainFilterRect, "AnotherTweaks.LogFilter.ErrorContain".Translate(), cfg.errorContain);
                    containFilteredItems = _logMessages.Where(x => x.type == _logMessageType && !cfg.errorContain.ContainFilter(x.text));
                    break;
                case LogMessageType.Warning:
                    cfg.warningContain = Widgets.TextEntryLabeled(teContainFilterRect, "AnotherTweaks.LogFilter.WarningContain".Translate(), cfg.warningContain);
                    containFilteredItems = _logMessages.Where(x => x.type == _logMessageType && !cfg.warningContain.ContainFilter(x.text));
                    break;
                case LogMessageType.Message:
                    cfg.messageContain = Widgets.TextEntryLabeled(teContainFilterRect, "AnotherTweaks.LogFilter.MessageContain".Translate(), cfg.messageContain);
                    containFilteredItems = _logMessages.Where(x => x.type == _logMessageType && !cfg.messageContain.ContainFilter(x.text));
                    break;
            }

            List<LogMessageHided> drawItems;
            if (String.IsNullOrWhiteSpace(_searchBuf))
            {
                drawItems = containFilteredItems.ToList();
            }
            else
            {
                drawItems = containFilteredItems.Where(x => x.text.ToLower().Contains(_searchBuf.ToLower())).ToList();
            }

            float y = teContainFilterRect.yMax;
            Rect outRect = new Rect(x: 0f, y: y, width: rect.width, height: rect.height - y);
            Rect viewRect = new Rect(x: 0f, y: y, width: rect.width - 30f, height: drawItems.Count * controlHeight);
            Widgets.BeginScrollView(outRect: outRect, scrollPosition: ref _scrollPosition, viewRect: viewRect);
            foreach (var item in drawItems)
            {
                Rect chkItemRect = new Rect(x: 0, y: y, width: viewRect.width, height: controlHeight);
                Widgets.CheckboxLabeled(chkItemRect, item.text, ref item.show);
                TooltipHandler.TipRegion(chkItemRect, item.text);
                y += controlHeight/*chkItemRect.yMax*/;
            }
            Widgets.EndScrollView();
        }
    }
}