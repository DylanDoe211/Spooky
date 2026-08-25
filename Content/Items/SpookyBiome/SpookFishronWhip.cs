using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.WhipDebuff;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronWhip : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 200;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 46;
            Item.height = 46;
			Item.useTime = 38;
			Item.useAnimation = 38;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<SpookFishronWhipProj>();
			Item.shootSpeed = 4f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}
    }

	public class SpookFishronWhipEffectPlayer : ModPlayer
	{
		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
            if (target.HasBuff<SpookFishronWhipDebuff>() && !proj.npcProj && !proj.trap && (proj.minion || ProjectileID.Sets.MinionShot[proj.type]))
            {
				Projectile.NewProjectile(proj.GetSource_FromAI(), target.Center, new Vector2(0, Main.rand.Next(4, 9)).RotatedByRandom(360), ModContent.ProjectileType<SpookFishronWhipBubble>(), proj.damage, 0, Player.whoAmI);
            }
		}
	}
}