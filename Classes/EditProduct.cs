using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class EditProduct
    {
        private readonly MySqlConnection mySqlConnection;
        private static TextBox txtScannedBarcode;
        private Panel panelProductQuantity;
        private static Panel panelUpdateProductQuantity;
        private static Label lblTotalPrice;
        private Label lblNumberOfProducts;
        private static Label lblUpdateNumberOfProducts;
        private static FlowLayoutPanel flowLayoutPanel1;
        private static Button btnUpdateProduct;

        private static string selectedPanelTag;
        public EditProduct(MySqlConnection mySqlConnection, TextBox txtScannedBarcode, Panel panelProductQuantity, Label lblTotalPrice, Label lblNumberOfProducts, FlowLayoutPanel flowLayoutPanel1, Panel panelUpdateProductQuantity, Label lblUpdateNumberOfProducts, Button btnUpdateProduct)
        {
            this.mySqlConnection = mySqlConnection;
            EditProduct.txtScannedBarcode = txtScannedBarcode;
            this.panelProductQuantity = panelProductQuantity;
            EditProduct.lblTotalPrice = lblTotalPrice;
            this.lblNumberOfProducts = lblNumberOfProducts;
            EditProduct.flowLayoutPanel1 = flowLayoutPanel1;
            EditProduct.panelUpdateProductQuantity = panelUpdateProductQuantity;
            EditProduct.lblUpdateNumberOfProducts = lblUpdateNumberOfProducts;
            EditProduct.btnUpdateProduct = btnUpdateProduct;
        }
        public async Task finishProduct()
        {
            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            string getProductByBarcode = "SELECT * FROM Product";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    bool productAlreadyExists = false;
                    bool productFound = false;

                    while (productReader.Read())
                    {
                        if (productReader["Barcode"].ToString() == Form1.scannedBarcode)
                        {
                            double priceOfProduct = 0;
                            int columnIndex = productReader.GetOrdinal("DiscountId");

                            if (!productReader.IsDBNull(columnIndex))
                            {
                                priceOfProduct = Convert.ToDouble(productReader["DiscountPrice"]) * ModifyQuantity.quantity;
                            }
                            else
                            {
                                priceOfProduct = Convert.ToDouble(productReader["Price"]) * ModifyQuantity.quantity;
                            }

                            int indexOfFoundProduct = 0;

                            for (int i = 0; i < GlobalData.listIdsOfProducts.Count; i++)
                            {
                                if (GlobalData.listIdsOfProducts[i] == Convert.ToInt32(productReader["ID"]))
                                {
                                    GlobalData.listQuantityOfProducts[i] += ModifyQuantity.quantity;
                                    GlobalData.listPriceOfProducts[i] += priceOfProduct;
                                    indexOfFoundProduct = i;
                                    productAlreadyExists = true;
                                    break;
                                }
                            }

                            if (productAlreadyExists)
                            {
                                foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
                                {
                                    if ((int)panel.Tag == indexOfFoundProduct)
                                    {
                                        Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                                        Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;
                                        labelTotalPrice.Text = GlobalData.listPriceOfProducts[indexOfFoundProduct].ToString();
                                        labelQuantity.Text = GlobalData.listQuantityOfProducts[indexOfFoundProduct].ToString();
                                    }
                                }
                            }

                            if (!productAlreadyExists)
                            {
                                int columnIndex1 = productReader.GetOrdinal("DiscountId");

                                if (!productReader.IsDBNull(columnIndex1))
                                {
                                    UI.addNewProduct(productReader["Name"].ToString(), priceOfProduct.ToString(), productReader["DiscountPrice"].ToString(), ModifyQuantity.quantity.ToString(), false, flowLayoutPanel1);
                                }
                                else
                                {
                                    UI.addNewProduct(productReader["Name"].ToString(), priceOfProduct.ToString(), productReader["Price"].ToString(), ModifyQuantity.quantity.ToString(), false, flowLayoutPanel1);
                                }

                                GlobalData.listQuantityOfProducts.Add(ModifyQuantity.quantity);
                                GlobalData.listPricePerUnitOfProducts.Add(Convert.ToDouble(productReader["Price"]));
                                GlobalData.listPriceOfProducts.Add(priceOfProduct);
                                GlobalData.listNameOfProducts.Add(productReader["Name"].ToString());
                                GlobalData.listIdsOfProducts.Add(Convert.ToInt32(productReader["ID"]));
                                GlobalData.listTypeOfProducts.Add(productReader["Type"].ToString());
                                GlobalData.listManufacturerOfProducts.Add(productReader["Manufacturer"].ToString());
                                GlobalData.listInStockOfProducts.Add(Convert.ToInt32(productReader["InStock"]));
                            }
                            productFound = true;
                            break;
                        }
                    }
                    if (!productFound)
                    {
                        MessageBox.Show("Product doesn't exist");
                    } 

                    double totalPrice = 0;
                    for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
                    {
                        totalPrice += GlobalData.listPriceOfProducts[i];
                    }
                    lblTotalPrice.Text = totalPrice.ToString() + "KM";

                    txtScannedBarcode.Clear();
                    txtScannedBarcode.Focus();
                    ModifyQuantity.quantity = 1;
                    lblNumberOfProducts.Text = ModifyQuantity.quantity.ToString();

                    Form1.isBarcodeScanned = false;
                }
            }
        }

        public void UpdateProduct()
        {
            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            panelUpdateProductQuantity.Visible = false;

            foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
            {
                if (panel.Tag.ToString() == selectedPanelTag)
                {
                    Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                    Label labelPricePerUnit = panel.Controls.Find("lblPricePerUnit", true).FirstOrDefault() as Label;
                    Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;

                    /*foreach(Control c in panel.Controls)
                    {
                        if(c is Label lbl && lbl.Name == "lblPricePerUnit")
                    }*/

                    double priceOfProduct = Convert.ToDouble(labelPricePerUnit.Text) * ModifyQuantity.updateQuantity;

                    /*double priceOfProduct = Convert.ToDouble(GlobalData.listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)] * ModifyQuantity.updateQuantity);
                    double pricePerUnit = GlobalData.listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)];
                    string productName = GlobalData.listNameOfProducts[Convert.ToInt32(selectedPanelTag)];*/

                    labelTotalPrice.Text = priceOfProduct.ToString();
                    labelPricePerUnit.Text = labelPricePerUnit.Text;
                    labelQuantity.Text = ModifyQuantity.updateQuantity.ToString();

                    GlobalData.listPriceOfProducts[Convert.ToInt32(selectedPanelTag)] = priceOfProduct;
                    GlobalData.listQuantityOfProducts[Convert.ToInt32(selectedPanelTag)] = ModifyQuantity.updateQuantity;
                }
            }

            double totalPrice = 0;
            for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
            {
                totalPrice += GlobalData.listPriceOfProducts[i];
                lblTotalPrice.Text = totalPrice.ToString();
            }

            txtScannedBarcode.Clear();
            txtScannedBarcode.Focus();
            ModifyQuantity.updateQuantity = 1;
            lblUpdateNumberOfProducts.Text = ModifyQuantity.updateQuantity.ToString();

            Form1.isDoubleClicked = false;

        }

        public static void RemoveProduct(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;
            if (clickedButton != null)
            {
                Panel parentPanel = clickedButton.Parent as Panel;
                if (parentPanel != null)
                {
                    flowLayoutPanel1.Controls.Remove(parentPanel);
                    parentPanel.Dispose();

                    UI.numberOfProducts--;

                    GlobalData.listNameOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listPriceOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listPricePerUnitOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listQuantityOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listIdsOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listManufacturerOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listTypeOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    GlobalData.listInStockOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));

                    double totalPrice = 0;
                    for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
                    {
                        totalPrice += GlobalData.listPriceOfProducts[i];
                    }
                    lblTotalPrice.Text = totalPrice.ToString();

                    UI.updatePanelTag(flowLayoutPanel1);

                    txtScannedBarcode.Focus();
                }
            }
        }

        public static void PanelDoubleClick(object sender, EventArgs e)
        {
            double quantityOfSelectedProduct;
            if (!Form1.isScrolling && !Form1.isBarcodeScanned)
            {
                Form1.isDoubleClicked = true;

                btnUpdateProduct.Focus();
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    for (int i = 0; i < GlobalData.listQuantityOfProducts.Count; i++)
                    {
                        if (Convert.ToInt32(panel.Tag) == i)
                        {
                            quantityOfSelectedProduct = GlobalData.listQuantityOfProducts[i];
                            lblUpdateNumberOfProducts.Text = quantityOfSelectedProduct.ToString();
                            ModifyQuantity.updateQuantity = GlobalData.listQuantityOfProducts[i];

                            selectedPanelTag = panel.Tag.ToString();

                            break;
                        }
                    }


                    UI.updatePanelTag(flowLayoutPanel1);

                    panelUpdateProductQuantity.Visible = true;
                }
            }
        }
    }
}
