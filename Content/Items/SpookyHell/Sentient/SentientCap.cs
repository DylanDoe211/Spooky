using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
	[AutoloadEquip(EquipType.Head)]
	public class SentientCap : ModItem, ICauldronOutput
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.vanity = true;
			Item.rare = ModContent.RarityType<SentientRarity>();
		}

        public override void EquipFrameEffects(Player player, EquipType type)
        {
			player.GetModPlayer<SentientCapPlayer>().SentientCap = true;
        }
    }

	public class SentientCapPlayer : ModPlayer
    {
        public bool SentientCap = false;

        public static readonly SoundStyle CapSound1 = new("Spooky/Content/Sounds/SentientCap1", SoundType.Sound);
        public static readonly SoundStyle CapSound2 = new("Spooky/Content/Sounds/SentientCap2", SoundType.Sound);
        public static readonly SoundStyle CapSound3 = new("Spooky/Content/Sounds/SentientCap3", SoundType.Sound);

		public override void ResetEffects()
        {
            SentientCap = false;
		}

		public override void PreUpdate()
		{
			//sentient cap random dialogue
            if (SentientCap && Main.rand.NextBool(1000))
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                    {
                        SoundEngine.PlaySound(CapSound1, Player.Center);
                        break;
                    }
                    case 1:
                    {
                        SoundEngine.PlaySound(CapSound2, Player.Center);
                        break;
                    }
                    case 2:
                    {
                        SoundEngine.PlaySound(CapSound3, Player.Center);
                        break;
                    }
                }

                CustomPopupText.SpawnText(Player.Top, Language.GetTextValue("Mods.Spooky.Dialogue.SentientCap.Dialogue" + Main.rand.Next(1, 7).ToString()), Color.DarkOrchid, Player.velocity, 85);
            }
		}
	}
}