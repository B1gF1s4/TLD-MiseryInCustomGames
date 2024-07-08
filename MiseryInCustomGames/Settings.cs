using ModData;
using ModSettings;

namespace MiseryInCustomGames
{

	public enum MiseryType
	{
		Disabled,
		Vanilla,
		OnSpawn
	}

	public static class Settings
	{
		public const string MISERY_TYPE_FIELD_NAME = "miseryType";
		public const string MISERY_INIT_FIELD_NAME = "miseryAfflictionsInitialized";
		public const string MISERY_HOURS_NEXT_FIELD_NAME = "miseryAfflictionsHourseNext";

		internal static readonly ModSettings ModSettings = new();
		internal static readonly ModDataManager DataManager = new(nameof(MiseryInCustomGames), false);

		public static void OnLoad()
		{
			ModSettings.AddToCustomModeMenu(Position.BelowHealth);
		}

		public static MiseryType? GetMiseryTypeFromModdata()
		{
			var value = DataManager.Load(MISERY_TYPE_FIELD_NAME);

			if (string.IsNullOrEmpty(value))
				return null;

			if (Enum.TryParse(typeof(MiseryType), value, true, out var miseryType))
			{
				if (miseryType == null)
					return MiseryType.Disabled;

				return (MiseryType)miseryType;
			}

			return MiseryType.Disabled;
		}
	}

	public class ModSettings : ModSettingsBase
	{
		[Name("Misery Afflictions")]
		[Description("Vanilla: regular timeline, OnSpawn: all afflictions from the start")]
		public MiseryType MiseryType = MiseryType.Disabled;
	}
}
