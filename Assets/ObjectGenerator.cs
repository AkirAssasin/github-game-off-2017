using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {

	public int width;
	public int height;

	public bool[,] solids;

	void Start () {

		GenerateObject(width,height);

	}

	void GenerateObject (int width, int height) {

		solids = new bool[width,height];

		bool[] stripX = GenerateStrip(width);
		bool[] stripY = GenerateStrip(height);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				solids[x,y] = stripX[x] && stripY[y];

			}
		}

	}

	void Update () {

		Debug.DrawLine(new Vector3(0,0,0),new Vector3(width - 1,0,0));
		Debug.DrawLine(new Vector3(0,0,0),new Vector3(0,height - 1,0));

		Debug.DrawLine(new Vector3(width - 1,height - 1,0),new Vector3(width - 1,0,0));
		Debug.DrawLine(new Vector3(width - 1,height - 1,0),new Vector3(0,height - 1,0));


		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {

				if (solids[x,y]) {
					Debug.DrawLine(new Vector3(x + 0.25f,y + 0.25f,0),new Vector3(x - 0.25f,y - 0.25f,0));
					Debug.DrawLine(new Vector3(x - 0.25f,y + 0.25f,0),new Vector3(x + 0.25f,y - 0.25f,0));
				}

			}
		}

		if (Input.GetKeyDown(KeyCode.U)) {
			GenerateObject(width,height);
		}

	}
		
	bool[] GenerateStrip (int width) {

		// === INITIALIZE ===
		width += 1 - width%2;
		bool[] result = new bool[width];
		if (width < 3) return result;

		// === DISTRIBUTE ===
		int wallScore = Random.Range(1,Mathf.Min(((width - 1)/2) + 1,3));
		int[] scores = DistributeIntegers(width - (wallScore * 2),2);

		Debug.Log("DISTANCE FROM WALLS: " + wallScore);

		// === CALCULATE ===
		List<int> cScores = GetFactors(scores[1]);
		for (int i = 0; i < cScores.Count; i++) cScores[i]++;
		List<int> commons = cScores.Intersect(GetFactors(scores[0])).ToList<int>();

		if (commons.Count <= 0) return result;
		//for (int i = 0; i < commons.Count; i++) Debug.Log(commons[i]);

		int n = commons[Random.Range(0,commons.Count)];
		int spacing = scores[1] / (n - 1);
		int size = scores[0] / n;
		Debug.Log("COUNT: " + n + " SIZE: " + size + " SPACING: " + spacing);

		// === QUANTIFY ===
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
		
}
