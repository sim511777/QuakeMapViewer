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
using System.Windows.Media;
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

      private void LoadFile(string filePath) {
         var buf = File.ReadAllBytes(filePath);
         this.bsp = Bsp.Read(buf);
      }

      Point3D camPos = new Point3D();
      double camYaw = 0;
      double camPitch = 0;

      private void LoadCamera() {
         try {
            var infostart = this.bsp.entities.FirstOrDefault((entity) => entity.classname == "info_player_start");
            var words = infostart.items["origin"].Split(' ');
            PerspectiveCamera camera = new PerspectiveCamera();
            camPos = new Point3D(double.Parse(words[0]), double.Parse(words[1]), double.Parse(words[2]));
            camYaw = double.Parse(infostart.items["angle"]) * Math.PI * 2 / 360;
            camPitch = 0;
         } catch {
            camPos = new Point3D(0, 0, 0);
            camYaw = 0;
            camPitch = 0;
         }
      }

      private void UpdateCamera() {
         PerspectiveCamera camera = new PerspectiveCamera();
         camera.Position = camPos;
         camera.LookDirection = new Vector3D(Math.Cos(camPitch)*Math.Cos(camYaw), Math.Cos(camPitch)*Math.Sin(camYaw), Math.Sin(camPitch));
         camera.UpDirection = new Vector3D(0, 0, 1);
         this.view.Camera = camera;
      }

      private void LoadMap() {
         Model3DGroup modelGroup = new Model3DGroup();
         foreach (var model in this.bsp.geoModels)
            modelGroup.Children.Add(model);
         AmbientLight alit = new AmbientLight(Colors.White);
         modelGroup.Children.Add(alit);
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
         this.UpdateCamera();
         this.LoadMap();
      }

      bool mlb = false;
      Point oldPt = new Point(0, 0);
      private void view_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
         mlb = true;
         oldPt = e.GetPosition(this.view);
      }

      private void view_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
         mlb = false;
      }

      private void view_MouseMove(object sender, MouseEventArgs e) {
         if (!mlb)
            return;

         var newPt = e.GetPosition(this.view);
         var diff = newPt-oldPt;
         oldPt = newPt;

         this.camPitch -= diff.Y * 0.001;
         if (this.camPitch > Math.PI / 2)
            this.camPitch = Math.PI / 2;
         if (this.camPitch < -Math.PI / 2)
            this.camPitch = -Math.PI / 2;
         this.camYaw -= diff.X * 0.001;
         UpdateCamera();
      }
   }
}
