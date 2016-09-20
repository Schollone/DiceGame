using UnityEngine.Networking;

public class ActionMsg {
	
	public static short ClientReady = MsgType.Highest + 1;
	public static short DicesThrown = MsgType.Highest + 2;
	public static short EnterBid = MsgType.Highest + 3;
	public static short CallOutBluff = MsgType.Highest + 4;
	public static short DeclareBidSpotOn = MsgType.Highest + 5;
	public static short UnlockControlButtons = MsgType.Highest + 6;
	public static short LockControlButtons = MsgType.Highest + 7;

}