﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Euclid {
	public class Sphere : Figure {
		public Vector3 center { get; }
		public float radius { get; }

		public Sphere (Vector3 center, float radius) {
			this.center = center;
			this.radius = radius;
		}

        public Sphere (Vector3 center, Vector3 p) {
            this.center = center;
            this.radius = (p - center).magnitude;
        }

        public override List<Figure> Intersection(Point point) {
            return point.Intersection(this);
        }
        public override List<Figure> Intersection(Line line) {
            return line.Intersection(this);
        }
        public override List<Figure> Intersection(Circle circle) {
            List<Figure> sphereIntersects = (new Plane(circle.center, circle.normal)).Intersection(this);
            if (sphereIntersects.Count == 0)
                return new List<Figure>();
            if (sphereIntersects[0] is Point) {
                Point p = sphereIntersects[0] as Point;
                if (Util.Approximately((p.p - circle.center).magnitude, circle.radius))
                    return new List<Figure> { p };
                return new List<Figure>();
            }
            else {
                return sphereIntersects[0].Intersection(circle);
            }
        }
        public override List<Figure> Intersection(Plane plane) {
            return plane.Intersection(this);
        }
        public override List<Figure> Intersection(Sphere sphere) {
            if (this == sphere) return new List<Figure> { this };
            float d = (center - sphere.center).magnitude;
            if (Util.Approximately(d, radius + sphere.radius)) {
                Vector3 p = (sphere.center - center) * radius / (radius + sphere.radius) + center;
                return new List<Figure> { new Point(p) };
            }
            if (Util.Approximately(d + radius, sphere.radius)) {
                Vector3 p = (center - sphere.center) / d * sphere.radius + sphere.center;
                return new List<Figure> { new Point(p) };
            }
            if (Util.Approximately(d + sphere.radius, radius)) {
                Vector3 p = (sphere.center - center) / d * radius + center;
                return new List<Figure> { new Point(p) };
            }
            if (d > radius + sphere.radius || radius > d + sphere.radius || sphere.radius > d + radius)
                return new List<Figure>();
            float h = .5f + (radius * radius - sphere.radius * sphere.radius) / (2 * d * d);
            float r = Mathf.Sqrt(radius * radius - h * h * d * d);
            Vector3 c = center + h * (sphere.center - center);
            Vector3 norm = sphere.center - center;
            return new List<Figure> { new Circle(c, r, norm) };
        }

        public override Figure Binormal(Point point) {
            return new Point(center).Binormal(point);
        }
        public override Figure Binormal(Line line) {
            return new Point(center).Binormal(line);
        }
        public override Figure Binormal(Plane plane) {
            return new Point(center).Binormal(plane);
        }

        public override Figure PointOn () {
            return new Point(Random.onUnitSphere * radius + center);
        }

        public override Figure Center() {
            return new Point(center);
        }

        public override string ToString() {
            return "Sphere " + center.ToString() + " " + radius;
        }

        // static methods
        public static bool operator ==(Sphere a, Sphere b) {
            return Util.Approximately(a.center, b.center) && Util.Approximately(a.radius, b.radius);
        }
        public static bool operator !=(Sphere a, Sphere b) {
            return !(a == b);
        }
        public override bool Equals(object obj) {
            return obj is Sphere && this == obj as Sphere;
        }
        public override int GetHashCode() {
            return center.GetHashCode() + radius.GetHashCode();
        }
    }
}
