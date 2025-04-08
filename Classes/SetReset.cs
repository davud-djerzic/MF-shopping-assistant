using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class SetReset
    {
        private MySqlConnection mySqlConnection;
        private static Panel homePagePanel;
        private static Panel panelDiscountPage;
        private static FlowLayoutPanel flowLayoutPanel1;
        private static FlowLayoutPanel flowLayoutPanel2;
        private static Panel panelConfirmPayment;
        private static TextBox txtScannedBarcode;
        private static Label lblTotalPrice;
        private Label lblHomePage;
        private Label lblClickToPay;
        private FlowLayoutPanel flpFruit;
        private Label lblNumberOfProducts;
        private Label lblUpdateNumberOfProducts;
        private Panel panelProductQuantity;
        private Panel panelUpdateProductQuantity;
        private Button btnUpdateProduct;
        private Label lblProductNameP1;
        private Label lblDiscountPercentageP1;
        private Label lblOldPriceP1;
        private Label lblDiscountPriceP1;
        private Label lblProductNameP2;
        private Label lblDiscountPercentageP2;
        private Label lblOldPriceP2;
        private Label lblDiscountPriceP2;
        private Label lblProductNameP3;
        private Label lblOldPriceP3;
        private Label lblDiscountPriceP3;
        private Label lblProductNameP4;
        private Label lblOldPriceP4;
        private Label lblDiscountPriceP4;
        private Button btnPay;

        /*private Fruit fruit;
        private ModifyQuantity modifyQuantity;
        private EditProduct editProduct;
        private UI ui;
        private DiscountProduct discountProduct;
        private Payment payment;*/


        public SetReset(MySqlConnection mySqlConnection, Panel homePagePanel, Panel panelDiscountPage, FlowLayoutPanel flowLayoutPanel1, FlowLayoutPanel flowLayoutPanel2, Panel panelConfirmPayment, TextBox txtScannedBarcode, Label lblTotalPrice, Button btnPay,  Label lblHomePage,
         Label lblClickToPay,
         FlowLayoutPanel flpFruit,
         Label lblNumberOfProducts,
         Label lblUpdateNumberOfProducts,
         Panel panelProductQuantity,
         Panel panelUpdateProductQuantity,
         Button btnUpdateProduct,
         Label lblProductNameP1,
         Label lblDiscountPercentageP1,
         Label lblOldPriceP1,
         Label lblDiscountPriceP1,
         Label lblProductNameP2,
         Label lblDiscountPercentageP2,
         Label lblOldPriceP2,
         Label lblDiscountPriceP2,
         Label lblProductNameP3,
         Label lblOldPriceP3,
         Label lblDiscountPriceP3,
         Label lblProductNameP4,
         Label lblOldPriceP4,
         Label lblDiscountPriceP4)
        {
            this.mySqlConnection = mySqlConnection;
            /*this.fruit = fruit;
            this.modifyQuantity = modifyQuantity;
            this.editProduct = editProduct;
            this.ui = ui;
            this.discountProduct = discountProduct;
            this.payment = payment;*/
            SetReset.homePagePanel = homePagePanel;
            SetReset.panelDiscountPage = panelDiscountPage;
            SetReset.flowLayoutPanel1 = flowLayoutPanel1;
            SetReset.flowLayoutPanel2 = flowLayoutPanel2;
            SetReset.panelConfirmPayment = panelConfirmPayment;
            SetReset.txtScannedBarcode = txtScannedBarcode;
            SetReset.lblTotalPrice = lblTotalPrice;
            this.btnPay = btnPay;
            this.lblHomePage = lblHomePage;

            this.lblClickToPay = lblClickToPay;
            this.flpFruit = flpFruit;
            this.lblClickToPay = lblClickToPay;
            this.lblNumberOfProducts = lblNumberOfProducts;
            this.lblUpdateNumberOfProducts = lblUpdateNumberOfProducts;
            this.panelProductQuantity = panelProductQuantity;
            this.panelUpdateProductQuantity = panelUpdateProductQuantity;
            this.btnUpdateProduct = btnUpdateProduct;
             this. lblProductNameP1 = lblProductNameP1;
             this. lblDiscountPercentageP1 = lblDiscountPercentageP1;
             this. lblOldPriceP1 = lblOldPriceP1;
             this. lblDiscountPriceP1 = lblDiscountPriceP1;
             this. lblProductNameP2 = lblProductNameP2;
             this.lblDiscountPercentageP2 = lblDiscountPercentageP2;
             this. lblOldPriceP2 = lblOldPriceP2;
             this. lblDiscountPriceP2 = lblDiscountPriceP2;
             this. lblProductNameP3 = lblProductNameP3;
             this. lblOldPriceP3 = lblOldPriceP3;
             this. lblDiscountPriceP3 = lblDiscountPriceP3;
             this. lblProductNameP4 = lblProductNameP4;
             this. lblOldPriceP4 = lblOldPriceP4;
            this.lblDiscountPriceP4 = lblDiscountPriceP4;  
        }

        public async Task LoadAsync(Fruit fruit, ModifyQuantity modifyQuantity, EditProduct editProduct, UI ui, DiscountProduct discountProduct, Payment payment)
        {
            //Env.Load();
            //string connectionString = Env.GetString("Connection_String");
            //MessageBox.Show(connectionString);
            
            txtScannedBarcode.Focus();

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;
            homePagePanel.Click += HomePage_Click;
            lblHomePage.Click += HomePage_Click;

            panelDiscountPage.Click += DiscountPanelPage_Click;

            lblTotalPrice.Click += payment.PanelConfirmPayment_Click;
            lblClickToPay.Click += payment.PanelConfirmPayment_Click;

            addClickOnPanels(panelDiscountPage);

            await Fruit.loadFruit();
        }

        public static void reset()
        {
            homePagePanel.Visible = true;
            GlobalData.listIdsOfProducts.Clear();
            GlobalData.listNameOfProducts.Clear();
            GlobalData.listTypeOfProducts.Clear();
            GlobalData.listManufacturerOfProducts.Clear();
            GlobalData.listPriceOfProducts.Clear();
            GlobalData.listPricePerUnitOfProducts.Clear();
            GlobalData.listQuantityOfProducts.Clear();
            GlobalData.listInStockOfProducts.Clear();
            UI.numberOfProducts = 0;
            ModifyQuantity.quantity = 1;
            ModifyQuantity.updateQuantity = 1;
            Form1.isBarcodeScanned = false;
            Form1.passedSeconds = 0;

            GlobalData.panelsOfProducts.Clear();
            GlobalData.listDiscountProductId.Clear();
            GlobalData.listDiscountProductDiscountId.Clear();
            GlobalData.listDiscountProductPrice.Clear();
            GlobalData.listDiscountProductName.Clear();
            GlobalData.listDiscountProductType.Clear();
            GlobalData.listDiscountProductManufacturer.Clear();

            Form1.isDoubleClicked = false;
            Form1.isScrolling = false;

            panelDiscountPage.Visible = true;
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Visible = true;
            panelConfirmPayment.Visible = true;
            txtScannedBarcode.Focus();
            lblTotalPrice.Text = "0";

            Form1.isIntroFinish = false;
        }
        private async void HomePage_Click(object sender, EventArgs e)
        {
            homePagePanel.Visible = false;
            panelDiscountPage.Location = new Point(homePagePanel.Location.X, homePagePanel.Location.Y);
            DiscountProduct discountProduct = new DiscountProduct(mySqlConnection, lblProductNameP1, lblDiscountPercentageP1, lblOldPriceP1, lblDiscountPriceP1, lblProductNameP2, lblDiscountPercentageP2, lblOldPriceP2, lblDiscountPriceP2, lblProductNameP3, lblOldPriceP3, lblDiscountPriceP3, lblProductNameP4, lblOldPriceP4, lblDiscountPriceP4);
            await discountProduct.getDiscountProducts();
        }

        private void DiscountPanelPage_Click(object sender, EventArgs e)
        {
            panelDiscountPage.Visible = false;
            flowLayoutPanel1.Visible = true;
            txtScannedBarcode.Clear();
            Form1.isIntroFinish = true;
        }

        private void addClickOnPanels(Control parentPanel)
        {
            foreach (Control control in parentPanel.Controls)
            {
                control.Click += DiscountPanelPage_Click;

                if (control.HasChildren)
                {
                    addClickOnPanels(control);
                }
            }
        }
    }
}
