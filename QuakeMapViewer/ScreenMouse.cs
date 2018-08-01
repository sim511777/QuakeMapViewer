using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuakeMapViewer {
   class ScreenMouse {
      public static System.Windows.Point Position {
         get {
            var pt = System.Windows.Forms.Cursor.Position;
            return new System.Windows.Point(pt.X, pt.Y);
         }
         set {
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)value.X, (int)value.Y);
         }
      }
   }
}
