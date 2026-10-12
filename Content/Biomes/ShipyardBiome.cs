using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Backgrounds.Shipyard;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class ShipyardBiome : ModBiome
    {
        /*
		public static Asset<Effect> SporeMist;
		public static Asset<Texture2D> NoiseTexture;

		public override void Load()
        {
			On_Main.DrawInfernoRings += SporeFogDraw;

			SporeMist = ModContent.Request<Effect>("Spooky/Effects/MoldMist");
			NoiseTexture = ModContent.Request<Texture2D>("Spooky/Effects/noiseFog");
		}

        float FogAlpha = 0f;

		private void SporeFogDraw(On_Main.orig_DrawInfernoRings orig, Main self)
		{
			orig(self);

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<ShipyardBiome>()))
			{
				if (FogAlpha < 1f)
				{
					FogAlpha += 0.01f;
				}
			}
			else
			{
				if (FogAlpha > 0f)
				{
					FogAlpha -= 0.1f;
				}
			}

			if (FogAlpha > 0f)
			{
                Effect sporeEffect = SporeMist.Value;

				void DrawFog(Texture2D tex, Vector2 offset, bool usePosOffset = false)
				{
					for (int i = -1; i < 2; i += 2)
					{
						for (int j = -1; j < 2; j += 2)
						{
							Vector2 origPosOffset = usePosOffset ? new Vector2(offset.X, offset.Y) : Vector2.Zero;

							Vector2 origPos = Main.LocalPlayer.Center - origPosOffset - new Vector2(Main.screenWidth * i, Main.screenHeight * j) * 0.5f;

							Vector2 pos = new Vector2(MathF.Floor(origPos.X / (Main.screenWidth)), MathF.Floor(origPos.Y / (Main.screenHeight))) * new Vector2(Main.screenWidth, Main.screenHeight) - Main.screenPosition + offset;

							Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, Main.screenWidth, Main.screenHeight);
							if (!rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight)))
							{
								continue;
							}

							Main.spriteBatch.Draw(tex, rect, Color.White);
						}
					}
				}

                ResetState();

				sporeEffect.Parameters["uOpacityTotal"].SetValue(1.2f * FogAlpha);
				sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 60);
				sporeEffect.Parameters["uColor"].SetValue(Color.White.ToVector4());
				sporeEffect.Parameters["uExponent"].SetValue(3f);
				sporeEffect.Parameters["uScale"].SetValue(0.2f);
				DrawFog(NoiseTexture.Value, new Vector2(Main.screenWidth * 3f, Main.screenHeight * 3f) * 0.125f, true);
			}

            return;

            void ResetState()
            {
                Effect sporeEffect = SporeMist.Value;
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, (Effect)sporeEffect, Main.GameViewMatrix.TransformationMatrix);
                
                for (int i = 0; i < 4; i++)
                    Main.graphics.GraphicsDevice.SamplerStates[i] = SamplerState.LinearWrap;
                
                Main.graphics.GraphicsDevice.Textures[1] = NoiseTexture.Value;
            }
        }
        */

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
                        if (Main.dayTime)
                        {
                            music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
                        }
                        else
                        {
                            music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/CemeteryNight");
                        }
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
        public override string BestiaryIcon => "Spooky/Content/Biomes/ShipyardBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:ShipyardSky", isActive, player.Center);
        }

		public override void OnInBiome(Player player)
        {
            //graveyard visuals
            if (!player.InModBiome(ModContent.GetInstance<CatacombBiome>()) && !player.InModBiome(ModContent.GetInstance<CatacombBiome2>()))
            {
                player.ZoneGraveyard = true;
                Main.GraveyardVisualIntensity = 0.05f;
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