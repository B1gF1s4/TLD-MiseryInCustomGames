using ModData;
using ModSettings;

namespace MiseryInCustomGames
{
	public static class Settings
	{

		public const string MISERY_ENABLED_FIELD_NAME = "miseryAfflictionsEnabled";
		public const string MISERY_INIT_FIELD_NAME = "miseryAfflictionsInitialized";
		public const string MISERY_HOURS_NEXT_FIELD_NAME = "miseryAfflictionsHourseNext";

		internal static readonly ModSettings ModSettings = new();
		internal static readonly ModDataManager DataManager = new(nameof(MiseryInCustomGames), false);

		public static void OnLoad()
		{
			ModSettings.AddToCustomModeMenu(Position.BelowHealth);
		}

		public static bool IsMiseryEnabledInModdata()
		{
			var setting = DataManager.Load(MISERY_ENABLED_FIELD_NAME);

			if (string.IsNullOrEmpty(setting))
				return false;

			return bool.Parse(setting);
		}
	}

	public class ModSettings : ModSettingsBase
	{
		[Name("Misery Afflictions")]
		[Description("Enable Misery Afflictions")]
		public bool EnableMisery = false;

	}
}
