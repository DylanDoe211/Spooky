using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class GeminiEntertainmentGame : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 42;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 10);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<GeminiEntertainmentGamePlayer>().GeminiEntertainmentGame = true;
			player.bleed = true;
        }
    }

    public class GeminiEntertainmentGamePlayer : ModPlayer
    {
        public bool GeminiEntertainmentGame = false;

        public override void ResetEffects()
        {
            GeminiEntertainmentGame = false;
        }

        public override void PreUpdate()
        {
            if (GeminiEntertainmentGame && Player.ownedProjectileCounts[ModContent.ProjectileType<NaturesMockery>()] < 1)
            {
                Vector2 center = new Vector2(Player.Center.X, Player.Center.Y + Player.height / 4);
                center.X += Main.rand.Next(-125, 126);
                int numtries = 0;
                int x = (int)(center.X / 16);
                int y = (int)(center.Y / 16);
                while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) &&
                Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
                {
                    y++;
                    center.Y = y * 16;
                }
                while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
                {
                    numtries++;
                    y--;
                    center.Y = y * 16;
                }

                int NewProj = Projectile.NewProjectile(null, center.X, center.Y, 0, -0.3f, ModContent.ProjectileType<NaturesMockery>(), 0, 0, Player.whoAmI, 0, 0, 4);
                Main.projectile[NewProj].frame = Player.GetModPlayer<AnalogHorrorTapePlayer>().AnalogHorrorTape ? 4 : Main.rand.Next(0, 4);
            }
        }
    }
}