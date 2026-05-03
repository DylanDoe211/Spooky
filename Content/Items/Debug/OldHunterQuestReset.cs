using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;

namespace Spooky.Content.Items.Debug
{
    public class OldHunterQuestReset : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

		public override bool? UseItem(Player player)
		{
            Flags.OldHunterDefeatDialogue = false;
			Flags.OldHunterQuest1 = false;
            Flags.OldHunterQuest2 = false;
            Flags.OldHunterQuest3 = false;
            Flags.OldHunterQuest4 = false;
            Flags.OldHunterQuest5 = false;
            Flags.OldHunterQuest6 = false;
            Flags.OldHunterQuest7 = false;
            Flags.OldHunterQuest8 = false;

			return true;
		}
    }
}