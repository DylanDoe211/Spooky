using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class PeptoStomach : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<VeinChain>();
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<PeptoStomachPlayer>().PeptoStomach = true;
		}
    }

    public class PeptoStomachPlayer : ModPlayer
    {
		public bool PeptoStomach = false;

        public override void ResetEffects()
        {
			PeptoStomach = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (PeptoStomach && !target.boss && !target.IsTechnicallyBoss() && Main.rand.NextBool(20) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                target.AddBuff(ModContent.BuffType<PeptoDebuff>(), int.MaxValue);
            }
        }
    }
}