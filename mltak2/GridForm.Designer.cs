namespace mltak2
{
    partial class GridForm
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
            this.grid = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.learnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qLearningToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SARSAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.qLambdaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sARSALambdaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.examToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qTableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.utilityProgressShowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tDLambdaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripLabel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MarkStartPointGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.MarkGoalPointGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.aDPToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid
            // 
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(0, 24);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(456, 238);
            this.grid.TabIndex = 0;
            this.grid.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.configurationsToolStripMenuItem,
            this.learnToolStripMenuItem,
            this.examToolStripMenuItem,
            this.qTableToolStripMenuItem,
            this.utilityProgressShowToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(456, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newConfigurationToolStripMenuItem,
            this.saveConfigurationToolStripMenuItem,
            this.loadConfigurationToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newConfigurationToolStripMenuItem
            // 
            this.newConfigurationToolStripMenuItem.Name = "newConfigurationToolStripMenuItem";
            this.newConfigurationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newConfigurationToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.newConfigurationToolStripMenuItem.Text = "New Configuration";
            this.newConfigurationToolStripMenuItem.Click += new System.EventHandler(this.newConfigurationToolStripMenuItem_Click);
            // 
            // saveConfigurationToolStripMenuItem
            // 
            this.saveConfigurationToolStripMenuItem.Name = "saveConfigurationToolStripMenuItem";
            this.saveConfigurationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveConfigurationToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.saveConfigurationToolStripMenuItem.Text = "Save Configuration";
            this.saveConfigurationToolStripMenuItem.Click += new System.EventHandler(this.saveConfigurationToolStripMenuItem_Click);
            // 
            // loadConfigurationToolStripMenuItem
            // 
            this.loadConfigurationToolStripMenuItem.Name = "loadConfigurationToolStripMenuItem";
            this.loadConfigurationToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.loadConfigurationToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.loadConfigurationToolStripMenuItem.Text = "Load Configuration";
            this.loadConfigurationToolStripMenuItem.Click += new System.EventHandler(this.loadConfigurationToolStripMenuItem_Click);
            // 
            // configurationsToolStripMenuItem
            // 
            this.configurationsToolStripMenuItem.Name = "configurationsToolStripMenuItem";
            this.configurationsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.configurationsToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.configurationsToolStripMenuItem.Text = "&Configurations";
            this.configurationsToolStripMenuItem.Click += new System.EventHandler(this.configurationsToolStripMenuItem_Click);
            // 
            // learnToolStripMenuItem
            // 
            this.learnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.qLearningToolStripMenuItem,
            this.SARSAToolStripMenuItem,
            this.toolStripSeparator1,
            this.qLambdaToolStripMenuItem,
            this.sARSALambdaToolStripMenuItem});
            this.learnToolStripMenuItem.Name = "learnToolStripMenuItem";
            this.learnToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.learnToolStripMenuItem.Text = "&Learn";
            // 
            // qLearningToolStripMenuItem
            // 
            this.qLearningToolStripMenuItem.Name = "qLearningToolStripMenuItem";
            this.qLearningToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.qLearningToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.qLearningToolStripMenuItem.Text = "QLearning";
            this.qLearningToolStripMenuItem.Click += new System.EventHandler(this.learnGridToolStripMenuItem_Click);
            // 
            // SARSAToolStripMenuItem
            // 
            this.SARSAToolStripMenuItem.Name = "SARSAToolStripMenuItem";
            this.SARSAToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.SARSAToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.SARSAToolStripMenuItem.Text = "SARSA";
            this.SARSAToolStripMenuItem.Click += new System.EventHandler(this.learnGridToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(230, 6);
            // 
            // qLambdaToolStripMenuItem
            // 
            this.qLambdaToolStripMenuItem.Name = "qLambdaToolStripMenuItem";
            this.qLambdaToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.Q)));
            this.qLambdaToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.qLambdaToolStripMenuItem.Text = "Q(Lambda)";
            this.qLambdaToolStripMenuItem.Click += new System.EventHandler(this.learnGridToolStripMenuItem_Click);
            // 
            // sARSALambdaToolStripMenuItem
            // 
            this.sARSALambdaToolStripMenuItem.Name = "sARSALambdaToolStripMenuItem";
            this.sARSALambdaToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.R)));
            this.sARSALambdaToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.sARSALambdaToolStripMenuItem.Text = "SARSA(Lambda)";
            this.sARSALambdaToolStripMenuItem.Click += new System.EventHandler(this.learnGridToolStripMenuItem_Click);
            // 
            // examToolStripMenuItem
            // 
            this.examToolStripMenuItem.Enabled = false;
            this.examToolStripMenuItem.Name = "examToolStripMenuItem";
            this.examToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.examToolStripMenuItem.Text = "&Exam";
            this.examToolStripMenuItem.Click += new System.EventHandler(this.examToolStripMenuItem_Click);
            // 
            // qTableToolStripMenuItem
            // 
            this.qTableToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.qTableToolStripMenuItem.Name = "qTableToolStripMenuItem";
            this.qTableToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.qTableToolStripMenuItem.Text = "&Policy";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
                        | System.Windows.Forms.Keys.O)));
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.loadToolStripMenuItem.Text = "&Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // utilityProgressShowToolStripMenuItem
            // 
            this.utilityProgressShowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tDLambdaToolStripMenuItem,
            this.aDPToolStripMenuItem});
            this.utilityProgressShowToolStripMenuItem.Name = "utilityProgressShowToolStripMenuItem";
            this.utilityProgressShowToolStripMenuItem.Size = new System.Drawing.Size(130, 20);
            this.utilityProgressShowToolStripMenuItem.Text = "&Utility Progress Show";
            // 
            // tDLambdaToolStripMenuItem
            // 
            this.tDLambdaToolStripMenuItem.Name = "tDLambdaToolStripMenuItem";
            this.tDLambdaToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.tDLambdaToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.tDLambdaToolStripMenuItem.Text = "TDLambda";
            this.tDLambdaToolStripMenuItem.Click += new System.EventHandler(this.tDLambdaProgressShowToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.toolStrip.Location = new System.Drawing.Point(0, 237);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(456, 25);
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(0, 22);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MarkStartPointGrid,
            this.MarkGoalPointGrid});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(143, 48);
            // 
            // MarkStartPointGrid
            // 
            this.MarkStartPointGrid.Name = "MarkStartPointGrid";
            this.MarkStartPointGrid.Size = new System.Drawing.Size(142, 22);
            this.MarkStartPointGrid.Text = "Mark as &Start";
            this.MarkStartPointGrid.Click += new System.EventHandler(this.MarkStartPointGrid_Click);
            // 
            // MarkGoalPointGrid
            // 
            this.MarkGoalPointGrid.Name = "MarkGoalPointGrid";
            this.MarkGoalPointGrid.Size = new System.Drawing.Size(142, 22);
            this.MarkGoalPointGrid.Text = "Mark as &Goal";
            this.MarkGoalPointGrid.Click += new System.EventHandler(this.MarkGoalPointGrid_Click);
            // 
            // aDPToolStripMenuItem
            // 
            this.aDPToolStripMenuItem.Name = "aDPToolStripMenuItem";
            this.aDPToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.aDPToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.aDPToolStripMenuItem.Text = "ADP";
            this.aDPToolStripMenuItem.Click += new System.EventHandler(this.tDLambdaProgressShowToolStripMenuItem_Click);
            // 
            // GridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 262);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GridForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grid";
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox grid;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadConfigurationToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripStatus;
        private System.Windows.Forms.ToolStripMenuItem newConfigurationToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem MarkStartPointGrid;
        private System.Windows.Forms.ToolStripMenuItem MarkGoalPointGrid;
        private System.Windows.Forms.ToolStripMenuItem configurationsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem learnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qLearningToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SARSAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem qLambdaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sARSALambdaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qTableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem utilityProgressShowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tDLambdaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aDPToolStripMenuItem;
    }
}

