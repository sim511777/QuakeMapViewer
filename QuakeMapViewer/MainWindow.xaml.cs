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
      private Bsp bsp = null;
      private byte[] vis = new byte[Bsp.MAX_MAP_LEAFS / 8];
      private AmbientLight ambientLight = new AmbientLight(Color.FromRgb(255,255,255));
      private Model3DGroup modelGroup;

      public MainWindow() {
         InitializeComponent();
         Bsp.LoadPalette();

         this.modelGroup = new Model3DGroup();
         ModelVisual3D visual = new ModelVisual3D();
         visual.Content = this.modelGroup;
         this.view.Children.Add(visual);
         CompositionTarget.Rendering += CompositionTarget_Rendering;
      }

      double keyAngleSpeed = 5;
      double mouseAngleSPeed = 0.1;
      double moveSpeed = 500;

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

      private void ProcessInput(double dTime) {
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
      }

      private void UpdateScene(bool novis) {
         var models = new Model3DCollection();
         models.Add(this.ambientLight);

         if (this.bsp == null)
            return;

         if (novis) {
            foreach (var geoModel in this.bsp.geoModels)
               models.Add(geoModel);
         } else {
            int leafNum = 0;
            int faceNum = 0;
            var currLeafIdx = GetCurrLeafIdx();
            var currLeaf = this.bsp.leafs[currLeafIdx];
            this.bsp.DecompressVis(this.vis, currLeaf);
            for (int i = 1; i < this.bsp.models[0].numleafs; i++) {
               var bVisible = vis[i >> 3] & (1 << (i & 7));
               if (bVisible != 0) {
                  leafNum++;
                  var leaf = this.bsp.leafs[i];
                  int firstLFace = leaf.lface_id;
                  int lastLFace = firstLFace + leaf.lface_num;
                  for (int lFaceId = firstLFace, cnt = 0; lFaceId < lastLFace; lFaceId++, cnt++) {
                     int surfaceId = this.bsp.lface[lFaceId];
                     models.Add(this.bsp.geoModels[surfaceId]);
                     faceNum++;
                  }
               }
            }
            this.tbkVis.Text = $"currLeafIdx: {currLeafIdx}, leafNum: {leafNum}, faceNum: {faceNum}";
         }

         this.modelGroup.Children = models;
      }

      private int GetCurrLeafIdx() {
		   short iNode = (short)this.bsp.models[0].node_id0;
         while (true) {
            if (iNode < 0)
               return -iNode - 1;
				var node = this.bsp.nodes[iNode];
            var plane = this.bsp.planes[node.plane_id];
            var dist = Vector3D.DotProduct((Vector3D)this.camPos, plane.normal) - plane.dist;
            if (dist > 0.0f)
					iNode = node.front;
				else
					iNode = node.back;
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

      private void btnLoad_Click(object sender, RoutedEventArgs e) {
         OpenFileDialog dlg = new OpenFileDialog();
         if (dlg.ShowDialog(this) == false)
            return;

         var buf = File.ReadAllBytes(dlg.FileName);
         this.LoadBsp(buf);
      }

      private void btnE1m1_Click(object sender, RoutedEventArgs e) {
         var buf = Properties.Resources.e1m1;
         this.LoadBsp(buf);
      }

      private void LoadBsp(byte[] buf) {
         this.bsp = Bsp.Read(buf, (bool)this.rdoTexture.IsChecked);
         this.LoadCamera();
      }

      private void CompositionTarget_Rendering(object sender, EventArgs e) {
         DateTime timeNow = DateTime.Now;
         TimeSpan timeSpan = timeNow - timeOld;
         timeOld = timeNow;
         double dTime = timeSpan.TotalSeconds;
         double fps = 1/dTime;
         this.tbkFps.Text = $"FPS:{fps:0.}";

         if (this.viewFocus) {
            ProcessInput(dTime);
         }

         UpdateScene((bool)this.chkNoVis.IsChecked);
         UpdateCamera();
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
