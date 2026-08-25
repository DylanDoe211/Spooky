using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class StonedKidney : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GiantEar>();
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.GetModPlayer<StonedKidneyPlayer>().StonedKidney = true;
		}
	}

    public class StonedKidneyPlayer : ModPlayer
    {
		public bool StonedKidney = false;
        public float StonedKidneyCharge = 0f;

        public override void ResetEffects()
        {
			StonedKidney = false;
        }

        public override void PreUpdate()
        {
            if (StonedKidney)
			{
                bool PlayerHoldingWeapon = ItemGlobal.ActiveItem(Player).damage > 0 && ItemGlobal.ActiveItem(Player).pick <= 0 && ItemGlobal.ActiveItem(Player).hammer <= 0 && 
			    ItemGlobal.ActiveItem(Player).axe <= 0 && ItemGlobal.ActiveItem(Player).mountType <= 0;

				if ((!Player.controlUseItem || !PlayerHoldingWeapon) && StonedKidneyCharge <= 7.5f)
				{
					StonedKidneyCharge += 0.05f;
				}
			}
			else
			{
				StonedKidneyCharge = 0;
			}
        }

        public override void PostUpdate()
		{
            if (StonedKidneyCharge >= 7.5f)
            {
                Player.GetDamage(DamageClass.Generic) += 0.15f;
            }
		}
    }
}