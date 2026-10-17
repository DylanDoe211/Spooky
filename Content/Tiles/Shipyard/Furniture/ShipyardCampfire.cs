using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Spooky.Content.Tiles.Shipyard.Furniture
{
	public class ShipyardCampfire : ModTile
	{
		public const int FrameCount = 9;

		public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.Campfire[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.StyleLineSkip = 9;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(254, 121, 2), Lang.GetItemName(ItemID.Campfire));
            AnimationFrameHeight = 36;
			DustType = -1;
			AdjTiles = new int[] { TileID.Campfire };
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return true;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frame = Main.tileFrame[TileID.Campfire];
			frameCounter = Main.tileFrameCounter[TileID.Campfire];
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Tile tile = Main.tile[i, j];
			if (!closer && tile.TileFrameY < AnimationFrameHeight)
			{
				Main.SceneMetrics.HasCampfire = true;
			}
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY < AnimationFrameHeight)
			{
				frameYOffset = Main.tileFrame[Type] * AnimationFrameHeight;
			}
			else
			{
				frameYOffset = (FrameCount - 2) * AnimationFrameHeight;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY < AnimationFrameHeight)
			{
				float divide = 300f;

                r = 64f / divide;
                g = 172f / divide;
                b = 207f / divide;
			}
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameY < AnimationFrameHeight && tile.TileFrameY == 0)
			{
				if (!Main.gamePaused && Main.instance.IsActive && (!Lighting.UpdateEveryFrame || Main.rand.NextBool(4)))
                {
                    if ((!Main.drawToScreen || Main.rand.NextBool(4)) && Main.rand.NextBool(3))
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(i * 16 + 2, j * 16 - 4), 4, 8, DustID.Smoke, 0f, 0f, 100);
                        if (tile.TileFrameX == 0)
                        {
                            dust.position.X += Main.rand.Next(8);
                        }

                        if (tile.TileFrameX == 36)
                        {
                            dust.position.X -= Main.rand.Next(8);
                        }

                        dust.alpha += Main.rand.Next(100);
                        dust.velocity *= 0.2f;
                        dust.velocity.Y -= 0.5f + Main.rand.Next(10) * 0.1f;
                        dust.fadeIn = 0.5f + Main.rand.Next(10) * 0.1f;
                    }
                }
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			if (!TileDrawing.IsVisible(tile))
			{
				return;
			}

			if (tile.TileFrameY >= AnimationFrameHeight)
			{
				return;
			}

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			int height = 16;

			int offsetX = 0;
			int offsetY = 0;
			TileLoader.SetAnimationFrame(Type, i, j, ref offsetX, ref offsetY);

			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "Glow").Value, new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y + 2) + zero, new Rectangle(tile.TileFrameX + offsetX, tile.TileFrameY + offsetY, 16, height), Color.White * 0.75f, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<ShipyardCampfireItem>();
		}

		public override bool RightClick(int i, int j)
		{
			SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
			HitWire(i, j);
			return true;
		}

		public override void HitWire(int i, int j)
		{
			int x = i - Main.tile[i, j].TileFrameX / 18 % 3;
			int y = j - Main.tile[i, j].TileFrameY / 18 % 2;
			int change = AnimationFrameHeight;
			if (Main.tile[x, y].TileFrameY >= AnimationFrameHeight)
			{
				change = -AnimationFrameHeight;
			}

			Tile tile;
			for (int l = x; l < x + 3; l++)
			{
				for (int m = y; m < y + 2; m++)
				{
					tile = Framing.GetTileSafely(l, m);
					if (tile.HasTile && tile.TileType == Type)
					{
						tile.TileFrameY = (short)(tile.TileFrameY + change);
					}
				}
			}

			if (Wiring.running)
			{
				Wiring.SkipWire(x, y);
				Wiring.SkipWire(x, y + 1);
				Wiring.SkipWire(x + 1, y);
				Wiring.SkipWire(x + 1, y + 1);
				Wiring.SkipWire(x + 2, y);
				Wiring.SkipWire(x + 2, y + 1);
			}
            
			NetMessage.SendTileSquare(-1, x + 1, y, 3);
		}
	}
}