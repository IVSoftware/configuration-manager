namespace configuration_manager
{
    partial class MainForm
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
            textBox = new TextBox();
            checkBox = new CheckBox();
            numericUpDown1 = new NumericUpDown();
            trackBar1 = new TrackBar();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Location = new Point(30, 35);
            textBox.Margin = new Padding(4, 4, 4, 4);
            textBox.Name = "textBox";
            textBox.Size = new Size(331, 39);
            textBox.TabIndex = 0;
            // 
            // checkBox
            // 
            checkBox.AutoSize = true;
            checkBox.Location = new Point(30, 81);
            checkBox.Name = "checkBox";
            checkBox.Size = new Size(132, 36);
            checkBox.TabIndex = 1;
            checkBox.Text = "Checked";
            checkBox.UseVisualStyleBackColor = true;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(385, 36);
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(81, 39);
            numericUpDown1.TabIndex = 2;
            numericUpDown1.TextAlign = HorizontalAlignment.Center;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(30, 140);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(331, 69);
            trackBar1.TabIndex = 3;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(478, 244);
            Controls.Add(trackBar1);
            Controls.Add(numericUpDown1);
            Controls.Add(checkBox);
            Controls.Add(textBox);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Margin = new Padding(4, 4, 4, 4);
            Name = "MainForm";
            Text = "Main Form";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox;
        private CheckBox checkBox;
        private NumericUpDown numericUpDown1;
        private TrackBar trackBar1;
    }
}