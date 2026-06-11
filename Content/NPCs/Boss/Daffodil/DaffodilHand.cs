using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    public class DaffodilHandLeft : ModNPC
    {
        Vector2 SavePlayerPosition;

        public bool HasHitSurface = false;

        public enum AnimationState
		{
			Normal, HandOpen, Fist
		}

		private AnimationState CurrentAnimation
        {
			get => (AnimationState)NPC.ai[3];
			set => NPC.ai[3] = (float)value;
		}

        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(HasHitSurface);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();

            //bools
            HasHitSurface = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.width = 56;
            NPC.height = 56;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm");
                
                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            if (CurrentAnimation == AnimationState.Normal)
			{
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (CurrentAnimation == AnimationState.HandOpen)
			{
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (CurrentAnimation == AnimationState.Fist)
			{
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            Player player = Main.player[Parent.target];

            //kill the hand if the parent does not exist
            if (!Parent.active || Parent.type != ModContent.NPCType<DaffodilEye>())
            {
                NPC.active = false;
            }

            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                //set rotation based on the parent npc
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                float RotateX = Parent.Center.X - vector.X;
                float RotateY = Parent.Center.Y - vector.Y;
                NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            }

            bool RightHand = NPC.type == ModContent.NPCType<DaffodilHandRight>();

            switch ((int)Parent.ai[0])
            {
                case -5: 
                {
                    GoToPosition(0, 0);
                    break;
                }

                case -4: 
                {
                    GoToPosition(0, 0);
                    break;
                }

                case -3: 
                {
                    GoToPosition(0, 0, false);
                    break;
                }

                case -2: 
                {
                    if (Parent.localAI[0] < 180 || Parent.localAI[0] >= 360)
                    {
                        CurrentAnimation = AnimationState.Normal;

                        GoToPosition(130, 180);
                    }

                    if (Parent.localAI[0] == 180)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }

                    if (Parent.localAI[0] > 180 && Parent.localAI[0] <= 300)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 250f);

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 2, (float)NPC.height / 2) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.12f);
                            Main.dust[dustEffect].color = Color.Green;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (Parent.localAI[0] > 180 && Parent.localAI[0] <= 360)
                    {
                        GoToPosition(240, 25);
                    }

                    break;
                }

                case -1: 
                {
                    GoToPosition(130, 180);
                    break;
                }

                case 0: 
                {
                    GoToPosition(130, 180);
                    break;
                }

                case 1: 
                {
                    GoToPosition(300, -25);

                    int TimeOffset = RightHand ? 15 : 0;

                    //shoot out flowers with a delay so it alternates from left hand to right hand
                    if (Parent.localAI[0] == 90 + TimeOffset || Parent.localAI[0] == 120 + TimeOffset || Parent.localAI[0] == 150 + TimeOffset || 
                    Parent.localAI[0] == 180 + TimeOffset || Parent.localAI[0] == 210 + TimeOffset || Parent.localAI[0] == 240 + TimeOffset)
                    {
                        CurrentAnimation = AnimationState.HandOpen;

                        SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                        Vector2 ShootSpeed = NPC.Center - Parent.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 10f;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<HomingDaffodil>(), NPC.damage, 4.5f);
                    }

                    if (Parent.localAI[0] >= 250)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    break;
                }

                case 2: 
                {
                    GoToPosition(10, 200, false);

                    if (Parent.localAI[0] == 80)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }
                    if (Parent.localAI[0] == 370)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    break;
                }

                case 3: 
                {
                    GoToPosition(10, 150, false);

                    if (Parent.localAI[0] == 80)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }
                    if (Parent.localAI[0] == 710)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    break;
                }

                case 4: 
                {
                    GoToPosition(200, 0, false);

                    if (Parent.localAI[0] == 50)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }

                    if (Parent.localAI[0] >= 60 && Parent.localAI[0] <= 180 && Parent.localAI[0] % 20 == 0)
                    {
                        CurrentAnimation = AnimationState.HandOpen;

                        SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                        Vector2 ShootSpeed = NPC.Center - Parent.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 6f;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(ShootSpeed.X, Main.rand.NextFloat(-3f, 1f)), ModContent.ProjectileType<ThornPillarSeed>(), NPC.damage, 4.5f, ai2: NPC.ai[2]);
                    }

                    if (Parent.localAI[0] == 220)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    break;
                }

                case 5: 
                {
                    GoToPosition(130, 180);
                    break;
                }

                case 6: 
                {
                    GoToPosition(130, 180);
                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y, bool ExtraMovement = true)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            bool Switch = NPC.type == ModContent.NPCType<DaffodilHandRight>();

            float goToX = (Parent.Center.X + (!Switch ? -X : X));
            float goToY = (Parent.Center.Y + Y);

            if (ExtraMovement)
            {
                NPC.localAI[1]++;

                if (!Switch)
                {
                    goToX += (float)Math.Sin(NPC.localAI[1] / 30) * 15;
                }
                else
                {
                    goToX -= (float)Math.Sin(NPC.localAI[1] / 30) * 15;
                }

                goToY += (float)Math.Sin(NPC.localAI[1] / 30) * 15;
            }

            if (NPC.Distance(new Vector2(goToX, goToY)) <= 5f)
            {
                if (!ExtraMovement)
                {
                    NPC.velocity = Vector2.Zero;
                }
                else
                {
                    NPC.velocity *= 0.9f;
                }
            }
			else
			{
				Vector2 desiredVelocity = NPC.DirectionTo(new Vector2(goToX, goToY)) * 8;
				NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
			}
        }
    }

    public class DaffodilHandRight : DaffodilHandLeft
    {
        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm");
                
                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }
    }
}