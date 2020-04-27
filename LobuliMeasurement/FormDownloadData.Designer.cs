namespace LobuliMeasurement
{
    partial class FormDownloadData
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDownloadData));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Age = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Genotype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Animal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CutIdentifier = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Method = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateMeasurement = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DateStaining = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Zoom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Layer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Note = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Reanalyze = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ID,
            this.Age,
            this.Genotype,
            this.Animal,
            this.CutIdentifier,
            this.Method,
            this.DateMeasurement,
            this.DateStaining,
            this.Zoom,
            this.Layer,
            this.Note,
            this.Reanalyze});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(1192, 517);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ID
            // 
            this.ID.HeaderText = "ID";
            this.ID.MinimumWidth = 2;
            this.ID.Name = "ID";
            this.ID.ReadOnly = true;
            this.ID.Width = 50;
            // 
            // Age
            // 
            this.Age.HeaderText = "Age";
            this.Age.Name = "Age";
            this.Age.ReadOnly = true;
            this.Age.Width = 50;
            // 
            // Genotype
            // 
            this.Genotype.HeaderText = "GenoTypeIsKO";
            this.Genotype.Name = "Genotype";
            this.Genotype.ReadOnly = true;
            this.Genotype.Width = 75;
            // 
            // Animal
            // 
            this.Animal.HeaderText = "Animal";
            this.Animal.Name = "Animal";
            this.Animal.ReadOnly = true;
            // 
            // CutIdentifier
            // 
            this.CutIdentifier.HeaderText = "CutIdentifier";
            this.CutIdentifier.Name = "CutIdentifier";
            this.CutIdentifier.ReadOnly = true;
            // 
            // Method
            // 
            this.Method.HeaderText = "Method";
            this.Method.Name = "Method";
            this.Method.ReadOnly = true;
            // 
            // DateMeasurement
            // 
            this.DateMeasurement.HeaderText = "DateMeasurement";
            this.DateMeasurement.Name = "DateMeasurement";
            this.DateMeasurement.ReadOnly = true;
            this.DateMeasurement.Width = 120;
            // 
            // DateStaining
            // 
            this.DateStaining.HeaderText = "DateStaining";
            this.DateStaining.Name = "DateStaining";
            this.DateStaining.ReadOnly = true;
            this.DateStaining.Width = 120;
            // 
            // Zoom
            // 
            this.Zoom.HeaderText = "Zoom";
            this.Zoom.Name = "Zoom";
            this.Zoom.ReadOnly = true;
            this.Zoom.Width = 50;
            // 
            // Layer
            // 
            this.Layer.HeaderText = "Layer";
            this.Layer.Name = "Layer";
            this.Layer.ReadOnly = true;
            // 
            // Note
            // 
            this.Note.HeaderText = "Note";
            this.Note.Name = "Note";
            this.Note.ReadOnly = true;
            this.Note.Width = 200;
            // 
            // Reanalyze
            // 
            this.Reanalyze.HeaderText = "Reanalyze";
            this.Reanalyze.Name = "Reanalyze";
            this.Reanalyze.ReadOnly = true;
            // 
            // FormDownloadData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 517);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormDownloadData";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reanalyze uploadet data";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Age;
        private System.Windows.Forms.DataGridViewTextBoxColumn Genotype;
        private System.Windows.Forms.DataGridViewTextBoxColumn Animal;
        private System.Windows.Forms.DataGridViewTextBoxColumn CutIdentifier;
        private System.Windows.Forms.DataGridViewTextBoxColumn Method;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateMeasurement;
        private System.Windows.Forms.DataGridViewTextBoxColumn DateStaining;
        private System.Windows.Forms.DataGridViewTextBoxColumn Zoom;
        private System.Windows.Forms.DataGridViewTextBoxColumn Layer;
        private System.Windows.Forms.DataGridViewTextBoxColumn Note;
        private System.Windows.Forms.DataGridViewButtonColumn Reanalyze;
    }
}