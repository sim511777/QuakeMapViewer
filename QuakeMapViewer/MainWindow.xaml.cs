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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace QuakeMapViewer {
   /// <summary>
   /// MainWindow.xaml에 대한 상호 작용 논리
   /// </summary>
   public partial class MainWindow : Window {
      private DispatcherTimer timer;
      private Bsp bsp = null;

      public MainWindow() {
         InitializeComponent();
         Bsp.LoadPalette();
         TimerStart();
      }

      private void TimerStart() {
         timer = new DispatcherTimer();
         timer.Interval = TimeSpan.FromMilliseconds(5);
         timer.Tick += timer_Tick;
         timer.Start();
      }

      double keyAngleSpeed = 5;
      double mouseAngleSPeed = 0.5;
      double moveSpeed = 500;

      private void timer_Tick(object sender, EventArgs e) {
         if (!this.viewFocus)
            return;

         DateTime timeNow = DateTime.Now;
         TimeSpan timeSpan = timeNow - timeOld;
         timeOld = timeNow;
         double dTime = timeSpan.TotalSeconds;
         double fps = 1/dTime;
         this.lblFps.Content = $"{fps:0.}";

         if (Keyboard.IsKeyDown(Key.Right))  this.camYaw    -= keyAngleSpeed * dTime;
         if (Keyboard.IsKeyDown(Key.Left))   this.camYaw    += keyAngleSpeed * dTime;
         if (Keyboard.IsKeyDown(Key.Up))     this.camPitch  += keyAngleSpeed * dTime;
         if (Keyboard.IsKeyDown(Key.Down))   this.camPitch  -= keyAngleSpeed * dTime;
         
         var diff = ScreenMouse.Position-viewCenterPt;
         ScreenMouse.Position = viewCenterPt;

         this.camPitch -= diff.Y * mouseAngleSPeed * dTime;
         if (this.camPitch > Math.PI / 2 -0.01)
            this.camPitch = Math.PI / 2 -0.01;
         if (this.camPitch < -Math.PI / 2 +0.01)
            this.camPitch = -Math.PI / 2 +0.01;
         this.camYaw -= diff.X * mouseAngleSPeed * dTime;

         Vector3D vMove = new Vector3D(0,0,0);
         if (Keyboard.IsKeyDown(Key.D))   vMove.X += 1;
         if (Keyboard.IsKeyDown(Key.A))   vMove.X -= 1;
         if (Keyboard.IsKeyDown(Key.W))   vMove.Y += 1;
         if (Keyboard.IsKeyDown(Key.S))   vMove.Y -= 1;
         if (vMove.Length != 0) {
            vMove.Normalize();
            vMove = vMove * moveSpeed * dTime;
            PerspectiveCamera cam = this.view.Camera as PerspectiveCamera;
            var vLook = cam.LookDirection;
            var vUp = cam.UpDirection;
            var vRight = Vector3D.CrossProduct(vLook, vUp);
            vRight.Normalize();
            var vMove2 = vLook * vMove.Y + vRight * vMove.X;
            this.camPos += vMove2;
         }

         UpdateCamera();
      }

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
         camera.FieldOfView = 90;
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

         this.view.Children.Clear();
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

      bool viewFocus = false;
      Point viewCenterPt = new Point(0,0);
      DateTime timeOld;
      private void view_MouseDown(object sender, MouseButtonEventArgs e) {
         viewFocus = true;
         this.Cursor = Cursors.None;
         this.viewCenterPt = ScreenMouse.Position;
         timeOld = DateTime.Now;
      }

      private void Window_KeyDown(object sender, KeyEventArgs e) {
         if (e.Key == Key.Escape) {
            viewFocus = false;
            this.Cursor = Cursors.Arrow; 
         }
      }
   }
}
