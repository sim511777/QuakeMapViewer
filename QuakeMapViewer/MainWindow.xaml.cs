﻿using System;
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
         timer.Interval = TimeSpan.FromMilliseconds(16.7);
         timer.Tick += timer_Tick;
         timer.Start();
      }

      private void timer_Tick(object sender, EventArgs e) {
         if (!this.view.IsFocused)
            return;

         double angleSpeed = 0.005;
         
         var newPt = ScreenMouse.Position;
         var diff = newPt-oldPt;
         ScreenMouse.Position = newPt;

         this.camPitch -= diff.Y * angleSpeed;
         if (this.camPitch > Math.PI / 2)
            this.camPitch = Math.PI / 2;
         if (this.camPitch < -Math.PI / 2)
            this.camPitch = -Math.PI / 2;
         this.camYaw -= diff.X * angleSpeed;

         double moveSpeed = 10;
         Vector3D vMove = new Vector3D(0,0,0);
         if (Keyboard.IsKeyDown(Key.Right))  vMove.X += 1;
         if (Keyboard.IsKeyDown(Key.Left))   vMove.X -= 1;
         if (Keyboard.IsKeyDown(Key.Up))     vMove.Y += 1;
         if (Keyboard.IsKeyDown(Key.Down))   vMove.Y -= 1;
         if (vMove.Length != 0) {
            vMove.Normalize();
            vMove = vMove * moveSpeed;
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

      Point oldPt = new Point(0,0);
      private void view_GotFocus(object sender, RoutedEventArgs e) {
         oldPt = ScreenMouse.Position;
         this.Cursor = Cursors.Cross;
      }

      private void view_LostFocus(object sender, RoutedEventArgs e) {
         this.Cursor = Cursors.Arrow;
      }

      private void Window_GotFocus(object sender, RoutedEventArgs e) {

      }

      private void Window_LostFocus(object sender, RoutedEventArgs e) {

      }
   }
}
