using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Projectiles.Cemetery;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class AnalogHorrorTape : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = Item.buyPrice(platinum: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AnalogHorrorTapePlayer>().AnalogHorrorTape = true;
            player.GetModPlayer<GeminiEntertainmentGamePlayer>().GeminiEntertainmentGame = true;
            player.GetModPlayer<MandelaCatalogueTVPlayer>().MandelaCatalogueTV = true;
            player.GetModPlayer<CarnisFlavorEnhancerPlayer>().CarnisFlavorEnhancer = true;
            player.GetModPlayer<BackroomsCorpsePlayer>().BackroomsCorpse = true;
            player.GetModPlayer<Local58TelescopePlayer>().Local58Telescope = true;

            //monument mythos pyramid defense
            if (!player.HasBuff(ModContent.BuffType<MonumentMythosCooldown>()))
            {
                player.GetModPlayer<MonumentMythosPyramidPlayer>().MonumentMythosPyramid = true;
                player.endurance += 0.35f;
            }

            //spawn orbiting moon
            bool MoonNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<Local58Moon>()] <= 0;
			if (MoonNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<Local58Moon>(), 200, 5f, player.whoAmI);
			}

            //spawn decay head
            bool HeadNotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<BackroomsCorpseHead>()] <= 0;
			if (HeadNotSpawned && player.whoAmI == Main.myPlayer)
			{
				Projectile.NewProjectile(null, player.Center.X, player.Center.Y, 0f, 0f, ModContent.ProjectileType<BackroomsCorpseHead>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GeminiEntertainmentGame>())
            .AddIngredient(ModContent.ItemType<MandelaCatalogueTV>())
            .AddIngredient(ModContent.ItemType<CarnisFlavorEnhancer>())
            .AddIngredient(ModContent.ItemType<BackroomsCorpse>())
            .AddIngredient(ModContent.ItemType<Local58Telescope>())
            .AddIngredient(ModContent.ItemType<MonumentMythosPyramid>())
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }

    public class AnalogHorrorTapePlayer : ModPlayer
    {
        public bool AnalogHorrorTape = false;

        public override void ResetEffects()
        {
            AnalogHorrorTape = false;
        }
    }
}