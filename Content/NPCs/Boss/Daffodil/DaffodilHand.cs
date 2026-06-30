using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;
using Spooky.Core;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

        private static Asset<Texture2D> ArmUpperTexture;
        private static Asset<Texture2D> ArmLowerTexture;

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
            NPC.width = 100;
            NPC.height = 104;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.dontCountMe = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
        }

        public static Vector2 OffsetWithRotation(float rotation, float x, float y) => PolarVector(x, rotation) + PolarVector(y, rotation + MathHelper.PiOver2);
        public static Vector2 PolarVector(float radius, float theta) => new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
            {
                ArmUpperTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArmUpper");
                ArmLowerTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArmLower");

				//get frame origins for arms
                Vector2 frameOriginUpper = (ArmUpperTexture.Size() / 2) + new Vector2(34, 11);
				Rectangle frameUpper = new Rectangle(0, 0, ArmUpperTexture.Width(), ArmUpperTexture.Height());

				Vector2 ParentCenter = Parent.Center + new Vector2(-40, 20);

				//rotation from the hand to the parent daffodil body offsets
				float RotationToParent = (float)Math.Atan2(ParentCenter.Y - NPC.Center.Y, ParentCenter.X - NPC.Center.X) + 4.71f;

				//bottom of the upper arm, where the lower arm should be drawn from
				Vector2 LowerArmPos = ParentCenter + OffsetWithRotation(RotationToParent, -3, 199);

				//rotation of the upper arm from the bottom of the upper arm to the parent center offset
				float upperArmRotation = (float)Math.Atan2(LowerArmPos.Y - ParentCenter.Y, (LowerArmPos.X + 50) - ParentCenter.X) + 1.57f;

				//change lower arm position again based on the rotation of the upper arm
				LowerArmPos = ParentCenter + OffsetWithRotation(upperArmRotation, -3, 199);

				//draw upper arm segment
                Color UpperArmColor = Lighting.GetColor((int)ParentCenter.X / 16, (int)ParentCenter.Y / 16);
				spriteBatch.Draw(ArmUpperTexture.Value, ParentCenter - screenPos, null, NPC.GetAlpha(UpperArmColor),
				upperArmRotation, new Vector2(34, 11), NPC.scale, SpriteEffects.None, 0);

				//lower arm rotation is based off of the bottom of the upper arm to the hand
				float lowerArmRotation = (LowerArmPos - NPC.Center).ToRotation() + MathHelper.PiOver2;

				//scale the lower arm texture based on the distance of the hand to the position so that the hand does not randomly just disconnect from the bottom of the lower arm
				Vector2 armScale = new(1, NPC.Distance(LowerArmPos) / 136f);

                //draw lower arm segment
                Color LowerArmColor = Lighting.GetColor((int)LowerArmPos.X / 16, (int)LowerArmPos.Y / 16);
				spriteBatch.Draw(ArmLowerTexture.Value, LowerArmPos - screenPos, null, NPC.GetAlpha(LowerArmColor), 
				lowerArmRotation, new Vector2(29, 8), armScale, SpriteEffects.None, 0);
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

			bool RightHand = NPC.type == ModContent.NPCType<DaffodilHandRight>();

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<DaffodilEye>())
            {
                NPC.active = false;
            }

			if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
			{
				Vector2 ParentCenter = Parent.Center + new Vector2(RightHand ? 40 : -40, 20);

				float RotationToParent = (float)Math.Atan2(Parent.Center.Y - NPC.Center.Y, Parent.Center.X - NPC.Center.X) + 4.71f;

				Vector2 LowerArmPos = ParentCenter + OffsetWithRotation(RotationToParent, RightHand ? 3 : -3, 199);

				float upperArmRotation = (float)Math.Atan2(LowerArmPos.Y - ParentCenter.Y, (LowerArmPos.X + (RightHand ? -50 : 50)) - ParentCenter.X) + 1.57f;

				LowerArmPos = ParentCenter + OffsetWithRotation(upperArmRotation, RightHand ? 3 : -3, 199);

				NPC.rotation = (float)Math.Atan2(LowerArmPos.Y - NPC.Center.Y, LowerArmPos.X - NPC.Center.X) + MathHelper.PiOver2;
			}

            switch ((int)Parent.ai[0])
            {
                case -5: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                case -4: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                case -3: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                case -2: 
                {
                    if (Parent.localAI[0] < 140 || Parent.localAI[0] >= 360)
                    {
                        CurrentAnimation = AnimationState.Normal;

                        GoToPosition(150, 300);
                    }

                    if (Parent.localAI[0] == 140)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }

                    if (Parent.localAI[0] > 140 && Parent.localAI[0] <= 300)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 5f, 250f);

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 2, (float)NPC.height / 2) * Main.rand.NextFloat(0.2f, 1.12f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.12f);
                            Main.dust[dustEffect].color = Color.Green;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (Parent.localAI[0] > 140 && Parent.localAI[0] <= 360)
                    {
                        GoToPosition(400, 0);
                    }

                    break;
                }

                case -1: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                //pollen attack
                case 0: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                //homing flowers
                case 1: 
                {
                    if (Parent.localAI[0] < 390)
                    {
                        GoToPosition(400, 0);
                    }

                    int TimeOffset = RightHand ? 15 : 0;

                    if (Parent.localAI[0] == 70)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }

                    //shoot out flowers with a delay so it alternates from left hand to right hand
                    if (Parent.localAI[0] == 90 + TimeOffset || Parent.localAI[0] == 120 + TimeOffset || Parent.localAI[0] == 150 + TimeOffset || 
                    Parent.localAI[0] == 180 + TimeOffset || Parent.localAI[0] == 210 + TimeOffset || Parent.localAI[0] == 240 + TimeOffset)
                    {
                        SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                        Vector2 ShootSpeed = NPC.Center - Parent.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 10f;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<HomingDaffodil>(), NPC.damage, 4.5f);
                    }

                    if (Parent.localAI[0] >= 320)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    //preemptively go to the position for the next attack
                    if (Parent.localAI[0] >= 390)
                    {
                        GoToPosition(5, 300, false);
                    }

                    break;
                }

                //thornball vine attacks
                case 2: 
                {
                    GoToPosition(5, 300, false);

                    if (Parent.localAI[0] == 80)
                    {
                        CurrentAnimation = AnimationState.HandOpen;
                    }
                    if (Parent.localAI[0] == 590)
                    {
                        CurrentAnimation = AnimationState.Normal;
                    }

                    break;
                }

                //corpsebloom fly attack
                case 3: 
                {
                    GoToPosition(10, 165, false);

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

                //spawn seeds out of hands
                case 4: 
                {
                    if (Parent.localAI[0] <= 50)
                    {
                        GoToPosition(300, 180);
                    }
                    else
                    {
                        GoToPosition(400, 0);
                    }

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

                //sweeping laser attack phase 1
                case 5: 
                {
                    GoToPosition(150, 300);
                    break;
                }

                //solar laser attack phase 2
                case 6: 
                {
                    GoToPosition(150, 300);
                    break;
                }
            }
        }

        public void GoToPosition(float X, float Y, bool ExtraMovement = true)
        {
            /*
            var ParentPos = target.Center - new Vector2(0, 16 * 5);
            float speed = MathHelper.Clamp(NPC.Distance(ParentPos) / 16, 0, 8);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(ParentPos) * speed, .0075f);
            */

            NPC Parent = Main.npc[(int)NPC.ai[2]];

            bool Switch = NPC.type == ModContent.NPCType<DaffodilHandRight>();

            float goToX = (!Switch ? -X : X);
            float goToY = Y;

            if (ExtraMovement)
            {
                NPC.localAI[1]++;

                if (!Switch)
                {
                    goToX += (float)Math.Sin(NPC.localAI[1] / 30) * 20;
                }
                else
                {
                    goToX -= (float)Math.Sin(NPC.localAI[1] / 30) * 20;
                }

                goToY += (float)Math.Sin(NPC.localAI[1] / 30) * 20;
            }

            Vector2 ParentPos = Parent.Center + new Vector2(goToX, goToY);
            float speed = MathHelper.Clamp(NPC.Distance(ParentPos) / 16, 0, 12);
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(ParentPos) * speed, 0.075f);
        }
    }

    public class DaffodilHandRight : DaffodilHandLeft
    {
		private static Asset<Texture2D> ArmUpperTexture;
		private static Asset<Texture2D> ArmLowerTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPC Parent = Main.npc[(int)NPC.ai[2]];

			//only draw if the parent is active
			if (Parent.active && Parent.type == ModContent.NPCType<DaffodilEye>())
			{
				ArmUpperTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArmUpper");
				ArmLowerTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArmLower");

				//get frame origins for arms
				Vector2 frameOriginUpper = (ArmUpperTexture.Size() / 2) + new Vector2(34, 11);
				Rectangle frameUpper = new Rectangle(0, 0, ArmUpperTexture.Width(), ArmUpperTexture.Height());

				Vector2 ParentCenter = Parent.Center + new Vector2(40, 20);

				//rotation from the hand to the parent daffodil body offsets
				float RotationToParent = (float)Math.Atan2(ParentCenter.Y - NPC.Center.Y, ParentCenter.X - NPC.Center.X) + 4.71f;

				//bottom of the upper arm, where the lower arm should be drawn from
				Vector2 LowerArmPos = ParentCenter + OffsetWithRotation(RotationToParent, 3, 199);

				//rotation of the upper arm from the bottom of the upper arm to the parent center offset
				float upperArmRotation = (float)Math.Atan2(LowerArmPos.Y - ParentCenter.Y, (LowerArmPos.X - 50) - ParentCenter.X) + 1.57f;

				//change lower arm position again based on the rotation of the upper arm
				LowerArmPos = ParentCenter + OffsetWithRotation(upperArmRotation, 3, 199);

				//draw upper arm segment
                Color UpperArmColor = Lighting.GetColor((int)ParentCenter.X / 16, (int)ParentCenter.Y / 16);
				spriteBatch.Draw(ArmUpperTexture.Value, ParentCenter - screenPos, null, NPC.GetAlpha(UpperArmColor),
				upperArmRotation, new Vector2(34, 11), NPC.scale, SpriteEffects.FlipHorizontally, 0);

				//lower arm rotation is based off of the bottom of the upper arm to the hand
				float lowerArmRotation = (LowerArmPos - NPC.Center).ToRotation() + MathHelper.PiOver2;

				//scale the lower arm texture based on the distance of the hand to the position so that the hand does not randomly just disconnect from the bottom of the lower arm
				Vector2 armScale = new(1, NPC.Distance(LowerArmPos) / 136f);

                //draw lower arm segment
                Color LowerArmColor = Lighting.GetColor((int)LowerArmPos.X / 16, (int)LowerArmPos.Y / 16);
				spriteBatch.Draw(ArmLowerTexture.Value, LowerArmPos - screenPos, null, NPC.GetAlpha(LowerArmColor),
				lowerArmRotation, new Vector2(29, 8), armScale, SpriteEffects.FlipHorizontally, 0);
			}

			return true;
		}
	}
}