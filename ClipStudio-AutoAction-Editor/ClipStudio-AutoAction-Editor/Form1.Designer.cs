namespace ClipStudio_AutoAction_Editor {
    partial class Form1 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.AALabel1 = new System.Windows.Forms.Label();
            this.AALabel2 = new System.Windows.Forms.Label();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.SaveButton1 = new System.Windows.Forms.Button();
            this.SaveButton2 = new System.Windows.Forms.Button();
            this.buttonToLeft = new System.Windows.Forms.Button();
            this.buttonToRight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.AllowDrop = true;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(15, 28);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(218, 220);
            this.listBox1.TabIndex = 0;
            this.listBox1.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox1_DragDrop);
            this.listBox1.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox1_DragEnter);
            // 
            // AALabel1
            // 
            this.AALabel1.AutoSize = true;
            this.AALabel1.Location = new System.Drawing.Point(13, 13);
            this.AALabel1.Name = "AALabel1";
            this.AALabel1.Size = new System.Drawing.Size(68, 12);
            this.AALabel1.TabIndex = 1;
            this.AALabel1.Text = "AutoAction1";
            // 
            // AALabel2
            // 
            this.AALabel2.AutoSize = true;
            this.AALabel2.Location = new System.Drawing.Point(360, 13);
            this.AALabel2.Name = "AALabel2";
            this.AALabel2.Size = new System.Drawing.Size(68, 12);
            this.AALabel2.TabIndex = 2;
            this.AALabel2.Text = "AutoAction2";
            // 
            // listBox2
            // 
            this.listBox2.AllowDrop = true;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(362, 28);
            this.listBox2.Name = "listBox2";
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox2.Size = new System.Drawing.Size(218, 220);
            this.listBox2.TabIndex = 3;
            this.listBox2.DragDrop += new System.Windows.Forms.DragEventHandler(this.listBox2_DragDrop);
            this.listBox2.DragEnter += new System.Windows.Forms.DragEventHandler(this.listBox2_DragEnter);
            // 
            // SaveButton1
            // 
            this.SaveButton1.Location = new System.Drawing.Point(15, 254);
            this.SaveButton1.Name = "SaveButton1";
            this.SaveButton1.Size = new System.Drawing.Size(218, 23);
            this.SaveButton1.TabIndex = 4;
            this.SaveButton1.Text = "Save";
            this.SaveButton1.UseVisualStyleBackColor = true;
            this.SaveButton1.Click += new System.EventHandler(this.SaveButton1_Click);
            // 
            // SaveButton2
            // 
            this.SaveButton2.Location = new System.Drawing.Point(362, 254);
            this.SaveButton2.Name = "SaveButton2";
            this.SaveButton2.Size = new System.Drawing.Size(218, 23);
            this.SaveButton2.TabIndex = 5;
            this.SaveButton2.Text = "Save";
            this.SaveButton2.UseVisualStyleBackColor = true;
            this.SaveButton2.Click += new System.EventHandler(this.SaveButton2_Click);
            // 
            // buttonToLeft
            // 
            this.buttonToLeft.Location = new System.Drawing.Point(259, 28);
            this.buttonToLeft.Name = "buttonToLeft";
            this.buttonToLeft.Size = new System.Drawing.Size(75, 23);
            this.buttonToLeft.TabIndex = 6;
            this.buttonToLeft.Text = "<-";
            this.buttonToLeft.UseVisualStyleBackColor = true;
            this.buttonToLeft.Click += new System.EventHandler(this.buttonToLeft_Click);
            // 
            // buttonToRight
            // 
            this.buttonToRight.Location = new System.Drawing.Point(259, 83);
            this.buttonToRight.Name = "buttonToRight";
            this.buttonToRight.Size = new System.Drawing.Size(75, 23);
            this.buttonToRight.TabIndex = 7;
            this.buttonToRight.Text = "->";
            this.buttonToRight.UseVisualStyleBackColor = true;
            this.buttonToRight.Click += new System.EventHandler(this.buttonToRight_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 286);
            this.Controls.Add(this.buttonToRight);
            this.Controls.Add(this.buttonToLeft);
            this.Controls.Add(this.SaveButton2);
            this.Controls.Add(this.SaveButton1);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.AALabel2);
            this.Controls.Add(this.AALabel1);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Auto Action Editor";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label AALabel1;
        private System.Windows.Forms.Label AALabel2;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button SaveButton1;
        private System.Windows.Forms.Button SaveButton2;
        private System.Windows.Forms.Button buttonToLeft;
        private System.Windows.Forms.Button buttonToRight;
    }
}

