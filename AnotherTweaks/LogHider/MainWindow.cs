using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;

namespace AnotherTweaks
{
    public class LogMessageExposable : LogMessage, IExposable
    {
        public LogMessageExposable() : base(null)
        {
        }

        public LogMessageExposable(string text) : base(text)
        {
        }

        public LogMessageExposable(LogMessageType type, string text) : base(text)
        {
            this.type = type;
        }

        public LogMessageExposable(LogMessageType type, string text, string stackTrace) : base(type, text, stackTrace)
        {
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref text, "text");
            Scribe_Values.Look(ref type, "type");
            //Scribe_Values.Look(ref repeats, "repeats");
            //Scribe_Values.Look(ref stackTrace, "stackTrace");
        }

        public static LogMessageExposable FromLogMessage(LogMessage logMessage) => new LogMessageExposable(logMessage.type, logMessage.text);
    }

    public abstract class LogTab
    {
        protected const float ElementHeight = 20;
        protected List<LogMessage> _logMessages;
        protected List<LogMessageExposable> _logMessagesHided;

        protected LogTab(List<LogMessage> logMessages, List<LogMessageExposable> logMessagesHided) =>
            (_logMessages, _logMessagesHided) = (logMessages, logMessagesHided);

        protected abstract bool IsVisible(LogMessage msg);

        public bool ShowHided { get; set; }

        public virtual IEnumerable<LogMessage> Items => ShowHided ? (IEnumerable<LogMessage>)_logMessagesHided : _logMessages;

        public virtual float ViewHeight
        {
            get
            {
                float height = ElementHeight;
                foreach (var logMessage in Items)
                {
                    if (IsVisible(logMessage))
                    {
                        height += ElementHeight;
                    }
                }
                return height;
            }
        }

        public virtual void Draw(Rect rect)
        {
            foreach (var logMessage in Items)
            {
                
            }
        }
    }

    public class LogHiderWindow : Window
    {
        private LogTab _activeTab;
        private Vector2 _scrollPosition = new Vector2(0, 0);
        private List<LogMessage> _logMessages;
        private bool _showHided;
        private const int controlHeight = 20;
        private const int controlWidth = 100;

        public override Vector2 InitialSize => new Vector2(800f, 600f);

        public LogHiderWindow()
        {
            //optionalTitle = "WealthWatcher";
            preventCameraMotion = false;
            absorbInputAroundWindow = false;
            //draggable = true;
            doCloseX = true;
            _logMessages = Log.Messages.ToList();
        }

        public override void DoWindowContents(Rect rect)
        {
            Rect chkShowHidedRect = new Rect(x: 10f, y: 0f, width: controlWidth, height: controlHeight);
            Rect btnErrorsRect = new Rect(x: 10f, y: chkShowHidedRect.yMax, width: controlWidth, height: controlHeight);
            Rect btnWarningsRect = new Rect(x: btnErrorsRect.xMax, y: btnErrorsRect.y, width: controlWidth, height: controlHeight);
            Rect btnMessagesRect = new Rect(x: btnWarningsRect.xMax, y: btnErrorsRect.y, width: controlWidth, height: controlHeight);

            Widgets.CheckboxLabeled(chkShowHidedRect, "AnotherTweaks.ShowHided".Translate(), ref _showHided);
            if (Widgets.ButtonText(btnErrorsRect, "AnotherTweaks.ErrorsTab".Translate()))
            {

            }
            if (Widgets.ButtonText(btnWarningsRect, "AnotherTweaks.WarningsTab".Translate()))
            {

            }
            if (Widgets.ButtonText(btnMessagesRect, "AnotherTweaks.MessagesTab".Translate()))
            {

            }

            float y = btnMessagesRect.yMax;
            Rect outRect = new Rect(x: 0f, y: y, width: rect.width, height: rect.height - y);
            Rect viewRect = new Rect(x: 0f, y: y, width: rect.width - 30f, height: _activeTab?.ViewHeight ?? 0f);

            Widgets.BeginScrollView(outRect: outRect, scrollPosition: ref _scrollPosition, viewRect: viewRect);
            _activeTab?.Draw(outRect, viewRect, _scrollPosition);
            Widgets.EndScrollView();

            // draw button select tab
            float btnWidth = 150f;
            float btnHeight = 30f;
            float lblOrderWidth = 80f;
            float btnOrderWidth = 120f;
            Rect btnSelectTabRect = new Rect(x: 10f, y: y, width: btnWidth, height: btnHeight);
            Rect lblOrderRect = new Rect(x: btnSelectTabRect.xMax + 10f, y: y, width: lblOrderWidth, height: btnHeight);
            Rect btnOrder1Rect = new Rect(x: lblOrderRect.xMax, y: y, width: btnOrderWidth, height: btnHeight);
            Rect btnOrder2Rect = new Rect(x: btnOrder1Rect.xMax, y: y, width: btnOrderWidth, height: btnHeight);
            string btnCaption = _activeTab?.Caption ?? "capSelectTab".Translate();
            
            if (Widgets.ButtonText(btnSelectTabRect, "btnSelectTab".Translate(btnCaption)))
            {
                Find.WindowStack.Add( new FloatMenu(this.GetTabsList().ToList()));
            }

            Widgets.Label(lblOrderRect, "lblOrder".Translate());

            if (Widgets.ButtonText(btnOrder1Rect, _sort1.Name))
            {
                Find.WindowStack.Add(new FloatMenu(this.GetSortersList(comparer =>
                {
                    _sort1 = comparer;
                    _activeTab?.Sort(_sort1, _sort2);
                }).ToList()));
            }
            if (Widgets.ButtonText(btnOrder2Rect, _sort2.Name))
            {
                Find.WindowStack.Add(new FloatMenu(this.GetSortersList(comparer =>
                {
                    _sort2 = comparer;
                    _activeTab?.Sort(_sort1, _sort2);
                }).ToList()));
            }

            // draw raid points
            if (_raidPoints != null)
            {
                Rect labelRaidPointsRect = new Rect(x: btnOrder2Rect.xMax + 10f, y: y, width: 100f, height: btnHeight);
                Widgets.Label(labelRaidPointsRect, _raidPoints);
            }

            y += btnHeight + 20f;

            // draw content in scroll view
            Rect outRect = new Rect(x: 0f, y: y, width: rect.width, height: rect.height - y);
            Rect viewRect = new Rect(x: 0f, y: y, width: rect.width - 30f, height: _activeTab?.ViewHeight ?? 0f);

            Widgets.BeginScrollView(outRect: outRect, scrollPosition: ref _scrollPosition, viewRect: viewRect);
            _activeTab?.Draw(outRect, viewRect, _scrollPosition);
            Widgets.EndScrollView();

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void SelectTab<T>() where T: ITab, new()
        {
            _activeTab = new T();
            _activeTab.Update();
            _activeTab.Sort(_sort1, _sort2);
        }

        private IEnumerable<FloatMenuOption> GetTabsList()
        {
            yield return new FloatMenuOption("capSelectTab".Translate(), () => _activeTab = null);
            yield return new FloatMenuOption(ItemsTab.CAPTION, SelectTab<ItemsTab>);
            yield return new FloatMenuOption(BuildingsTab.CAPTION, SelectTab<BuildingsTab>);
            yield return new FloatMenuOption(PawnsTab.CAPTION, SelectTab<PawnsTab>);
            
            if (Settings.ShowRadePoints)
                yield return new FloatMenuOption(RadePointsTab.CAPTION, SelectTab<RadePointsTab>);
        }

        private IEnumerable<FloatMenuOption> GetSortersList(Action<TabComparer> setSort)
        {
            foreach (var comparer in _listComparers)
            {
                yield return new FloatMenuOption(comparer.Name, () => setSort(comparer));
            }
        }
    }
}