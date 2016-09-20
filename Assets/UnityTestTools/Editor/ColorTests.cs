using UnityEngine;
using System;
using NUnit.Framework;
using MW_DiceGame;

[TestFixture]
[Category ("Color Tests")]
internal class ColorTests {

	[Test]
	[Category ("GetColor Tests")]
	public void GetColorTest () {
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Black.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Blue.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Empty.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Green.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Purple.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Red.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.White.GetColor ());
		Assert.AreNotEqual (UnityEngine.Color.clear, Colors.Yellow.GetColor ());
	}

	[Test]
	[Category ("GetDiceMaterial Tests")]
	public void GetDiceMaterialTest () {
		Assert.NotNull (Colors.Black.GetDiceMaterial ());
		Assert.NotNull (Colors.Blue.GetDiceMaterial ());
		Assert.NotNull (Colors.Empty.GetDiceMaterial ());
		Assert.NotNull (Colors.Green.GetDiceMaterial ());
		Assert.NotNull (Colors.Purple.GetDiceMaterial ());
		Assert.NotNull (Colors.Red.GetDiceMaterial ());
		Assert.NotNull (Colors.White.GetDiceMaterial ());
		Assert.NotNull (Colors.Yellow.GetDiceMaterial ());
	}

	[Test]
	[Category ("GetDieFaceImage Tests")]
	public void GetDieFaceImageTest () {
		Assert.Null (Colors.Black.GetDieFaceImage (0));
		Assert.NotNull (Colors.Black.GetDieFaceImage (1));
		Assert.NotNull (Colors.Black.GetDieFaceImage (2));
		Assert.NotNull (Colors.Black.GetDieFaceImage (3));
		Assert.NotNull (Colors.Black.GetDieFaceImage (4));
		Assert.NotNull (Colors.Black.GetDieFaceImage (5));
		Assert.NotNull (Colors.Black.GetDieFaceImage (6));
		Assert.Null (Colors.Black.GetDieFaceImage (7));

		Assert.Null (Colors.Blue.GetDieFaceImage (0));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (1));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (2));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (3));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (4));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (5));
		Assert.NotNull (Colors.Blue.GetDieFaceImage (6));
		Assert.Null (Colors.Blue.GetDieFaceImage (7));

		Assert.Null (Colors.Empty.GetDieFaceImage (0));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (1));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (2));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (3));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (4));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (5));
		Assert.NotNull (Colors.Empty.GetDieFaceImage (6));
		Assert.Null (Colors.Empty.GetDieFaceImage (7));

		Assert.Null (Colors.Green.GetDieFaceImage (0));
		Assert.NotNull (Colors.Green.GetDieFaceImage (1));
		Assert.NotNull (Colors.Green.GetDieFaceImage (2));
		Assert.NotNull (Colors.Green.GetDieFaceImage (3));
		Assert.NotNull (Colors.Green.GetDieFaceImage (4));
		Assert.NotNull (Colors.Green.GetDieFaceImage (5));
		Assert.NotNull (Colors.Green.GetDieFaceImage (6));
		Assert.Null (Colors.Green.GetDieFaceImage (7));

		Assert.Null (Colors.Purple.GetDieFaceImage (0));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (1));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (2));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (3));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (4));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (5));
		Assert.NotNull (Colors.Purple.GetDieFaceImage (6));
		Assert.Null (Colors.Purple.GetDieFaceImage (7));

		Assert.Null (Colors.Red.GetDieFaceImage (0));
		Assert.NotNull (Colors.Red.GetDieFaceImage (1));
		Assert.NotNull (Colors.Red.GetDieFaceImage (2));
		Assert.NotNull (Colors.Red.GetDieFaceImage (3));
		Assert.NotNull (Colors.Red.GetDieFaceImage (4));
		Assert.NotNull (Colors.Red.GetDieFaceImage (5));
		Assert.NotNull (Colors.Red.GetDieFaceImage (6));
		Assert.Null (Colors.Red.GetDieFaceImage (7));

		Assert.Null (Colors.White.GetDieFaceImage (0));
		Assert.NotNull (Colors.White.GetDieFaceImage (1));
		Assert.NotNull (Colors.White.GetDieFaceImage (2));
		Assert.NotNull (Colors.White.GetDieFaceImage (3));
		Assert.NotNull (Colors.White.GetDieFaceImage (4));
		Assert.NotNull (Colors.White.GetDieFaceImage (5));
		Assert.NotNull (Colors.White.GetDieFaceImage (6));
		Assert.Null (Colors.White.GetDieFaceImage (7));

		Assert.Null (Colors.Yellow.GetDieFaceImage (0));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (1));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (2));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (3));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (4));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (5));
		Assert.NotNull (Colors.Yellow.GetDieFaceImage (6));
		Assert.Null (Colors.Yellow.GetDieFaceImage (7));
	}

	[Test]
	[Category ("GetIndex Tests")]
	public void GetIndexTest () {
		Debug.Log (Colors.Yellow.GetIndex ());
		Assert.AreEqual (Colors.Yellow.GetIndex (), 0);
		Assert.AreEqual (Colors.Blue.GetIndex (), 1);
		Assert.AreEqual (Colors.Red.GetIndex (), 2);
		Assert.AreEqual (Colors.Green.GetIndex (), 3);
		Assert.AreEqual (Colors.Purple.GetIndex (), 4);
		Assert.AreEqual (Colors.Black.GetIndex (), 5);
		Assert.AreEqual (Colors.White.GetIndex (), 6);
		Assert.AreEqual (Colors.Empty.GetIndex (), 7);
	}

	[Test]
	[Category ("Parse Tests")]
	public void ParseTest () {
		Assert.AreEqual (Colors.Yellow, ColorMethods.Parse (0));
		Assert.AreEqual (Colors.Blue, ColorMethods.Parse (1));
		Assert.AreEqual (Colors.Red, ColorMethods.Parse (2));
		Assert.AreEqual (Colors.Green, ColorMethods.Parse (3));
		Assert.AreEqual (Colors.Purple, ColorMethods.Parse (4));
		Assert.AreEqual (Colors.Black, ColorMethods.Parse (5));
		Assert.AreEqual (Colors.White, ColorMethods.Parse (6));
		Assert.AreEqual (Colors.Empty, ColorMethods.Parse (7));
	}

	[Test]
	[Category ("NextColor Tests")]
	public void NextColorTest () {
		Assert.AreEqual (Colors.Blue, ColorMethods.NextColor (Colors.Yellow));
		Assert.AreEqual (Colors.Red, ColorMethods.NextColor (Colors.Blue));
		Assert.AreEqual (Colors.Green, ColorMethods.NextColor (Colors.Red));
		Assert.AreEqual (Colors.Purple, ColorMethods.NextColor (Colors.Green));
		Assert.AreEqual (Colors.Black, ColorMethods.NextColor (Colors.Purple));
		Assert.AreEqual (Colors.White, ColorMethods.NextColor (Colors.Black));
		Assert.AreEqual (Colors.Yellow, ColorMethods.NextColor (Colors.White));

		Assert.AreEqual (Colors.Yellow, ColorMethods.NextColor (Colors.Empty));
	}

}