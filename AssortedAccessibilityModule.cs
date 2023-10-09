using System;
using Mono.Cecil.Cil;
using Monocle;
using MonoMod.Cil;

namespace Celeste.Mod.AssortedAccessibility; 

// ReSharper disable once UnusedType.Global
public class AssortedAccessibilityModule : EverestModule {
    // ReSharper disable once MemberCanBePrivate.Global
    public static AssortedAccessibilityModule Instance { get; private set; }

    public override Type SettingsType => typeof(AssortedAccessibilityModuleSettings);
    public static AssortedAccessibilityModuleSettings Settings => (AssortedAccessibilityModuleSettings) Instance._Settings;

    public AssortedAccessibilityModule() {
        Logger.SetLogLevel("AssAcc", LogLevel.Debug);
        Instance = this;
    }

    public override void Load() {
        On.Celeste.DustStyles.Get_Session += DustStylesOnGet;
        IL.Celeste.Level.Render += LevelOnRender;
    }

    public override void Unload() {
        On.Celeste.DustStyles.Get_Session -= DustStylesOnGet;
        IL.Celeste.Level.Render -= LevelOnRender;
    }

    private static void LevelOnRender(ILContext il) {
        var cursor = new ILCursor(il);

        if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdfld<Level>("Wipe"), instr => instr.MatchBrfalse(out _))) {
            cursor.Emit(OpCodes.Call, typeof(AssortedAccessibilityModule).GetProperty(nameof(Settings))!.GetGetMethod());
            cursor.Emit(OpCodes.Callvirt, typeof(AssortedAccessibilityModuleSettings).GetProperty(nameof(AssortedAccessibilityModuleSettings.DisableScreenWipes))!.GetGetMethod());
            ILLabel label = cursor.DefineLabel();
            cursor.Emit(OpCodes.Brtrue, label);
            cursor.GotoNext(instr => instr.MatchLdarg(0), instr => instr.MatchLdfld<Level>("HiresSnow"));
            cursor.MarkLabel(label);
        } else {
            Logger.Log(LogLevel.Error, "AssAcc", "Failed IL hooking Level.Render to hide screen wipes");
        }
    }
    
    private static DustStyles.DustStyle DustStylesOnGet(On.Celeste.DustStyles.orig_Get_Session orig, Session session) {
        DustStyles.DustStyle origDustStyle = orig(session);
        if (!Settings.ChangeDustSpriteColors) {
            return origDustStyle;
        }
        return new DustStyles.DustStyle {
            EdgeColors = new[] {
                Calc.HexToColor(Settings.DustEdgeColor1).ToVector3(),
                Calc.HexToColor(Settings.DustEdgeColor2).ToVector3(),
                Calc.HexToColor(Settings.DustEdgeColor3).ToVector3()
            },
            EyeColor = Calc.HexToColor(Settings.DustEyeColor),
            EyeTextures = origDustStyle.EyeTextures
        };
    }
}