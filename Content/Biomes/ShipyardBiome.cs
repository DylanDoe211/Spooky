using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.Shipyard;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class ShipyardBiome : ModBiome
    {
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<ShipyardBG>();

		//set the music to be consistent with vanilla's music priorities
		public override int Music
		{
			get
			{
				int music = Main.curMusic;

				if (!Main.bloodMoon && !Main.eclipse)
				{
					//play town music if enough town npcs exist
					if (Main.LocalPlayer.townNPCs > 2f)
					{
						if (Main.dayTime)
						{
							music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownDay");
						}
						else
						{
							music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownNight");
						}
					}
					//play normal music
					else
					{
						music = MusicID.Eerie;
					}
				}
				//blood moon theme takes priority over everything
				else
				{
					if (Main.bloodMoon)
					{
						music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBloodmoon");
					}

					if (Main.eclipse)
					{
						music = MusicID.Eclipse;
					}
				}

				return music;
			}
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<ShipyardWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<CemeteryBiomeTorchItem>();

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CemeteryBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            isActive = player.InModBiome<ShipyardBiome>() && !player.InModBiome(ModContent.GetInstance<RaveyardBiome>()) &&
            !player.InModBiome(ModContent.GetInstance<CatacombBiome>()) && !player.InModBiome(ModContent.GetInstance<CatacombBiome2>());
            player.ManageSpecialBiomeVisuals("Spooky:ShipyardSky", isActive, player.Center);
        }

		public override void OnInBiome(Player player)
        {
            //graveyard visuals
            if (!player.InModBiome(ModContent.GetInstance<CatacombBiome>()) && !player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
            {
                player.ZoneGraveyard = true;
                Main.GraveyardVisualIntensity = 0.1f;
            }
        }

		public bool InOcean(int x, int y)
		{
			if ((double)y > WorldGen.oceanLevel)
			{
				return false;
			}
			if (x < (WorldGen.beachDistance - 45) || x > Main.maxTilesX - (WorldGen.beachDistance - 45))
			{
				return true;
			}
			
			return false;
		}

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
			int PlayerX = (int)player.Center.X / 16;
			int PlayerY = (int)player.Center.Y / 16;

            bool BiomeCondition = ModContent.GetInstance<TileCount>().shipyardTiles >= 700;
            bool TileComparison = ModContent.GetInstance<TileCount>().shipyardTiles > ModContent.GetInstance<TileCount>().cemeteryTiles;
            bool SurfaceCondition = player.ZoneOverworldHeight;
			bool NotInBeach = !InOcean(PlayerX, PlayerY);

            return BiomeCondition && SurfaceCondition && TileComparison && NotInBeach;
        }
    }
}