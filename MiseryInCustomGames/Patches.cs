using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Gameplay;

namespace MiseryInCustomGames
{

	public static class AfflictionManager
	{
		public static void ApplyAfflictions()
		{
			GameManager.m_MiseryManager.m_IsActiveTagFound = true;

			ConsoleManager.CONSOLE_brokenbody();
			ConsoleManager.CONSOLE_poorcirculation();
			ConsoleManager.CONSOLE_sourstomach();
			ConsoleManager.CONSOLE_unsettledsleep();
			ConsoleManager.CONSOLE_weakconstitution();
			ConsoleManager.CONSOLE_weakjoints();
		}
	}

	[HarmonyPatch(typeof(PlayerManager), "TeleportPlayerAfterSceneLoad")]
	internal class InitMiseryPatch
	{
		private static bool _isInitialized = false;

		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (Settings.ModSettings.MiseryType == MiseryType.Vanilla)
			{
				if (_isInitialized)
					return;

				var prevInit = Settings.DataManager.Load(Settings.MISERY_INIT_FIELD_NAME);

				if (!string.IsNullOrEmpty(prevInit))
				{
					if (bool.Parse(prevInit))
					{
						// reset previously initialized misery state after load
						_isInitialized = true;
						return;
					}
				}

				GameManager.m_MiseryManager.RollMiseryStages();
				GameManager.m_MiseryManager.m_IsActiveTagFound = true;
				GameManager.m_MiseryManager.m_NextStageHours = 24;

				_isInitialized = true;
			}
			else if (Settings.ModSettings.MiseryType == MiseryType.OnSpawn)
			{
				// maybe check if afflictions already applied
				AfflictionManager.ApplyAfflictions();
			}
		}
	}

	[HarmonyPatch(typeof(MiseryManager), "Update")]
	internal class MiseryManagerUpdatePatch
	{
		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			var miseryType = Settings.GetMiseryTypeFromModdata();

			if (miseryType == null || miseryType == MiseryType.Disabled)
				return;

			if (GameManager.m_MiseryManager.m_IsActiveTagFound)
				return;

			if (miseryType == MiseryType.Vanilla)
			{
				GameManager.m_MiseryManager.m_IsActiveTagFound = true;
			}
			else if (miseryType == MiseryType.OnSpawn)
			{
				AfflictionManager.ApplyAfflictions();
			}
		}
	}

	[HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen")]
	internal class GameManagerLoadSceneWithLoadingScreenPatch
	{
		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			var miseryType = Settings.GetMiseryTypeFromModdata();

			if (miseryType == null ||
				miseryType == MiseryType.Disabled)
				return;

			if (miseryType == MiseryType.Vanilla)
			{
				GameManager.m_MiseryManager.m_IsActiveTagFound = true;

				if (GameManager.m_MiseryManager.m_NextStageHours <= 1)
				{
					var hoursValue = Settings.DataManager.Load(Settings.MISERY_HOURS_NEXT_FIELD_NAME);

					if (string.IsNullOrEmpty(hoursValue))
						return;

					var hours = int.Parse(hoursValue);
					GameManager.m_MiseryManager.m_NextStageHours = hours;
				}
			}
			else if (miseryType == MiseryType.OnSpawn)
			{
				AfflictionManager.ApplyAfflictions();
			}
		}
	}


	[HarmonyPatch(typeof(SaveGameSlots), "SaveDataToSlot")]
	internal class SaveGameSlotsSaveDataToSlotPatch
	{
		internal static void Postfix()
		{
			// rewrite!!

			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			var miseryType = Settings.GetMiseryTypeFromModdata();

			if (miseryType == null)
			{
				// first save after spawn
				Settings.DataManager.Save(Settings.ModSettings.MiseryType.ToString(),
					Settings.MISERY_TYPE_FIELD_NAME);

				miseryType = Settings.ModSettings.MiseryType;
			}

			if (miseryType == MiseryType.Disabled)
				return;

			if (miseryType == MiseryType.Vanilla)
			{
				if (GameManager.m_MiseryManager.m_NextStageHours > 0)
				{
					Settings.DataManager.Save($"{GameManager.m_MiseryManager.m_NextStageHours}",
						Settings.MISERY_HOURS_NEXT_FIELD_NAME);
				}

				Settings.DataManager.Save("true", Settings.MISERY_INIT_FIELD_NAME);
			}
		}
	}

}