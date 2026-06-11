using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class DaffodilBeam : ModProjectile
    {
        const float FirstSegmentDrawDist = 5;

        public float Frame
        {
            get => Projectile.localAI[1];
            set => Projectile.localAI[1] = value;
        }

        public float LaserLength = 0;
        public int LaserSegmentLength = 10;
        public int LaserWidth = 40;
        public int LaserEndSegmentLength = 22;

        public Vector2 endPoint;
        public bool NewCollision;

        public int MaxLaserLength = 2000;
        public int maxLaserFrames = 1;
        public int LaserFrameDelay = 5;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 100;
        }

        public void DrawLaser(Texture2D texture, Vector2 start, Vector2 unit, float rotation = 0f, float scale = 1f, float maxDist = 2000f, Color color = default, int transDist = 1)
        {
            float beamRotation = unit.ToRotation() + rotation;
            
            for (float i = transDist; i <= (maxDist * (1 / Projectile.scale)); i += LaserSegmentLength)
            {
                //scaling stuff to make the laser look wavy, helix must only be applied to the X-scale and not the Y-scale otherwise it will make the laser look weird
                float alphaMult = (255 - Projectile.alpha) / 255f;
                float otherMult = 1f - i / (maxDist * (1 / Projectile.scale));
                float helix = otherMult * alphaMult * 0.8f * (float)Math.Sin(MathHelper.ToRadians(Projectile.localAI[0] * -16 + i));

                for (int j = 0; j < 360; j += 90)
                {
                    Vector2 circular = new Vector2(6f, 0).RotatedBy(MathHelper.ToRadians(j));
                    Color RealColor = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(color);

                    //laser body
                    var origin = start + i * unit;
                    Main.EntitySpriteDraw(texture, origin + circular - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle((int)(LaserWidth * Frame), LaserEndSegmentLength, LaserWidth, LaserSegmentLength), 
                    RealColor, beamRotation, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), new Vector2(scale + (helix / 1.75f), scale), 0, 0);

                    //base
                    if (i == transDist)
                    {
                        Main.EntitySpriteDraw(texture, start + unit * (transDist - LaserEndSegmentLength) + circular - Main.screenPosition + new Vector2(0, Projectile.gfxOffY),
                        new Rectangle((int)(LaserWidth * Frame), 0, LaserWidth, LaserEndSegmentLength), RealColor, beamRotation, 
                        new Vector2(LaserWidth / 2, LaserSegmentLength / 2), new Vector2(scale + (helix / 1.75f), scale), 0, 0);
                    }
                
                    //tip
                    if (i == (maxDist * (1 / Projectile.scale)))
                    {
                        Main.EntitySpriteDraw(texture, start + maxDist * (1 / scale) * unit + circular - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), 
                        new Rectangle((int)(LaserWidth * Frame), LaserSegmentLength + LaserEndSegmentLength, LaserWidth, LaserEndSegmentLength), 
                        RealColor, beamRotation, new Vector2(LaserWidth / 2, LaserSegmentLength / 2), new Vector2(scale + (helix / 1.75f), scale), 0, 0);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser(ModContent.Request<Texture2D>(Texture).Value, Projectile.Center + (new Vector2(Projectile.width, 0).RotatedBy(Projectile.rotation) * Projectile.scale), 
            new Vector2(1f, 0).RotatedBy(Projectile.rotation) * Projectile.scale, -1.57f, 1f, LaserLength, Projectile.GetAlpha(Color.Gold * 0.85f), (int)FirstSegmentDrawDist);

            return false;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= LaserFrameDelay)
            {
                Projectile.frameCounter = 0;
                Frame++;
                if (Frame >= maxLaserFrames)
                {
                    Frame = 0;
                }
            }

            Projectile.localAI[0]++;

            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            if (!Parent.active || (Parent.type != ModContent.NPCType<DaffodilEye>()))
            {
                Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 LaserOrigin = new Vector2(Parent.Center.X, Parent.Center.Y + 10);
            Projectile.Center = LaserOrigin;

            if (Projectile.timeLeft > 10)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);
            }
            else
            {
                Projectile.alpha += 25;
            }

            EndpointTileCollision();
        }

        public void EndpointTileCollision()
        {
            //beam tile collission and produce dust where the beam iss hitting the ground
            for (LaserLength = FirstSegmentDrawDist; LaserLength < MaxLaserLength; LaserLength += LaserSegmentLength)
            {
                Vector2 start = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * LaserLength;
                if (!Collision.CanHitLine(Projectile.Center, 10, 10, start, 20, 20))
                {
                    float angle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
                    Vector2 velocity = angle.ToRotationVector2();
                    Dust dust = Dust.NewDustDirect(start, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X, velocity.Y, 0, Color.Gold, 0.3f);
                    dust.color = Color.Gold;
                    dust.noGravity = true;

                    LaserLength -= LaserSegmentLength;
                    break;
                }
            }
        }

        public static bool CheckLinearCollision(Vector2 point1, Vector2 point2, Rectangle hitbox, out Vector2 intersectPoint)
        {
            intersectPoint = Vector2.Zero;

            return LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.TopRight(), out intersectPoint) ||
            LinesIntersect(point1, point2, hitbox.TopLeft(), hitbox.BottomLeft(), out intersectPoint) ||
            LinesIntersect(point1, point2, hitbox.BottomLeft(), hitbox.BottomRight(), out intersectPoint) ||
            LinesIntersect(point1, point2, hitbox.TopRight(), hitbox.BottomRight(), out intersectPoint);
        }

        public static bool LinesIntersect(Vector2 point1, Vector2 point2, Vector2 point3, Vector2 point4, out Vector2 intersectPoint) //algorithm taken from http://web.archive.org/web/20060911055655/http://local.wasp.uwa.edu.au/~pbourke/geometry/lineline2d/
        {
            intersectPoint = Vector2.Zero;

            var denominator = (point4.Y - point3.Y) * (point2.X - point1.X) - (point4.X - point3.X) * (point2.Y - point1.Y);

            var a = (point4.X - point3.X) * (point1.Y - point3.Y) - (point4.Y - point3.Y) * (point1.X - point3.X);
            var b = (point2.X - point1.X) * (point1.Y - point3.Y) - (point2.Y - point1.Y) * (point1.X - point3.X);

            if (denominator == 0)
            {
                if (a == 0 || b == 0) //lines are coincident
                {
                    intersectPoint = point3; //possibly not the best fallback?
                    return true;
                }

                else return false; //lines are parallel
            }

            var ua = a / denominator;
            var ub = b / denominator;

            if (ua > 0 && ua < 1 && ub > 0 && ub < 1)
            {
                intersectPoint = new Vector2(point1.X + ua * (point2.X - point1.X), point1.Y + ua * (point2.Y - point1.Y));
                return true;
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 unit = new Vector2(1.1f, 0).RotatedBy(Projectile.rotation);
            float point = 0f;

            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + unit * LaserLength, Projectile.width * Projectile.scale, ref point))
            {
                return true;
            }

            return false;
        }
    }
}