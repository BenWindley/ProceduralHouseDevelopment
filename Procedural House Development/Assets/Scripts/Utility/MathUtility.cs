using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtility
{
	/// <summary>
	/// Closest point on a line to another line
	/// </summary>
	public static bool LineLineIntersection(Vector3 line1point, Vector3 line1Dir, Vector3 line2Point, Vector3 line2Dir, out Vector3 intersection)
	{

		Vector3 lineVec3 = line2Point - line1point;
		Vector3 crossVec1and2 = Vector3.Cross(line1Dir, line2Dir);
		Vector3 crossVec3and2 = Vector3.Cross(lineVec3, line2Dir);

		float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = line1point + (line1Dir * s);
			return true;
		}
		else
		{
			intersection = Vector3.zero;
			return false;
		}
	}

	/// <summary>
	/// Closest point on a line to another point
	/// </summary>
	public static Vector3 NearestPointToLine(Vector3 linePoint, Vector3 lineDir, Vector3 point)
	{
		lineDir.Normalize();
		Vector3 v = point - linePoint;
		float d = Vector3.Dot(v, lineDir);
		return linePoint + lineDir * d;
	}

	public static float NearestDistanceToLineSegment(Vector3 p1, Vector3 p2, Vector3 point)
	{
		Vector3 nearestPoint = NearestPointToLine(p1, (p2 - p1).normalized, point);
		float invLerp = InverseLerp(p1, p2, nearestPoint);

		if (invLerp < 0)
			return Vector3.Distance(p1, point);
		if (invLerp > 1)
			return Vector3.Distance(p2, point);

		return Vector3.Distance(nearestPoint, point);
	}

	public static bool PointOnLineSegment(Vector3 p1, Vector3 p2, Vector3 point)
	{
		Vector3 lineVec = p2 - p1;
		Vector3 pointVec = point - p1;

		float dot = Vector3.Dot(pointVec, lineVec);

		if (dot > 0)
			if (pointVec.magnitude <= lineVec.magnitude)
				return true;

		return false;
	}

	public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
	{
		Vector3 AB = b - a;
		Vector3 AV = value - a;
		return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
	}

	/// <summary>
	/// Get the perpendicular direction to the lineDir using the left-hand rule
	/// </summary>
	public static Vector3 Perpendicular(Vector3 lineDir, bool left = true)
	{
		return Vector3.Cross(lineDir, Vector3.forward).normalized * (left ? 1 : -1);
	}

	public static bool LineSegmentLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
	{
		intersection = Vector2.zero;

		var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

		if (d == 0.0f)
		{
			return false;
		}

		var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
		var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

		if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
		{
			return false;
		}

		intersection.x = p1.x + u * (p2.x - p1.x);
		intersection.y = p1.y + u * (p2.y - p1.y);

		return true;
	}
}
