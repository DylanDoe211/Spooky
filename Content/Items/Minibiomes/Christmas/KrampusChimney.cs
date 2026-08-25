using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Christmas;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    [AutoloadEquip(EquipType.Back)]
    public class KrampusChimney : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<KrampusChimneyPlayer>().KrampusChimney = true;
        }
    }

    public class KrampusChimneyPlayer : ModPlayer
    {
        public bool KrampusChimney = false;
        public float KrampusChimneyCharge = 0f;
        public int KrampusChimneyProjTimer = 0;

        public override void ResetEffects()
        {
			KrampusChimney = false;
		}

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //do not allow hotkeys to do anything if you are dead
            if (Player.dead)
            {
                return;
            }

            if (Spooky.AccessoryHotkey.JustPressed && Main.myPlayer == Player.whoAmI)
            {
                if (KrampusChimney && KrampusChimneyCharge >= 10f && KrampusChimneyProjTimer <= 0)
                {
                    KrampusChimneyProjTimer = 120;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (KrampusChimney)
            {
                KrampusChimneyCharge = 0;
            }
		}

        public override void PreUpdate()
        {
            if (KrampusChimney)
			{
				if (KrampusChimneyCharge < 10.5f && KrampusChimneyProjTimer <= 0)
				{
					KrampusChimneyCharge += 0.025f;
				}

                if (KrampusChimneyProjTimer > 0)
                {
                    KrampusChimneyProjTimer--;

                    if (KrampusChimneyProjTimer % 10 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item73, Player.Center);

                        Vector2 velocity = new Vector2(0, -25).RotatedByRandom(MathHelper.ToRadians(35));

                        Vector2 Position = Player.Top + new Vector2(Player.direction == 1 ? -10 : 10, 0);

						Projectile.NewProjectile(null, Position, velocity, ModContent.ProjectileType<ChimneyCoal>(), 40, 0, Player.whoAmI);

                        for (int j = 0; j < 10; j++)
                        {
                            Vector2 dustVelocity = new Vector2(0, -25).RotatedByRandom(MathHelper.ToRadians(35));

                            Dust dust = Dust.NewDustPerfect(Position, DustID.Torch, dustVelocity, default, default, 1f);
                            dust.velocity *= Main.rand.NextFloat(0.1f, 0.001f);
                        }
                    }

                    if (KrampusChimneyCharge > 0)
                    {
                        KrampusChimneyCharge -= 0.1f;
                    }
                }
			}
			else
			{
				KrampusChimneyCharge = 0;
                KrampusChimneyProjTimer = 0;
			}
        }
    }
}