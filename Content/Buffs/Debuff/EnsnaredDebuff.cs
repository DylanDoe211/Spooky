using Spooky.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class EnsnaredDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.friendly && !npc.boss && !npc.IsTechnicallyBoss())
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GreenMoss);
				
				npc.velocity.X *= 0.7f;
                if (!npc.noGravity)
                {
                    npc.velocity.Y *= 0.7f;
                }
			}
		}
    }
}
