using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BSPLeaf {
	
	public BSPLeaf[] leaflets;

	public int x1;
	public int y1;
	public int x2;
	public int y2;

	public int x;
	public int y;

	public int split;
	public bool splitMode;

	public Color debugColor;

	public BSPLeaf (int _x1, int _y1, int _x2, int _y2, float _minDiv, float _maxDiv, Color _debugColor, int totalIteration, int currentIteration) {

		x1 = Mathf.Min(_x1,_x2);
		y1 = Mathf.Min(_y1,_y2);
		x2 = Mathf.Max(_x1,_x2);
		y2 = Mathf.Max(_y1,_y2);

		x = (int)Mathf.Lerp(x1,x2 - 1,0.5f);
		y = (int)Mathf.Lerp(y1,y2 - 1,0.5f);

		splitMode = Mathf.Abs(x1 - x2) < Mathf.Abs(y1 - y2);

		if (currentIteration >= totalIteration) {
			
			leaflets = new BSPLeaf[0];

			debugColor = _debugColor;

		} else {
			
			_debugColor = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));

			leaflets = new BSPLeaf[2];

			float s = Random.Range(_minDiv,_maxDiv);
			split = Mathf.RoundToInt((float)(splitMode ? y2 - y1 : x2 - x1) * s);
			split += (Random.value > 0.5f ? 1 : -1) * (1 - (split%2));
			//split += 1 - (split%2);

			if (splitMode) {

				leaflets[0] = new BSPLeaf(x1,y1,x2,y1 + split,_minDiv,_maxDiv,_debugColor,totalIteration,currentIteration + 1);
				leaflets[1] = new BSPLeaf(x1,y1 + split + 1,x2,y2,_minDiv,_maxDiv,_debugColor,totalIteration,currentIteration + 1);

				leaflets[0].PositionNearPoint(new Vector2Int(x,y1 + split));
				leaflets[1].PositionNearPoint(new Vector2Int(x,y1 + split));

			} else {

				leaflets[0] = new BSPLeaf(x1,y1,x1 + split,y2,_minDiv,_maxDiv,_debugColor,totalIteration,currentIteration + 1);
				leaflets[1] = new BSPLeaf(x1 + split + 1,y1,x2,y2,_minDiv,_maxDiv,_debugColor,totalIteration,currentIteration + 1);

				leaflets[0].PositionNearPoint(new Vector2Int(x1 + split,y));
				leaflets[1].PositionNearPoint(new Vector2Int(x1 + split,y));

			}

		}

	}
		
	public void DebugRender () {

		if (leaflets.Length > 0) {

			leaflets[0].DebugRender();
			leaflets[1].DebugRender();

			Debug.DrawLine(new Vector3(leaflets[0].x,leaflets[0].y,0),new Vector3(leaflets[1].x,leaflets[1].y,0),Color.yellow);

		} else {

			Debug.DrawLine(new Vector3(x1,y1,0),new Vector3(x1,y2 - 1,0),debugColor);
			Debug.DrawLine(new Vector3(x2 - 1,y1,0),new Vector3(x2 - 1,y2 - 1,0),debugColor);

			Debug.DrawLine(new Vector3(x1,y1,0),new Vector3(x2 - 1,y1,0),debugColor);
			Debug.DrawLine(new Vector3(x1,y2 - 1,0),new Vector3(x2 - 1,y2 - 1,0),debugColor);	
		}

	}

	public void Trim (int trim) {

		if (leaflets.Length > 0) {

			leaflets[0].Trim(trim);
			leaflets[1].Trim(trim);

		} else {

			bool xd = x2 - x1 > 0;
			x1 += (xd?1:-1) * trim;
			x2 += (xd?-1:1) * trim;

			bool yd = y2 - y1 > 0;
			y1 += (yd?1:-1) * trim;
			y2 += (yd?-1:1) * trim;

			x = Mathf.RoundToInt((float)(x1 + x2)/2);
			y = Mathf.RoundToInt((float)(y1 + y2)/2);

		}

	}

	public void PositionNearPoint (Vector2Int point) {

		if (leaflets.Length > 0) {

			List<BSPLeaf> ends = GetEndLeaflets();

			Vector2Int result = new Vector2Int(ends[0].x,ends[0].y);
			float dist = Vector2.Distance(result,point);

			for (int i = 1; i < ends.Count; i++) {

				Vector2Int p = new Vector2Int(ends[i].x,ends[i].y);
				float d = Vector2Int.Distance(p,point);

				if (d < dist) {
					result = p;
					dist = d;
				}

			}

			x = result.x;
			y = result.y;

		} else {
			
			x = Mathf.RoundToInt((float)(x1 + x2)/2);
			y = Mathf.RoundToInt((float)(y1 + y2)/2);

		}
	}

	public List<BSPLeaf> GetEndLeaflets () {

		List<BSPLeaf> actives = new List<BSPLeaf>();
		List<BSPLeaf> ends = new List<BSPLeaf>();

		for (int i = 0; i < leaflets.Length; i++) {
			actives.Add(leaflets[i]);
		}

		while (actives.Count > 0) {
			
			BSPLeaf leaf = actives[actives.Count - 1];

			if (leaf.leaflets.Length > 0) {
				actives.Add(leaf.leaflets[0]);
				actives.Add(leaf.leaflets[1]);
			} else {
				ends.Add(leaf);
			}

			actives.Remove(leaf);

		}

		return ends;

	}

}

