using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
	public class CobwebParticle : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.05f;
            dust.velocity.Y *= 0.5f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 0;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(0.6f, 0.9f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 18, 20, 20);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override bool Update(Dust dust)
        {
            if (Main.rand.NextBool(10) || WorldGen.SolidTile(Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16]))
            {
                dust.alpha += 1;
            }

            if (Main.rand.NextBool(12))
            {
                dust.velocity.X += Main.rand.NextFloat(-0.1f, 0.1f);
                dust.velocity.Y += Main.rand.NextFloat(0.02f, 0.1f);
            }

            dust.position += dust.velocity;

            if (WorldGen.SolidTile(Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16]))
			{
				dust.velocity = Vector2.Zero;
			}

            if (dust.alpha > 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}