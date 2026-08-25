using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [AutoloadEquip(EquipType.Face)]
    public class DaffodilHairpin : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 20);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<DaffodilHairpinPlayer>().DaffodilHairpin = true;
        }
    }

    public class DaffodilHairpinPlayer : ModPlayer
    {
        public bool DaffodilHairpin = false;
        public int DaffodilHairpinTimer = 0;

        public override void ResetEffects()
        {
            DaffodilHairpin = false;
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (DaffodilHairpin)
			{
				foreach (var Proj in Main.ActiveProjectiles)
                {
					if (Proj.type == ModContent.ProjectileType<DaffodilHairpinPetal>() && Proj.owner == Player.whoAmI)
					{
						Proj.damage = info.Damage < 40 ? 40 : info.Damage;
						Proj.ai[1] = 1;
					}
				}
			}
        }

        public override void PreUpdate()
        {
            if (DaffodilHairpin)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<DaffodilHairpinPetal>()] < 6)
                {
                    DaffodilHairpinTimer++;
                    if (DaffodilHairpinTimer % 17 == 0)
                    {
                        int PetalType = ModContent.ProjectileType<DaffodilHairpinPetal>();

						SoundEngine.PlaySound(SoundID.Grass with { Volume = 0.2f }, Player.Center);
                        Projectile.NewProjectile(null, Player.Center, Vector2.Zero, PetalType, 0, 3f, Player.whoAmI, ai0: Player.ownedProjectileCounts[PetalType]);
                    }
                }
            }
            else
            {
                DaffodilHairpinTimer = 0;
            }
        }
    }
}