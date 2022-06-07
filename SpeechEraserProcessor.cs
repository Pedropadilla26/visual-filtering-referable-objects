using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace Visual_filtering_referable_objects
{
	internal class SpeechEraserProcessor
	{
		string positionInterpreter = "";

		public SpeechEraserProcessor()
		{
		}
		public SpeechEraserProcessor(string interpreterOption)
		{
			this.positionInterpreter = interpreterOption;
		}
		public void ChangePositionInterpreter(ReadOnlyCollection<RecognizedWordUnit> words)
        {
			string newInterpretation = words[words.Count-1].Text.ToLower();
			if (newInterpretation != this.positionInterpreter)
			{

				CustomMessageBox.AddTextSystem("Se va a cambiar la interpretación de posiciones a " + newInterpretation);
				this.positionInterpreter = words[words.Count-1].Text.ToLower();
			}
		}
		public List<Shape> EraseShapes(List<Shape> initialShapes, ReadOnlyCollection<RecognizedWordUnit> words)
		{
			Quadrants quadrantToSearch1 = Quadrants.None;
			Quadrants quadrantToSearch2 = Quadrants.None;
			ShapeType shapeToSearch = ShapeType.None;
			SolidColorBrush color = System.Windows.Media.Brushes.White;
			Size size = Size.None;
			Positions positionToSearch = Positions.None;

			bool isValidStart = false;
			string startCommand = words[0].Text.ToLower() + " " + words[1].Text.ToLower();
			string shapeString = words[2].Text.ToLower();
			SearchType searchType = startCommand == "borra los" ? SearchType.Multiple : startCommand == "borra el" ? SearchType.Single : SearchType.None;
			string wholeText = "";
			for (int i = 0; i < words.Count; i++)
			{
				wholeText = wholeText + ' ' + words[i].Text;
			}

			CustomMessageBox.AddTextSystem("realizando borrado...");

			switch (searchType)
			{
				case SearchType.Multiple:
					switch (shapeString)
					{
						case "triángulos":
							shapeToSearch = ShapeType.Triangle;
							isValidStart = true;
							break;
						case "cuadrados":
							shapeToSearch = ShapeType.Square;
							isValidStart = true;
							break;
						case "círculos":
							shapeToSearch = ShapeType.Circle;
							isValidStart = true;
							break;
						default:
							break;
					}
					if (isValidStart)
					{
						//CustomMessageBox.AddTextSystem("Se va a ejecutar la siguiente acción de borrado: " + wholeText);

						for (int i = 3; i < words.Count; i++)
						{
							string word = words[i].Text.ToLower();
							switch (word)
							{
								case "azules":
									color = System.Windows.Media.Brushes.Blue;
									break;
								case "negros":
									color = System.Windows.Media.Brushes.Black;
									break;
								case "rojos":
									color = System.Windows.Media.Brushes.Red;
									break;
								case "morados":
									color = System.Windows.Media.Brushes.Purple;
									break;
								case "amarillos":
									color = System.Windows.Media.Brushes.Yellow;
									break;
								case "verdes":
									color = System.Windows.Media.Brushes.Green;
									break;
								case "naranjas":
									color = System.Windows.Media.Brushes.Orange;
									break;
								case "rosas":
									color = System.Windows.Media.Brushes.Pink;
									break;

								case "estén":
									string firstPositionWord = words[i + 1].Text.ToLower();
									string secondPositionWord = "";
									switch (firstPositionWord)
									{
										case "arriba":
											secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : "";
											switch (secondPositionWord)
											{
												case "a la izquierda":
													quadrantToSearch1 = Quadrants.Top_left;
													break;
												case "a la derecha":
													quadrantToSearch1 = Quadrants.Top_right;
													break;
												default:
													quadrantToSearch1 = Quadrants.Top_left;
													quadrantToSearch2 = Quadrants.Top_right;
													break;
											}
											break;
										case "abajo":
											secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : "";
											switch (secondPositionWord)
											{
												case "a la izquierda":
													quadrantToSearch1 = Quadrants.Bottom_left;
													break;
												case "a la derecha":
													quadrantToSearch1 = Quadrants.Bottom_right;
													break;
												default:
													quadrantToSearch1 = Quadrants.Bottom_left;
													quadrantToSearch2 = Quadrants.Bottom_right;
													break;
											}
											break;
										case "a":
											secondPositionWord = words.Count > i + 3 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() : "";
											if (this.positionInterpreter == "")
											{
												string messageAboutPositionInterpreter = "Las posiciones que ha dicho son ambiguas, no sé si se refiere a los " + shapeString + " en esa posición con respecto a los otros o con respecto a todas las figuras." +
													"\nPor defecto, asumo que se refiere a todas las figuras, si no habría dicho 'los " + shapeString + " más a " + secondPositionWord +
													"'\nSi quiere cambiar esto, use el comando de voz 'Interpreta las posiciones como absolutas/relativas";

												CustomMessageBox.AddTextSystem(messageAboutPositionInterpreter);
												this.positionInterpreter = "absolutas";
											}
											if (this.positionInterpreter == "absolutas")
											{
												switch (secondPositionWord)
												{
													case "la izquierda":
														quadrantToSearch1 = Quadrants.Bottom_left;
														quadrantToSearch2 = Quadrants.Top_left;
														break;
													case "la derecha":
														quadrantToSearch1 = Quadrants.Bottom_right;
														quadrantToSearch1 = Quadrants.Top_right;
														break;
													default:
														break;
												}
											}
											else if (this.positionInterpreter == "relativas")
                                            {
												switch (secondPositionWord)
												{
													case "la izquierda":
														positionToSearch = Positions.Left;
														break;
													case "la derecha":
														positionToSearch = Positions.Right;
														break;
													case "arriba":
														positionToSearch = Positions.Top;
														break;
													case "abajo":
														positionToSearch = Positions.Bottom;
														break;
												}
											}
											break;
										case "en":
											string centerWord = words.Count > i + 3 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() : "";
											if (centerWord == "el centro")
												quadrantToSearch1 = Quadrants.Center;
											break;
										case "más":
											secondPositionWord = words.Count > i + 4 ? words[i + 2].Text.ToLower() + " " + words[i + 3].Text.ToLower() + " " + words[i + 4].Text.ToLower() : words[i + 2].Text.ToLower();
                                            switch (secondPositionWord)
                                            {
												case "a la izquierda":
													positionToSearch = Positions.Left;
													break;
												case "a la derecha":
													positionToSearch = Positions.Right;
													break;
												case "arriba":
													positionToSearch = Positions.Top;
													break;
												case "abajo":
													positionToSearch = Positions.Bottom;
													break;
											}
											break;
									}

									break;

								case "grandes":
									size = Size.Big;
									break;
								case "medianos":
									size = Size.Medium;
									break;
								case "pequeños":
									size = Size.Small;
									break;
								default:
									break;
							}
						}

					}
					break;
				case SearchType.Single:
					//CustomMessageBox.AddTextSystem("Se va a ejecutar la siguiente acción de borrado: " + wholeText);
					switch (shapeString)
					{
						case "triángulo":
							shapeToSearch = ShapeType.Triangle;
							isValidStart = true;
							break;
						case "cuadrado":
							shapeToSearch = ShapeType.Square;
							isValidStart = true;
							break;
						case "círculo":
							shapeToSearch = ShapeType.Circle;
							isValidStart = true;
							break;
						default:
							break;
					}
					if (isValidStart)
					{
						string word = words.Count() > 3 ? words[3].Text.ToLower() : "";
						string wordFor = words.Count() > 4 ? words[4].Text.ToLower() : "";
						switch (word)
						{
							case "azul":
								color = System.Windows.Media.Brushes.Blue;
								break;
							case "negro":
								color = System.Windows.Media.Brushes.Black;
								break;
							case "rojo":
								color = System.Windows.Media.Brushes.Red;
								break;
							case "morado":
								color = System.Windows.Media.Brushes.Purple;
								break;
							case "amarillo":
								color = System.Windows.Media.Brushes.Yellow;
								break;
							case "verde":
								color = System.Windows.Media.Brushes.Green;
								break;
							case "naranja":
								color = System.Windows.Media.Brushes.Orange;
								break;
							case "rosa":
								color = System.Windows.Media.Brushes.Pink;
								break;
						}
						if (word == "más" || wordFor == "más")
						{
							string sizeWord = word == "más" ? words[4].Text.ToLower() : words[5].Text.ToLower();
							switch (sizeWord)
							{
								case "grande":
									size = Size.Big;
									break;
								case "mediano":
									size = Size.Medium;
									break;
								case "pequeño":
									size = Size.Small;
									break;
								case "a":
									string positionWord = word == "más" ? words[6].Text.ToLower() : words[7].Text.ToLower();
									switch (positionWord)
                                    {
										case "izquierda":
											positionToSearch = Positions.Left;
											break;
										case "derecha":
											positionToSearch = Positions.Right;
											break;
										default:
											break;
									}
									break;
								case "arriba":
									positionToSearch = Positions.Top;
									break;
								case "abajo":
									positionToSearch = Positions.Bottom;
									break;
								default:
									break;
							}
						}
					}
					break;
			}
			if (isValidStart)
			{
				Boolean anyMatch = false;

				if (searchType == SearchType.Multiple)
				{
					List<Shape> shapesCopy = new List<Shape>(initialShapes);
					for (int i = 0; i < shapesCopy.Count(); i++)
					{
						Shape shape = shapesCopy[i];
						shape.Size = CalculateSizeFromIterator(shapesCopy, i);

						if (MatchesShape(shapesCopy, shape, shapeToSearch, color, size, quadrantToSearch1, quadrantToSearch2, positionToSearch))
						{
							initialShapes.Remove(shapesCopy[i]);
							anyMatch = true;
						}
					}
				}
				else if (searchType == SearchType.Single)
				{
					List<Shape> geometricShapesList = GetSortedShapesOfType(shapeToSearch, initialShapes);
					Boolean warnedAboutPlural = false;

					for (int i = 0; i < geometricShapesList.Count(); i++)
					{
						Shape shape = geometricShapesList[i];
						if ((color == shape.Color || color == System.Windows.Media.Brushes.White) && IsBiggestOrSmallestShape(geometricShapesList, size, i) && IsExtremePosition(geometricShapesList, positionToSearch, shape))
						{
							if (anyMatch && !warnedAboutPlural) {
								string shapePropertiesMessage = "Hay más de un " + shapeString;
								if (words.Count() > 3){
									shapePropertiesMessage += " de color " + words[3].Text.ToLower();
								}
								CustomMessageBox.AddTextSystem(shapePropertiesMessage + ", los voy a borrar todos, pero mejor hable en plural si hay varios.");
								warnedAboutPlural = true;
							}
							initialShapes.Remove(geometricShapesList[i]);
							anyMatch = true;
						}
					}
				}

				if (!anyMatch)
				{
					CustomMessageBox.AddTextSystem("No encuentro ninguna forma que coincida con la descripción");
				}
				else
                {
					CustomMessageBox.AddTextSystem("He borrado las figuras que coincidían.");
				}
			}
			return initialShapes;
		}
		private Boolean MatchesShape(
			List<Shape> shapes,
			Shape shape,
			ShapeType shapeToSearch,
			SolidColorBrush color,
			Size size,
			Quadrants quadrantToSearch1,
			Quadrants quadrantToSearch2,
			Positions positionToSearch)
		{

			if (shapeToSearch != shape.GeometricShape)
			{
				return false;
			}
			if (color != shape.Color && color != System.Windows.Media.Brushes.White)
			{
				return false;
			}
			if (size != shape.Size && size != Size.None)
			{
				return false;
			}
			if (positionToSearch != Positions.None) {
				if (!IsExtremePositionSoft(shapes, positionToSearch, shape, shapeToSearch)) return false;
			}
			else if (quadrantToSearch1 != shape.Quadrant && quadrantToSearch1 != Quadrants.None && quadrantToSearch2 != shape.Quadrant)
			{
				return false;
			}

			return true;
		}

		private Size CalculateSizeFromIterator(List<Shape> list, int i)
		{
			if (i <= list.Count() / 3)
			{
				return Size.Big;
			}
			else if (i <= list.Count() * 2 / 3)
			{
				return Size.Medium;
			}
			else
			{
				return Size.Small;
			}
		}

		private Boolean IsBiggestOrSmallestShape(List<Shape> list, Size size, int i)
		{
			if (list.Count() == 1 || size == Size.None)
			{
				return true;
			}
			if (size == Size.Big)
			{
				if (i == 0)
				{
					return true;
				}
			}
			else if (size == Size.Small)
			{
				if (i == list.Count() - 1)
				{
					return true;
				}
			}
			return false;
		}

		private List<Shape> GetSortedShapesOfType(ShapeType shapeType, List<Shape> shapes)
		{
			List<Shape> geometricShapesList = new List<Shape>();
			foreach (var shape in shapes)
			{
				if (shapeType == shape.GeometricShape)
				{
					geometricShapesList.Add(shape);
				}
			}
			return geometricShapesList;
		}

		public Boolean IsExtremePositionSoft(List<Shape> shapes, Positions positionToSearch, Shape shapeToSearch, ShapeType shapeType)
        {
			Boolean result = false;

			List<Shape> geometricShapesList = GetSortedShapesOfType(shapeType, shapes);


			if (geometricShapesList.Count < 2)
            {
				return true;
            }

			switch (positionToSearch)
			{
				case Positions.Left:
					geometricShapesList.Sort(delegate (Shape x, Shape y)
					{
						return (x.Points[0].X < y.Points[0].X ? -1 : 1);
					});
					break;
				case Positions.Right:
					geometricShapesList.Sort(delegate (Shape x, Shape y)
					{
						return (x.Points[0].X < y.Points[0].X ? 1 : -1);
					});
					break;
				case Positions.Top:
					geometricShapesList.Sort(delegate (Shape x, Shape y)
					{
						return (x.Points[0].Y > y.Points[0].Y ? 1 : -1);
					});
					break;
				case Positions.Bottom:
					geometricShapesList.Sort(delegate (Shape x, Shape y)
					{
						return (x.Points[0].Y > y.Points[0].Y ? -1 : 1);
					});
					break;
			
			}
			if (geometricShapesList.Count() < 3)
            {
				if (geometricShapesList.IndexOf(shapeToSearch) == 0) result = true;
			}
			else if (geometricShapesList.IndexOf(shapeToSearch) < geometricShapesList.Count / 3) result = true;
			return result;
        }


		public Boolean IsExtremePosition(List<Shape> shapes, Positions positionToSearch, Shape shapeToSearch)
		{
			if (positionToSearch == Positions.None || shapes.Count < 2)
			{
				return true;
			}
			Boolean result = true;
			foreach (Shape shape in shapes)
			{
				if (shapeToSearch != shape)
				{
					switch (positionToSearch)
					{
						case Positions.Left:
							if (shape.Points[0].X < shapeToSearch.Points[0].X)
							{
								result = false;
							}
							break;
						case Positions.Right:
							if (shape.Points[0].X > shapeToSearch.Points[0].X)
							{
								result = false;
							}
							break;
						case Positions.Top:
							if (shape.Points[0].Y < shapeToSearch.Points[0].Y)
							{
								result = false;
							}
							break;
						case Positions.Bottom:
							if (shape.Points[0].Y > shapeToSearch.Points[0].Y)
							{
								result = false;
							}
							break;
					}
				}
			}
			return result;
		}

	}
}