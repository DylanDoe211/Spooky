using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [LegacyName("BoneMask")]
    public class FlowerPotShoe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 44;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<FlowerPotShoePlayer>().FlowerPotShoe = true;
        }
    }

    public class FlowerPotShoePlayer : ModPlayer
    {
        public bool FlowerPotShoe = false;
        public int FlowerPotShoeTimer = 0;

        public override void ResetEffects()
        {
            FlowerPotShoe = false;
        }

        public override void PreUpdate()
        {
            //shoot skulls with big bones expert item
            if (FlowerPotShoe && !Player.dead)
            {
                //do not shoot skulls under 20mph (basically if you are not moving fast enough)
                if (SpookyPlayer.PlayerSpeedToMPH(Player) >= 20)
                {
                    FlowerPotShoeTimer++;
                    if (FlowerPotShoeTimer >= 180 / (SpookyPlayer.PlayerSpeedToMPH(Player) / 10))
                    {
                        SoundEngine.PlaySound(SoundID.Item8, Player.Center);

                        Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                        Vector2 newVelocity = Player.velocity.Y == 0 ? new Vector2(Speed.X, Main.rand.Next(-10, -3)) : Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));

                        //scale the damage based on the player's current speed
                        int damage = 80 + ((int)SpookyPlayer.PlayerSpeedToMPH(Player) / 3);

                        int newProj = Projectile.NewProjectile(null, Player.Center.X, Player.Center.Y, newVelocity.X, newVelocity.Y,
                        ModContent.ProjectileType<FlowerPotShoeFlower>(), damage, 0f, Player.whoAmI);
                        Main.projectile[newProj].frame = Main.rand.Next(0, 4);

                        FlowerPotShoeTimer = 0;
                    }
                }
                else
                {
                    FlowerPotShoeTimer = 0;
                }
            }
            else
            {
                FlowerPotShoeTimer = 0;
            }
        }

        public override void PostUpdateRunSpeeds()
        {
			if (FlowerPotShoe)
			{
				Player.maxRunSpeed += 7f;
				Player.runAcceleration += 0.075f;
			}
        }
    }
}