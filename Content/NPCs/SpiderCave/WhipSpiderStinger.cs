using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class WhipSpiderStinger : ModNPC
	{
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 45;
            NPC.defense = 25;
            NPC.width = 20;
            NPC.height = 20;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawTailSegments(false);

			return true;
		}

        public void DrawTailSegments(bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];

			if (Parent.active && Parent.type == ModContent.NPCType<WhipSpider>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Segment");

				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center;
				Vector2 p0 = Parent.Center - new Vector2(32 * (flip ? -1 : 1), 10);
				Vector2 p1 = Parent.Center - new Vector2(45 * (flip ? -1 : 1), 15);
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 0);
				Vector2 p3 = myCenter;

				int segments = 10;

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

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 10;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					
					if (Main.netMode != NetmodeID.Server)
					{
						//Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/EarWormSegmentGore").Type);
					}
				}
			}
		}

        public override bool CheckActive()
        {
            return false;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 300);
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[2]];
            Player player = Main.player[Parent.target];

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<WhipSpider>())
            {
                NPC.active = false;
            }

            NPC.direction = Parent.direction;
            NPC.spriteDirection = Parent.spriteDirection;

            Vector2 GoTo = new Vector2(Parent.Center.X - (18 * NPC.direction), Parent.Center.Y - 45);

            if (NPC.Distance(GoTo) >= 10)
            {
                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }
            else
            {
                NPC.velocity *= 0.95f;
            }

			if (NPC.Distance(player.Center) <= 160f)
			{
				if (NPC.Distance(GoTo) <= 15)
				{
					Vector2 ChargeDirection = player.Center - NPC.Center;
					ChargeDirection.Normalize();
					ChargeDirection *= 25;
					NPC.velocity = ChargeDirection;
				}
			}
		}
	}
}