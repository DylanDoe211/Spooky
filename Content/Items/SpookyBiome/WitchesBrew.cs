using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Projectiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class WitchesBrew : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.autoReuse = true; 
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.width = 18;
            Item.height = 26;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<WitchesBrewProj>();
            Item.shootSpeed = 10f;
        }
    }
}