[System.Serializable]
public class Tile {

	public bool solid;
	public BlobAdjacents adjacents;
	public int style;

	public bool focus;
	public Vector2 focalPoint;

	public Tile (bool _solid, bool _focus) {
		solid = _solid;
		style = 0;
		focus = _focus;
		focalPoint = Vector2.zero;
	}

}
	
public class MapGenerator : MonoBehaviour {

	public BSPLeaf tree;
	public int width;
	public int height;

	public int totalDivisions;

	public float minimumDivision;
	public float maximumDivision;

	public Tile[,] map;

	public Sprite[] tileIndexSprites;
	public GameObject mapSpritePrefab;

	public GameObject solidMapPrefab;

	public Color gridColor;

	public TextAsset tileSystemJSON;
	public Dictionary<BlobAdjacents,int> tileSystemIndex;

	public List<WallSprite> mapWalls = new List<WallSprite>();
	public List<InstantiatedWall> solidMapWalls = new List<InstantiatedWall>();

	public List<GameObject> tileObjects = new List<GameObject>();

	public GameObject[] singleTilePrefabs;

	private int tileStylesCount;

	public Transform floorTileTransform;
	public SpriteRenderer floorRenderer;

	void Start () {

		tileSystemIndex = JsonUtility.FromJson<BlobTileSystem>(tileSystemJSON.text).GetDictionary();
		tileStylesCount = tileIndexSprites.Length / 50;
		floorTileTransform.position = new Vector3((float)(width - 1)/2,(float)(height - 1)/2);
		floorRenderer.size = new Vector2(width,height);
		GenerateMap();

	}

