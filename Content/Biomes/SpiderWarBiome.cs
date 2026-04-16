using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpiderWarBiome : ModBiome
    {
		public override int Music => MusicID.Boss3;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        //bestiary stuff
		public override string BestiaryIcon => "Spooky/Content/Biomes/SpiderWarBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            return SpiderWarWorld.SpiderWarActive && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>());
        }
	}
}