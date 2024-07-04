using HarmonyLib;
using Il2Cpp;

namespace MiseryInCustomGames
{
	[HarmonyPatch(typeof(PlayerManager), "PickRandomSpawnPoint")]
	internal class InitMiseryPatch
	{

		internal static void Postfix(GameManager __instance)
		{
			if (ExperienceModeManager.GetCurrentExperienceModeType() != ExperienceModeType.Custom)
				return;

			if (!Settings.ModSettings.EnableMisery)
				return;

			if (GameManager.m_MiseryManager == null)
				return;

			GameManager.m_MiseryManager.RollMiseryStages();
			GameManager.m_MiseryManager.m_IsActiveTagFound = true;
			GameManager.m_MiseryManager.m_NextStageHours = 24;
		}
	}
}