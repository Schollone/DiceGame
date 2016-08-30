using System.Collections;
using UnityEngine.Networking;

public interface IAction {

	void ExecuteAction ();
}

public class ActionMsg {
	public static short EnterBid = MsgType.Highest + 1;
	public static short CallOutBluff = MsgType.Highest + 2;
	public static short DeclareBidSpotOn = MsgType.Highest + 3;
}