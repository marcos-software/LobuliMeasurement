namespace LobuliMeasurement
{
    partial class FormMain
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.lblHead = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnDpMeasSingle = new System.Windows.Forms.Button();
            this.btnDpMeasMulti = new System.Windows.Forms.Button();
            this.btnDp = new System.Windows.Forms.Button();
            this.btnMl = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblHead
            // 
            this.lblHead.AutoSize = true;
            this.lblHead.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHead.Location = new System.Drawing.Point(12, 29);
            this.lblHead.Name = "lblHead";
            this.lblHead.Size = new System.Drawing.Size(250, 29);
            this.lblHead.TabIndex = 0;
            this.lblHead.Text = "Lobuli Measurement";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(215, 434);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(14, 58);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(45, 13);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Version:";
            // 
            // btnDpMeasSingle
            // 
            this.btnDpMeasSingle.Location = new System.Drawing.Point(17, 94);
            this.btnDpMeasSingle.Name = "btnDpMeasSingle";
            this.btnDpMeasSingle.Size = new System.Drawing.Size(272, 23);
            this.btnDpMeasSingle.TabIndex = 3;
            this.btnDpMeasSingle.Text = "Analyze local file";
            this.btnDpMeasSingle.UseVisualStyleBackColor = true;
            this.btnDpMeasSingle.Click += new System.EventHandler(this.btnDpMeasSingle_Click);
            // 
            // btnDpMeasMulti
            // 
            this.btnDpMeasMulti.Location = new System.Drawing.Point(17, 132);
            this.btnDpMeasMulti.Name = "btnDpMeasMulti";
            this.btnDpMeasMulti.Size = new System.Drawing.Size(272, 23);
            this.btnDpMeasMulti.TabIndex = 4;
            this.btnDpMeasMulti.Text = "Reanalyze specific uploaded data";
            this.btnDpMeasMulti.UseVisualStyleBackColor = true;
            this.btnDpMeasMulti.Click += new System.EventHandler(this.btnDpMeasMulti_Click);
            // 
            // btnDp
            // 
            this.btnDp.Location = new System.Drawing.Point(17, 248);
            this.btnDp.Name = "btnDp";
            this.btnDp.Size = new System.Drawing.Size(272, 23);
            this.btnDp.TabIndex = 5;
            this.btnDp.Text = "Show results";
            this.btnDp.UseVisualStyleBackColor = true;
            this.btnDp.Click += new System.EventHandler(this.btnDp_Click);
            // 
            // btnMl
            // 
            this.btnMl.Location = new System.Drawing.Point(17, 289);
            this.btnMl.Name = "btnMl";
            this.btnMl.Size = new System.Drawing.Size(272, 23);
            this.btnMl.TabIndex = 6;
            this.btnMl.Text = "Machine Learning";
            this.btnMl.UseVisualStyleBackColor = true;
            this.btnMl.Click += new System.EventHandler(this.btnMl_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 172);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(272, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Automatic Reanalyze all uploaded data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(18, 330);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(272, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Calculate Ratio";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(18, 371);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(272, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Correct Fissure Perimeter";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 469);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnMl);
            this.Controls.Add(this.btnDp);
            this.Controls.Add(this.btnDpMeasMulti);
            this.Controls.Add(this.btnDpMeasSingle);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblHead);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lobuli Measurement";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHead;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Button btnDpMeasSingle;
        private System.Windows.Forms.Button btnDpMeasMulti;
        private System.Windows.Forms.Button btnDp;
        private System.Windows.Forms.Button btnMl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

