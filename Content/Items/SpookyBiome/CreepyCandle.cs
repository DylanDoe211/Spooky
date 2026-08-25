using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CreepyCandle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CreepyCandlePlayer>().CreepyCandle = true;
        }
    }

    public class CreepyCandlePlayer : ModPlayer
    {
        public bool CreepyCandle = false;

        public override void ResetEffects()
        {
			CreepyCandle = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                if (CreepyCandle && hit.DamageType == DamageClass.Magic)
                {
                    if (Main.rand.NextBool(3))
                    {
                        target.AddBuff(BuffID.OnFire, 120);
                    }
                }
            }
        }
    }
}