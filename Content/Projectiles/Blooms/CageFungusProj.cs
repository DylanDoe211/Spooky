using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class CageFungusProj : ModProjectile
    {
        private static Asset<Texture2D> ChainTexture1;
        private static Asset<Texture2D> ChainTexture2;
        private static Asset<Texture2D> ChainTexture3;
        private static Asset<Texture2D> ChainTexture4;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.hide = true;
            Projectile.timeLeft = 420;
			Projectile.penetrate = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            ChainTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CageFungusChain1");
            ChainTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CageFungusChain2");
            ChainTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CageFungusChain3");
            ChainTexture4 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CageFungusChain4");
            
            bool flip = false;
            if (Parent.direction == -1)
            {
                flip = true;
            }

            Vector2 drawOrigin = new Vector2(0, ChainTexture1.Height() / 2);
            Vector2 myCenter = Projectile.Center;
            Vector2 p0 = Parent.Center;
            Vector2 p1 = Parent.Center;
            Vector2 p2 = myCenter;
            Vector2 p3 = myCenter;

            int segments = 6;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;
                Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                t = (i + 1) / (float)segments;
                Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                Vector2 toNext = drawPosNext - drawPos2;
                float rotation = toNext.ToRotation();
                float distance = toNext.Length();

                Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

                if (i == 4)
                {
                    Main.EntitySpriteDraw(ChainTexture1.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, 
                    drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), SpriteEffects.None, 0f);
                }
                else if (i == 3)
                {
                    Main.EntitySpriteDraw(ChainTexture2.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, 
                    drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), SpriteEffects.None, 0f);
                }
                else if (i == 2)
                {
                    Main.EntitySpriteDraw(ChainTexture3.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, 
                    drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), SpriteEffects.None, 0f);
                }
                else
                {
                    Main.EntitySpriteDraw(ChainTexture4.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, 
                    drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), SpriteEffects.None, 0f);
                }
            }

			return true;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            Projectile.frame = (int)Projectile.ai[1];

            if (Projectile.timeLeft <= 2)
            {
                Parent.GetGlobalNPC<NPCGlobal>().HasRedCageAttached = false;
            }
            else
            {
                Parent.GetGlobalNPC<NPCGlobal>().HasRedCageAttached = true;
            }

            if (!Parent.active)
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 5;
            }

            Projectile.ai[2]++;
            Vector2 destination = new Vector2(Parent.Center.X, Parent.Top.Y - 35 + (float)Math.Sin(Projectile.ai[2] / 30) * 15);
            Projectile.Center = destination;

            if (!Parent.boss && !Parent.IsTechnicallyBoss())
            {
                Parent.velocity.X *= 0.95f;
                Parent.velocity.Y += !Parent.noTileCollide ? 0.10f : 0.01f;
            }
        }
    }
}