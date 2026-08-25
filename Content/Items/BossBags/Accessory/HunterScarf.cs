using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.BossBags.Accessory
{
    [AutoloadEquip(EquipType.Neck)]
	public class HunterScarf : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 32;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(gold: 18);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<HunterScarfPlayer>().HunterScarf = true;
        }
	}

    public class HunterScarfPlayer : ModPlayer
    {
        public bool HunterScarf = false;

        public override void ResetEffects()
        {
            HunterScarf = false;
        }

        public override void PreUpdate()
        {
            if (HunterScarf)
            {
                foreach (var NPC in Main.ActiveNPCs)
                {
                    if (!NPC.friendly && !NPC.immortal && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Player.Distance(NPC.Center) <= 350f)
                    {
                        NPC.AddBuff(ModContent.BuffType<HunterScarfMark>(), 10);
                    }
                }
            }
        }
    }
}
