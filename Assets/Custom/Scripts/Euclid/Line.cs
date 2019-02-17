﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Euclid {
	public class Line : Figure {
		// a point on the line
		public Vector3 p { get; }
		public Vector3 slope { get; }

		public Line (Vector3 p, Vector3 slope) {
			this.p = p;
			this.slope = slope;

			if (!Util.Approximately(this.slope.sqrMagnitude, 1)) {
				this.slope = this.slope.normalized;
			}
		}

        public override List<Figure> Intersection(Point point) {
            return point.Intersection(this);
        }
        public override List<Figure> Intersection(Line line) {
            if (this == line)
                return new List<Figure> { this };
            Vector3 a = p;
            Vector3 b = p + slope;
            Vector3 c = line.p;
            Vector3 d = line.p + line.slope;
            if (Util.Approximately(Vector3.Dot(d - a, Vector3.Cross(b - a, c - a)), 0))
                return new List<Figure>();
            float u = Vector3.Cross(p - line.p, line.slope).magnitude / Vector3.Cross(slope, line.slope).magnitude;
            return new List<Figure> { new Point(p + u * slope) };
        }
        public override List<Figure> Intersection(Circle circle) {
            return new List<Figure>();
        }
        public override List<Figure> Intersection(Plane plane) {
            if (Util.Approximately(Vector3.Dot(slope, plane.normal), 0)) {
                if (Util.Approximately(Vector3.Dot(p - plane.p, plane.normal), 0)) {
                    return new List<Figure> { this };
                }
                return new List<Figure>();
            }
            float d = Vector3.Dot(slope, plane.normal) / Vector3.Dot(p - plane.p, plane.normal);
            return new List<Figure> { new Point(p + d * slope) };
        }
        public override List<Figure> Intersection(Sphere sphere) {
            return new List<Figure>();
        }

        public override Figure Binormal(Point point) {
            return point.Binormal(this);
        }
        public override Figure Binormal(Line line) {
            if (Util.Approximately(slope, line.slope))
                return new Null();
            Vector3 normal = Vector3.Cross(slope, line.slope);
            Plane plane = new Plane(p, Vector3.Cross(slope, normal)); // the plane made by sweeping [this] along [normal]
            List<Figure> intersection = line.Intersection(plane);
            return new Line((intersection[0] as Point).p, normal);
        }
        // Binormal is null for a line and a plane

        public override Figure PointOn() {
            return new Point(Util.RandomValue() * slope + p);
        }

        public override string ToString() {
            return "Line " + p.ToString() + " " + slope.ToString();
        }

        public static bool operator ==(Line a, Line b) {
            return Util.Approximately(Vector3.Cross(a.slope, b.slope), Vector3.zero) 
                && Util.Approximately(Vector3.Cross(a.slope, a.p - b.p), Vector3.zero);
        }
        public static bool operator !=(Line a, Line b) {
            return !(a == b);
        }
        public override bool Equals(object obj) {
            return obj is Line && this == obj as Line;
        }
        public override int GetHashCode() {
            return p.GetHashCode() + slope.GetHashCode();
        }
    }
}
