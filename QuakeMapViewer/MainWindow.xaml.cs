using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace QuakeMapViewer {
   /// <summary>
   /// MainWindow.xaml에 대한 상호 작용 논리
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();
         this.LoadPalette();
      }

      public Color[] palette;
      public int[] colorMap;
      public Color ColorMapColor(int idx) {
         return palette[colorMap[idx]];
      }
      public Color ColorMapColor(int palIdx, int brightness) {
         int darkness = 15-brightness;
         return palette[colorMap[darkness*256+palIdx]];
      }

      private void LoadPalette() {
         var bytes = File.ReadAllBytes("palette.lmp");
         int palCnt = bytes.Length/3; 
         this.palette = Enumerable.Range(0, palCnt).Select(palIdx=>Color.FromArgb(bytes[palIdx*3], bytes[palIdx*3+1], bytes[palIdx*3+2])).ToArray();
         this.colorMap = File.ReadAllBytes("colormap.lmp").Take(64*256).Select(b=>(int)b).ToArray();
      }

      private Bsp bsp = null;

      private void btnLoad_Click(object sender, RoutedEventArgs e) {
         OpenFileDialog dlg = new OpenFileDialog();
         if (dlg.ShowDialog(this) == false)
            return;
         string filePath = dlg.FileName;
         var buf = File.ReadAllBytes(filePath);
         this.bsp = Bsp.Read(buf);
      }
   }
}
