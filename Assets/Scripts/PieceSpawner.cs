using UnityEngine;
using System.Collections;

public class PieceSpawner : MonoBehaviour {
	
	public Transform[] piecePrefab;
	public bool isSpawning = false;
	public ShiftDirection direction;
	public bool continuous;
	
	private int pieces;
	private float startTime;
	private float spawnTime;
	private bool paused;
	private int maxPiece = 0;
	
	// Use this for initialization
	void Start () {
		
		maxPiece = PlayerPrefs.GetInt("ColorsSelected", 3);
		
		SpawnPieces(1);
		
		SetTimers();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!paused)
		{
		
			if(isSpawning)
			{
				if(pieces > 0 || continuous)
				{
					if(Time.time - startTime >= spawnTime)
					{
						Spawn();	
					}
				}else{
					isSpawning = false;	
				}
			}
			
		}
		
	}
	
	void SpawnPieces(int num) {
	
		pieces = num;
		isSpawning = true;
		SetTimers();
		
	}
	
	void Spawn() {
	 		
		Transform piece = piecePrefab[Random.Range(0,maxPiece)];
		
		Transform spawnedPiece = Instantiate(piece, transform.position - Vector3.up, Quaternion.identity) as Transform;
		
		Piece p = spawnedPiece.GetComponent<Piece>();
		
		p.shiftDirection = direction;		
		
		if(continuous)
		{
			spawnedPiece.SendMessage("SetFalling");
		}
		
		
		pieces--;
		
		SetTimers();
		
	}
	
	void SetTimers() {
	
		startTime = Time.time;
		
		if(continuous)
		{
			spawnTime = Random.Range(1.0f, 5.0f);	
		}else{
			spawnTime = 0.1f;	
		}
		
	}	
	
	void Stop() {
		isSpawning = false;
		continuous = false;
		pieces = 0;
	}
	
	void Pause(bool isPaused) {			
		paused = isPaused;
	}
}
