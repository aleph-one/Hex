using UnityEngine;
using System.Collections;

public class HexGrid : MonoBehaviour {
	//following public variable is used to store the hex model prefab;
	//instantiate it by dragging the prefab on this variable using unity editor
	public GameObject Hex;
	//next two variables can also be instantiated using unity editor
	public int gridWidthInHexes = 50;
	public int gridHeightInHexes = 50;
	
	//Hexagon tile width and height in game world
	private float hexWidth;
	private float hexHeight;
	private GameObject[,] map;

	private float minFov = 15f;
	private float maxFov = 90f;
	private float sensitivity = 30f;
	private float scrollSpeed = 3;
	private float scrollBorder = 10;
	
	void Update() {
		//zoom
		float fov = Camera.main.fieldOfView;
		fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
		fov = Mathf.Clamp(fov, minFov, maxFov);
		Camera.main.fieldOfView = fov;


		//move
		if (Input.mousePosition.x > Screen.width - scrollBorder) {
			transform.position += new Vector3 (scrollSpeed * Time.deltaTime, 0, 0); 
		} else if (Input.mousePosition.x < 0 + scrollBorder) {
			transform.position -= new Vector3 (scrollSpeed * Time.deltaTime, 0, 0); 
		}
		
		if (Input.mousePosition.y > Screen.height - scrollBorder) {
			transform.position += new Vector3 (0, 0, scrollSpeed * Time.deltaTime); 
		} else if (Input.mousePosition.y < 0 + scrollBorder) {
			transform.position -= new Vector3 (0, 0, scrollSpeed * Time.deltaTime); 
		}
	}

	//Method to initialise Hexagon width and height
	void setSizes() {
		//renderer component attached to the Hex prefab is used to get the current width and height
		hexWidth = Hex.GetComponent<Renderer>().bounds.size.x;
		hexHeight = Hex.GetComponent<Renderer>().bounds.size.z;
	}
	
	//Method to calculate the position of the first hexagon tile
	//The center of the hex grid is (0,0,0)
	Vector3 calcInitPos() {
		Vector3 initPos = new Vector3(-hexWidth * gridWidthInHexes / 2f + hexWidth / 2, 0,
		                      gridHeightInHexes / 2f * hexHeight - hexHeight / 2);
		
		return initPos;
	}
	
	//method used to convert hex grid coordinates to game world coordinates
	public Vector3 calcWorldCoord(Vector2 gridPos) {
		//Position of the first hex tile
		Vector3 initPos = calcInitPos();
		//Every second row is offset by half of the tile width
		float offset = 0;
		if (gridPos.y % 2 != 0)
			offset = hexWidth / 2;
		
		float x =  initPos.x + offset + gridPos.x * hexWidth;
		//Every new line is offset in z direction by 3/4 of the hexagon height
		float z = initPos.z - gridPos.y * hexHeight * 0.75f;
		return new Vector3(x, 0, z);
	}
	
	//Finally the method which initialises and positions all the tiles
	void createGrid() {
		//Game object which is the parent of all the hex tiles
		GameObject hexGridGO = new GameObject("HexGrid");
		map = new GameObject[gridWidthInHexes, gridHeightInHexes];

		for (float y = 0; y < gridHeightInHexes; y++) {
			for (float x = 0; x < gridWidthInHexes; x++) {
				//GameObject assigned to Hex public variable is cloned
				GameObject hex = (GameObject)Instantiate(Hex);
				map[(int)x, (int)y] = hex;
				//Current position in grid
				Vector2 gridPos = new Vector2(x, y);
				hex.transform.position = calcWorldCoord(gridPos);
				hex.transform.parent = hexGridGO.transform;
			}
		}


		for (int i = 0; i < 4; i++) {
			int x1 = (int)Random.Range (0, gridHeightInHexes);
			int y1 = (int)Random.Range (0, gridWidthInHexes);
			float h = Random.Range(0, 0.5f);
			GameObject hex = map[x1, y1];
			hex.transform.position += new Vector3(0, h, 0);
		}
		for (float y = 0; y < gridHeightInHexes; y++) {
			for (float x = 0; x < gridWidthInHexes; x++) {

			}
		}
	}
	
	//The grid should be generated on game start
	void Start() {
		setSizes();
		createGrid();
	}

}
