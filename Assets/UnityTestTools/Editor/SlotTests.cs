using UnityEngine;
using System;
using NUnit.Framework;
using MW_DiceGame;

[TestFixture]
[Category ("Slot Tests")]
internal class SlotTests {

	[Test]
	[Category ("GetIndex Tests")]
	public void GetIndexTest () {
		Assert.AreEqual (Slots.First.GetIndex (), 0);
		Assert.AreEqual (Slots.Second.GetIndex (), 1);
		Assert.AreEqual (Slots.Third.GetIndex (), 2);
		Assert.AreEqual (Slots.Fourth.GetIndex (), 3);
	}

	[Test]
	[Category ("Parse Tests")]
	public void ParseTest () {
		Assert.AreEqual (Slots.First, SlotMethods.Parse (0));
		Assert.AreEqual (Slots.Second, SlotMethods.Parse (1));
		Assert.AreEqual (Slots.Third, SlotMethods.Parse (2));
		Assert.AreEqual (Slots.Fourth, SlotMethods.Parse (3));
	}

	[Test]
	[Category ("NextSlot Tests")]
	public void NextSlotTest () {
		Assert.AreEqual (Slots.Second, Slots.First.NextSlot ());
		Assert.AreEqual (Slots.Third, Slots.Second.NextSlot ());
		Assert.AreEqual (Slots.Fourth, Slots.Third.NextSlot ());
		Assert.AreEqual (Slots.First, Slots.Fourth.NextSlot ());
	}

}