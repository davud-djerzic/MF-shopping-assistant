using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotNetEnv;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Runtime.Remoting.Messaging;
using System.IO;
using System.Net.Sockets;
using System.Security.Policy;
using System.Reflection;
using MF_Shopping_Assistant.Classes;
using iTextSharp.text.pdf;
using System.Diagnostics;
using Button = System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace MF_Shopping_Assistant
{
    public partial class Form1 : Form
    {
        private Fruit fruit;
        private ModifyQuantity modifyQuantity;
        private EditProduct editProduct;
        private UI ui;
        private DiscountProduct discountProduct;
        private Payment payment;
        private SetReset setReset;

        public static string scannedBarcode;
        public static bool isBarcodeScanned = false;
        public static int passedSeconds = 0;
        private MySqlConnection mySqlConnection;

        private int locationLabelNameX = 17, locationLabelNameY = 21;
        private int locationLabelTotalPriceX = 298, locationLabelTotalPriceY = 48;
        private int locationLabelPricePerUnitX = 142, locationLabelPricePerUnitY = 48;
        private int locationLabelQuantityX = 17, locationLabelQuantityY = 48;

        public static bool isScrolling = false;
        public static bool isDoubleClicked = false;
        public static bool isIntroFinish = false;

        private int scrollStartY;
        private int scrollPosition;

        public static bool isPdfGenerated = false;

        public static Process process;

        public static bool isOpenAnything = false;
        public static bool isInvalidEmailFormat = false;

        public static int currentPage = 0;


        public Form1()
        {
            InitializeComponent();
        }

        private void Fruit_OnWeightReceived(float weight)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    lblFruitWeight.Text = $"{weight:F2} g";
                }));
            }
            else
            {
                lblFruitWeight.Text = $"{weight:F2} g";
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            string connectionString = Connections.Connection_String_Raspberry;

            mySqlConnection = new MySqlConnection(connectionString);
            try
            {
                await mySqlConnection.OpenAsync();
                MessageBox.Show("Connection opened succesfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            fruit = new Fruit(mySqlConnection, flpFruit, homePagePanel, lblTotalPrice, txtScannedBarcode, flowLayoutPanel1, panelClickToPay, panelSlider);
            modifyQuantity = new ModifyQuantity(mySqlConnection, lblNumberOfProducts, lblUpdateNumberOfProducts);
            editProduct = new EditProduct(mySqlConnection, txtScannedBarcode, panelProductQuantity, lblTotalPrice, lblNumberOfProducts, flowLayoutPanel1, panelUpdateProductQuantity, lblUpdateNumberOfProducts, btnUpdateProduct, panelDisableBackground, panelClickToPay, panelMessageBoxOk, lblMessageBoxOK);
            ui = new UI(flowLayoutPanel1);
            discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
            payment = new Payment(mySqlConnection, flowLayoutPanel1, flowLayoutPanel2, btnPay, panelConfirmPayment, panelHeader, panelPaymentTotalPrice, lblPaymentTotalPrice, lblMessageBoxMessage, btnMessageBoxYes, btnMessageBoxNo, txtSendEmail, panelEmail, btnSendEmail, panelMessageBox, panelDisableBackground, txtScannedBarcode, panelClickToPay, panelMessageBoxOk, lblMessageBoxOK, btnBack);

            setReset = new SetReset(mySqlConnection, homePagePanel, panelDiscountPage, flowLayoutPanel1, flowLayoutPanel2, panelConfirmPayment, txtScannedBarcode, lblTotalPrice, btnPay, lblHomePage, lblClickToPay, flpFruit, lblNumberOfProducts, lblUpdateNumberOfProducts, panelProductQuantity, panelUpdateProductQuantity, btnUpdateProduct, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4, button1, panelHeader, panelPaymentTotalPrice, panelClickToPay, panelEmail, btnBack);
            await setReset.LoadAsync(fruit, modifyQuantity, editProduct, ui, discountProduct, payment);

            fruit.OnWeightReceived += Fruit_OnWeightReceived;

            SetReset.reset();
        }

        
        private void btnQuantityIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                modifyQuantity.QuantityIncrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnQuantityDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                modifyQuantity.QuantityDecrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnFinishProduct_Click(object sender, EventArgs e)
        {
            try
            {
                await editProduct.finishProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtScannedBarcode_TextChanged(object sender, EventArgs e)
        {
            if (txtScannedBarcode.Text != "" && !isDoubleClicked && isIntroFinish && !isOpenAnything)
            {
                scannedBarcode = txtScannedBarcode.Text;
                isBarcodeScanned = true;
                timer1.Start();
            }
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            panelProductQuantity.Visible = true;
            EditProduct.isUpdateQuantityPanelOpen = true;
            UI.HideBackground(panelProductQuantity, panelDisableBackground);
            panelClickToPay.Visible = false;
        }

        

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseDown(sender, e);
        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
             UI.flowLayoutPanel1Y_MouseMove(sender, e);
        }

        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseUp(sender, e);
        }
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            try
            {
                editProduct.UpdateProduct();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdateQuantityDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                modifyQuantity.UpdateQuantityDecrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdateQuantityIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                modifyQuantity.UpdateQuantityIncrease();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void panelConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                payment.PanelConfirmPayment_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                await payment.Pay();

                UI.HideBackground(panelMessageBox, panelDisableBackground);
                lblMessageBoxMessage.Text = "Payment completed successfully" + Environment.NewLine + Environment.NewLine + "Do you want to receive the reciept from an email";
                btnMessageBoxYes.Click -= payment.ConfirmPaymentYes;
                btnMessageBoxYes.Click += Payment.SendEmailYes;
                btnMessageBoxNo.Click -= payment.ConfirmPaymentNo;
                btnMessageBoxNo.Click += Payment.SendEmailNo;

                panelEmail.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnMessageBoxYes_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            passedSeconds++;
            if (passedSeconds == 1)
            {
                timer1.Stop();
                if (await editProduct.GetInStockOfProduct(scannedBarcode))
                {
                    button1.Focus();
                }   
                else
                {
                    txtScannedBarcode.Clear();
                    isBarcodeScanned = false;
                }
                passedSeconds = 0;
            }
        }

        private async void btnShowFruit_Click(object sender, EventArgs e)
        {
            try
            {
                await fruit.btnShowFruit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEmail_Click(object sender, EventArgs e)
        {
            btnSendEmail.Visible = true;
            txtSendEmail.Visible = true;
        }

        private void btnNotEmail_Click(object sender, EventArgs e)
        {
            panelEmail.Visible = false;
            txtSendEmail.Visible = false;
            txtSendEmail.Text = "";
            btnSendEmail.Visible = false;
            SetReset.reset();
        }

        private async void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SendEmail.ValidateEmail(txtSendEmail.Text))
                {
                    throw new Exception("Invalid email format");
                }
                isInvalidEmailFormat = false;

                GeneratePdf generatePdf = new GeneratePdf();
                string pdfName = $"Invoice{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                //string pdfFolder = Path.Combine(Application.StartupPath, "pdfs");
                //string pdfPath = Path.Combine(pdfFolder, pdfName);


                string pdfPath = Path.Combine(Application.StartupPath, "/home/pi/pdfs/", pdfName);
                generatePdf.GeneratePdfFile(pdfName, txtSendEmail.Text);
                isPdfGenerated = true;

                await SendEmail.SendPdfEmail(txtSendEmail.Text, pdfPath);
                if (SendEmail.isEmailSendCorrectly)
                {
                    panelMessageBoxOk.Visible = true;
                    panelMessageBoxOk.Location = new Point(100, 100);
                    lblMessageBoxOK.Text = "Invoice sent successfully";
                } else
                {
                    panelMessageBoxOk.Visible = true;
                    panelMessageBoxOk.Location = new Point(100, 100);
                    lblMessageBoxOK.Text = "Error with sending email";
                }
                

                panelEmail.Visible = false;
                txtSendEmail.Text = "";
                txtSendEmail.Visible = false;
                btnSendEmail.Visible = false;

                process.Kill();
            } catch(Exception ex)
            {
                txtSendEmail.Text = "";
                isInvalidEmailFormat = true;

                UI.HideBackground(panelMessageBoxOk, panelDisableBackground);
                panelMessageBoxOk.Visible = true;
                panelMessageBoxOk.Location = new Point(100, 100);
                lblMessageBoxOK.Text = ex.Message;
            }

        }

        private bool isOpenFruitPanel = false;
        private async void btnShowFruit1_Click(object sender, EventArgs e)
        {
            try
            {
                if (!EditProduct.isUpdateQuantityPanelOpen && !isOpenAnything)
                {
                    panelClickToPay.Visible = false;
                    panelSlider.Visible = true;
                    isOpenAnything = true;
                    await fruit.btnShowFruit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void btnShowDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                if (!EditProduct.isUpdateQuantityPanelOpen && !isOpenAnything)
                {
                    panelDiscountPage.Visible = true;
                    DiscountProduct discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
                    isOpenAnything = true;
                    await discountProduct.getDiscountProducts();
                    panelClickToPay.Visible = false;
 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSendEmail_Click(object sender, EventArgs e)
        {
            process = Process.Start("matchbox-keyboard");
        }

        private void flowLayoutPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseDown(sender, e);
        }

        private void flowLayoutPanel2_MouseMove(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseMove(sender, e);
        }

        private void flowLayoutPanel2_MouseUp(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseUp(sender, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            payment.BackToShopping();
        }

        private void panelPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                int scrollX = currentPage * 600;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
                
            }
            
        }

        private void panelNextPage_Click(object sender, EventArgs e)
        {
            if (currentPage < Fruit.slides.Count - 1)
            {
                currentPage++;
                int scrollX = currentPage * 600;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
                
            }
        }

        private void btnMessageBoxOK_Click(object sender, EventArgs e)
        {
            if (EditProduct.isAddedExtraProduct)
            {
                UI.ShowBackground(panelMessageBoxOk, panelDisableBackground);
                EditProduct.isAddedExtraProduct = false;
            }
            if (isInvalidEmailFormat)
            {
                UI.ShowBackground(panelMessageBoxOk, panelDisableBackground);
                process = Process.Start("matchbox-keyboard");
                panelDisableBackground.Location = new Point(0, 0);
                // panelDisableBackground.Size = new Size(518, 717);
                panelDisableBackground.Size = new Size(690, 782);
                txtSendEmail.Focus();
            }
            panelMessageBoxOk.Visible = false;
            if (isPdfGenerated) {
                UI.ShowBackground(panelEmail, panelDisableBackground);
                SetReset.reset();
            }
            
            if (!isInvalidEmailFormat)
            {
                txtScannedBarcode.Clear();
                isOpenAnything = false;
                txtScannedBarcode.Focus();
            }
            
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                await Fruit.btnChooseFruitToBuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}

    

