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

            fruit = new Fruit(mySqlConnection, flpFruit, homePagePanel, lblTotalPrice, txtScannedBarcode, flowLayoutPanel1);
            modifyQuantity = new ModifyQuantity(mySqlConnection, lblNumberOfProducts, lblUpdateNumberOfProducts);
            editProduct = new EditProduct(mySqlConnection, txtScannedBarcode, panelProductQuantity, lblTotalPrice, lblNumberOfProducts, flowLayoutPanel1, panelUpdateProductQuantity, lblUpdateNumberOfProducts, btnUpdateProduct);
            ui = new UI(flowLayoutPanel1);
            discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
            payment = new Payment(mySqlConnection, flowLayoutPanel1, flowLayoutPanel2, btnPay, panelConfirmPayment);

            setReset = new SetReset(mySqlConnection, homePagePanel, panelDiscountPage, flowLayoutPanel1, flowLayoutPanel2, panelConfirmPayment, txtScannedBarcode, lblTotalPrice, btnPay, lblHomePage, lblClickToPay, flpFruit, lblNumberOfProducts, lblUpdateNumberOfProducts, panelProductQuantity, panelUpdateProductQuantity, btnUpdateProduct, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
            await setReset.LoadAsync(fruit, modifyQuantity, editProduct, ui, discountProduct, payment);

            fruit.OnWeightReceived += Fruit_OnWeightReceived;
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
        }

        private void flowLayoutPanel1_MouseDown(object sender, MouseEventArgs e)
        {
           UI.flowLayoutPanel1_MouseDown(sender, e);
           /* scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel1.AutoScrollPosition.Y;
            isScrolling = false;*/
        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            UI.flowLayoutPanel1_MouseMove(sender, e);
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
            UI.flowLayoutPanel1_MouseUp(sender, e);
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
        }

        private async void btnPay_Click(object sender, EventArgs e)
        {
            await payment.Pay();
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            passedSeconds++;
            if (passedSeconds == 1)
            {
                timer1.Stop();
                button1.Focus();
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

        private void btnSendEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (!SendEmail.ValidateEmail(txtSendEmail.Text)) throw new Exception("Invalid email format");

                GeneratePdf generatePdf = new GeneratePdf();
                string pdfName = $"Invoice{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                //string pdfFolder = Path.Combine(Application.StartupPath, "pdfs");
                //string pdfPath = Path.Combine(pdfFolder, pdfName);


                string pdfPath = Path.Combine(Application.StartupPath, "/home/pi/pdfs/", pdfName);
                generatePdf.GeneratePdfFile(pdfName, txtSendEmail.Text);

                SendEmail.SendPdfEmail(txtSendEmail.Text, pdfPath);

                panelEmail.Visible = false;
                txtSendEmail.Text = "";
                txtSendEmail.Visible = false;
                btnSendEmail.Visible = false;

                SetReset.reset();
            } catch(Exception ex)
            {
                txtSendEmail.Text = "";
                MessageBox.Show(ex.Message);
            }

        }

        private async void buttonBuyFruit_Click(object sender, EventArgs e)
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
        }

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

    

