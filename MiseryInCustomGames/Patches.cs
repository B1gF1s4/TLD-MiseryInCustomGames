using HarmonyLib;
using Il2Cpp;
using Il2CppTLD.Gameplay;

namespace MiseryInCustomGames
{

	[HarmonyPatch(typeof(PlayerManager), "PickRandomSpawnPoint")]
	internal class InitMiseryPatch
	{
		private static bool _isInitialized = false;

		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (!Settings.ModSettings.EnableMisery)
				return;

			if (GameManager.m_MiseryManager == null)
				return;

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
	}

	[HarmonyPatch(typeof(MiseryManager), "Update")]
	internal class MiseryManagerUpdatePatch
	{
		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (!Settings.IsMiseryEnabledInModdata() &&
				Settings.ModSettings.EnableMisery)
				return;

			if (GameManager.m_MiseryManager == null)
				return;

			if (GameManager.m_MiseryManager.m_IsActiveTagFound)
				return;

			GameManager.m_MiseryManager.m_IsActiveTagFound = true;
		}
	}

	[HarmonyPatch(typeof(GameManager), "LoadSceneWithLoadingScreen")]
	internal class GameManagerLoadSceneWithLoadingScreenPatch
	{
		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (!Settings.IsMiseryEnabledInModdata())
				return;

			if (GameManager.m_MiseryManager == null)
				return;

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
	}

	[HarmonyPatch(typeof(SaveGameSlots), "SaveDataToSlot")]
	internal class SaveGameSlotsSaveDataToSlotPatch
	{
		internal static void Postfix()
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (!Settings.IsMiseryEnabledInModdata() &&
				!Settings.ModSettings.EnableMisery)
				return;

			if (GameManager.m_MiseryManager == null)
				return;

			if (GameManager.m_MiseryManager.m_NextStageHours > 0)
			{
				Settings.DataManager.Save($"{GameManager.m_MiseryManager.m_NextStageHours}",
					Settings.MISERY_HOURS_NEXT_FIELD_NAME);
			}

			Settings.DataManager.Save("true", Settings.MISERY_ENABLED_FIELD_NAME);
			Settings.DataManager.Save("true", Settings.MISERY_INIT_FIELD_NAME);
		}
	}

}