using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class SpiderWarWebSlowDebuff : ModBuff
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
				if (Main.rand.NextBool(10))
				{
					Dust.NewDust(npc.position, npc.width, npc.height, DustID.Web, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, default, default, 1.5f);
				}

                npc.velocity.X *= 0.95f;
                if (!npc.noGravity)
                {
                    npc.velocity.Y *= 0.95f;
                }
            }
        }
    }
}
