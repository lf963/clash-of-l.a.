using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point {
	
	public int X {
		get;
		set;
	}

	public int Y {
		get;
		set;
	}

	public Point (int x, int y) {
		this.X = x;
		this.Y = y;
	}

	public static bool operator == (Point first, Point second) {
		return first.X == second.X && first.Y == second.Y;
	}

	public static bool operator != (Point first, Point second) {
		return first.X != second.X || first.Y != second.Y;
	}
}
