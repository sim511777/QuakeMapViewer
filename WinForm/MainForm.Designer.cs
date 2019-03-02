namespace WinForm {
    partial class MainForm {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlView = new System.Windows.Forms.Panel();
            this.lblFps = new System.Windows.Forms.Label();
            this.lblFace = new System.Windows.Forms.Label();
            this.rdoTexture = new System.Windows.Forms.RadioButton();
            this.rdoLightmap = new System.Windows.Forms.RadioButton();
            this.chkNovis = new System.Windows.Forms.CheckBox();
            this.chkDrawEntity = new System.Windows.Forms.CheckBox();
            this.btnstart = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnE1m1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnE1m1);
            this.panel1.Controls.Add(this.btnLoad);
            this.panel1.Controls.Add(this.btnstart);
            this.panel1.Controls.Add(this.chkDrawEntity);
            this.panel1.Controls.Add(this.chkNovis);
            this.panel1.Controls.Add(this.rdoLightmap);
            this.panel1.Controls.Add(this.rdoTexture);
            this.panel1.Controls.Add(this.lblFace);
            this.panel1.Controls.Add(this.lblFps);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(222, 483);
            this.panel1.TabIndex = 0;
            // 
            // pnlView
            // 
            this.pnlView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlView.Location = new System.Drawing.Point(222, 0);
            this.pnlView.Name = "pnlView";
            this.pnlView.Size = new System.Drawing.Size(578, 483);
            this.pnlView.TabIndex = 1;
            // 
            // lblFps
            // 
            this.lblFps.AutoSize = true;
            this.lblFps.Location = new System.Drawing.Point(12, 9);
            this.lblFps.Name = "lblFps";
            this.lblFps.Size = new System.Drawing.Size(44, 12);
            this.lblFps.TabIndex = 0;
            this.lblFps.Text = "FTP:60";
            // 
            // lblFace
            // 
            this.lblFace.AutoSize = true;
            this.lblFace.Location = new System.Drawing.Point(84, 9);
            this.lblFace.Name = "lblFace";
            this.lblFace.Size = new System.Drawing.Size(57, 12);
            this.lblFace.TabIndex = 0;
            this.lblFace.Text = "face:2000";
            // 
            // rdoTexture
            // 
            this.rdoTexture.AutoSize = true;
            this.rdoTexture.Location = new System.Drawing.Point(14, 24);
            this.rdoTexture.Name = "rdoTexture";
            this.rdoTexture.Size = new System.Drawing.Size(66, 16);
            this.rdoTexture.TabIndex = 1;
            this.rdoTexture.TabStop = true;
            this.rdoTexture.Text = "Texture";
            this.rdoTexture.UseVisualStyleBackColor = true;
            // 
            // rdoLightmap
            // 
            this.rdoLightmap.AutoSize = true;
            this.rdoLightmap.Location = new System.Drawing.Point(86, 24);
            this.rdoLightmap.Name = "rdoLightmap";
            this.rdoLightmap.Size = new System.Drawing.Size(75, 16);
            this.rdoLightmap.TabIndex = 1;
            this.rdoLightmap.TabStop = true;
            this.rdoLightmap.Text = "Lightmap";
            this.rdoLightmap.UseVisualStyleBackColor = true;
            // 
            // chkNovis
            // 
            this.chkNovis.AutoSize = true;
            this.chkNovis.Location = new System.Drawing.Point(14, 46);
            this.chkNovis.Name = "chkNovis";
            this.chkNovis.Size = new System.Drawing.Size(58, 16);
            this.chkNovis.TabIndex = 2;
            this.chkNovis.Text = "NoVis";
            this.chkNovis.UseVisualStyleBackColor = true;
            // 
            // chkDrawEntity
            // 
            this.chkDrawEntity.AutoSize = true;
            this.chkDrawEntity.Location = new System.Drawing.Point(86, 46);
            this.chkDrawEntity.Name = "chkDrawEntity";
            this.chkDrawEntity.Size = new System.Drawing.Size(84, 16);
            this.chkDrawEntity.TabIndex = 2;
            this.chkDrawEntity.Text = "DrawEntity";
            this.chkDrawEntity.UseVisualStyleBackColor = true;
            // 
            // btnstart
            // 
            this.btnstart.Location = new System.Drawing.Point(12, 68);
            this.btnstart.Name = "btnstart";
            this.btnstart.Size = new System.Drawing.Size(75, 23);
            this.btnstart.TabIndex = 3;
            this.btnstart.Text = "start";
            this.btnstart.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(93, 68);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(75, 23);
            this.btnLoad.TabIndex = 3;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            // 
            // btnE1m1
            // 
            this.btnE1m1.Location = new System.Drawing.Point(14, 97);
            this.btnE1m1.Name = "btnE1m1";
            this.btnE1m1.Size = new System.Drawing.Size(75, 23);
            this.btnE1m1.TabIndex = 3;
            this.btnE1m1.Text = "e1m1";
            this.btnE1m1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 483);
            this.Controls.Add(this.pnlView);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlView;
        private System.Windows.Forms.Label lblFace;
        private System.Windows.Forms.Label lblFps;
        private System.Windows.Forms.RadioButton rdoTexture;
        private System.Windows.Forms.RadioButton rdoLightmap;
        private System.Windows.Forms.Button btnE1m1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnstart;
        private System.Windows.Forms.CheckBox chkDrawEntity;
        private System.Windows.Forms.CheckBox chkNovis;
    }
}

