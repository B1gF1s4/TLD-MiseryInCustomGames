using ModSettings;

namespace MiseryInCustomGames
{
	public static class Settings
	{

		internal static readonly ModSettings ModSettings = new();

		public static void OnLoad()
		{
			ModSettings.AddToCustomModeMenu(Position.BelowHealth);
		}
	}

	public class ModSettings : ModSettingsBase
	{
		[Name("Misery Afflictions")]
		[Description("Enable Misery Afflictions")]
		public bool EnableMisery = false;
	}
}
