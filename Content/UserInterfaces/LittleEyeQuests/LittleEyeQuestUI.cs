using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Quest;
using System;
using System.Collections.Generic;

namespace Spooky.Content.UserInterfaces.LittleEyeQuests;

public class LittleEyeQuestUI : ModSystem
{
	private readonly record struct TextureOptions(Asset<Texture2D> NotComplete, Asset<Texture2D> Complete, Asset<Texture2D> Locked);
	private readonly record struct FlagSet(bool IsLocked, bool IsDown, bool InProgress);
	private readonly record struct HookSet(Action Start, Action NewItem, DialogueChain.EndTrigger EndTrigger);

	internal static int Delay = 0;
	internal static int LittleEye = -1;
	internal static bool UIOpen = false;
	internal static bool IsHoveringOverAnyButton = false;

	public static readonly Vector2 PositionModifier = new(-200, -75);
	public static Vector2 UICenter = new(Main.screenWidth / 2, Main.screenHeight / 2);

	public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/TalkSounds/LittleEyeTalk", SoundType.Sound) { Volume = 3f, PitchVariance = 0.75f };

	// Quest dialogue, set once for simplicity
	private static readonly LocalizedText[] QuestConditionTexts = new LocalizedText[5];

	private static string QuestIcon5LockedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Locked");

	private static string QuestAcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccept");
	private static string QuestWarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyWarning");
	private static string Quest5AcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Accept");
	private static string Quest5WarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Warning");
	private static string QuestAcceptedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccepted");
	private static string QuestCompleteText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompleted");
	private static string QuestCompleteRematchText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompletedItem");
	private static string QuestNewItemText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyNewItem");

	// Actual icon textures
	private static readonly Asset<Texture2D>[] BountyIconDone = new Asset<Texture2D>[5];
	private static readonly Asset<Texture2D>[] BountyIconNotDone = new Asset<Texture2D>[5];

	// Misc icon textures
	private static Asset<Texture2D> BountyIconSelectedOutline;
	private static Asset<Texture2D> BountyIconLocked;
	private static Asset<Texture2D> BountyIcon5Locked;

	internal static Asset<Texture2D> UITexture;
	internal static Asset<Texture2D> ButtonTex;
	internal static Asset<Texture2D> DialogueUIPlayer;

	/// <summary>
	/// Used to allow scissor rectangles.
	/// </summary>
	private static readonly RasterizerState RasterState;

	private static string _hoverText = null;
	private static int _firstVisibleQuest = 0;
	private static float _xOffset = 0;
	private static bool _firstDraw = true;

	static LittleEyeQuestUI()
	{
		RasterState = RasterizerState.CullCounterClockwise;
		RasterState.ScissorTestEnable = true;
	}

	public override void Load()
	{
		for (int i = 0; i < 5; ++i)
		{
			int index = i + 1;
			BountyIconDone[i] = ModContent.Request<Texture2D>($"Spooky/Content/UserInterfaces/LittleEyeQuests/Icons/BountyIcon{index}Done");
			BountyIconNotDone[i] = ModContent.Request<Texture2D>($"Spooky/Content/UserInterfaces/LittleEyeQuests/Icons/BountyIcon{index}NotDone");
		}

		//misc icon textures
		BountyIconSelectedOutline ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuests/Icons/BountyIconSelectedOutline");
		BountyIconLocked ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuests/Icons/BountyIconLocked");
		BountyIcon5Locked ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuests/Icons/BountyIcon5Locked");

		UITexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye");
		ButtonTex ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuests/PageButton");
		DialogueUIPlayer ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer");
	}

	public override void PostSetupContent()
	{
		for (int i = 0; i < 5; ++i)
			QuestConditionTexts[i] = Language.GetText($"Mods.Spooky.UI.LittleEyeBounties.Bounty{i + 1}Condition");

		QuestIcon5LockedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Locked");

		QuestAcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccept");
		QuestWarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyWarning");
		Quest5AcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Accept");
		Quest5WarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Warning");

		QuestAcceptedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccepted");

		QuestCompleteText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompleted");
		QuestCompleteRematchText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompletedItem");
		QuestNewItemText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyNewItem");
	}

