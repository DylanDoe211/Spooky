using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Tiles.Shipyard.Furniture;

namespace Spooky.Content.Projectiles.Shipyard
{
    public class PirateChestVisuals : ModNPC  
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        float BeamScale = 0f;

        private static Asset<Texture2D> SpotlightTexture;

        public static readonly SoundStyle OpenSound = new("Spooky/Content/Sounds/PirateChestOpen", SoundType.Sound);
        public static readonly SoundStyle BreakSound = new("Spooky/Content/Sounds/WoodBreaking", SoundType.Sound);

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 20;
			NPC.height = 20;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpotlightTexture ??= ModContent.Request<Texture2D>("Spooky/Effects/LightConeUp");

            Vector2 frameOrigin = new Vector2(SpotlightTexture.Width() / 2f, SpotlightTexture.Height());
            Vector2 pos = NPC.Center - screenPos;

            float Rotation = Main.GlobalTimeWrappedHourly * 1.5f;

            float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 1f;
            DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, pos, Color.Gold, Color.Gold, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2((0.4f * BeamScale * 2) * time, (0.3f * BeamScale * 2) * time), new Vector2(5, 5));

            Main.EntitySpriteDraw(SpotlightTexture.Value, pos, null, new Color(Color.Gold.R, Color.Gold.G, Color.Gold.B, 0), MathF.Sin(Rotation), frameOrigin, new Vector2(BeamScale * 0.5f, BeamScale), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(SpotlightTexture.Value, pos, null, new Color(Color.Gold.R, Color.Gold.G, Color.Gold.B, 0), MathF.Sin(-Rotation), frameOrigin, new Vector2(BeamScale * 0.5f, BeamScale), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(SpotlightTexture.Value, pos, null, new Color(Color.Gold.R, Color.Gold.G, Color.Gold.B, 0), MathF.Cos(Rotation), frameOrigin, new Vector2(BeamScale * 0.5f, BeamScale), SpriteEffects.None, 0);

            return false;
        }

        public void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;

            for (int i = 0; i < 360; i += 60)
			{
                Vector2 circular = new Vector2(Main.rand.NextFloat(0f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

                Color RealColor = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(color);
                Color RealColor2 = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(color);

                Main.EntitySpriteDraw(Texture, drawpos + circular, null, RealColor, (float)Math.PI / 2f + rotation, origin, vector, dir);
                Main.EntitySpriteDraw(Texture, drawpos + circular, null, RealColor, 0f + rotation, origin, vector2, dir);
            }
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

        public override void AI()
        {
            NPC.ai[0]++;

            Tile tile = Framing.GetTileSafely((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16);

            if (NPC.ai[0] % 6 == 0 && tile.TileFrameY < 270 && tile.TileType == ModContent.TileType<GiantPirateChest>())
            {
                int left = (int)(NPC.Center.X / 16) - tile.TileFrameX / 18 % 3;
				int top = (int)(NPC.Center.Y / 16) - tile.TileFrameY / 18 % 3;

				for (int x = left; x < left + 3; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						Tile CheckTile = Framing.GetTileSafely(x, y);
						CheckTile.TileFrameY += 54;
                    }
                }
            }

			Lighting.AddLight(NPC.position, 0.6f * BeamScale, 0.5f * BeamScale, 0.4f * BeamScale);

            if (NPC.ai[0] == 1)
            {
                SoundEngine.PlaySound(OpenSound, NPC.Center);
            }

            if (NPC.ai[0] >= 60 && NPC.ai[0] < 130)
            {
                if (BeamScale < 0.5f)
                {
                    BeamScale += 0.01f;
                }
            }

            if (NPC.ai[0] == 130)
            {
                //int Pylon = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<KrampusPylonItem>());
                //if (Main.netMode == NetmodeID.Server)
                //{
                    //NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Pylon, 1f);
                //}
            }
            
            if (NPC.ai[0] >= 160)
            {
                if (BeamScale > 0f)
                {
                    BeamScale -= 0.015f;
                }
            }

            //kill tile
            if (NPC.ai[0] >= 200)
            {
                if (tile.TileType == ModContent.TileType<GiantPirateChest>())
                {
                    SoundEngine.PlaySound(BreakSound, NPC.Center);

                    WorldGen.KillTile((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, fail: false);

                    if (Main.netMode != NetmodeID.Server)
                    {
                        for (int numGores = 1; numGores <= 15; numGores++)
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -4).RotatedByRandom(360), GoreID.ShadowMimicCoins);
                        }
                    }
                }

                NPC.active = false;
            }
        }
    }
}