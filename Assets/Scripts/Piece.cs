using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PieceType
{	
	red = 0,
	green = 1,
	blue = 2,
	yellow = 3,
	purple = 4,
	orange = 5,
	white = 6
};

public enum ShiftDirection
{
	left = 0,
	right = 1,
	none = 2
}

public enum FallDirection
{
	left = 0,
	right = 1,
	up = 3,
	down = 4
}

public class Piece : MonoBehaviour {
	
	public PieceType pieceType;
	public ShiftDirection shiftDirection;
	public Transform destroyParticle;
	
	float damping = 0.7f;
	bool shifting = false;
	bool falling = false;
	float fallSpeed = 0.0f;
	
	bool paused;
	bool deleting;
	
	
	// Use this for initialization
	void Start () {
		Player p = Camera.main.GetComponent<Player>();
		
		if(p != null)
		{		
			fallSpeed = p.GetCurrentSpeed();			
		}else{
			fallSpeed = 4.0f;	
		}
	}

	// Update is called once per frame
	void Update () {
		
//		if (!paused) {
//		
//			if (falling) {		
//				
//				RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up, 0.5f);
//
//				print (hit);
//
//				if (hit != null) {
//								
//					if (hit.transform.gameObject.CompareTag ("Piece")) {
//						Piece piece = hit.transform.GetComponent<Piece> ();
//						
//						if (!piece.isFalling () && !piece.isDeleting ()) {
//							transform.position = new Vector3 (transform.position.x, hit.transform.position.y + 1, transform.position.z);
//							falling = false;
//						} else {
//							fallSpeed = piece.fallSpeed;	
//						}
//					} else if (hit.transform.gameObject.CompareTag ("Bottom")) {
//						transform.position = new Vector3 (transform.position.x, hit.transform.position.y + 1, transform.position.z);
//						falling = false;
//					} else {
//						MoveDown ();		
//					}
//					
//				} else {
//					MoveDown ();		
//				}
//			}
//						
//		}	

        if(shifting)
        {
            ValidateShift();
        }

	}
	
	void CheckShift() {
		shifting = true;
	}
	
	void SetFalling() {
		falling = true;	
	}
	
	void ReleaseStackedPieces() {
	
		RaycastHit[] stackedPieces = Physics.RaycastAll(transform.position, transform.up, 25.0f);
		
		foreach(RaycastHit stackedPiece in stackedPieces)
		{
			if(stackedPiece.transform.CompareTag("Piece"))
			{
				stackedPiece.transform.SendMessage("SetFalling");
			}
			
		}
		
	}
	
	void ValidateShift() {
					
		Vector2 direction = Vector2.right;
		
		if (transform.position.x > 0.0f) {
			direction = -Vector2.right;	
		}
		
//		if(shiftDirection == ShiftDirection.left)
//		{
//			direction = -transform.right;	
//		}else if(shiftDirection == ShiftDirection.right){
//			direction = transform.right;
//		}else{
//			
//			shifting = false;			
//			return;	
//		}
		
		RaycastHit2D hit = Physics2D.Raycast (transform.position, -Vector2.up);
			
		if (hit.transform.CompareTag ("Bottom")) {				
							
			if (!Physics2D.Raycast (transform.position, direction, 0.5f)) {	
								
				RaycastHit2D[] stackedPieces = Physics2D.RaycastAll (transform.position, transform.up);
										
				Shift ();
				
				foreach (RaycastHit2D stackedPiece in stackedPieces) {
					if (stackedPiece.transform.CompareTag ("Piece")) {
						stackedPiece.transform.SendMessage ("Shift");
					}
				}				
				
			} else {
				
				shifting = false;
					
			}
		} else {
			
			shifting = false;
				
		}
			
	}
	
	void Shift() {
	
		if(transform.position.x > 0.0f)
		{
			MoveLeft();	
		}else if(transform.position.x < 0.0f)
		{
			MoveRight();
		}
		
	}
	
	void MoveUp() {
		
		transform.position += transform.up * fallSpeed * Time.deltaTime;
		
	}
	
	void MoveDown() {
		
		transform.position -= transform.up * fallSpeed * Time.deltaTime;
		
	}
	
	void MoveLeft() {
		
		transform.position -= transform.right * fallSpeed * Time.deltaTime;
		
	}
	
	void MoveRight() {
		
		transform.position += transform.right * fallSpeed * Time.deltaTime;
		
	}
		
	void OnCollisionEnter(Collision other){
			
		if(other.transform.CompareTag("DestroyZone"))
		{
			Destroy(this.gameObject);	
		}
	}
	
	public bool isFalling() {
		return falling;	
	}
	
	public bool isDeleting() {
		return deleting;	
	}
	
	void SetDeleting() {
		deleting = true;
	}
	
	void ClearDeleting() {
		deleting = false;	
	}
	
	public void SetFallSpeed(float speed) {	
		fallSpeed = speed;		
	}
	
	void Pause(bool isPaused) {
		paused = isPaused;	
	}
}
