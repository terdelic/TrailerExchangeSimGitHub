namespace Prekapcanje3
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.gmap = new GMap.NET.WindowsForms.GMapControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCurrentEvalTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rtxtEvaluiranaPrekapcanja = new System.Windows.Forms.RichTextBox();
            this.checkPrekapcanje = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnZavrsi = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVozilo2EndTime = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVozilo1EndTime = new System.Windows.Forms.TextBox();
            this.btnSimRealTime = new System.Windows.Forms.Button();
            this.btnUcitaj = new System.Windows.Forms.Button();
            this.timerRealTime = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.gmap, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(939, 607);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // gmap
            // 
            this.gmap.Bearing = 0F;
            this.gmap.CanDragMap = true;
            this.gmap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gmap.EmptyTileColor = System.Drawing.Color.Navy;
            this.gmap.GrayScaleMode = false;
            this.gmap.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gmap.LevelsKeepInMemory = 5;
            this.gmap.Location = new System.Drawing.Point(163, 3);
            this.gmap.MarkersEnabled = true;
            this.gmap.MaxZoom = 2;
            this.gmap.MinZoom = 2;
            this.gmap.MouseWheelZoomEnabled = true;
            this.gmap.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            this.gmap.Name = "gmap";
            this.gmap.NegativeMode = false;
            this.gmap.PolygonsEnabled = true;
            this.gmap.RetryLoadTile = 0;
            this.gmap.RoutesEnabled = true;
            this.gmap.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gmap.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gmap.ShowTileGridLines = false;
            this.gmap.Size = new System.Drawing.Size(773, 601);
            this.gmap.TabIndex = 0;
            this.gmap.Zoom = 0D;
            this.gmap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseDown);
            this.gmap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseMove);
            this.gmap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gmap_MouseUp);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtCurrentEvalTime);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.rtxtEvaluiranaPrekapcanja);
            this.groupBox1.Controls.Add(this.checkPrekapcanje);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.btnZavrsi);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtVozilo2EndTime);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtVozilo1EndTime);
            this.groupBox1.Controls.Add(this.btnSimRealTime);
            this.groupBox1.Controls.Add(this.btnUcitaj);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 601);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Options";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 112);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 16);
            this.label6.TabIndex = 20;
            this.label6.Text = "Current time period:";
            // 
            // txtCurrentEvalTime
            // 
            this.txtCurrentEvalTime.Location = new System.Drawing.Point(0, 134);
            this.txtCurrentEvalTime.Name = "txtCurrentEvalTime";
            this.txtCurrentEvalTime.Size = new System.Drawing.Size(154, 22);
            this.txtCurrentEvalTime.TabIndex = 19;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(2, 318);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 16);
            this.label5.TabIndex = 18;
            this.label5.Text = "Exchnage point details";
            // 
            // rtxtEvaluiranaPrekapcanja
            // 
            this.rtxtEvaluiranaPrekapcanja.Location = new System.Drawing.Point(5, 337);
            this.rtxtEvaluiranaPrekapcanja.Name = "rtxtEvaluiranaPrekapcanja";
            this.rtxtEvaluiranaPrekapcanja.Size = new System.Drawing.Size(142, 130);
            this.rtxtEvaluiranaPrekapcanja.TabIndex = 17;
            this.rtxtEvaluiranaPrekapcanja.Text = "";
            // 
            // checkPrekapcanje
            // 
            this.checkPrekapcanje.AutoSize = true;
            this.checkPrekapcanje.Location = new System.Drawing.Point(1, 50);
            this.checkPrekapcanje.Name = "checkPrekapcanje";
            this.checkPrekapcanje.Size = new System.Drawing.Size(129, 20);
            this.checkPrekapcanje.TabIndex = 16;
            this.checkPrekapcanje.Text = "Check exchange";
            this.checkPrekapcanje.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1, 191);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(154, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Continue simulation";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnZavrsi
            // 
            this.btnZavrsi.Location = new System.Drawing.Point(1, 162);
            this.btnZavrsi.Name = "btnZavrsi";
            this.btnZavrsi.Size = new System.Drawing.Size(151, 23);
            this.btnZavrsi.TabIndex = 13;
            this.btnZavrsi.Text = "Stop simulation";
            this.btnZavrsi.UseVisualStyleBackColor = true;
            this.btnZavrsi.Click += new System.EventHandler(this.btnZavrsi_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 271);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Predicted time trailer 2:";
            // 
            // txtVozilo2EndTime
            // 
            this.txtVozilo2EndTime.Location = new System.Drawing.Point(1, 290);
            this.txtVozilo2EndTime.Name = "txtVozilo2EndTime";
            this.txtVozilo2EndTime.Size = new System.Drawing.Size(151, 22);
            this.txtVozilo2EndTime.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 219);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Predicted time trailer 1:";
            // 
            // txtVozilo1EndTime
            // 
            this.txtVozilo1EndTime.Location = new System.Drawing.Point(0, 238);
            this.txtVozilo1EndTime.Name = "txtVozilo1EndTime";
            this.txtVozilo1EndTime.Size = new System.Drawing.Size(152, 22);
            this.txtVozilo1EndTime.TabIndex = 5;
            // 
            // btnSimRealTime
            // 
            this.btnSimRealTime.Location = new System.Drawing.Point(0, 86);
            this.btnSimRealTime.Name = "btnSimRealTime";
            this.btnSimRealTime.Size = new System.Drawing.Size(154, 23);
            this.btnSimRealTime.TabIndex = 4;
            this.btnSimRealTime.Text = "Real-time simulation";
            this.btnSimRealTime.UseVisualStyleBackColor = true;
            this.btnSimRealTime.Click += new System.EventHandler(this.btnSimRealTime_Click);
            // 
            // btnUcitaj
            // 
            this.btnUcitaj.Location = new System.Drawing.Point(0, 21);
            this.btnUcitaj.Name = "btnUcitaj";
            this.btnUcitaj.Size = new System.Drawing.Size(154, 23);
            this.btnUcitaj.TabIndex = 2;
            this.btnUcitaj.Text = "Load data";
            this.btnUcitaj.UseVisualStyleBackColor = true;
            this.btnUcitaj.Click += new System.EventHandler(this.btnUcitaj_Click_1);
            // 
            // timerRealTime
            // 
            this.timerRealTime.Interval = 2000;
            this.timerRealTime.Tick += new System.EventHandler(this.timerRealTime_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 607);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Map";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private GMap.NET.WindowsForms.GMapControl gmap;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUcitaj;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtVozilo2EndTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtVozilo1EndTime;
        private System.Windows.Forms.Button btnSimRealTime;
        private System.Windows.Forms.Timer timerRealTime;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkPrekapcanje;
        private System.Windows.Forms.Button btnZavrsi;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCurrentEvalTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox rtxtEvaluiranaPrekapcanja;
    }
}

