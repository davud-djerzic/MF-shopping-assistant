namespace MF_Shopping_Assistant
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtScannedBarcode = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnQuantityIncrease = new System.Windows.Forms.Button();
            this.btnQuantityDecrease = new System.Windows.Forms.Button();
            this.lblNumberOfProducts = new System.Windows.Forms.Label();
            this.btnFinishProduct = new System.Windows.Forms.Button();
            this.panelProductQuantity = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.panelConfirmPayment = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.panelUpdateProductQuantity = new System.Windows.Forms.Panel();
            this.btnUpdateProduct = new System.Windows.Forms.Button();
            this.lblUpdateNumberOfProducts = new System.Windows.Forms.Label();
            this.btnUpdateQuantityDecrease = new System.Windows.Forms.Button();
            this.btnUpdateQuantityIncrease = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.btnPay = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panelProductQuantity.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelConfirmPayment.SuspendLayout();
            this.panelUpdateProductQuantity.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 300;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(550, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(346, 68);
            this.panel1.TabIndex = 2;
            this.panel1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(315, 1);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 4;
            this.button2.Tag = "btnRemoveProduct";
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(142, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "label4";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "label3";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(298, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "label2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // txtScannedBarcode
            // 
            this.txtScannedBarcode.Location = new System.Drawing.Point(560, 357);
            this.txtScannedBarcode.Name = "txtScannedBarcode";
            this.txtScannedBarcode.Size = new System.Drawing.Size(206, 22);
            this.txtScannedBarcode.TabIndex = 0;
            this.txtScannedBarcode.TextChanged += new System.EventHandler(this.txtScannedBarcode_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(581, 397);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Enter += new System.EventHandler(this.button1_Enter);
            // 
            // btnQuantityIncrease
            // 
            this.btnQuantityIncrease.Location = new System.Drawing.Point(112, 64);
            this.btnQuantityIncrease.Name = "btnQuantityIncrease";
            this.btnQuantityIncrease.Size = new System.Drawing.Size(75, 23);
            this.btnQuantityIncrease.TabIndex = 0;
            this.btnQuantityIncrease.Text = "+";
            this.btnQuantityIncrease.UseVisualStyleBackColor = true;
            this.btnQuantityIncrease.Click += new System.EventHandler(this.btnQuantityIncrease_Click);
            // 
            // btnQuantityDecrease
            // 
            this.btnQuantityDecrease.Location = new System.Drawing.Point(31, 64);
            this.btnQuantityDecrease.Name = "btnQuantityDecrease";
            this.btnQuantityDecrease.Size = new System.Drawing.Size(75, 23);
            this.btnQuantityDecrease.TabIndex = 1;
            this.btnQuantityDecrease.Text = "-";
            this.btnQuantityDecrease.UseVisualStyleBackColor = true;
            this.btnQuantityDecrease.Click += new System.EventHandler(this.btnQuantityDecrease_Click);
            // 
            // lblNumberOfProducts
            // 
            this.lblNumberOfProducts.AutoSize = true;
            this.lblNumberOfProducts.Location = new System.Drawing.Point(92, 22);
            this.lblNumberOfProducts.Name = "lblNumberOfProducts";
            this.lblNumberOfProducts.Size = new System.Drawing.Size(14, 16);
            this.lblNumberOfProducts.TabIndex = 2;
            this.lblNumberOfProducts.Text = "1";
            // 
            // btnFinishProduct
            // 
            this.btnFinishProduct.Location = new System.Drawing.Point(64, 109);
            this.btnFinishProduct.Name = "btnFinishProduct";
            this.btnFinishProduct.Size = new System.Drawing.Size(75, 23);
            this.btnFinishProduct.TabIndex = 3;
            this.btnFinishProduct.Text = "Finish";
            this.btnFinishProduct.UseVisualStyleBackColor = true;
            this.btnFinishProduct.Click += new System.EventHandler(this.btnFinishProduct_Click);
            // 
            // panelProductQuantity
            // 
            this.panelProductQuantity.Controls.Add(this.btnFinishProduct);
            this.panelProductQuantity.Controls.Add(this.lblNumberOfProducts);
            this.panelProductQuantity.Controls.Add(this.btnQuantityDecrease);
            this.panelProductQuantity.Controls.Add(this.btnQuantityIncrease);
            this.panelProductQuantity.Location = new System.Drawing.Point(611, 185);
            this.panelProductQuantity.Name = "panelProductQuantity";
            this.panelProductQuantity.Size = new System.Drawing.Size(200, 135);
            this.panelProductQuantity.TabIndex = 6;
            this.panelProductQuantity.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "label8";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(298, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(44, 16);
            this.label7.TabIndex = 1;
            this.label7.Text = "label7";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(44, 16);
            this.label6.TabIndex = 2;
            this.label6.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(142, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "label5";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(315, 1);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(28, 23);
            this.button3.TabIndex = 4;
            this.button3.Tag = "btnRemoveProduct";
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Location = new System.Drawing.Point(550, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(346, 68);
            this.panel2.TabIndex = 5;
            this.panel2.Visible = false;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(476, 389);
            this.flowLayoutPanel1.TabIndex = 7;
            this.flowLayoutPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseDown);
            this.flowLayoutPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseMove);
            this.flowLayoutPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.flowLayoutPanel1_MouseUp);
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.AutoSize = true;
            this.lblTotalPrice.Location = new System.Drawing.Point(404, 17);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(33, 16);
            this.lblTotalPrice.TabIndex = 8;
            this.lblTotalPrice.Text = "0KM";
            // 
            // panelConfirmPayment
            // 
            this.panelConfirmPayment.Controls.Add(this.label9);
            this.panelConfirmPayment.Controls.Add(this.lblTotalPrice);
            this.panelConfirmPayment.Location = new System.Drawing.Point(2, 404);
            this.panelConfirmPayment.Name = "panelConfirmPayment";
            this.panelConfirmPayment.Size = new System.Drawing.Size(476, 49);
            this.panelConfirmPayment.TabIndex = 9;
            this.panelConfirmPayment.Click += new System.EventHandler(this.panelConfirmPayment_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "Click to pay";
            // 
            // panelUpdateProductQuantity
            // 
            this.panelUpdateProductQuantity.Controls.Add(this.btnUpdateProduct);
            this.panelUpdateProductQuantity.Controls.Add(this.lblUpdateNumberOfProducts);
            this.panelUpdateProductQuantity.Controls.Add(this.btnUpdateQuantityDecrease);
            this.panelUpdateProductQuantity.Controls.Add(this.btnUpdateQuantityIncrease);
            this.panelUpdateProductQuantity.Location = new System.Drawing.Point(851, 185);
            this.panelUpdateProductQuantity.Name = "panelUpdateProductQuantity";
            this.panelUpdateProductQuantity.Size = new System.Drawing.Size(200, 135);
            this.panelUpdateProductQuantity.TabIndex = 7;
            this.panelUpdateProductQuantity.Visible = false;
            // 
            // btnUpdateProduct
            // 
            this.btnUpdateProduct.Location = new System.Drawing.Point(64, 109);
            this.btnUpdateProduct.Name = "btnUpdateProduct";
            this.btnUpdateProduct.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateProduct.TabIndex = 3;
            this.btnUpdateProduct.Text = "Finish";
            this.btnUpdateProduct.UseVisualStyleBackColor = true;
            this.btnUpdateProduct.Click += new System.EventHandler(this.btnUpdateProduct_Click);
            // 
            // lblUpdateNumberOfProducts
            // 
            this.lblUpdateNumberOfProducts.AutoSize = true;
            this.lblUpdateNumberOfProducts.Location = new System.Drawing.Point(92, 22);
            this.lblUpdateNumberOfProducts.Name = "lblUpdateNumberOfProducts";
            this.lblUpdateNumberOfProducts.Size = new System.Drawing.Size(14, 16);
            this.lblUpdateNumberOfProducts.TabIndex = 2;
            this.lblUpdateNumberOfProducts.Text = "1";
            // 
            // btnUpdateQuantityDecrease
            // 
            this.btnUpdateQuantityDecrease.Location = new System.Drawing.Point(31, 64);
            this.btnUpdateQuantityDecrease.Name = "btnUpdateQuantityDecrease";
            this.btnUpdateQuantityDecrease.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateQuantityDecrease.TabIndex = 1;
            this.btnUpdateQuantityDecrease.Text = "-";
            this.btnUpdateQuantityDecrease.UseVisualStyleBackColor = true;
            this.btnUpdateQuantityDecrease.Click += new System.EventHandler(this.btnUpdateQuantityDecrease_Click);
            // 
            // btnUpdateQuantityIncrease
            // 
            this.btnUpdateQuantityIncrease.Location = new System.Drawing.Point(112, 64);
            this.btnUpdateQuantityIncrease.Name = "btnUpdateQuantityIncrease";
            this.btnUpdateQuantityIncrease.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateQuantityIncrease.TabIndex = 0;
            this.btnUpdateQuantityIncrease.Text = "+";
            this.btnUpdateQuantityIncrease.UseVisualStyleBackColor = true;
            this.btnUpdateQuantityIncrease.Click += new System.EventHandler(this.btnUpdateQuantityIncrease_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label10);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(484, 12);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(473, 341);
            this.flowLayoutPanel2.TabIndex = 0;
            this.flowLayoutPanel2.Visible = false;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(3, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(470, 22);
            this.label10.TabIndex = 10;
            this.label10.Text = "label10";
            // 
            // btnPay
            // 
            this.btnPay.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPay.Location = new System.Drawing.Point(479, 357);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(75, 44);
            this.btnPay.TabIndex = 11;
            this.btnPay.Text = "Pay";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Visible = false;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1177, 483);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.panelUpdateProductQuantity);
            this.Controls.Add(this.btnPay);
            this.Controls.Add(this.panelConfirmPayment);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panelProductQuantity);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtScannedBarcode);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelProductQuantity.ResumeLayout(false);
            this.panelProductQuantity.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panelConfirmPayment.ResumeLayout(false);
            this.panelConfirmPayment.PerformLayout();
            this.panelUpdateProductQuantity.ResumeLayout(false);
            this.panelUpdateProductQuantity.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtScannedBarcode;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnQuantityIncrease;
        private System.Windows.Forms.Button btnQuantityDecrease;
        private System.Windows.Forms.Label lblNumberOfProducts;
        private System.Windows.Forms.Button btnFinishProduct;
        private System.Windows.Forms.Panel panelProductQuantity;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label lblTotalPrice;
        private System.Windows.Forms.Panel panelConfirmPayment;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panelUpdateProductQuantity;
        private System.Windows.Forms.Button btnUpdateProduct;
        private System.Windows.Forms.Label lblUpdateNumberOfProducts;
        private System.Windows.Forms.Button btnUpdateQuantityDecrease;
        private System.Windows.Forms.Button btnUpdateQuantityIncrease;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnPay;
    }
}

