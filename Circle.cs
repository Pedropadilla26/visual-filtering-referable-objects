﻿/// 
/// Creado por Pedro Padilla Reyes para el trabajo fin de grado en Ingeniería Informática por la UGR.
/// 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{

	internal class Circle : Shape
	{

		public int Radius { get; set; }

		public Circle(SolidColorBrush color, Size size, Quadrants quadrant, PointCollection points, int radius)
			: base(ShapeType.Circle, color, size, quadrant, points)
		{
            this.Radius = radius;
            this.Area = Math.PI * Math.Pow(this.Radius, 2);
        }
    }
}
