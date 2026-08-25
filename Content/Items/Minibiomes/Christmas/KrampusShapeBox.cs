using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class KrampusShapeBox : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<KrampusShapeBoxPlayer>().KrampusShapeBox = true;
        }
    }

    public class KrampusShapeBoxPlayer : ModPlayer
    {
		public bool KrampusShapeBox = false;
        public int KrampusShapeBuffStacks = 0;

		public override void ResetEffects()
        {
			KrampusShapeBox = false;
		}

        public override void PreUpdate()
        {   
            if (!KrampusShapeBox || !Player.HasBuff(ModContent.BuffType<KrampusShapeBuff>()))
            {
                KrampusShapeBuffStacks = 0;
            }

            if (KrampusShapeBox && KrampusShapeBuffStacks < 5 && Player.ownedProjectileCounts[ModContent.ProjectileType<ShapeBoxProj>()] <= 0)
            {
                if (Main.rand.NextBool(225))
                {
                    float PositionX = Player.Center.X + Main.rand.Next(-250, 251);
                    float PositionY = Player.Center.Y - 450;

                    int FrameToUse = 0;
                    if (KrampusShapeBuffStacks == 1)
                    {
                        FrameToUse = 1;
                    }
                    if (KrampusShapeBuffStacks == 2)
                    {
                        FrameToUse = 2;
                    }
                    if (KrampusShapeBuffStacks == 3)
                    {
                        FrameToUse = 3;
                    }
                    if (KrampusShapeBuffStacks == 4) 
                    {
                        FrameToUse = 4;
                    }

                    Projectile.NewProjectile(null, new Vector2(PositionX, PositionY), Vector2.Zero, ModContent.ProjectileType<ShapeBoxProj>(), 0, 0, Player.whoAmI, ai1: FrameToUse);
                }
            }
        }
    }
}