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
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Media3D;

namespace QuakeMapViewer {
   /// <summary>
   /// MainWindow.xaml에 대한 상호 작용 논리
   /// </summary>
   public partial class MainWindow : Window {
      public MainWindow() {
         InitializeComponent();
         Bsp.LoadPalette();
      }

      private Bsp bsp = null;
      private Model3DGroup modelGroup;

      private void LoadFile(string filePath) {
         var buf = File.ReadAllBytes(filePath);
         this.bsp = Bsp.Read(buf);
      }

      private void LoadCamera() {
         var infostart = this.bsp.entities.FirstOrDefault((entity)=>entity.classname == "info_player_start");
         var words = infostart.items["origin"].Split(' ');
         PerspectiveCamera camera = new PerspectiveCamera();
         camera.Position = new Point3D(double.Parse(words[0]), double.Parse(words[1]), double.Parse(words[2]));
         double angle = double.Parse(infostart.items["angle"]) * Math.PI * 2 / 360;
         camera.LookDirection = new Vector3D(Math.Cos(angle), Math.Sin(angle), 0);
         camera.UpDirection = new Vector3D(0, 0, 1);
         this.view.Camera = camera;

      }

      private void LoadMap() {
         GeometryModel3D model = new GeometryModel3D();
         Model3DGroup modelGroup = new Model3DGroup();
         modelGroup.Children.Add(model);
         ModelVisual3D visual = new ModelVisual3D();
         visual.Content = modelGroup;
         this.view.Children.Add(visual);
      }

      private void btnLoad_Click(object sender, RoutedEventArgs e) {
         OpenFileDialog dlg = new OpenFileDialog();
         if (dlg.ShowDialog(this) == false)
            return;

         this.LoadFile(dlg.FileName);
         this.LoadCamera();
         this.LoadMap();
      }
   }
}
