using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace AnotherTweaks
{
    public class LogExtended
    {
        public static void Patch(Harmony h)
        {
            var doWindowContents = AccessTools.Method(typeof(EditWindow_Log), nameof(EditWindow_Log.DoWindowContents));
            var doMessagesListing = AccessTools.Method(typeof(EditWindow_Log), nameof(EditWindow_Log.DoMessagesListing));
            var selectLastMessage = AccessTools.Method(typeof(EditWindow_Log), nameof(EditWindow_Log.SelectLastMessage));
            var copyAllMessagesToClipboard = AccessTools.Method(typeof(EditWindow_Log), nameof(EditWindow_Log.CopyAllMessagesToClipboard));

            var _this = typeof(LogExtended);
            h.Patch(doWindowContents, transpiler: new HarmonyMethod(_this, nameof(EditWindow_Log_DoWindowContents)));
            h.Patch(doMessagesListing, transpiler: new HarmonyMethod(_this, nameof(Log_Messages_Replacer)));
            h.Patch(selectLastMessage, transpiler: new HarmonyMethod(_this, nameof(Log_Messages_Replacer)));
            h.Patch(copyAllMessagesToClipboard, transpiler: new HarmonyMethod(_this, nameof(Log_Messages_Replacer)));
        }

        public static IEnumerable<CodeInstruction> EditWindow_Log_DoWindowContents(IEnumerable<CodeInstruction> instruction, ILGenerator ilGen)
        {
            var copyAllMessagesToClipboard = AccessTools.Method(typeof(EditWindow_Log), nameof(EditWindow_Log.CopyAllMessagesToClipboard));
            var drawButton = AccessTools.Method(typeof(LogExtended), nameof(DrawButton));
            var code = instruction.ToList();
            bool ok = false;
            for (int i = 0; i < code.Count; i++)
            {
                //74	00EB	call	instance void Verse.EditWindow_Log::CopyAllMessagesToClipboard()
                yield return code[i];
                if (code[i].opcode == OpCodes.Call && code[i].operand == copyAllMessagesToClipboard && code[i - 2].opcode == OpCodes.Brfalse_S)
                {
                    var fixedLabel = ilGen.DefineLabel();
                    code[i - 2].operand = fixedLabel;
                    yield return new CodeInstruction(OpCodes.Ldloc_2).WithLabels(fixedLabel);
                    yield return new CodeInstruction(OpCodes.Call, drawButton);
                    ok = true;
                }
            }
            if (!ok) Log.Error($"Transpiler EditWindow_Log_DoWindowContents failed");
        }

        public static IEnumerable<CodeInstruction> Log_Messages_Replacer(IEnumerable<CodeInstruction> instructions, ILGenerator ilGen)
        {
            return instructions.MethodReplacer(AccessTools.PropertyGetter(typeof(Log), nameof(Log.Messages)), AccessTools.Method(typeof(LogExtended), nameof(Messages)));
        }

        public enum ShowMode
        {
            All,
            Messages,
            Warnings,
            Errors
        }

        public static ShowMode _showMode = ShowMode.All;

        public static void DrawButton(WidgetRow widgetRow)
        {
            if (widgetRow.ButtonText($"Show: {_showMode}", "Show selected type of messages."))
            {
                var options = new[]
                {
                    new FloatMenuOption("All", () => _showMode = ShowMode.All), 
                    new FloatMenuOption("Messages", () => _showMode = ShowMode.Messages), 
                    new FloatMenuOption("Warnings", () => _showMode = ShowMode.Warnings), 
                    new FloatMenuOption("Errors", () => _showMode = ShowMode.Errors), 
                };
                Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>(options)));
            }
        }

        public static IEnumerable<LogMessage> Messages()
        {
            if (_showMode == ShowMode.All)
                return Log.messageQueue.Messages;

            if (_showMode == ShowMode.Messages)
                return Log.messageQueue.Messages.Where(m => m.type == LogMessageType.Message);
            else if (_showMode == ShowMode.Warnings)
                return Log.messageQueue.Messages.Where(m => m.type == LogMessageType.Warning);
            else if (_showMode == ShowMode.Errors)
                return Log.messageQueue.Messages.Where(m => m.type == LogMessageType.Error);
            return Enumerable.Empty<LogMessage>();
        }
    }
}