	//check if little eye is close enough
	public static bool InRangeOfNPC()
	{
		if (!Main.npc.IndexInRange(LittleEye) || !Main.npc[LittleEye].active)
			return false;

		Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);

		return validTalkArea.Intersects(Main.npc[LittleEye].Hitbox);
	}

	//used to draw individual icons over the main UI box
	public static void DrawIcon(Vector2 drawPos, Texture2D texture) 
		=> Main.spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);

	//check if the mouse is hovering over a specific button or UI box
	public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
	{
		var backgroundArea = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)(texture.Width * backgroundScale.X), (int)(texture.Height * backgroundScale.Y));

		if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			return true;
		else
			return false;
	}

	//check if the mouse is hovering over the UI
	public static bool IsMouseOverUIBox(Vector2 TopLeft, Texture2D texture, Vector2 scale)
	{
		var backgroundArea = new Rectangle((int)TopLeft.X - (int)(texture.Width / 2 * scale.X),
		(int)TopLeft.Y - (int)(texture.Height / 2 * scale.Y),
		(int)(texture.Width * scale.X), (int)(texture.Height * scale.Y));

		if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			return true;
		else
			return false;
	}

	public static void Draw()
	{
		UICenter.X = Main.screenWidth / 2;
		UICenter.Y = Main.screenHeight / 2 - 94;

		//dont draw at all if the UI isnt open
		if (!UIOpen)
		{
			LittleEye = -1;
			return;
		}

		if (DialogueUI.Visible && DialogueUI.Dialogue.Count > 0)
			return;

		Player player = Main.LocalPlayer;

		//stop the UI from being open if the player is doing other stuff
		if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
		{
			UIOpen = false;
			return;
		}

		if (player.controlInv)
		{
			if (player.talkNPC > -1)
			{
				player.SetTalkNPC(-1);
				Main.npcChatText = string.Empty;
			}

			LittleEye = -1;
			UIOpen = false;
		}

		Main.LocalPlayer.mouseInterface = true;
		Main.LocalPlayer.GetModPlayer<SpookyPlayer>().DisablePlayerControls = true;

		if (ModContent.GetInstance<SpookyConfig>().DialogueFocus)
			Main.instance.CameraModifiers.Add(new CameraPanning(Main.npc[LittleEye].Center, 20));

		Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestUIBar").Value;
		Vector2 scale = Vector2.One * Main.UIScale;
		int backWidth = UIBoxTexture.Width;

		//draw the main UI box
		Main.spriteBatch.Draw(UIBoxTexture, UICenter, null, Color.White, 0f, UIBoxTexture.Size() / 2, scale, SpriteEffects.None, 0f);

		if (LittleEyeCrossmod.QuestsByMod.Count > 0)
		{
			Texture2D tex = ButtonTex.Value;
			Vector2 origin = UIBoxTexture.Size() * new Vector2(0.5f, 0);
			int yOff = -UIBoxTexture.Height / 2;

			// Left arrow
			bool canClick = _firstVisibleQuest > 0;
			Vector2 pos = UICenter + new Vector2(-38, yOff);
			bool hover = new Rectangle((int)pos.X - 218, (int)pos.Y, 32, 90).Contains(Main.MouseScreen.ToPoint()) && canClick;
			var src = new Rectangle(hover ? 34 : 0, 0, 32, 90);

			if (Main.mouseLeftRelease && Main.mouseLeft && hover && canClick)
				_firstVisibleQuest--;

			Main.spriteBatch.Draw(tex, pos, src, canClick ? Color.White : Color.Gray, 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);

			canClick = _firstVisibleQuest < 1;
			pos = UICenter + new Vector2(backWidth + 4, yOff);
			hover = new Rectangle((int)pos.X - 218, (int)pos.Y, 32, 90).Contains(Main.MouseScreen.ToPoint()) && canClick;
			src = new Rectangle(hover ? 34 : 0, 0, 32, 90);

			if (Main.mouseLeftRelease && Main.mouseLeft && hover && canClick)
				_firstVisibleQuest++;

			Main.spriteBatch.Draw(tex, pos, src, canClick ? Color.White : Color.Gray, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		Main.spriteBatch.End();

		int backHeight = UIBoxTexture.Height;
		Rectangle priorRectangle = Main.instance.GraphicsDevice.ScissorRectangle;
		Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle((int)(UICenter.X - backWidth / 2 * scale.X) + 8, (int)UICenter.Y - (int)(backHeight / 2 * scale.Y), backWidth - 16, backHeight);

		UICenter.X -= _xOffset;
		_xOffset = MathHelper.Lerp(_xOffset, _firstVisibleQuest * 86, 0.2f);

		Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, RasterState, null, Main.UIScaleMatrix);

		//prevent any mouse interactions while the mouse is hovering over this UI
		if (IsMouseOverUIBox(UICenter, UIBoxTexture, scale))
		{
			IsHoveringOverAnyButton = false;

			player.mouseInterface = true;
		}

		var buttonTopLeft = (UICenter - UIBoxTexture.Size() / 2f * scale).ToPoint();

		if (Delay <= 20 && !Main.mouseLeft)
			Delay++;

		Quest1Logic(player, buttonTopLeft); // Frank the Goblin
		Quest2Logic(player, buttonTopLeft += new Point(84, 0)); // Tome of the Spirits
		Quest3Logic(player, buttonTopLeft += new Point(84, 0)); // Spider Grotto
		Quest4Logic(player, buttonTopLeft += new Point(84, 0)); // Eye Wizard
		OrroboroLogic(player, scale, buttonTopLeft += new Point(84, 0)); // Orroboro

		// Also for some reason, on first draw the scissor rectangle doesn't work properly.
		// This avoids a visual issue when opening the UI for the first time.
		if (!_firstDraw)
		{
			// For some reason this was hardcoded into the quest logic, didn't care to remove it, do it here
			buttonTopLeft.X += 315;
			buttonTopLeft.Y -= 24;

			foreach (List<CrossmodQuest> quests in LittleEyeCrossmod.QuestsByMod.Values)
			{
				foreach (CrossmodQuest quest in quests)
				{
					buttonTopLeft.X += 84;
					HandleCustomQuest(player, buttonTopLeft, quest);
				}
			}
		}

		_firstDraw = false;

		Main.spriteBatch.End();
		Main.instance.GraphicsDevice.ScissorRectangle = priorRectangle;
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

		if (_hoverText is not null)
		{
			Vector2 position = UICenter + new Vector2(10 + _xOffset, -40);
			ReLogic.Graphics.DynamicSpriteFont font = FontAssets.DeathText.Value;
			Vector2 size = ChatManager.GetStringSize(font, _hoverText, Vector2.One);
			size.Y = 0;
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, _hoverText, position, Color.White, 0f, size / 2f, new(0.35f));
		}

		_hoverText = null;
	}

	public static void HandleCustomQuest(Player player, Point buttonTopLeft, CrossmodQuest quest)
	{
		int y = 164;
		bool locked = !quest.IsLocked?.Invoke() is true;

		if (!quest.CompleteCheck())
			y = 82;
		else if (!locked)
			y = 0;

		Texture2D tex = quest.Icon.Value;
		Rectangle hitbox = new Rectangle(buttonTopLeft.X, buttonTopLeft.Y, (int)(80 * Main.UIScale), (int)(80 * Main.UIScale));
		var drawPosition = buttonTopLeft.ToVector2();
		Main.spriteBatch.Draw(tex, drawPosition, new Rectangle(0, y, 80, 80), Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);

		if (hitbox.Contains(Main.MouseScreen.ToPoint()))
		{
			IsHoveringOverAnyButton = false;
			player.mouseInterface = true;

			if (locked)
				return;

			DrawIcon(drawPosition, BountyIconSelectedOutline.Value);
			bool inBounds = Main.instance.GraphicsDevice.ScissorRectangle.Contains(Main.MouseScreen.ToPoint());

			// Post-complete refight
			if (quest.CompleteCheck())
			{
				if (inBounds && Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
				{
					DialogueChain chain = quest.RecoverChain();
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

					UIOpen = false;
				}
			}
			else
			{
				if (inBounds && Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
				{
					DialogueChain chain = quest.IsActive() ? quest.RecoverChain() : quest.Chain();
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

					UIOpen = false;
				}
			}
		}
	}

	private static void OrroboroLogic(Player player, Vector2 scale, Point topLeft)
	{
		Vector2 Icon5TopLeft = topLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

		bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
		DrawIcon(Icon5TopLeft, !downedAllMechs ? BountyIcon5Locked.Value : Flags.downedOrroboro ? BountyIconDone[4].Value : BountyIconNotDone[4].Value);
		bool inBounds = Main.instance.GraphicsDevice.ScissorRectangle.Contains(Main.MouseScreen.ToPoint());

		if (IsMouseOverUI(Icon5TopLeft, BountyIconDone[4].Value, scale))
		{
			IsHoveringOverAnyButton = true;

			if (!downedAllMechs)
				DrawIcon(Icon5TopLeft, BountyIconSelectedOutline.Value);
			else if (Flags.downedOrroboro)
			{
				if (inBounds && Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<Concoction>()))
				{
					DialogueChain chain = new();
					chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, PositionModifier, true));
					chain.OnPlayerResponseTrigger += PlayerResponse;
					chain.OnEndTrigger += EndDialogueQuestAccept5;
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

					UIOpen = false;
				}
			}
			//display the actual quest text if you havent killed orro-boro but you killed the mechs
			else if (inBounds && Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
			{
				//quest accept dialogue
				if (!Flags.downedOrroboro)
					EyeQuestDialogue.OrroborroDialogue(PositionModifier);

				UIOpen = false;
			}
		}
	}

	private static void Quest4Logic(Player player, Point ButtonTopLeft)
	{
		bool locked = !Flags.BountyInProgress4 && (Flags.BountyInProgress1 || Flags.BountyInProgress2 || Flags.BountyInProgress3);
		var options = new TextureOptions(BountyIconNotDone[3], BountyIconDone[3], BountyIconLocked);
		var flags = new FlagSet(locked, Flags.LittleEyeBounty4, Flags.BountyInProgress4);
		var hooks = new HookSet(EyeQuestDialogue.BountyFour, EyeQuestDialogue.BountyFourNewItem, AcceptFour);

		QuestLogic<SummonItem4>(player, ButtonTopLeft, flags, options, QuestConditionTexts[3].Value, Color.HotPink, hooks);
	}

	private static void Quest3Logic(Player player, Point ButtonTopLeft)
	{
		bool locked = !Flags.BountyInProgress3 && (Flags.BountyInProgress1 || Flags.BountyInProgress2 || Flags.BountyInProgress4);
		var options = new TextureOptions(BountyIconNotDone[2], BountyIconDone[2], BountyIconLocked);
		var flags = new FlagSet(locked, Flags.LittleEyeBounty3, Flags.BountyInProgress3);
		var hooks = new HookSet(EyeQuestDialogue.BountyThree, EyeQuestDialogue.BountyThreeNewItem, AcceptThree);

		QuestLogic<SummonItem3>(player, ButtonTopLeft, flags, options, QuestConditionTexts[2].Value, Color.Chocolate, hooks);
	}

	private static void Quest2Logic(Player player, Point ButtonTopLeft)
	{
		bool locked = !Flags.BountyInProgress2 && (Flags.BountyInProgress1 || Flags.BountyInProgress3 || Flags.BountyInProgress4);
		var options = new TextureOptions(BountyIconNotDone[1], BountyIconDone[1], BountyIconLocked);
		var flags = new FlagSet(locked, Flags.LittleEyeBounty2, Flags.BountyInProgress2);
		var hooks = new HookSet(EyeQuestDialogue.BountyTwo, EyeQuestDialogue.BountyTwoNewItem, AcceptTwo);

		QuestLogic<SummonItem2>(player, ButtonTopLeft, flags, options, QuestConditionTexts[1].Value, Color.SeaGreen, hooks);
	}

	private static void Quest1Logic(Player player, Point buttonTopLeft)
	{
		bool locked = !Flags.BountyInProgress1 && (Flags.BountyInProgress2 || Flags.BountyInProgress3 || Flags.BountyInProgress4);
		var options = new TextureOptions(BountyIconNotDone[0], BountyIconDone[0], BountyIconLocked);
		var flags = new FlagSet(locked, Flags.LittleEyeBounty1, Flags.BountyInProgress1);
		var hooks = new HookSet(EyeQuestDialogue.BountyOne, EyeQuestDialogue.BountyOneNewItem, AcceptOne);

		QuestLogic<SummonItem1>(player, buttonTopLeft, flags, options, QuestConditionTexts[0].Value, Color.OrangeRed, hooks);
	}

	/// <summary>
	/// Manages common quest logic for the four base quests - excluding Orroboro - keeping logic all in one call.
	/// </summary>
	private static void QuestLogic<T>(Player player, Point uiTopLeft, FlagSet flags, TextureOptions textures, string conditionText, Color acceptColor, HookSet hooks) where T : ModItem
	{
		Vector2 scale = Vector2.One * Main.UIScale;
		Vector2 topLeft = uiTopLeft.ToVector2();
		Texture2D icon = (flags.IsLocked ? textures.Locked : flags.IsDown ? textures.Complete : textures.NotComplete).Value;
		DrawIcon(topLeft, icon);

		if (IsMouseOverUI(topLeft, textures.Complete.Value, scale))
		{
			IsHoveringOverAnyButton = true;

			bool inBounds = Main.instance.GraphicsDevice.ScissorRectangle.Contains(Main.MouseScreen.ToPoint());

			if (flags.IsLocked)
			{
				_hoverText = QuestAcceptedText;
				DrawIcon(topLeft, BountyIconSelectedOutline.Value);
			}
			else if (flags.IsDown || Flags.PokedLittleEye)
			{
				//give the player the item again if they wish to rematch the miniboss
				if (inBounds && Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<T>()))
				{
					DialogueChain chain = new();
					chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, PositionModifier, true));
					chain.OnPlayerResponseTrigger += PlayerResponse;
					chain.OnEndTrigger += hooks.EndTrigger;
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

					UIOpen = false;
				}
			}
			else
			{
				//accept bounty
				if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && inBounds)
				{
					//quest accept dialogue
					if (!flags.InProgress)
						hooks.Start();
					//if the player needs a new item
					else
						hooks.NewItem();

					UIOpen = false;
				}
			}
		}
	}

	/// <summary>
	/// Spawns a <typeparamref name="T"/> item on the local player, syncs it, updates the quest state and hides the UI.
	/// </summary>
	internal static void QuestAcceptAndEnd<T>(SpookyMessageType bountyType, ref bool flag) where T : ModItem
	{
		Player player = Main.LocalPlayer;
		int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem1>());

		if (Main.netMode != NetmodeID.SinglePlayer)
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

		if (!Flags.LittleEyeBounty1)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Spooky.mod.GetPacket();
				packet.Write((byte)bountyType);
				packet.Send();
			}
			else
				flag = true;
		}

		DialogueUI.Visible = false;
	}

	public static void AcceptOne(Dialogue dialogue, int ID) => QuestAcceptAndEnd<SummonItem1>(SpookyMessageType.BountyAccepted1, ref Flags.BountyInProgress1);
	public static void AcceptTwo(Dialogue dialogue, int ID) => QuestAcceptAndEnd<SummonItem2>(SpookyMessageType.BountyAccepted2, ref Flags.BountyInProgress2);
	public static void AcceptThree(Dialogue dialogue, int ID) => QuestAcceptAndEnd<SummonItem3>(SpookyMessageType.BountyAccepted3, ref Flags.BountyInProgress3);
	public static void AcceptFour(Dialogue dialogue, int ID) => QuestAcceptAndEnd<SummonItem4>(SpookyMessageType.BountyAccepted4, ref Flags.BountyInProgress4);

	public static void EndDialogueQuestAccept5(Dialogue dialogue, int ID)
	{
		int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<Concoction>());
		NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

		DialogueUI.Visible = false;
	}

	public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
	{
		Dialogue newDialogue = new(DialogueUIPlayer.Value, Main.LocalPlayer, Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
		DialogueUI.Visible = true;
		DialogueUI.Add(newDialogue);
	}

	internal static void Open()
	{
		UIOpen = true;
		_firstVisibleQuest = 0;
	}
}