	void GenerateMap () {

		tree = new BSPLeaf(0,0,width,height,minimumDivision,maximumDivision,Color.white,totalDivisions,0);
		tree.Trim(2);

		for (int i = 0; i < tileObjects.Count; i++) {
			Destroy(tileObjects[i]);
		}
		tileObjects.Clear();

		List<BSPLeaf> leaflets = tree.GetEndLeaflets();

		map = new Tile[width,height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x,y] = new Tile(true, false);
			}
		}

		DigCorridors(tree,1,3);

		for (int i = 0; i < leaflets.Count; i++) {

			BSPLeaf leaf = leaflets[i];

			Vector2 trueCenter = new Vector2(Mathf.Lerp(leaf.x1,leaf.x2 - 1,0.5f),Mathf.Lerp(leaf.y1,leaf.y2 - 1,0.5f));

			for (int x = leaf.x1; x < leaf.x2; x++) {
				for (int y = leaf.y1; y < leaf.y2; y++) {

					map[x,y].solid = false;
					map[x,y].focus = true;
					map[x,y].focalPoint = trueCenter;

				}
			}

		}

		//TrimSingleWidthLines();


		for (int i = 0; i < leaflets.Count; i++) {

			BSPLeaf leaf = leaflets[i];

			int leafwidth = leaf.x2 - leaf.x1;
			int leafheight = leaf.y2 - leaf.y1;
			int leafstyle = Random.Range(Mathf.Min(tileStylesCount,1),tileStylesCount);

			bool[,] objects = GenerateCompositeObject(leafwidth,leafheight,3);

			for (int x = 0; x < leafwidth; x++) {
				for (int y = 0; y < leafheight; y++) {

					if (objects[x,y]) {
						map[leaf.x1 + x,leaf.y1 + y].solid = true;
						map[leaf.x1 + x,leaf.y1 + y].style = leafstyle;
					}

				}
			}

			int singleTileReplacement = Random.Range(0,singleTilePrefabs.Length);

				if (singleTileReplacement > 0) {
				List<Vector2Int> singles = new List<Vector2Int>();

				for (int x = leaf.x1; x < leaf.x2; x++) {
					for (int y = leaf.y1; y < leaf.y2; y++) {

						if (map[x,y].solid && !TileIsSolid(x + 1,y) && !TileIsSolid(x - 1,y) && !TileIsSolid(x,y + 1) && !TileIsSolid(x,y - 1)) {
							singles.Add(new Vector2Int(x,y));
						}

					}
				}

				for (int s = 0; s < singles.Count; s++) {
					map[singles[s].x,singles[s].y].solid = false;
					var p = (GameObject)Instantiate(singleTilePrefabs[singleTileReplacement],new Vector3(singles[s].x,singles[s].y,0),Quaternion.identity);
					tileObjects.Add(p);
				}
			
			}

		}

		ComputeAdjacence();

		ConstructMap();

	}

	bool TileIsSolid (int x, int y) {

		if (x >= 0 && x < width && y >= 0 && y < height) {
			return map[x,y].solid;
		} else {
			return true;
		}

	}

	public bool GetFocalPoint (int x, int y, out Vector3 result) {

		if (map[x,y].focus) {
			result = map[x,y].focalPoint;
			return true;
		} else {
			result = Vector3.zero;
			return false;
		}

	}

	void DigMap (int x, int y, int size, bool solid) {

		x = Mathf.Clamp(x,size,width - 1 - size);
		y = Mathf.Clamp(y,size,height - 1 - size);

		for (int ix = x - size; ix <= x + size; ix++) {
			for (int iy = y - size; iy <= y + size; iy++) {

				map[ix,iy].solid = solid;

			}
		}

	}

	void ComputeAdjacence () {

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				map[x,y].adjacents = new BlobAdjacents(TileIsSolid(x,y + 1),TileIsSolid(x,y - 1),TileIsSolid(x - 1,y),TileIsSolid(x + 1,y),
					TileIsSolid(x - 1,y + 1),TileIsSolid(x + 1,y + 1),TileIsSolid(x - 1,y - 1),TileIsSolid(x + 1,y - 1));

			}
		}


	}

	void TrimSingleWidthLines () {

		bool unfinished = true;

		while (unfinished) {

			unfinished = false;

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {

					if (map[x,y].solid) {

						int neighbourCount = 0;

						if (TileIsSolid(x - 1,y)) {
							neighbourCount++;
						}
						if (TileIsSolid(x + 1,y)) {
							neighbourCount++;
						}
						if (TileIsSolid(x,y - 1)) {
							neighbourCount++;
						}
						if (TileIsSolid(x,y + 1)) {
							neighbourCount++;
						}

						if (neighbourCount == 1) {
							map[x,y].solid = false;

							//Debug.DrawLine(new Vector3(x + 0.25f,y + 0.25f,0),new Vector3(x - 0.25f,y - 0.25f,0),Color.red,3);
							//Debug.DrawLine(new Vector3(x - 0.25f,y + 0.25f,0),new Vector3(x + 0.25f,y - 0.25f,0),Color.red,3);

							unfinished = true;
						}

					}

				}
			}

		}

	}

	void DigCorridors (BSPLeaf baseLeaf, int size, int minSpace) {

		List<BSPLeaf> actives = new List<BSPLeaf>();
		actives.Add(baseLeaf);

		while (actives.Count > 0) {

			BSPLeaf leaf = actives[actives.Count - 1];

			if (leaf.leaflets.Length > 0) {

				BSPLeaf start = leaf.leaflets[0];
				BSPLeaf end = leaf.leaflets[1];

				actives.Add(start);
				actives.Add(end);

				int dx = end.x - start.x;
				int dy = end.y - start.y;

				int midX = (start.x2 + end.x1) / 2;
				int spaceX = end.x1 - start.x2;
				if (dx < 0) {
					midX = (start.x1 + end.x2) / 2;
					spaceX = start.x1 - end.x2;
				}

				int midY = (start.y2 + end.y1) / 2;
				int spaceY = end.y1 - start.y2;
				if (dy < 0) {
					midY = (start.y1 + end.y2) / 2;
					spaceY = start.y1 - end.y2;
				}

				bool mode = Mathf.Abs(dx) > Mathf.Abs(dy);

				if (mode && spaceX < minSpace && spaceX >= 0) mode = false;
				if (!mode && spaceY < minSpace && spaceY >= 0) mode = true;

				if (!mode) {

					if (dx >= 0) {
						for (int x = start.x; x <= midX; x++) {
							DigMap(x,start.y,size,false);
						}
					} else {
						for (int x = end.x; x <= midX; x++) {
							DigMap(x,end.y,size,false);
						}
					}

					if (dy >= 0) {
						for (int y = start.y; y <= end.y; y++) {
							DigMap(midX,y,size,false);
						}
					} else {
						for (int y = end.y; y <= start.y; y++) {
							DigMap(midX,y,size,false);
						}
					}

					if (dx >= 0) {
						for (int x = midX; x <= end.x; x++) {
							DigMap(x,end.y,size,false);
						}
					} else {
						for (int x = midX; x <= start.x; x++) {
							DigMap(x,start.y,size,false);
						}
					}


				} else {

					if (dy >= 0) {
						for (int y = start.y; y <= midY; y++) {
							DigMap(start.x,y,size,false);
						}
					} else {
						for (int y = end.y; y <= midY; y++) {
							DigMap(end.x,y,size,false);
						}
					}

					if (dx >= 0) {
						for (int x = start.x; x <= end.x; x++) {
							DigMap(x,midY,size,false);
						}
					} else {
						for (int x = end.x; x <= start.x; x++) {
							DigMap(x,midY,size,false);
						}
					}

					if (dy >= 0) {
						for (int y = midY; y <= end.y; y++) {
							DigMap(end.x,y,size,false);
						}
					} else {
						for (int y = midY; y <= start.y; y++) {
							DigMap(start.x,y,size,false);
						}
					}
						
				}

			}

			actives.Remove(leaf);

		}

	}

	void Update () {


		if (Input.GetKey(KeyCode.I)) {
			tree.DebugRender();
		}
			
		if (Input.GetKeyDown(KeyCode.P)) {

			GenerateMap();

		}

	}

	bool[,] GenerateCompositeObject (int width, int height, int iteration) {

		bool[,] results = new bool[width,height];

		for (int i = 0; i < iteration; i++) {

			bool[,] it = GenerateObject(width,height);

			for (int x = 0; x < width; x++) {
				for (int y = 0; y < height; y++) {
					results[x,y] = results[x,y] || it[x,y];
				}
			}

		}

		return results;

	}

	bool[,] GenerateObject (int width, int height) {

		bool[,] solids = new bool[width,height];

		bool[] stripX = GenerateStrip(width);
		bool[] stripY = GenerateStrip(height);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				solids[x,y] = stripX[x] && stripY[y];

			}
		}

		return solids;

	}

	bool[] GenerateStrip (int width) {

		// === INITIALIZE ===
		//width += 1 - width%2;
		bool[] result = new bool[width];

		/*
		for (int x = 1; x < width - 1; x++) {
			result[x] = true;
		}
		*/

		if (width < 3) return result;

		// === DISTRIBUTE ===
		int wallScore = Random.Range(1,Mathf.Min(((width - 1)/2) + 1,(width/3) + 1));
		int[] scores = DistributeIntegers(width - (wallScore * 2),2);

		if (scores[1] > 2 && scores[0] <= 0) {
			scores[1]--;
			scores[0]++;
		}

		//Debug.Log("DISTANCE FROM WALLS: " + wallScore);

		// === CALCULATE ===
		List<int> cScores = GetFactors(scores[1]);
		for (int i = 0; i < cScores.Count; i++) cScores[i]++;
		List<int> commons = cScores.Intersect(GetFactors(scores[0])).ToList<int>();

		if (commons.Count <= 0) return result;
		//for (int i = 0; i < commons.Count; i++) Debug.Log(commons[i]);

		int n = commons[Random.Range(0,commons.Count)];
		int spacing = scores[1] / (n - 1);
		int size = scores[0] / n;
		//Debug.Log("COUNT: " + n + " SIZE: " + size + " SPACING: " + spacing);

		// === QUANTIFY ===

		for (int x = 0; x < width; x++) {
			result[x] = false;
		}

		for (int x = wallScore; x < width - wallScore; x += (size + spacing)) {

			for (int xx = 0; xx < size; xx++) {
				result[x + xx] = true;
			}

		}

		return result;

	}

	List<int> GetFactors (int n) {

		List<int> factors = new List<int>();

		for (int i = 1; i <= n; i++) {
			if (n%i == 0) factors.Add(i);
		}

		return factors;

	}

	int[] DistributeIntegers (int total, int size) {

		int[] result = new int[size];

		for (int i = 0; i < total; i++) {
			result[Random.Range(0,size)]++;
		}

		return result;

	}

	InstantiatedWall GetInstantiatedWall (int i) {

		InstantiatedWall result;

		if (i >= solidMapWalls.Count) {
			solidMapWalls.Add(((GameObject)Instantiate(solidMapPrefab)).GetComponent<InstantiatedWall>());
		}

		result = solidMapWalls[i];

		return result;

	}

	WallSprite GetWallSprite (int i) {

		WallSprite result;

		if (i >= mapWalls.Count) {
			mapWalls.Add(((GameObject)Instantiate(mapSpritePrefab)).GetComponent<WallSprite>());
		}

		result = mapWalls[i];

		return result;

	}

	void ConstructMap () {

		for (int i = 0; i < solidMapWalls.Count; i++) {
			solidMapWalls[i].Disable();
		}
		for (int i = 0; i < mapWalls.Count; i++) {
			mapWalls[i].Disable();
		}

		List<Vector2Int> stack = new List<Vector2Int>();

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (map[x,y].solid) {
					if (map[x,y].adjacents.GetCount() < 8) {
						stack.Add(new Vector2Int(x,y));
					}
				}

			}
		}

		int dirID = 2; // 0 = up, 1 = right, 2 = down, 3 = left
		Vector2Int[] direction = { new Vector2Int(0,1), new Vector2Int(1,0), new Vector2Int(0,-1), new Vector2Int(-1,0) };
		int wallLength = 1;

		Vector2Int current = stack[stack.Count - 1];
		stack.RemoveAt(stack.Count - 1);

		Vector2Int starting = current;

		//Color debugColor = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));

		int wi = 0;

		while (stack.Count > 0) {

			int result = -1;
			int search = 0;

			while (search < 4) {

				result = stack.IndexOf(current + direction[dirID]);

				if (result >= 0) {

					if (search > 0) {
						wallLength = 1;
						starting = current;
					} else {
						wallLength++;
					}

					search = 4;

					current = stack[result];
					stack.RemoveAt(result);

				} else {

					if (wallLength > 0) {
						GetInstantiatedWall(wi).Initialize(
							new Vector3((starting.x + current.x)/2f,(starting.y + current.y)/2f),
							new Vector3(Mathf.Abs(starting.x - current.x) + 1,Mathf.Abs(starting.y - current.y) + 1,1));
						wi++;
					}

					//wallLength = 0;

					//debugColor = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));

					dirID++;
					if (dirID > 3) dirID = 0;

					search++;

				}

			}

			if (search >= 4 && result < 0) {

				if (wallLength > 0) {
					GetInstantiatedWall(wi).Initialize(
						new Vector3((starting.x + current.x)/2f,(starting.y + current.y)/2f,-3),
						new Vector3(Mathf.Abs(starting.x - current.x) + 1,Mathf.Abs(starting.y - current.y) + 1,1));
					wi++;
				}

				//debugColor = new Color(Random.Range(0.5f,1f),Random.Range(0.5f,1f),Random.Range(0.5f,1f));

				current = stack[stack.Count - 1];
				stack.RemoveAt(stack.Count - 1);
				starting = current;

				wallLength = 1;

			}

			/*
			Debug.DrawLine(new Vector3(current.x + 0.5f,current.y + 0.5f,-1),new Vector3(current.x - 0.5f,current.y - 0.5f,-1),debugColor,60);
			Debug.DrawLine(new Vector3(current.x - 0.5f,current.y + 0.5f,-1),new Vector3(current.x + 0.5f,current.y - 0.5f,-1),debugColor,60);

			Debug.DrawLine(new Vector3(current.x,current.y,-1),new Vector3(current.x + direction[dirID].x,current.y + direction[dirID].y,-1),debugColor,60);
			*/

		}

		if (wallLength > 0) {
			GetInstantiatedWall(wi).Initialize(
				new Vector3((starting.x + current.x)/2f,(starting.y + current.y)/2f),
				new Vector3(Mathf.Abs(starting.x - current.x) + 1,Mathf.Abs(starting.y - current.y) + 1,1));
			wi++;
		}

		int si = 0;

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (map[x,y].solid) {
					
					int ind = 0;
					if (tileSystemIndex.TryGetValue(map[x,y].adjacents,out ind)) {

						GetWallSprite(si).Initialize(new Vector3(x,y,0),tileIndexSprites[(50 * map[x,y].style) + ind]);
						si++;

					}
				}

			}
		}



	}

}
