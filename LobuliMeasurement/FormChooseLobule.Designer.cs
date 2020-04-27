namespace LobuliMeasurement
{
    partial class FormChooseLobule
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txbStartX = new System.Windows.Forms.TextBox();
            this.txbEndX = new System.Windows.Forms.TextBox();
            this.txbStartY = new System.Windows.Forms.TextBox();
            this.txbEndY = new System.Windows.Forms.TextBox();
            this.btnPickStart = new System.Windows.Forms.Button();
            this.btnPickEnd = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnNotExist = new System.Windows.Forms.Button();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.picZoom = new System.Windows.Forms.PictureBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.lbladvice = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose coordinates for lobule:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(168, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "#1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Start point";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 105);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "End point";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(72, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "X:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(72, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "X:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(139, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(139, 105);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Y:";
            // 
            // txbStartX
            // 
            this.txbStartX.Location = new System.Drawing.Point(87, 64);
            this.txbStartX.Name = "txbStartX";
            this.txbStartX.Size = new System.Drawing.Size(41, 20);
            this.txbStartX.TabIndex = 8;
            // 
            // txbEndX
            // 
            this.txbEndX.Location = new System.Drawing.Point(87, 102);
            this.txbEndX.Name = "txbEndX";
            this.txbEndX.Size = new System.Drawing.Size(41, 20);
            this.txbEndX.TabIndex = 9;
            // 
            // txbStartY
            // 
            this.txbStartY.Location = new System.Drawing.Point(153, 64);
            this.txbStartY.Name = "txbStartY";
            this.txbStartY.Size = new System.Drawing.Size(41, 20);
            this.txbStartY.TabIndex = 10;
            // 
            // txbEndY
            // 
            this.txbEndY.Location = new System.Drawing.Point(153, 102);
            this.txbEndY.Name = "txbEndY";
            this.txbEndY.Size = new System.Drawing.Size(41, 20);
            this.txbEndY.TabIndex = 11;
            // 
            // btnPickStart
            // 
            this.btnPickStart.Location = new System.Drawing.Point(210, 62);
            this.btnPickStart.Name = "btnPickStart";
            this.btnPickStart.Size = new System.Drawing.Size(37, 23);
            this.btnPickStart.TabIndex = 12;
            this.btnPickStart.Text = "Pick";
            this.btnPickStart.UseVisualStyleBackColor = true;
            this.btnPickStart.Click += new System.EventHandler(this.btnPickStart_Click);
            // 
            // btnPickEnd
            // 
            this.btnPickEnd.Location = new System.Drawing.Point(210, 100);
            this.btnPickEnd.Name = "btnPickEnd";
            this.btnPickEnd.Size = new System.Drawing.Size(37, 23);
            this.btnPickEnd.TabIndex = 13;
            this.btnPickEnd.Text = "Pick";
            this.btnPickEnd.UseVisualStyleBackColor = true;
            this.btnPickEnd.Click += new System.EventHandler(this.btnPickEnd_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(171, 214);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 14;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnNotExist
            // 
            this.btnNotExist.Location = new System.Drawing.Point(9, 214);
            this.btnNotExist.Name = "btnNotExist";
            this.btnNotExist.Size = new System.Drawing.Size(147, 23);
            this.btnNotExist.TabIndex = 15;
            this.btnNotExist.Text = "Lobule does not exist";
            this.btnNotExist.UseVisualStyleBackColor = true;
            this.btnNotExist.Click += new System.EventHandler(this.btnNotExist_Click);
            // 
            // picImage
            // 
            this.picImage.Cursor = System.Windows.Forms.Cursors.Default;
            this.picImage.Location = new System.Drawing.Point(264, 18);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(1024, 768);
            this.picImage.TabIndex = 16;
            this.picImage.TabStop = false;
            this.picImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseClick);
            this.picImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picImage_MouseMove);
            // 
            // picZoom
            // 
            this.picZoom.Location = new System.Drawing.Point(12, 324);
            this.picZoom.Name = "picZoom";
            this.picZoom.Size = new System.Drawing.Size(240, 240);
            this.picZoom.TabIndex = 17;
            this.picZoom.TabStop = false;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(9, 139);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 18;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // lbladvice
            // 
            this.lbladvice.AutoSize = true;
            this.lbladvice.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbladvice.ForeColor = System.Drawing.Color.Red;
            this.lbladvice.Location = new System.Drawing.Point(12, 270);
            this.lbladvice.Name = "lbladvice";
            this.lbladvice.Size = new System.Drawing.Size(164, 16);
            this.lbladvice.TabIndex = 19;
            this.lbladvice.Text = "Now pick START point";
            // 
            // FormChooseLobule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1299, 794);
            this.Controls.Add(this.lbladvice);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.picZoom);
            this.Controls.Add(this.picImage);
            this.Controls.Add(this.btnNotExist);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnPickEnd);
            this.Controls.Add(this.btnPickStart);
            this.Controls.Add(this.txbEndY);
            this.Controls.Add(this.txbStartY);
            this.Controls.Add(this.txbEndX);
            this.Controls.Add(this.txbStartX);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormChooseLobule";
            this.Text = "Choose Lobule Points";
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txbStartX;
        private System.Windows.Forms.TextBox txbEndX;
        private System.Windows.Forms.TextBox txbStartY;
        private System.Windows.Forms.TextBox txbEndY;
        private System.Windows.Forms.Button btnPickStart;
        private System.Windows.Forms.Button btnPickEnd;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnNotExist;
        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.PictureBox picZoom;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lbladvice;
    }
}