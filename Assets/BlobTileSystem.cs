using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlobTileSystem {

	public BlobAdjacents[] index;

	public Dictionary<BlobAdjacents,int> GetDictionary () {

		Dictionary<BlobAdjacents,int> result = new Dictionary<BlobAdjacents,int>();
		for (int i = 0; i < index.Length; i++) {
			result.Add(index[i],i);
		}
		return result;

	}

}

[System.Serializable]
public struct BlobAdjacents {

	public int u, b, l, r, ul, ur, bl, br; // 0 - false, 1 - true, 2 - don't care

	public BlobAdjacents (bool _u, bool _b, bool _l, bool _r, bool _ul, bool _ur, bool _bl, bool _br) {

		u = _u?1:0;
		b = _b?1:0;
		l = _l?1:0;
		r = _r?1:0;
		ul = _ul?1:0;
		ur = _ur?1:0;
		bl = _bl?1:0;
		br = _br?1:0;

	}

	public int GetCount () {
		return u + b + l + r + ul + ur + bl + br;
	}

	public override bool Equals (object o) {

		if (o is BlobAdjacents) return Equals((BlobAdjacents)o);
		else return base.Equals(o);
	}

	public bool Equals (BlobAdjacents adj) {

		if (adj == null) return false;

		if (u < 2 && adj.u < 2 && adj.u != u)  return false;
		if (b < 2 && adj.b < 2 && adj.b != b)  return false;
		if (l < 2 && adj.l < 2 && adj.l != l)  return false;
		if (r < 2 && adj.r < 2 && adj.r != r)  return false;
		if (ul < 2 && adj.ul < 2 && adj.ul != ul)  return false;
		if (ur < 2 && adj.ur < 2 && adj.ur != ur)  return false;
		if (bl < 2 && adj.bl < 2 && adj.bl != bl)  return false;
		if (br < 2 && adj.br < 2 && adj.br != br)  return false;

		return true;

	}

	public static bool operator == (BlobAdjacents adj1, BlobAdjacents adj2) {

		if (Object.ReferenceEquals(adj1,adj2)) {
			return true;
		}

		if (adj1 == null || adj2 == null) {
			return false;
		}

		return adj1.Equals(adj2);

	}

	public static bool operator != (BlobAdjacents adj1, BlobAdjacents adj2) {

		if (Object.ReferenceEquals(adj1,adj2)) {
			return false;
		}

		if (adj1 == null || adj2 == null) {
			return true;
		}

		return !adj1.Equals(adj2);
	}

	// this implementation is not necessary
	// Only the override is
	public override int GetHashCode () {

		int hash = 17;
		// Suitable nullity checks etc, of course :)
		hash = hash * 23 + u.GetHashCode();
		hash = hash * 23 + b.GetHashCode();
		return hash;

	}
		
}