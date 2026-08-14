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
using Terraria.ModLoader.UI;
using tModPorter;

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
	public static Vector2 UITopLeft = new(Main.screenWidth / 2, Main.screenHeight / 2);

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

	private static string _hoverText = null;
	private static int _firstVisibleQuest = 0;
	private static float _xOffset = 0;

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

	public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Condition, string Accept, string Warning, Color ConditionColor)
	{
		return;
		Vector2 scale = new Vector2(1f, 1.025f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

		//first draw the condition text for the biome you find the miniboss in
		foreach (string TextLine in Utils.WordwrapString(Condition, FontAssets.MouseText.Value, 600, 16, out _))
		{
			if (string.IsNullOrEmpty(TextLine))
				continue;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, ConditionColor, 0f, Vector2.Zero, scale);
			TextTopLeft.Y += Main.UIScale * 16f;
		}

		//draw the text to tell players they have to click the button to accept the bounty
		foreach (string TextLine in Utils.WordwrapString(Accept, FontAssets.MouseText.Value, 600, 16, out _))
		{
			if (string.IsNullOrEmpty(TextLine))
				continue;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Lime, 0f, Vector2.Zero, scale);
			TextTopLeft.Y += Main.UIScale * 16f;
		}

		//finally display the warning that you cant accept another bounty until the selected one is done
		foreach (string TextLine in Utils.WordwrapString(Warning, FontAssets.MouseText.Value, 600, 16, out _))
		{
			if (string.IsNullOrEmpty(TextLine))
				continue;

			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Red, 0f, Vector2.Zero, scale);
			TextTopLeft.Y += Main.UIScale * 16f;
		}
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
		//dont draw at all if the UI isnt open
		if (!UIOpen)
		{
			LittleEye = -1;
			return;
		}

		UITopLeft.X = Main.screenWidth / 2;

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
		Main.spriteBatch.Draw(UIBoxTexture, UITopLeft - new Vector2(0, 94), null, Color.White, 0f, UIBoxTexture.Size() / 2, scale, SpriteEffects.None, 0f);

		if (LittleEyeCrossmod.QuestsByMod.Count > 0)
		{
			Texture2D tex = ButtonTex.Value;
			Vector2 origin = UIBoxTexture.Size() * new Vector2(0.5f, 0);
			int yOff = -139;

			// Left arrow
			bool canClick = _firstVisibleQuest > 0;
			Vector2 pos = UITopLeft + new Vector2(-38, yOff);
			bool hover = new Rectangle((int)pos.X - 218, (int)pos.Y, 32, 90).Contains(Main.MouseScreen.ToPoint());
			var src = new Rectangle(hover ? 34 : 0, 0, 32, 90);

			if (Main.mouseLeftRelease && Main.mouseLeft && hover && canClick)
				_firstVisibleQuest--;

			Main.spriteBatch.Draw(tex, pos, src, canClick ? Color.White : Color.Gray, 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);

			canClick = _firstVisibleQuest < 1;
			pos = UITopLeft + new Vector2(backWidth + 4, yOff);
			hover = new Rectangle((int)pos.X - 218, (int)pos.Y, 32, 90).Contains(Main.MouseScreen.ToPoint());
			src = new Rectangle(hover ? 34 : 0, 0, 32, 90);

			if (Main.mouseLeftRelease && Main.mouseLeft && hover && canClick)
				_firstVisibleQuest++;

			Main.spriteBatch.Draw(tex, pos, src, canClick ? Color.White : Color.Gray, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		Main.spriteBatch.End();

		int backHeight = UIBoxTexture.Height;
		Rectangle priorRectangle = Main.instance.GraphicsDevice.ScissorRectangle;
		Main.instance.GraphicsDevice.ScissorRectangle = new Rectangle((int)UITopLeft.X - backWidth / 2 + 8, (int)UITopLeft.Y - 94 - backHeight / 2, backWidth - 16, backHeight);
		Main.Rasterizer.ScissorTestEnable = true;

		UITopLeft.X -= _xOffset;
		_xOffset = MathHelper.Lerp(_xOffset, _firstVisibleQuest * 86, 0.2f);

		Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, Main.Rasterizer, null, Main.UIScaleMatrix);

		//prevent any mouse interactions while the mouse is hovering over this UI
		if (IsMouseOverUIBox(UITopLeft, UIBoxTexture, scale))
		{
			IsHoveringOverAnyButton = false;

			player.mouseInterface = true;
		}

		var buttonTopLeft = (UITopLeft + new Vector2(-525f, -110f) * scale).ToPoint();

		if (Delay <= 20 && !Main.mouseLeft)
			Delay++;

		Quest1Logic(player, buttonTopLeft); // Frank the Goblin
		Quest2Logic(player, buttonTopLeft += new Point(84, 0)); // Tome of the Spirits
		Quest3Logic(player, buttonTopLeft += new Point(84, 0)); // Spider Grotto
		Quest4Logic(player, buttonTopLeft += new Point(84, 0)); // Eye Wizard
		OrroboroLogic(player, scale, buttonTopLeft += new Point(84, 0)); // Orroboro

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

		Main.spriteBatch.End();
		Main.instance.GraphicsDevice.ScissorRectangle = priorRectangle;
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

		if (_hoverText is not null)
		{
			Vector2 position = UITopLeft + new Vector2(10, -40);
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

		if (quest.CompleteCheck())
			y = 82;
		else if (!quest.IsLocked?.Invoke() is not true)
			y = 0;

		Texture2D tex = quest.Icon.Value;
		Rectangle hitbox = new Rectangle(buttonTopLeft.X, buttonTopLeft.Y, (int)(80 * Main.UIScale), (int)(80 * Main.UIScale));
		var drawPosition = buttonTopLeft.ToVector2();
		Main.spriteBatch.Draw(tex, drawPosition, new Rectangle(0, y, 80, 80), Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);

		if (hitbox.Contains(Main.MouseScreen.ToPoint()))
		{
			DrawIcon(drawPosition, BountyIconSelectedOutline.Value);

			IsHoveringOverAnyButton = false;
			player.mouseInterface = true;
		}
	}

	private static void OrroboroLogic(Player player, Vector2 scale, Point topLeft)
	{
		Vector2 Icon5TopLeft = topLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

		bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
		DrawIcon(Icon5TopLeft, !downedAllMechs ? BountyIcon5Locked.Value : Flags.downedOrroboro ? BountyIconDone[4].Value : BountyIconNotDone[4].Value);

		if (IsMouseOverUI(Icon5TopLeft, BountyIconDone[4].Value, scale))
		{
			IsHoveringOverAnyButton = true;
			DrawIcon(Icon5TopLeft, BountyIconSelectedOutline.Value);

			if (!downedAllMechs)
				DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestIcon5LockedText, string.Empty, string.Empty, Color.Red);
			else if (Flags.downedOrroboro)
			{
				DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

				if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<Concoction>()))
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
			else
			{
				DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestConditionTexts[4].Value, Quest5AcceptText, Quest5WarningText, Color.Magenta);

				//accept bounty (this specific bounty does not need to set the bounty accepted bool to true)
				if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
				{
					//quest accept dialogue
					if (!Flags.downedOrroboro)
						EyeQuestDialogue.OrroborroDialogue(PositionModifier);

					UIOpen = false;
				}
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
		Vector2 topLeft = uiTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;
		Texture2D icon = (flags.IsLocked ? textures.Locked : flags.IsDown ? textures.Complete : textures.NotComplete).Value;
		DrawIcon(topLeft, icon);

		if (IsMouseOverUI(topLeft, textures.Complete.Value, scale))
		{
			IsHoveringOverAnyButton = true;

			DrawIcon(topLeft, BountyIconSelectedOutline.Value);

			if (flags.IsLocked)
				_hoverText = QuestAcceptedText;
			//DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
			else if (flags.IsDown || Flags.PokedLittleEye)
			{
				DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

				//give the player the item again if they wish to rematch the miniboss
				if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<T>()))
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
				if (!flags.InProgress)
					DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, conditionText, QuestAcceptText, QuestWarningText, acceptColor);
				else
					DrawTextDescription(Main.spriteBatch, UITopLeft + new Vector2(-257f, -30f) * scale, QuestNewItemText, string.Empty, string.Empty, Color.White);

				//accept bounty
				if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
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