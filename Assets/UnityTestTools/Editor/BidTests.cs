using UnityEngine;
using System;
using NUnit.Framework;
using MW_DiceGame;

[TestFixture]
[Category ("Bid Tests")]
internal class BidTests {

	[Test]
	[Category ("Bluff Tests")]
	public void BluffTest () {
		Bid enteredBid = new Bid (DieFaces.Two, 2);

		bool isBluff = enteredBid.IsBluff (new Bid (DieFaces.Two, 1));
		Assert.True (isBluff);

		isBluff = enteredBid.IsBluff (new Bid (DieFaces.Two, 2));
		Assert.False (isBluff);

		isBluff = enteredBid.IsBluff (new Bid (DieFaces.Two, 3));
		Assert.False (isBluff);
	}

	[Test]
	[Category ("SpotOn Tests")]
	public void SpotOnTest () {
		Bid enteredBid = new Bid (DieFaces.Two, 2);

		bool IsSpotOn = enteredBid.IsSpotOn (new Bid (DieFaces.Two, 1));
		Assert.False (IsSpotOn);

		IsSpotOn = enteredBid.IsSpotOn (new Bid (DieFaces.Two, 2));
		Assert.True (IsSpotOn);

		IsSpotOn = enteredBid.IsSpotOn (new Bid (DieFaces.Two, 3));
		Assert.False (IsSpotOn);
	}

	[Test]
	[Category ("Can be replaced with Tests")]
	public void CanBeReplacedWithTest () {
		Debug.Log ("CanBeReplacedWithTest");
		Bid enteredBid = new Bid (DieFaces.Two, 2);

		bool canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Null, 1));
		Assert.False (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.One, 1));
		Assert.False (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Two, 1));
		Assert.False (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Two, 2));
		Assert.False (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Two, 3));
		Assert.True (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Six, 20));
		Assert.True (canBeReplacedWith);

		canBeReplacedWith = enteredBid.CanBeReplacedWith (new Bid (DieFaces.Six, 21));
		Assert.False (canBeReplacedWith);
	}

}