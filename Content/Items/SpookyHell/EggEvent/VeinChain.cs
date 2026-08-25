using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class VeinChain : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SmokerLung>();
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 62;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<VeinChainPlayer>().VeinChain = true;
		}
	}

    public class VeinChainPlayer : ModPlayer
    {
		public bool VeinChain = false;

        public override void ResetEffects()
        {
			VeinChain = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (VeinChain && Main.rand.NextBool(10) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type] && Vector2.Distance(Player.Center, target.Center) <= 370f)
            {
                int MaxChains = Player.statLife < (Player.statLifeMax / 4) ? 1 : (Player.statLife < (Player.statLifeMax / 2) ? 2 : 3);

                if (Player.ownedProjectileCounts[ModContent.ProjectileType<VeinChainProj>()] < MaxChains && !target.GetGlobalNPC<NPCGlobal>().HasVeinChainAttached)
                {
                    Projectile.NewProjectile(target.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<VeinChainProj>(), 35, 0, Player.whoAmI, 0, 0, target.whoAmI);
                    target.GetGlobalNPC<NPCGlobal>().HasVeinChainAttached = true;
                }
            }
        }
    }
}