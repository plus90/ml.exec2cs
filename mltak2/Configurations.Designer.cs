namespace mltak2
{
    partial class Configurations
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.alpha = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.maxIter = new System.Windows.Forms.NumericUpDown();
            this.cancel = new System.Windows.Forms.Button();
            this.save = new System.Windows.Forms.Button();
            this.gamma = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gridWidth = new System.Windows.Forms.ComboBox();
            this.lambda = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxIter)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Grid Size";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lambda);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.alpha);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.maxIter);
            this.groupBox1.Controls.Add(this.cancel);
            this.groupBox1.Controls.Add(this.save);
            this.groupBox1.Controls.Add(this.gamma);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.gridWidth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(394, 200);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurations";
            // 
            // alpha
            // 
            this.alpha.Location = new System.Drawing.Point(71, 79);
            this.alpha.Name = "alpha";
            this.alpha.Size = new System.Drawing.Size(137, 20);
            this.alpha.TabIndex = 2;
            this.alpha.Text = "0.2";
            this.alpha.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Alpha";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Maximum Learning Iter.";
            // 
            // maxIter
            // 
            this.maxIter.Location = new System.Drawing.Point(131, 136);
            this.maxIter.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxIter.Name = "maxIter";
            this.maxIter.Size = new System.Drawing.Size(75, 20);
            this.maxIter.TabIndex = 3;
            this.maxIter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maxIter.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.Location = new System.Drawing.Point(232, 162);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 32);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // save
            // 
            this.save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save.Location = new System.Drawing.Point(313, 162);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 32);
            this.save.TabIndex = 4;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // gamma
            // 
            this.gamma.Location = new System.Drawing.Point(71, 53);
            this.gamma.Name = "gamma";
            this.gamma.Size = new System.Drawing.Size(137, 20);
            this.gamma.TabIndex = 1;
            this.gamma.Text = "0.9";
            this.gamma.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Gamma";
            // 
            // gridWidth
            // 
            this.gridWidth.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.gridWidth.FormattingEnabled = true;
            this.gridWidth.Items.AddRange(new object[] {
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10"});
            this.gridWidth.Location = new System.Drawing.Point(71, 21);
            this.gridWidth.Name = "gridWidth";
            this.gridWidth.Size = new System.Drawing.Size(54, 21);
            this.gridWidth.TabIndex = 0;
            // 
            // lambda
            // 
            this.lambda.Location = new System.Drawing.Point(72, 106);
            this.lambda.Name = "lambda";
            this.lambda.Size = new System.Drawing.Size(137, 20);
            this.lambda.TabIndex = 12;
            this.lambda.Text = "0.9";
            this.lambda.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Lambda";
            // 
            // Configurations
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 224);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Configurations";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurations";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxIter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.TextBox gamma;
        private System.Windows.Forms.ComboBox gridWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown maxIter;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox alpha;
        private System.Windows.Forms.TextBox lambda;
        private System.Windows.Forms.Label label5;
    }
}