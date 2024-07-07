using MelonLoader;

namespace MiseryInCustomGames
{
	public class Mod : MelonMod
	{
		public override void OnInitializeMelon()
		{
			base.OnInitializeMelon();

			Settings.OnLoad();
		}
	}
}
