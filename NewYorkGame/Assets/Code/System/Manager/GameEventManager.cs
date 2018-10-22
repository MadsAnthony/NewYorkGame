using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager {

	public delegate void GameEventHandler(GameEvent e);
	public GameEventHandler OnGameEvent;
	private GameEvent reusableEvent = new GameEvent(GameEventType.BlockColored,null);

	public void Emit(GameEvent e) {
		if (OnGameEvent != null) {
			OnGameEvent(e);
		}
	}

	public void Emit(GameEventType type) {
		reusableEvent.type = type;
		reusableEvent.context = null;
		Emit(reusableEvent);
	}
}

public class GameEvent {
	public GameEventType type;
	public Object context;

	public GameEvent(GameEventType type, Object context) {
		this.type = type;
		this.context = context;
	}
}

public enum GameEventType {
	BlockColored,
	BlockUnColored,
	CollectableCollected,
	LevelCompleted,
	PieceDestroyed
}
