using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.WhipDebuff
{
	public class SpookFishronWhipDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";
		public static readonly int tagDamage = 10;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (!npc.friendly)
            {
				npc.lifeRegen -= 10;

				if (Main.rand.NextBool(4))
				{
					int newDust = Dust.NewDust(npc.position, npc.width, npc.height, DustID.Torch, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, default, default, 2.5f);
					Main.dust[newDust].noGravity = true;
				}
			}
		}
    }
}
