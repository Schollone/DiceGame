using UnityEngine;
using System;
using NUnit.Framework;
using MW_DiceGame;

[TestFixture]
[Category ("DieFace Tests")]
internal class DieFaceTests {

	[Test]
	[Category ("GetDieFaceImage Tests")]
	public void GetDieFaceImageTest () {
		Assert.Null (DieFaces.Null.GetDieFaceImage ());
		Assert.NotNull (DieFaces.One.GetDieFaceImage ());
		Assert.NotNull (DieFaces.Two.GetDieFaceImage ());
		Assert.NotNull (DieFaces.Three.GetDieFaceImage ());
		Assert.NotNull (DieFaces.Four.GetDieFaceImage ());
		Assert.NotNull (DieFaces.Five.GetDieFaceImage ());
		Assert.NotNull (DieFaces.Six.GetDieFaceImage ());
	}

	[Test]
	[Category ("GetIndex Tests")]
	public void GetIndexTest () {
		Assert.AreEqual (DieFaces.Null.GetIndex (), 0);
		Assert.AreEqual (DieFaces.One.GetIndex (), 1);
		Assert.AreEqual (DieFaces.Two.GetIndex (), 2);
		Assert.AreEqual (DieFaces.Three.GetIndex (), 3);
		Assert.AreEqual (DieFaces.Four.GetIndex (), 4);
		Assert.AreEqual (DieFaces.Five.GetIndex (), 5);
		Assert.AreEqual (DieFaces.Six.GetIndex (), 6);
	}

	[Test]
	[Category ("Parse Tests")]
	public void ParseTest () {
		Assert.AreEqual (DieFaces.Null, DieFaceMethods.Parse (0));
		Assert.AreEqual (DieFaces.One, DieFaceMethods.Parse (1));
		Assert.AreEqual (DieFaces.Two, DieFaceMethods.Parse (2));
		Assert.AreEqual (DieFaces.Three, DieFaceMethods.Parse (3));
		Assert.AreEqual (DieFaces.Four, DieFaceMethods.Parse (4));
		Assert.AreEqual (DieFaces.Five, DieFaceMethods.Parse (5));
		Assert.AreEqual (DieFaces.Six, DieFaceMethods.Parse (6));
	}

}