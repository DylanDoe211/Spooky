using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class GooChompers : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<PeptoStomach>();
        }

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<GooChompersPlayer>().GooChompers = true;
            player.GetCritChance(DamageClass.Generic) += 10;
		}
	}

    public class GooChompersPlayer : ModPlayer
    {
		public bool GooChompers = false;

        public override void ResetEffects()
        {
			GooChompers = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (GooChompers && Main.rand.NextBool(15) && !target.GetGlobalNPC<NPCGlobal>().HasGooChompterAttached && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<GooChomperProj>(), hit.Damage, 0, Player.whoAmI, target.whoAmI);
                target.GetGlobalNPC<NPCGlobal>().HasGooChompterAttached = true;
            }
        }
    }
}