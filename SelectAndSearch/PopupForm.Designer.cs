namespace SelectAndSearch {
    partial class PopupForm {
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
            FindedText = new RichTextBox();
            SuspendLayout();
            // 
            // FindedText
            // 
            FindedText.Location = new Point(-1, 0);
            FindedText.Name = "FindedText";
            FindedText.Size = new Size(802, 452);
            FindedText.TabIndex = 0;
            FindedText.Text = "";
            // 
            // PopupForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoValidate = AutoValidate.EnablePreventFocusChange;
            ClientSize = new Size(800, 450);
            Controls.Add(FindedText);
            FormBorderStyle = FormBorderStyle.None;
            Name = "PopupForm";
            Text = "PopupForm";
            ResumeLayout(false);
        }

        #endregion

        public RichTextBox FindedText;
    }
}