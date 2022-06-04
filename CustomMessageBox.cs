using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_filtering_referable_objects
{
    internal class CustomMessageBox
    {
		public static Boolean isActivated = true;

		public static void Show(string text)
		{
			if (isActivated)
			{
				System.Windows.MessageBox.Show(text);
			}
		}

		public static void SetActivated(Boolean setter)
		{
			isActivated = setter;
		}
	}
}
