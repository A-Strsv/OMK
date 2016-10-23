namespace OMK
{
    partial class FormInterpreterDialog
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
            this.label = new System.Windows.Forms.Label();
            this.textBox = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(12, 9);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(29, 13);
            this.label.TabIndex = 0;
            this.label.Text = "label";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(12, 96);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(179, 20);
            this.textBox.TabIndex = 1;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(197, 93);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "button1";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // FormInterpreterDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 128);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.label);
            this.Name = "FormInterpreterDialog";
            this.Text = "Запрос на ввод числа";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button buttonOK;

    }
}