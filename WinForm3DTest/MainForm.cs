using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;
using WinForm3DTest.Properties;

namespace WinForm3DTest {
    public partial class MainForm : Form {
        private Direct3D direct3D;
        private Device device;
        private VertexBuffer vertices;
        private Effect effect;
        private DateTime oldTime;

        public MainForm() {
            InitializeComponent();
        }

        // 초기화
        private void Init3d() {
            // D3D 라이브러리 초기화
            this.direct3D = new Direct3D();
            // 장치 초기화
            this.device = new Device(direct3D, 0, DeviceType.Hardware, this.pnlView.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(this.pnlView.ClientSize.Width, this.pnlView.ClientSize.Height));

            // 버텍스 버퍼 생성
            this.vertices = new VertexBuffer(device, Utilities.SizeOf<Vector4>() * 2 * 36, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            this.vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                // position                             color
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
            });
            this.vertices.Unlock();

            // 버텍스 버퍼를 장치 데이터 스트림에 바인딩
            device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() * 2);


            // 이펙트 생성
            this.effect = Effect.FromString(device, Resources.MiniCube, ShaderFlags.None);

            // 버텍스 엘리먼트 생성 (버텍스 버퍼에 대한 정보)
            var vertexElems = new[] {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            };

            // 버텍스 선언 생성
            var vertexDecl = new VertexDeclaration(device, vertexElems);
            // 디바이스에 버텍스 선언 세팅
            device.VertexDeclaration = vertexDecl;

            // 초기 시간 측정
            this.oldTime = DateTime.Now;
        }

        // 리소스 해제
        private void Free3d() {
            this.effect.Dispose();
            this.vertices.Dispose();
            this.device.Dispose();
            this.direct3D.Dispose();
        }

        // 렌더
        private float totalTime = 0.0f;
        private void RenderFrame(float time) {
            totalTime += time;

            // 장치 클리어
            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            // 장면 시작
            device.BeginScene();

            // 이펙트 시작
            effect.Begin();
            // 이펙트 패스 시작 (0번 테크닉으로)
            effect.BeginPass(0);

            // 행렬 계산
            var view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, this.pnlView.ClientSize.Width / (float)this.pnlView.ClientSize.Height, 0.1f, 100.0f);
            var viewProj = Matrix.Multiply(view, proj);
            var worldViewProj = Matrix.RotationX(totalTime) * Matrix.RotationY(totalTime * 2) * Matrix.RotationZ(totalTime * 0.7f) * viewProj;
            // 이펙트에 행렬 세팅
            effect.SetValue("worldViewProj", worldViewProj);

            // 실제로 그림
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);

            // 이펙트 패스 종료
            effect.EndPass();
            // 이펙트 종료
            effect.End();

            // 장면 종료
            device.EndScene();

            // 백버퍼의 내용을 화면에 표시
            device.Present();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            this.Init3d();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.Free3d();
        }

        private void tmr1_Tick(object sender, EventArgs e) {
            var nowTime = DateTime.Now;
            var time = (float)(nowTime - oldTime).TotalSeconds;
            oldTime = nowTime;

            this.RenderFrame(time);
        }
    }
}
