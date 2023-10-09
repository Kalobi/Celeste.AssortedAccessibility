// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Celeste.Mod.AssortedAccessibility; 

public class AssortedAccessibilityModuleSettings : EverestModuleSettings {
    [SettingName("KALOBI_ASSACC_DISABLESCREENWIPES")]
    public bool DisableScreenWipes { get; set; }

    public bool ChangeDustSpriteColors { get; set; }
    
    public string DustEdgeColor1 { get; set; } = "ffffff";
    public string DustEdgeColor2 { get; set; } = "ffffff";
    public string DustEdgeColor3 { get; set; } = "ffffff";
    public string DustEyeColor { get; set; } = "ffffff";
}