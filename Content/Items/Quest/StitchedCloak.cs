using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Quest
{
	[AutoloadEquip(EquipType.Front)]
	public class StitchedCloak : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 10);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<StitchedCloakPlayer>().StitchedCloak = true;
		}
	}

    public class StitchedCloakPlayer : ModPlayer
    {
		public bool StitchedCloak = false;

        public const int dashDown = 0;
		public const int dashUp = 1;
		public const int dashRight = 2;
		public const int dashLeft = 3;
		public int dashCooldown = 30;
		public int dashDuration = 15;
		public float dashVelocityY = 22f;
		public float dashVelocityX = 22f;
		public int dashDir = -1;
		public int dashDelay = 0;
		public int dashTimer = 0;

		public override void ResetEffects()
        {
			StitchedCloak = false;

            if (Player.controlUp && Player.releaseUp && Player.doubleTapCardinalTimer[dashUp] < 15)
			{
				dashDir = dashUp;
			}
			else if (Player.controlDown && Player.releaseDown && Player.doubleTapCardinalTimer[dashDown] < 15)
			{
				dashDir = dashDown;
			}
			else if (Player.controlRight && Player.releaseRight && Player.doubleTapCardinalTimer[dashRight] < 15)
			{
				dashDir = dashRight;
			}
			else if (Player.controlLeft && Player.releaseLeft && Player.doubleTapCardinalTimer[dashLeft] < 15)
			{
				dashDir = dashLeft;
			}
			else
			{
                if (dashTimer <= 0)
                {
				    dashDir = -1;
                }
			}
		}

		private bool CanUseDash()
		{
			return StitchedCloak && Player.dashType == 0 && !Player.setSolar && dashDir != -1 && dashDelay == 0 && !Player.mount.Active && !Player.HasBuff(ModContent.BuffType<StitchedCloakCooldown>());
		}
        
        public override void PreUpdateMovement()
		{
			// If the player can use our dash and has double tapped in a direction, then apply the dash
			if (CanUseDash())
			{
				Vector2 newVelocity = Player.velocity;

				switch (dashDir)
				{
                    case dashUp when Player.velocity.Y > -dashVelocityY:
					case dashDown when Player.velocity.Y < dashVelocityY:
					{
						float dashDirection = dashDir == dashDown ? 1 : -1;
						newVelocity.Y = dashDirection * dashVelocityY;
						break;
					}
					case dashLeft when Player.velocity.X > -dashVelocityX:
					case dashRight when Player.velocity.X < dashVelocityX:
					{
						float dashDirection = dashDir == dashRight ? 1 : -1;
						newVelocity.X = dashDirection * dashVelocityX;
						break;
					}
					default:
					{
						return;
					}
				}

				SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Player.position);

				dashDelay = dashCooldown;
				dashTimer = dashDuration;

				Player.velocity = newVelocity;

                Player.AddBuff(ModContent.BuffType<StitchedCloakCooldown>(), 300);
			}

			if (dashDelay > 0)
			{
				dashDelay--;
			}

			if (dashTimer > 0)
			{
				dashTimer--;

                Player.immune = true;
                Player.immuneTime = 2;
                Player.gravity = 0;

                if (dashTimer == 1)
			    {
                    if (dashDir == dashDown || dashDir == dashUp)
                    {
                        Player.velocity.Y *= 0.25f;
                    }
                    if (dashDir == dashLeft || dashDir == dashRight)
                    {
                        Player.velocity.X *= 0.25f;
                    }
                }
                else
                {
                    if (dashDir == dashDown || dashDir == dashUp)
                    {
                        Player.velocity.X *= 0.0001f;
                    }
                    if (dashDir == dashLeft || dashDir == dashRight)
                    {
                        Player.velocity.Y *= 0.0001f;
                    }
                }

				int dust = Dust.NewDust(Player.position, Player.width, Player.height, DustID.Web, 0, 0, default, default, Main.rand.NextFloat(0.75f, 1.5f));
				Main.dust[dust].velocity *= 0;
			}
		}

        public override void PostUpdateRunSpeeds()
        {
			if (StitchedCloak)
			{
				Player.runAcceleration += 0.025f;
			}
        }
	}
}
