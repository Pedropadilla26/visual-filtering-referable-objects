﻿/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
using System;
using System.Windows;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{

    internal class Circle : Shape
    {

        public int Radius { get; set; }

        public Circle(ColorsEnum colorEnumGenerated, SolidColorBrush color, Quadrants quadrant, PointCollection points, int radius)
            : base(ShapeType.Circle, colorEnumGenerated, color, quadrant, points)
        {
            Radius = radius;
            Area = Math.PI * Math.Pow(Radius, 2);
        }

        public override RectangleGeometry GetBoundingBox()
        {
            Point center = Points[0];
            Point top_left = new Point(center.X - Radius, center.Y - Radius);
            Point bottom_right = new Point(center.X + Radius, center.Y + Radius);
            return new RectangleGeometry(new Rect(top_left, bottom_right));
        }
    }
}
