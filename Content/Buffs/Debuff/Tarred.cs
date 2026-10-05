using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class Tarred : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            BuffGlobal.IsSpookyDebuffForAchievement[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (Main.rand.NextBool(10))
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Asphalt, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, default, default, 1.5f);
            }

            if (!npc.friendly && !npc.boss && !npc.IsTechnicallyBoss())
            {
                npc.velocity.X *= 0.9f;
                if (!npc.noGravity)
                {
                    npc.velocity.Y *= 0.9f;
                }
            }

            if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.color = storedColor;
            }
            else
            {
                Color color = npc.GetAlpha(Color.Black);
                npc.color = color;
            }
        }
    }
}
