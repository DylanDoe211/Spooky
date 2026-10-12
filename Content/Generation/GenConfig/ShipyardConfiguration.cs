namespace Spooky.Content.Generation.GenConfig;

public class ShipyardConfiguration
{
	[BindToGenConfig(1, 20, 1, true, true, true)]
	public static int CaveWaterChance { get; set; } = 2;

	[BindToGenConfig(1, 400, 1, true, true, true)]
	public static int MossOvalChance { get; set; } = 160;

	[BindToGenConfig(1, 20, 1, true)]
	public static int MossOvalSizeMin { get; set; } = 10;

	[BindToGenConfig(1, 20, 1, true)]
	public static int MossOvalSizeRange { get; set; } = 8;

	[BindToGenConfig(15, 150, 1, true, true, true)]
	public static int StructureChance { get; set; } = 35;

	[BindToGenConfig(3, 20, 1, true, true, true)]
	public static int BigStructureChance { get; set; } = 8;

	[BindToGenConfig(1, 150, 1, true, true, true)]
	public static int FenceChance { get; set; } = 55;

	[BindToGenConfig(1, 40, 1, true, true, true)]
	public static int GraveChance { get; set; } = 2;

	[BindToGenConfig(1, 120, 1, true, true, true)]
	public static int GiantChestChance { get; set; } = 20;

	[BindToGenConfig(1, 60, 1, true, true, true)]
	public static int CoralTreeChance { get; set; } = 2;

	[BindToGenConfig(1, 60, 1, true, true, true)]
	public static int MangroveTreeChance { get; set; } = 5;

	[BindToGenConfig(1, 120, 1, true, true, true)]
	public static int ShellChance { get; set; } = 12;

	[BindToGenConfig(1, 100, 1, true, true, true)]
	public static int AnchorChance { get; set; } = 5;

	[BindToGenConfig(1, 50, 1, true, true, true)]
	public static int VineChance { get; set; } = 2;

	[BindToGenConfig(1, 50, 1, true, true, true)]
	public static int StalactiteChance { get; set; } = 2;

	[BindToGenConfig(2, 200, 1, true, true, true)]
	public static int GiantCoralChance { get; set; } = 8;

	[BindToGenConfig(1, 80, 1, true, true, true)]
	public static int CoralChance { get; set; } = 2;

	[BindToGenConfig(1, 80, 1, true, true, true)]
	public static int StarfishChance { get; set; } = 5;

	[BindToGenConfig(1, 80, 1, true, true, true)]
	public static int SandPileChance { get; set; } = 3;

	[BindToGenConfig(1, 40, 1, true, true, true)]
	public static int RockPileChance { get; set; } = 4;

	[BindToGenConfig(1, 30, 1, true, true, true)]
	public static int PebbleChance { get; set; } = 3;

	[BindToGenConfig(1, 50, 1, true, true, true)]
	public static int SeaOatChance { get; set; } = 2;

	[BindToGenConfig(1, 30, 1, true, true, true)]
	public static int PotChance { get; set; } = 2;

	[BindToGenConfig(3, 400, 1, true, true, true)]
	public static int ShelfCoralSmallChance { get; set; } = 150;

	[BindToGenConfig(3, 400, 1, true, true, true)]
	public static int ShelfCoralBigChance { get; set; } = 135;
}
