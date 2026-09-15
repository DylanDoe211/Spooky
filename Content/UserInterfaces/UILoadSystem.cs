using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.UserInterfaces.LittleEyeQuests;

namespace Spooky.Content.UserInterfaces;

[Autoload(Side = ModSide.Client)]
public class UILoadSystem : ModSystem
{
	public UserInterface TextLayer;
	public DialogueUI TextBox;

	public override void Load()
	{
		TextLayer = new UserInterface();
		TextBox = new DialogueUI();
		TextLayer.SetState(TextBox);
	}

	public override void ClearWorld()
	{
		if (!Main.dedServ || !DialogueUI.Visible)
			DialogueUI.Clear();
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int resourceBarIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
		if (resourceBarIndex != -1)
		{
			//snotty schnoz UI
			layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Snotty Schnoz UI", () =>
			{
				MocoNoseBar.Draw(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.None));

			//stoned kidney UI
			layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Stoned Kidney UI", () =>
			{
				StonedKidneyBar.Draw(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.None));

			//krampus chimney UI
			layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Backpack Chimney UI", () =>
			{
				KrampusChimneyBar.Draw(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.None));
		}

		int mouseTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
		if (mouseTextIndex != -1)
		{
			//rotten depths lab UI
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Rotten Depths Lab Email UI", () =>
			{
				RottenDepthsEmailUI.Draw(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.None));

			//little eye bounty UI
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Bounty UI", () =>
				{
					LittleEyeQuestUI.Draw();
					return true;
				},
				InterfaceScaleType.None));

			//little eye dialogue UI
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Dialogue Choice UI", () =>
				{
					LittleEyeDialogueChoiceUI.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));

			//old hunter dialogue UI
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Old Hunter Dialogue Choice UI", () =>
				{
					OldHunterDialogueChoiceUI.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));
		}

		int inGameOptionsIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Ingame Options");
		if (inGameOptionsIndex != -1)
		{
			//bloom buff UI
			layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Bloom Buffs UI", () =>
			{
				BloomBuffUI.Draw(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.None));

			//new years resolution UI
			layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("New Years Resolution UI", () =>
				{
					KrampusResolutionUI.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));

			//dialogue UI
			layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Dialogue UI",
			delegate
			{
				if (DialogueUI.Visible)
				{
					TextLayer.Update(Main._drawInterfaceGameTime);
					TextBox.Draw(Main.spriteBatch);
				}

				return true;
			}, InterfaceScaleType.None));
		}

		if (EggEventWorld.EggEventActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
		{
			int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
			LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Egg Event UI",
			delegate
			{
				DrawEggEventUI(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.UI);

			layers.Insert(EventIndex, NewLayer);
		}

		if (PandoraBoxWorld.PandoraEventActive)
		{
			int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
			LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Pandora's Box UI",
			delegate
			{
				DrawPandoraBoxUI(Main.spriteBatch);
				return true;
			},
			InterfaceScaleType.UI);

			layers.Insert(EventIndex, NewLayer);
		}
	}

	public void DrawPandoraBoxUI(SpriteBatch spriteBatch)
	{
		const float Scale = 0.875f;
		const float Alpha = 0.5f;
		const int InternalOffset = 6;
		const int OffsetX = 20;
		const int OffsetY = 20;

		Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/PandoraBoxIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		Color NameBoxColor = new Color(39, 45, 90);
		Color ProgressBarColor1 = new Color(87, 60, 42);
		Color ProgressBarColor2 = new Color(35, 165, 161);

		int width = (int)(200f * Scale);
		int height = (int)(46f * Scale);

		Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
		Utils.DrawInvBG(spriteBatch, ProgressBackground, new Color(39, 45, 90, 255) * 0.785f);

		float divide = 0.05f;

		string ProgressText = Language.GetTextValue("Mods.Spooky.UI.PandoraBox.PandoraBoxBarProgress") + (PandoraBoxWorld.Wave + 1);
		Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + 5), Color.White, Scale, 0.5f, -0.1f);
		Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

		var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp((PandoraBoxWorld.Wave + 1) / divide, 0f, 100f)), TextureAssets.ColorBar.Height());
		var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
		spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, ProgressBarColor1 * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, ProgressBarColor2, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);

		Vector2 descSize = new Vector2(175, 40) * Scale;
		Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
		Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
		Utils.DrawInvBG(spriteBatch, descBackground, NameBoxColor * Alpha);

		int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
		var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * Scale), (int)(32 * Scale));
		spriteBatch.Draw(EventIcon, icon, Color.White);
		Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.Spooky.UI.PandoraBox.PandoraBoxBarDisplayName"), new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
	}

	public void DrawEggEventUI(SpriteBatch spriteBatch)
	{
		const float Scale = 0.875f;
		const float Alpha = 0.65f;
		const int InternalOffset = 6;
		const int OffsetX = 20;
		const int OffsetY = 20;

		Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/EggEventIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		Color NameBoxColor = new Color(113, 35, 206);
		Color ProgressBoxColor = new Color(113, 35, 206);
		Color ProgressBarColor1 = new Color(54, 35, 35);
		Color ProgressBarColor2 = new Color(108, 248, 71);

		int width = (int)(200f * Scale);
		int height = (int)(46f * Scale);

		Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
		Utils.DrawInvBG(spriteBatch, ProgressBackground, ProgressBoxColor * Alpha);

		float divide = 3.6f;

		float timeLeft = EggEventWorld.EventTimeLeft / 60;
		float timeLeftUI = EggEventWorld.EventTimeLeftUI / 60;

		TimeSpan time = TimeSpan.FromSeconds(timeLeftUI);
		string actualTime = string.Format("{0:D1}:{1:D2}", time.Minutes, time.Seconds);

		string ProgressText = Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarProgress") + " " + actualTime;

		Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y), Color.White, Scale, 0.5f, -0.1f);
		Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

		var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp((timeLeft / divide), 0f, 100f)), TextureAssets.ColorBar.Height());
		var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
		spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, ProgressBarColor1 * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, ProgressBarColor2, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);

		Vector2 descSize = new Vector2(175, 40) * Scale;
		Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
		Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
		Utils.DrawInvBG(spriteBatch, descBackground, NameBoxColor * Alpha);

		int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
		var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(28 * Scale), (int)(32 * Scale));
		spriteBatch.Draw(EventIcon, icon, Color.White);
		Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarDisplayName"), new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
	}
}