using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Achievements;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class Creepypasta : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CreepyPastaPlayer>().CreepyPasta = true;
            player.GetModPlayer<PolybiusArcadeGamePlayer>().PolybiusArcadeGame = true;
            player.GetModPlayer<SmileDogPicturePlayer>().SmileDogPicture = true;
            player.GetModPlayer<RedMistClarinetPlayer>().RedMistClarinet = true;
            player.GetModPlayer<SlendermanPagePlayer>().SlendermanPage = true;
            player.GetModPlayer<RedGodzillaCartridgePlayer>().RedGodzillaCartridge = true;
            player.GetModPlayer<HerobrineAltarPlayer>().HerobrineAltar = true;

            //spawn polybius swirl
            bool SwirlNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<PolybiusSwirl>()] <= 0;
			if (SwirlNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, Main.MouseWorld.X, Main.MouseWorld.Y, 0f, 0f, ModContent.ProjectileType<PolybiusSwirl>(), 100, 0f, player.whoAmI);
			}

            //spawn smile dog
            bool DogNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<SmilingDog>()] <= 0;
			if (DogNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<SmilingDog>(), 150, 2f, player.whoAmI);
			}

            //spawn slenderman tentacles
			if (player.ownedProjectileCounts[ModContent.ProjectileType<SlendermanTentacle>()] < 4 && Main.myPlayer == player.whoAmI)
            {
                bool[] spawnedTentacle = new bool[4];
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile projectile = Main.projectile[i];
                    if (projectile.active && projectile.type == ModContent.ProjectileType<SlendermanTentacle>() && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 4f)
                    {
                        spawnedTentacle[(int)projectile.ai[1]] = true;
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (!spawnedTentacle[i])
                    {
                        Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                        Projectile.NewProjectile(null, player.Center, vel, ModContent.ProjectileType<SlendermanTentacle>(), 100, 0f, Main.myPlayer, Main.rand.Next(120), i + 3);
                    }
                }
            }
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PolybiusArcadeGame>())
            .AddIngredient(ModContent.ItemType<SmileDogPicture>())
            .AddIngredient(ModContent.ItemType<RedMistClarinet>())
            .AddIngredient(ModContent.ItemType<SlendermanPage>())
            .AddIngredient(ModContent.ItemType<RedGodzillaCartridge>())
            .AddIngredient(ModContent.ItemType<HerobrineAltar>())
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }

    public class CreepyPastaPlayer : ModPlayer
    {
        public bool CreepyPasta = false;

        public override void ResetEffects()
        {
            CreepyPasta = false;
        }
    }
}