using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class BustlingGlowshroom : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<BustlingGlowshroomPlayer>().BustlingGlowshroom = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 15)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }

    public class BustlingGlowshroomPlayer : ModPlayer
    {
        public bool BustlingGlowshroom = false;
        public int BustlingHealTimer = 0;

        public override void ResetEffects()
        {
            BustlingGlowshroom = false;
        }

        public override void PreUpdate()
        {
            if (Player.velocity == Vector2.Zero && BustlingGlowshroom)
            {
                BustlingHealTimer++;

                //dont heal the player until after they are standing still for long enough
                if (BustlingHealTimer >= 60)
                {
                    Player.AddBuff(ModContent.BuffType<BustlingGlowshroomHeal>(), 2);
                }
            }
            else
            {
                //reset the time if you move at all
                BustlingHealTimer = 0;
            }
        }
    }
}