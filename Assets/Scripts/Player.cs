using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	public float startSpeed;
	public Text scoreObject;
    public Text levelObject;
    public Slider progressBar;
    public AudioSource scoreClip;
		
	int score;
	int pieceScore = 100;
	int pieceModifier = 15;
	
	bool hasLost;
	bool hasWon;
	bool paused;
	
	float currentSpeed;
    int currentLevel;
	
	float increaseTime = 5.0f;
	float startTime;
	
	int totalPieces;
	float neededPieces;
    float gainedPieces;

	
	
	// Use this for initialization
	void Start () {
		currentSpeed = startSpeed;
		startTime = Time.time;
		currentLevel = 1;
		totalPieces = 0;
		neededPieces = pieceModifier;
		Time.timeScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!hasWon && !hasLost && !paused)
		{					
			foreach (Touch touch in Input.touches) {
				if (touch.phase == TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				{
					OnClick(touch.position);	
				}			
			}	
			
			if(Application.isEditor)
			{
				if(Input.GetMouseButtonDown(0))
				{
					//print ("Clicked");

					OnClick(Input.mousePosition);	
				}
			}
		}			
	}
	
	void OnClick(Vector2 position) {
	
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(position);
		Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

		RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

		if(hit.collider != null)
		{
			if(hit.transform.CompareTag("Piece"))
			{
				//hit.transform.SendMessage("OnClick");
				
				Piece originPiece = hit.transform.GetComponent<Piece>();
				
				List<Transform> matches = findMatches(hit.transform.position, originPiece.pieceType);				

				int numMatches = matches.Count;

				//print ("Matches: " + numMatches);

				if(numMatches > 1)
				{					
					foreach(Transform match in matches)
					{								
						match.transform.SendMessage("ReleaseStackedPieces");
					
						//Instantiate(originPiece.destroyParticle, match.transform.position, Quaternion.identity);
						
						DestroyImmediate(match.gameObject);
						
						//Destroy(match.gameObject);	
					}	

					//DestroyImmediate (hit.transform.gameObject);
					
					OnScore(numMatches);
									
				}else{
									
					foreach(Transform match in matches)
					{								
						match.transform.SendMessage("ClearDeleting");	
					}
					
				}
				
			}
			
		}
		
	
		
	}
	
	List<Transform> findMatches(Vector3 origin, PieceType pType) {	

		List<Transform> matches = new List<Transform> ();

		//Right
		matches.AddRange (findMatch (origin + new Vector3(.75f, 0f, 0f), pType, Vector2.right));

		//Left
		matches.AddRange (findMatch (origin + new Vector3(-.75f, 0f, 0f), pType, -Vector2.right));

		//Up
		matches.AddRange (findMatch (origin + new Vector3(0f, .75f, 0f), pType, Vector2.up));

		//Down
		matches.AddRange (findMatch (origin + new Vector3(0f, -.75f, 0f), pType, -Vector2.up));

		return matches;
	}

	List<Transform> findMatch(Vector3 origin, PieceType pType, Vector2 direction){

		List<Transform> matches = new List<Transform> ();

		Vector2 startPos = new Vector2 (origin.x, origin.y);

		RaycastHit2D hit = Physics2D.Raycast (startPos, direction, 0.5f);

		if (hit.collider != null) {					
			if (hit.transform.CompareTag ("Piece")) {
				//print ("Hit Something");
				Piece hitPiece = hit.transform.GetComponent<Piece> ();

				if (hitPiece.pieceType == pType && !hitPiece.isDeleting ()) {
					//if (!matches.Contains (hit.transform)) {
					hit.transform.SendMessage ("SetDeleting");
					matches.Add (hit.transform);
					matches.AddRange (findMatches (hit.transform.position, pType));
					//}					
				}
			}
		}

		return matches;
	}
		
	
	void OnScore(int multiplier) {
	
		totalPieces += multiplier;
        gainedPieces += multiplier;

        scoreClip.Play();
		
		int tempScore = (pieceScore + (multiplier / 12)) * multiplier;
		
		score+= tempScore;
		
		GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
		
		foreach(GameObject piece in pieces)
		{
		
			piece.SendMessage("CheckShift");
			
		}				
		
		if(totalPieces >= neededPieces)
		{
			LevelUp();
        }
        else
        {
            progressBar.value = (gainedPieces / neededPieces);
        }

		scoreObject.text = score.ToString().PadLeft(10, '0');
	}
	
	void LevelUp() {
		
		//play a sound?
		
		currentLevel += 1;
		
//		if(currentLevel == 5)
//		{
//			currentSpeed = 	3.0f;
//		}else if(currentLevel == 10)
//		{
//			currentSpeed = 	3.0f;
//		}else if(currentLevel == 15)
//		{
//			currentSpeed = 	3.0f;
//		}
		
		currentSpeed += 0.25f;
		neededPieces = Mathf.RoundToInt(((currentLevel * pieceModifier / 8) + pieceModifier) * (currentLevel + 1));

        levelObject.text = currentLevel.ToString().PadLeft(3, '0');
        progressBar.value = 0.0f;
	}
	
	void InvalidSelection() {
	
		//play a sound?
		
	}
	
	void IncreaseSpeed() {
	
		currentSpeed += (currentSpeed * 0.007125f) + startSpeed;
		
		GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
		
		foreach(GameObject piece in pieces)
		{
			piece.SendMessage("SetFallSpeed", currentSpeed);	
		}
		startTime = Time.time;
		
	}
	
	public float GetCurrentSpeed() {
		
		return currentSpeed;
		
	}
	
	void Won() {
		
		if(!hasWon)
		{
		
			hasWon = true;
			PauseObjects(true);
		}
		
	}
	
	void Lost() {
		
		if(!hasLost)
		{
		
			hasLost = true;
			PauseObjects(true);
		}
			
	}
		
	void PauseObjects(bool isPaused) {
		
		Time.timeScale = 1.0f-Time.timeScale;
		
		GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
		
		foreach(GameObject spawner in spawners)
		{
			spawner.SendMessage("Pause", isPaused);	
		}
		
		GameObject[] pieces = GameObject.FindGameObjectsWithTag("Piece");
	
		foreach(GameObject piece in pieces)
		{
			piece.SendMessage("Pause", isPaused);	
		}				
		
				
	}
	
	public int GetCurrentLevel() {
	
		return currentLevel;
		
	}
}
