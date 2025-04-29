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

        private Process process;

        //private double quantityOfSelectedProduct;
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
                    lblFruitWeight.Text = $"Vrijednost: {weight:F2} g";
                }));
            }
            else
            {
                lblFruitWeight.Text = $"Vrijednost: {weight:F2} g";
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
            editProduct = new EditProduct(mySqlConnection, txtScannedBarcode, panelProductQuantity, lblTotalPrice, lblNumberOfProducts, flowLayoutPanel1, panelUpdateProductQuantity, lblUpdateNumberOfProducts, btnUpdateProduct, panelDisableBackground, panelClickToPay);
            ui = new UI(flowLayoutPanel1);
            discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
            payment = new Payment(mySqlConnection, flowLayoutPanel1, flowLayoutPanel2, btnPay, panelConfirmPayment, panelHeader, panelPaymentTotalPrice, lblPaymentTotalPrice, lblMessageBoxMessage, btnMessageBoxYes, btnMessageBoxNo, txtSendEmail, panelEmail, btnSendEmail, panelMessageBox, panelDisableBackground, txtScannedBarcode, panelClickToPay, panelMessageBoxOk, lblMessageBoxOK);

            setReset = new SetReset(mySqlConnection, homePagePanel, panelDiscountPage, flowLayoutPanel1, flowLayoutPanel2, panelConfirmPayment, txtScannedBarcode, lblTotalPrice, btnPay, lblHomePage, lblClickToPay, flpFruit, lblNumberOfProducts, lblUpdateNumberOfProducts, panelProductQuantity, panelUpdateProductQuantity, btnUpdateProduct, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4, button1, panelHeader, panelPaymentTotalPrice, panelClickToPay, panelEmail);
            await setReset.LoadAsync(fruit, modifyQuantity, editProduct, ui, discountProduct, payment);

            fruit.OnWeightReceived += Fruit_OnWeightReceived;

            SetReset.reset();
        }

        
        private void btnQuantityIncrease_Click(object sender, EventArgs e)
        {
            modifyQuantity.QuantityIncrease();
        }

        private void btnQuantityDecrease_Click(object sender, EventArgs e)
        {
            modifyQuantity.QuantityDecrease();
        }

        private async void btnFinishProduct_Click(object sender, EventArgs e)
        {
            await editProduct.finishProduct();
        }

        private void txtScannedBarcode_TextChanged(object sender, EventArgs e)
        {
            if (txtScannedBarcode.Text != "" && !isDoubleClicked && isIntroFinish)
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
            /*scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel1.AutoScrollPosition.Y;
            isScrolling = false;*/
        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
             UI.flowLayoutPanel1Y_MouseMove(sender, e);
            /*if (e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - scrollStartY;
                if (Math.Abs(deltaY) > 5)
                {
                    isScrolling = true;
                    flowLayoutPanel1.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
                }
            }*/
        }

        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseUp(sender, e);
           // flowLayoutPanel1.Capture = false;
        }
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            editProduct.UpdateProduct();
        }

        private void btnUpdateQuantityDecrease_Click(object sender, EventArgs e)
        {
            modifyQuantity.UpdateQuantityDecrease();
        }

        private void btnUpdateQuantityIncrease_Click(object sender, EventArgs e)
        {
            modifyQuantity.UpdateQuantityIncrease();
        }

        private void panelConfirmPayment_Click(object sender, EventArgs e)
        {
            payment.PanelConfirmPayment_Click(sender, e);
           /* if (GlobalData.listPriceOfProducts.Count != 0)
                
            else
            {
                panelMessageBoxOk.Visible = true;
                panelMessageBoxOk.Location = new Point(100, 100);
                lblMessageBoxOK.Text = "You don't have a product to buy";
            }*/
              
        }

        private void SendEmailYes(object sender, EventArgs e)
        {
            btnSendEmail.Visible = true;
            txtSendEmail.Visible = true;
            UI.ShowBackground(panelMessageBox, panelDisableBackground);
            panelEmail.Location = new Point(200, 200);
            panelEmail.Visible = true;
            flowLayoutPanel2.Visible = false;
            panelHeader.Visible = false;
            panelPaymentTotalPrice.Visible = false;
        }

        private void SendEmailNo(object sender, EventArgs e)
        {
            panelEmail.Visible = false;
            txtSendEmail.Visible = false;
            txtSendEmail.Text = "";
            btnSendEmail.Visible = false;
            UI.ShowBackground(panelMessageBox, panelDisableBackground);
            SetReset.reset();
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            await payment.Pay();
            UI.HideBackground(panelMessageBox, panelDisableBackground);
            lblMessageBoxMessage.Text = "Payment completed successfully" + Environment.NewLine + Environment.NewLine + "Do you want to receive the reciept from an email";
            btnMessageBoxYes.Click -= payment.ConfirmPaymentYes;
            btnMessageBoxYes.Click += Payment.SendEmailYes;
            btnMessageBoxNo.Click -= payment.ConfirmPaymentNo;
            btnMessageBoxNo.Click += Payment.SendEmailNo;

            /*DialogResult respond = MessageBox.Show("Do you want to receive the reciept from an email", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respond == DialogResult.Yes)
            {
                btnSendEmail.Visible = true;
                txtSendEmail.Visible = true;
            } else
            {
                panelEmail.Visible = false;
                txtSendEmail.Visible = false;
                txtSendEmail.Text = "";
                btnSendEmail.Visible = false;
                SetReset.reset();
            }*/
            
            panelEmail.Visible = true;
            /*string pdfPath = @"C:\Users\Korisnik\Desktop\MF Shopping Assistant\bin\Debug\pdfs\Order_20250408_150900.pdf";

            // Ukloni prethodne kontrole ako želiš
            //this.Controls.Clear();

            // Kreiraj PdfViewer
            var pdfViewer = new PdfViewer();
            pdfViewer.Dock = DockStyle.Fill;

            // Dodaj viewer na formu
            this.Controls.Add(pdfViewer);

            // Učitaj PDF fajl u viewer
            var document = PdfDocument.Load(pdfPath);
            pdfViewer.Document = document;*/
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
                    button1.Focus();
                else
                    txtScannedBarcode.Clear();
                passedSeconds = 0;
            }
        }

        private async void btnShowFruit_Click(object sender, EventArgs e)
        {
            await fruit.btnShowFruit();
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
                if (!SendEmail.ValidateEmail(txtSendEmail.Text)) throw new Exception("Invalid email format");

                GeneratePdf generatePdf = new GeneratePdf();
                string pdfName = $"Invoice{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string pdfFolder = Path.Combine(Application.StartupPath, "pdfs");
                string pdfPath = Path.Combine(pdfFolder, pdfName);


                //string pdfPath = Path.Combine(Application.StartupPath, "/home/pi/pdfs/", pdfName);
                generatePdf.GeneratePdfFile(pdfName, txtSendEmail.Text);

                await SendEmail.SendPdfEmail(txtSendEmail.Text, pdfPath);

                
                panelMessageBoxOk.Visible = true;
                panelMessageBoxOk.Location = new Point(100, 100);
                lblMessageBoxOK.Text = "Invoice sent successfully";
                isPdfGenerated = true;

                panelEmail.Visible = false;
                txtSendEmail.Text = "";
                txtSendEmail.Visible = false;
                btnSendEmail.Visible = false;

                //process.Kill();
                
                //SetReset.reset();
            } catch(Exception ex)
            {
                txtSendEmail.Text = "";
                MessageBox.Show(ex.Message);
            }

        }

        private void lblUpdateNumberOfProducts_Click(object sender, EventArgs e)
        {

        }

        private bool isOpenFruitPanel = false;
        private async void btnShowFruit1_Click(object sender, EventArgs e)
        {
            if (!EditProduct.isUpdateQuantityPanelOpen)
            {
                if (!isOpenFruitPanel)
                {
                    panelClickToPay.Visible = false;
                    panelSlider.Visible = true;
                    await fruit.btnShowFruit();
                }
                   
                else
                    flpFruit.Visible = false;
                isOpenFruitPanel = !isOpenFruitPanel;
            }
            
        }

        private async void btnShowDiscount_Click(object sender, EventArgs e)
        {
            if (!EditProduct.isUpdateQuantityPanelOpen)
            {
                panelDiscountPage.Visible = true;
                DiscountProduct discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
                await discountProduct.getDiscountProducts();
                panelClickToPay.Visible = false;
            }
        }

        private void txtSendEmail_Click(object sender, EventArgs e)
        {
            //process = Process.Start("matchbox-keyboard");
        }

        private void flowLayoutPanel2_MouseDown(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseDown(sender, e);
            /*scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel2.AutoScrollPosition.Y;
            isScrolling = false;*/

        }

        private void flowLayoutPanel2_MouseMove(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseMove(sender, e);
            /*if (e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - scrollStartY;
                if (Math.Abs(deltaY) > 5)
                {
                    isScrolling = true;
                    flowLayoutPanel2.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
                }
            }*/
        }

        private void flowLayoutPanel2_MouseUp(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1Y_MouseUp(sender, e);
           // flowLayoutPanel2.Capture = false;
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panelProduct2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblDiscountPercentageP2_Click(object sender, EventArgs e)
        {

        }

        private void lblClickToPay_Click(object sender, EventArgs e)
        {

        }

        private void panelPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                int scrollX = currentPage * 690;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
                
            }
            
        }

        private void panelNextPage_Click(object sender, EventArgs e)
        {
            if (currentPage < Fruit.stranice.Count - 1)
            {
                currentPage++;
                int scrollX = currentPage * 690;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
                
            }
        }

        private void btnMessageBoxOK_Click(object sender, EventArgs e)
        {
            panelMessageBoxOk.Visible = false;
            if (isPdfGenerated) SetReset.reset();
            txtScannedBarcode.Focus();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await Fruit.btnChooseFruitToBuy();
        }

        private static int currentPage = 0;
        private void btnNextPage_Click(object sender, EventArgs e)
        {
            if (currentPage < Fruit.stranice.Count - 1)
            {
                currentPage++;
                int scrollX = currentPage * 300;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
            }
           
            //fruitPanel.AutoScrollPosition = new Point(currentPage * 300, 0);
        }

        private void btnPreviousPage_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                int scrollX = currentPage * 300;
                flpFruit.AutoScrollPosition = new Point(scrollX, 0);
            }
            
           // fruitPanel.AutoScrollPosition = new Point(currentPage * 300, 0);
        }

        /*private async void buttonBuyFruit_Click(object sender, EventArgs e)
        {
            await Fruit.btnChooseFruitToBuy();
        }

        private async Task SendCommandToRaspberry(string command)
        {
            await Fruit.SendCommandToRaspberry("STOP");
        }

        private async void ReceiveWeightData()
        {
            await fruit.ReceiveWeightData();
        }

        private void FruitClick_Click(object sender, EventArgs e)
        {
            Fruit.FruitClick(sender, e);
        }*/

        /*private async Task loadFruit()
        {
            await Fruit.loadFruit();
        }*/

        /*private async Task getDiscountProducts()
        {
            await discountProduct.getDiscountProducts();
        }*/

    }
}

    

