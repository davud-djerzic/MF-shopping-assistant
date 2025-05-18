using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.Adapters;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class Payment
    {
        private readonly MySqlConnection mySqlConnection;
        private FlowLayoutPanel flowLayoutPanel1;
        private static FlowLayoutPanel flowLayoutPanel2;
        private Button btnPay;
        private Panel panelConfirmPayment;
        private static Panel panelHeader;
        private static Panel panelPaymentTotalPrice;
        private Label lblPaymentTotalPrice;
        private Label lblMessageBoxMessage;
        private Button btnMessageBoxYes;
        private Button btnMessageBoxNo;
        private static TextBox txtSendEmail;
        private static Panel panelEmail;
        private static Button btnSendEmail;
        private static Panel panelMessageBox;
        private static Panel panelDisableBackground;
        private static TextBox txtScannedBarcode;
        private static Panel panelClickToPay;
        private Panel panelMessageBoxOk;
        private Label lblMessageBoxOK;
        private static Button btnBack;

        public Payment(MySqlConnection mySqlConnection, FlowLayoutPanel flowLayoutPanel1, FlowLayoutPanel flowLayoutPanel2, Button btnPay, Panel panelConfirmPayment, Panel panelHeader, Panel panelPaymentTotalPrice, Label lblPaymentTotalPrice, Label lblMessageBoxMessage, Button btnMessageBoxYes, Button btnMessageBoxNo, TextBox txtSendEmail, Panel panelEmail, Button btnSendEmail, Panel panelMessageBox, Panel panelDisableBackground, TextBox txtScannedBarcode, Panel panelClickToPay, Panel panelMessageBoxOk, Label lblMessageBoxOK, Button btnBack)
        {
            this.mySqlConnection = mySqlConnection;
            this.flowLayoutPanel1 = flowLayoutPanel1;
            Payment.flowLayoutPanel2 = flowLayoutPanel2;
            this.btnPay = btnPay;
            this.panelConfirmPayment = panelConfirmPayment;
            Payment.panelHeader = panelHeader;
            Payment.panelPaymentTotalPrice = panelPaymentTotalPrice;
            this.lblPaymentTotalPrice = lblPaymentTotalPrice;
            this.lblMessageBoxMessage = lblMessageBoxMessage;
            this.btnMessageBoxYes = btnMessageBoxYes;
            this.btnMessageBoxNo = btnMessageBoxNo;
            Payment.txtSendEmail = txtSendEmail;
            Payment.panelEmail = panelEmail;
            Payment.btnSendEmail = btnSendEmail;
            Payment.panelMessageBox = panelMessageBox;
            Payment.panelDisableBackground = panelDisableBackground;
            Payment.txtScannedBarcode = txtScannedBarcode;
            Payment.panelClickToPay = panelClickToPay;
            Payment.panelHeader = panelHeader;
            this.panelMessageBoxOk = panelMessageBoxOk;
            this.lblMessageBoxOK = lblMessageBoxOK;
            Payment.btnBack = btnBack;
        }
        public void PanelConfirmPayment_Click(object sender, EventArgs e) 
        {
            if (!EditProduct.isUpdateQuantityPanelOpen && !Form1.isOpenAnything)
            {
                Form1.isOpenAnything = true;
                if (GlobalData.listPriceOfProducts.Count != 0)
                {
                    try
                    {
                        //btnMessageBoxYes -=
                        UI.HideBackground(panelMessageBox, panelDisableBackground);
                        lblMessageBoxMessage.Text = "Confirm purchase";
                        btnMessageBoxYes.Click -= SendEmailYes;
                        btnMessageBoxYes.Click += ConfirmPaymentYes;
                        btnMessageBoxNo.Click -= SendEmailNo;
                        btnMessageBoxNo.Click += ConfirmPaymentNo;

                        panelClickToPay.Visible = false;
                        panelMessageBox.Visible = true;

                        //DialogResult rez = MessageBox.Show("Confirm purchase", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        /*if (rez == DialogResult.Yes)
                        {
                            if (GlobalData.listPriceOfProducts.Count == 0) throw new Exception("You don't have product to buy");

                            flowLayoutPanel1.Visible = false;
                            flowLayoutPanel2.Visible = true;
                            flowLayoutPanel2.Location = new Point(80, 125);
                            btnPay.Visible = true;
                            panelConfirmPayment.Visible = false;

                            panelHeader.Visible = true;
                            panelHeader.Location = new Point(80, 2);

                            panelPaymentTotalPrice.Visible = true;
                            panelPaymentTotalPrice.Location = new Point(80, 457);

                            for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
                            {
                                Panel panel = new Panel()
                                {
                                    Size = new Size(370, 40),
                                    //Location = new Point(100, 12),
                                    //BackColor = Color.Wheat,
                                    BorderStyle = BorderStyle.FixedSingle
                                };
                                Label label = new Label()
                                {
                                    //Size = new Size(477, 35),
                                    //Location = new Point(10, i * 40),
                                    Location = new Point(6, 2),
                                };
                                label.Text = GlobalData.listNameOfProducts[i] + "   " + GlobalData.listTypeOfProducts[i] + "   " + GlobalData.listManufacturerOfProducts[i];
                                panel.Controls.Add(label);

                                Label labelQuantity = new Label()
                                {
                                    //Size = new Size(477, 35),
                                    //Location = new Point(10, i * 40),
                                    Location = new Point(6, 25),
                                };
                                labelQuantity.Text = GlobalData.listQuantityOfProducts[i].ToString();
                                panel.Controls.Add(labelQuantity);

                                Label labelPricePerUnit = new Label()
                                {
                                    //Size = new Size(477, 35),
                                    //Location = new Point(10, i * 40),
                                    Location = new Point(180, 25),
                                };
                                labelPricePerUnit.Text = GlobalData.listPricePerUnitOfProducts[i].ToString();
                                panel.Controls.Add(labelPricePerUnit);

                                Label labelTotalPriceOfProduct = new Label()
                                {
                                    //Size = new Size(477, 35),
                                    //Location = new Point(10, i * 40),
                                    Location = new Point(280, 25),
                                };
                                labelTotalPriceOfProduct.Text = Math.Round(GlobalData.listPriceOfProducts[i], 2).ToString();
                                panel.Controls.Add(labelTotalPriceOfProduct);

                                //label.Text = GlobalData.listNameOfProducts[i] + "   " + GlobalData.listTypeOfProducts[i] + "   " + GlobalData.listManufacturerOfProducts[i] + "\n" + GlobalData.listQuantityOfProducts[i] + "x" + "   " + GlobalData.listPricePerUnitOfProducts[i] + "    " + Math.Round(GlobalData.listPriceOfProducts[i], 2);

                                panel.MouseDown += UI.flowLayoutPanel1Y_MouseDown;
                                panel.MouseUp += UI.flowLayoutPanel1Y_MouseUp;
                                panel.MouseMove += UI.flowLayoutPanel1Y_MouseMove;

                                flowLayoutPanel2.Controls.Add(panel);
                            }
                            EditProduct.CalculateTotalPrice(lblPaymentTotalPrice);
                        }*/
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                } else
                {
                    Form1.isPdfGenerated = false;
                    panelMessageBoxOk.Visible = true;
                    panelMessageBoxOk.Location = new Point(100, 100);
                    lblMessageBoxOK.Text = "You don't have a product to buy";
                    //Form1.isThereAddedProduct = false;
                    //MessageBox.Show("You dont have product to buy");
                }
                
            }
        }

        public static void SendEmailYes(object sender, EventArgs e)
        {
            btnBack.Visible = false;
            btnSendEmail.Visible = true;
            txtSendEmail.Visible = true;
            txtSendEmail.Focus();
            UI.ShowBackground(panelMessageBox, panelDisableBackground);

            UI.HideBackground(panelEmail, panelDisableBackground);
            //panelEmail.Location = new Point(200, 200);
            panelEmail.Visible = true;
            flowLayoutPanel2.Visible = false;
            panelHeader.Visible = false;
            panelPaymentTotalPrice.Visible = false;

            Form1.process = Process.Start("matchbox-keyboard");
        }

        public static void SendEmailNo(object sender, EventArgs e)
        {
            btnBack.Visible = false;
            panelEmail.Visible = false;
            txtSendEmail.Visible = false;
            txtSendEmail.Text = "";
            btnSendEmail.Visible = false;
            UI.ShowBackground(panelMessageBox, panelDisableBackground);
            SetReset.reset();
        }

        public void ConfirmPaymentYes(object sender, EventArgs e)
        {
            try
            {
                if (GlobalData.listPriceOfProducts.Count == 0) throw new Exception("You don't have product to buy");

                UI.ShowBackground(panelMessageBox, panelDisableBackground);

                flowLayoutPanel1.Visible = false;
                flowLayoutPanel2.Visible = true;
                flowLayoutPanel2.Location = new Point(50, 125);
                panelConfirmPayment.Visible = false;

                panelHeader.Visible = true;
                panelHeader.Location = new Point(50, 2);

                panelPaymentTotalPrice.Visible = true;
                panelPaymentTotalPrice.Location = new Point(50, 557);

                btnPay.Visible = true;
                btnBack.Visible = true;

               // btnPay.Location = new Point(320, 550);
                btnPay.Location = new Point(410, 650);
                btnBack.Location = new Point(210, 650);
                flowLayoutPanel2.Controls.Clear();
                for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
                {
                    Panel panel = new Panel()
                    {
                        //Size = new Size(340, 40),
                        Size = new Size(495, 40),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    Label label = new Label()
                    {
                        Location = new Point(6, 2),
                    };
                    label.Text = GlobalData.listNameOfProducts[i] + "   " + GlobalData.listManufacturerOfProducts[i] + "   " +  GlobalData.listTypeOfProducts[i];
                    panel.Controls.Add(label);

                    Label labelQuantity = new Label()
                    {
                        Location = new Point(225, 25),
                    };
                    labelQuantity.Text = GlobalData.listQuantityOfProducts[i].ToString();
                    panel.Controls.Add(labelQuantity);

                    /*Label labelPricePerUnit = new Label()
                    {
                        //Size = new Size(477, 35),
                        //Location = new Point(10, i * 40),
                        Location = new Point(225, 25),
                    };
                    labelPricePerUnit.Text = GlobalData.listPricePerUnitOfProducts[i].ToString();
                    panel.Controls.Add(labelPricePerUnit);*/

                    Label labelTotalPriceOfProduct = new Label()
                    {
                        Location = new Point(400, 25),
                    };
                    labelTotalPriceOfProduct.Text = Math.Round(GlobalData.listPriceOfProducts[i], 2).ToString();
                    panel.Controls.Add(labelTotalPriceOfProduct);

                    panel.MouseDown += UI.flowLayoutPanel1Y_MouseDown;
                    panel.MouseUp += UI.flowLayoutPanel1Y_MouseUp;
                    panel.MouseMove += UI.flowLayoutPanel1Y_MouseMove;

                    flowLayoutPanel2.Controls.Add(panel);
                }
                EditProduct.CalculateTotalPrice(lblPaymentTotalPrice);

                txtScannedBarcode.Focus();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        public void ConfirmPaymentNo(object sender, EventArgs e)
        {
            UI.ShowBackground(panelMessageBox, panelDisableBackground);
            txtScannedBarcode.Focus();
            panelClickToPay.Visible = true;
            txtScannedBarcode.Clear();
            Form1.isOpenAnything = false;
        }

        public void BackToShopping()
        {
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Controls.Clear();
            flowLayoutPanel2.Visible = false;
            btnPay.Visible = false;
            panelHeader.Visible = false;
            btnBack.Visible = false;
            panelPaymentTotalPrice.Visible = false;
            panelClickToPay.Visible = true;
            panelConfirmPayment.Visible = true;

            Form1.isOpenAnything = false;
        }

        public async Task Pay()
        {
            try
            {
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = false;
                btnPay.Visible = false;
                panelHeader.Visible = false;
                panelPaymentTotalPrice.Visible = false;
                btnBack.Visible = false;

                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();
                string queryUpdateQuantityOfProduct = "UPDATE Product SET InStock = @Item WHERE Id = @Id";
                MySqlCommand cmdUpdateProduct1 = new MySqlCommand(queryUpdateQuantityOfProduct, mySqlConnection);
                for (int i = 0; i < GlobalData.listIdsOfProducts.Count; i++)
                {
                    cmdUpdateProduct1.Parameters.Clear();

                    cmdUpdateProduct1.Parameters.AddWithValue("@Item", GlobalData.listInStockOfProducts[i] - GlobalData.listQuantityOfProducts[i]);
                    cmdUpdateProduct1.Parameters.AddWithValue("@Id", GlobalData.listIdsOfProducts[i]);
                    await cmdUpdateProduct1.ExecuteNonQueryAsync();
                }
                
                await Fruit.loadFruit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
