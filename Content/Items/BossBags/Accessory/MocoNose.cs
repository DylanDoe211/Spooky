using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class MocoNose : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(gold: 15);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<MocoNosePlayer>().MocoNose = true;
        }
    }

    public class MocoNosePlayer : ModPlayer
    {
        public bool MocoNose = false;
        public int MocoBoogerCharge = 0;

        public override void ResetEffects()
        {
            MocoNose = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (MocoNose && MocoBoogerCharge < 15 && Main.rand.NextBool(12) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
            {
                int itemType = ModContent.ItemType<MocoNoseBooger>();
                int newItem = Item.NewItem(target.GetSource_OnHit(target), target.Hitbox, itemType);
                Main.item[newItem].noGrabDelay = 0;

                if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                {
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                }
            }
        }
    }
}