namespace CellularAutomaton.UI.WinForms
{
    partial class LifeViewer
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridPictureBox = new System.Windows.Forms.PictureBox();
            this.CreateRandomButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.SpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.X1Num = new System.Windows.Forms.NumericUpDown();
            this.X2Num = new System.Windows.Forms.NumericUpDown();
            this.RunButton = new System.Windows.Forms.Button();
            this.GenerationTextBox = new System.Windows.Forms.TextBox();
            this.generationLabel = new System.Windows.Forms.Label();
            this.LoadButton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.DelayTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpeedTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.X1Num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.X2Num)).BeginInit();
            this.SuspendLayout();
            // 
            // gridPictureBox
            // 
            this.gridPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPictureBox.Location = new System.Drawing.Point(9, 75);
            this.gridPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.gridPictureBox.Name = "gridPictureBox";
            this.gridPictureBox.Padding = new System.Windows.Forms.Padding(2);
            this.gridPictureBox.Size = new System.Drawing.Size(738, 568);
            this.gridPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.gridPictureBox.TabIndex = 0;
            this.gridPictureBox.TabStop = false;
            // 
            // CreateRandomButton
            // 
            this.CreateRandomButton.Location = new System.Drawing.Point(20, 9);
            this.CreateRandomButton.Name = "CreateRandomButton";
            this.CreateRandomButton.Size = new System.Drawing.Size(86, 27);
            this.CreateRandomButton.TabIndex = 1;
            this.CreateRandomButton.Text = "Create new";
            this.CreateRandomButton.UseVisualStyleBackColor = true;
            this.CreateRandomButton.Click += new System.EventHandler(this.CreateRandomButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(311, 9);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(86, 27);
            this.NextButton.TabIndex = 1;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // SpeedTrackBar
            // 
            this.SpeedTrackBar.LargeChange = 100;
            this.SpeedTrackBar.Location = new System.Drawing.Point(438, 7);
            this.SpeedTrackBar.Maximum = 1000;
            this.SpeedTrackBar.Name = "SpeedTrackBar";
            this.SpeedTrackBar.Size = new System.Drawing.Size(190, 45);
            this.SpeedTrackBar.SmallChange = 10;
            this.SpeedTrackBar.TabIndex = 2;
            this.SpeedTrackBar.TickFrequency = 50;
            this.SpeedTrackBar.Value = 1;
            this.SpeedTrackBar.ValueChanged += new System.EventHandler(this.SpeedTrackBar_ValueChanged);
            // 
            // X1Num
            // 
            this.X1Num.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.X1Num.Location = new System.Drawing.Point(128, 12);
            this.X1Num.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.X1Num.Name = "X1Num";
            this.X1Num.Size = new System.Drawing.Size(69, 23);
            this.X1Num.TabIndex = 3;
            this.X1Num.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.X1Num.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // X2Num
            // 
            this.X2Num.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.X2Num.Location = new System.Drawing.Point(203, 12);
            this.X2Num.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.X2Num.Name = "X2Num";
            this.X2Num.Size = new System.Drawing.Size(73, 23);
            this.X2Num.TabIndex = 4;
            this.X2Num.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.X2Num.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(311, 42);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(86, 27);
            this.RunButton.TabIndex = 1;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // GenerationTextBox
            // 
            this.GenerationTextBox.Location = new System.Drawing.Point(510, 47);
            this.GenerationTextBox.Name = "GenerationTextBox";
            this.GenerationTextBox.Size = new System.Drawing.Size(100, 23);
            this.GenerationTextBox.TabIndex = 5;
            this.GenerationTextBox.Text = "0";
            this.GenerationTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // generationLabel
            // 
            this.generationLabel.AutoSize = true;
            this.generationLabel.Location = new System.Drawing.Point(440, 54);
            this.generationLabel.Name = "generationLabel";
            this.generationLabel.Size = new System.Drawing.Size(68, 15);
            this.generationLabel.TabIndex = 6;
            this.generationLabel.Text = "Generation:";
            // 
            // LoadButton
            // 
            this.LoadButton.Location = new System.Drawing.Point(20, 42);
            this.LoadButton.Name = "LoadButton";
            this.LoadButton.Size = new System.Drawing.Size(86, 28);
            this.LoadButton.TabIndex = 7;
            this.LoadButton.Text = "Load";
            this.LoadButton.UseVisualStyleBackColor = true;
            this.LoadButton.Click += new System.EventHandler(this.LoadButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(112, 43);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(86, 27);
            this.SaveButton.TabIndex = 8;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "\"txt files (*.txt)|*.txt|All files (*.*)|*.*\"";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "\"txt files (*.txt)|*.txt|All files (*.*)|*.*\"";
            this.openFileDialog.RestoreDirectory = true;
            // 
            // DelayTextBox
            // 
            this.DelayTextBox.Location = new System.Drawing.Point(691, 11);
            this.DelayTextBox.Name = "DelayTextBox";
            this.DelayTextBox.ReadOnly = true;
            this.DelayTextBox.Size = new System.Drawing.Size(56, 23);
            this.DelayTextBox.TabIndex = 9;
            this.DelayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(647, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "Delay:";
            // 
            // LifeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(756, 651);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DelayTextBox);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.LoadButton);
            this.Controls.Add(this.generationLabel);
            this.Controls.Add(this.GenerationTextBox);
            this.Controls.Add(this.X2Num);
            this.Controls.Add(this.X1Num);
            this.Controls.Add(this.SpeedTrackBar);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.CreateRandomButton);
            this.Controls.Add(this.gridPictureBox);
            this.Controls.Add(this.RunButton);
            this.Name = "LifeViewer";
            this.Padding = new System.Windows.Forms.Padding(2);
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gridPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SpeedTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.X1Num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.X2Num)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox gridPictureBox;
        private System.Windows.Forms.Button CreateRandomButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.TrackBar SpeedTrackBar;
        private System.Windows.Forms.NumericUpDown X1Num;
        private System.Windows.Forms.NumericUpDown X2Num;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TextBox GenerationTextBox;
        private System.Windows.Forms.Label generationLabel;
        private System.Windows.Forms.Button LoadButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.TextBox DelayTextBox;
        private System.Windows.Forms.Label label1;
    }
}

