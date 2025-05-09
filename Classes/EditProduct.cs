using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
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
        private Panel panelClickToPay;
        private Panel panelMessageBoxOk;
        private Label lblMessageBoxOK;

        private static List<double> listAvailableQuantity = new List<double>();

        private static string selectedPanelTag;

        public static bool isUpdateQuantityPanelOpen = false;

        public static double productInStock;

        private static Panel panelDisableBackground;

        public static bool isAddedExtraProduct = false;

        public EditProduct(MySqlConnection mySqlConnection, TextBox txtScannedBarcode, Panel panelProductQuantity, Label lblTotalPrice, Label lblNumberOfProducts, FlowLayoutPanel flowLayoutPanel1, Panel panelUpdateProductQuantity, Label lblUpdateNumberOfProducts, Button btnUpdateProduct, Panel panelDisableBackground, Panel panelClickToPay, Panel panelMessageBoxOk, Label lblMessageBoxOK)
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
            EditProduct.panelDisableBackground = panelDisableBackground;
            this.panelClickToPay = panelClickToPay;
            this.panelMessageBoxOk = panelMessageBoxOk;
            this.lblMessageBoxOK = lblMessageBoxOK;
        }
        public async Task finishProduct()
        {

            UI.ShowBackground(panelProductQuantity, panelDisableBackground);

            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            string getProductByBarcode = "SELECT * FROM Product";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    bool productAlreadyExists = false;
                    //bool productFound = false;

                    //bool isEverythingWentOk = true;
                    try
                    {
                        while (productReader.Read())
                        {
                            if (productReader["Barcode"].ToString() == Form1.scannedBarcode)
                            {
                                double priceOfProduct = 0;
                                double pricePerUnitOfProduct = 0;
                                int columnIndex = productReader.GetOrdinal("DiscountId");

                                if (!productReader.IsDBNull(columnIndex))
                                {
                                    priceOfProduct = Convert.ToDouble(productReader["DiscountPrice"]) * ModifyQuantity.quantity;
                                    pricePerUnitOfProduct = (double)productReader["DiscountPrice"];
                                }
                                else
                                {
                                    priceOfProduct = Convert.ToDouble(productReader["Price"]) * ModifyQuantity.quantity;
                                    pricePerUnitOfProduct = (double)productReader["Price"];
                                }

                                int indexOfFoundProduct = 0;

                                for (int i = 0; i < GlobalData.listIdsOfProducts.Count; i++)
                                {
                                    if (GlobalData.listIdsOfProducts[i] == Convert.ToInt32(productReader["ID"]))
                                    {
                                        GlobalData.listQuantityOfProducts[i] += ModifyQuantity.quantity;
                                        GlobalData.listPriceOfProducts[i] += priceOfProduct;
                                        //GlobalData.listPricePerUnitOfProducts[i] = priceOfProduct;
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
                                            if (GlobalData.listQuantityOfProducts[indexOfFoundProduct] > (double)productReader["InStock"])
                                            {
                                                GlobalData.listQuantityOfProducts[indexOfFoundProduct] -= ModifyQuantity.quantity;
                                                
                                                GlobalData.listPriceOfProducts[indexOfFoundProduct] -= priceOfProduct;
                                                //isEverythingWentOk = false;
                                                throw new KeyNotFoundException("You selected more product than we have");
                                            } else
                                            {
                                                Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                                                Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;
                                                labelTotalPrice.Text = GlobalData.listPriceOfProducts[indexOfFoundProduct].ToString();
                                                labelQuantity.Text = GlobalData.listQuantityOfProducts[indexOfFoundProduct].ToString();

                                            }
                                        }
                                    }
                                }

                                if (!productAlreadyExists)
                                {
                                    int columnIndex1 = productReader.GetOrdinal("DiscountId");

                                    if (ModifyQuantity.quantity > (double)productReader["InStock"])
                                    {
                                        //isEverythingWentOk = false;
                                        throw new KeyNotFoundException("You selected more product than we have");
                                    }
                                    string productNameTypeManufacturer = productReader["Name"].ToString() + " " + productReader["Type"].ToString() + " " + productReader["Manufacturer"].ToString();
                                    if (!productReader.IsDBNull(columnIndex1))
                                    {
                                        UI.addNewProduct(productNameTypeManufacturer, priceOfProduct.ToString(), productReader["DiscountPrice"].ToString(), ModifyQuantity.quantity.ToString(), false, flowLayoutPanel1);
                                    }
                                    else
                                    {
                                        UI.addNewProduct(productNameTypeManufacturer, priceOfProduct.ToString(), productReader["Price"].ToString(), ModifyQuantity.quantity.ToString(), false, flowLayoutPanel1);
                                    }

                                    GlobalData.listQuantityOfProducts.Add(ModifyQuantity.quantity);
                                    GlobalData.listPricePerUnitOfProducts.Add(pricePerUnitOfProduct); // TODO: fix this
                                    //else GlobalData.listPricePerUnitOfProducts.Add(Convert.ToDouble(productReader["Price"]));
                                    GlobalData.listPriceOfProducts.Add(priceOfProduct);
                                    GlobalData.listNameOfProducts.Add(productReader["Name"].ToString());
                                    GlobalData.listIdsOfProducts.Add(Convert.ToInt32(productReader["ID"]));
                                    GlobalData.listTypeOfProducts.Add(productReader["Type"].ToString());
                                    GlobalData.listManufacturerOfProducts.Add(productReader["Manufacturer"].ToString());
                                    GlobalData.listInStockOfProducts.Add((double)productReader["InStock"]);
                                    //listAvailableQuantity.Add((double)productReader["InStock"]);

                                    //productInStock = (double)productReader["InStock"];
                                }
                                //productFound = true;
                                break;
                            }
                        }
                    }
                    catch (KeyNotFoundException ex)
                    {
                        UI.HideBackground(panelMessageBoxOk, panelDisableBackground);
                        panelMessageBoxOk.Visible = true;
                        panelMessageBoxOk.Location = new Point(100, 100);
                        lblMessageBoxOK.Text = ex.Message;
                        Form1.isOpenAnything = true;
                        isAddedExtraProduct = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                    
                    /*if (!productFound && isEverythingWentOk)
                    {
                        MessageBox.Show("Product doesn't exist");
                    }*/

                    CalculateTotalPrice(lblTotalPrice);

                    txtScannedBarcode.Clear();
                    txtScannedBarcode.Focus();
                    ModifyQuantity.quantity = 1;
                    lblNumberOfProducts.Text = ModifyQuantity.quantity.ToString();

                    Form1.isBarcodeScanned = false;
                }
            }
            isUpdateQuantityPanelOpen = false;
            panelClickToPay.Visible = true;
        }

        public void UpdateProduct()
        {
            UI.ShowBackground(panelUpdateProductQuantity, panelDisableBackground);

            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            panelUpdateProductQuantity.Visible = false;

            try
            {
                foreach(Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
                {
                    Button updateButton = panel.Controls.Find("Update", true).FirstOrDefault() as Button;
                    if (updateButton.Tag.ToString() == selectedPanelTag)
                    {
                        MessageBox.Show(ModifyQuantity.updateQuantity.ToString() + "  " + GlobalData.listInStockOfProducts[Convert.ToInt32(selectedPanelTag)]);
                        if (ModifyQuantity.updateQuantity > GlobalData.listInStockOfProducts[Convert.ToInt32(selectedPanelTag)])
                            throw new Exception("You selected more product than we have");

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

               // foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
               // {
                 //   if (panel.Tag.ToString() == selectedPanelTag)
                   // {
                     //   MessageBox.Show(ModifyQuantity.updateQuantity.ToString() + "  " + GlobalData.listInStockOfProducts[Convert.ToInt32(selectedPanelTag)]);
                       // if (ModifyQuantity.updateQuantity > GlobalData.listInStockOfProducts[Convert.ToInt32(selectedPanelTag)])
                         //   throw new Exception("You selected more product than we have");

                        //Label labelTotalPrice = panel.Controls.Find("lblTotalPrice", true).FirstOrDefault() as Label;
                        //Label labelPricePerUnit = panel.Controls.Find("lblPricePerUnit", true).FirstOrDefault() as Label;
                        //Label labelQuantity = panel.Controls.Find("lblQuantity", true).FirstOrDefault() as Label;

                        /*foreach(Control c in panel.Controls)
                        {
                            if(c is Label lbl && lbl.Name == "lblPricePerUnit")
                        }*/

                        //double priceOfProduct = Convert.ToDouble(labelPricePerUnit.Text) * ModifyQuantity.updateQuantity;

                        /*double priceOfProduct = Convert.ToDouble(GlobalData.listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)] * ModifyQuantity.updateQuantity);
                        double pricePerUnit = GlobalData.listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)];
                        string productName = GlobalData.listNameOfProducts[Convert.ToInt32(selectedPanelTag)];*/

                        //labelTotalPrice.Text = priceOfProduct.ToString();
                        //labelPricePerUnit.Text = labelPricePerUnit.Text;
                        //labelQuantity.Text = ModifyQuantity.updateQuantity.ToString();

//                        GlobalData.listPriceOfProducts[Convert.ToInt32(selectedPanelTag)] = priceOfProduct;
  //                      GlobalData.listQuantityOfProducts[Convert.ToInt32(selectedPanelTag)] = ModifyQuantity.updateQuantity;
    //                }
      //          }
           }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            CalculateTotalPrice(lblTotalPrice);

            txtScannedBarcode.Clear();
            txtScannedBarcode.Focus();
            ModifyQuantity.updateQuantity = 1;
            lblUpdateNumberOfProducts.Text = ModifyQuantity.updateQuantity.ToString();

            Form1.isDoubleClicked = false;

            isUpdateQuantityPanelOpen = false;

            panelClickToPay.Visible = true;
        }

        public static void RemoveProduct(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;
            if (clickedButton != null)
            {
                Panel parentPanel = clickedButton.Parent as Panel;
                Button updateButton = parentPanel.Controls.Find("Update", true).FirstOrDefault() as Button;

                if (parentPanel != null && updateButton != null)
                {
                    flowLayoutPanel1.Controls.Remove(parentPanel);
                    parentPanel.Dispose();

                    UI.numberOfProducts--;

                    GlobalData.listNameOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listPriceOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listPricePerUnitOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listQuantityOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listIdsOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listManufacturerOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listTypeOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                    GlobalData.listInStockOfProducts.RemoveAt(Convert.ToInt32(updateButton.Tag));
                   // listAvailableQuantity.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    CalculateTotalPrice(lblTotalPrice);

                    UI.updatePanelTag(flowLayoutPanel1);

                    txtScannedBarcode.Focus();
                }
            }
        }

        public static void UpdateProductByButton(object sender, EventArgs e)
        {
            double quantityOfSelectedProduct;
            MessageBox.Show(Form1.isScrolling + " " + Form1.isBarcodeScanned);
            //if (!Form1.isScrolling && !Form1.isBarcodeScanned)
            if (!Form1.isBarcodeScanned)
            {
                panelUpdateProductQuantity.Visible = true;
                Form1.isDoubleClicked = true;

                btnUpdateProduct.Focus();
                Button button = sender as Button;
                if (button != null)
                {
                    for (int i = 0; i < GlobalData.listQuantityOfProducts.Count; i++)
                    {
                        if (Convert.ToInt32(button.Tag) == i)
                        {
                            quantityOfSelectedProduct = GlobalData.listQuantityOfProducts[i];
                            lblUpdateNumberOfProducts.Text = quantityOfSelectedProduct.ToString();
                            ModifyQuantity.updateQuantity = GlobalData.listQuantityOfProducts[i];

                            selectedPanelTag = button.Tag.ToString();

                            productInStock = GlobalData.listInStockOfProducts[i];

                            break;
                        }
                    }


                    UI.updatePanelTag(flowLayoutPanel1);
   
                    /*Panel panel = sender as Panel;
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

                                productInStock = GlobalData.listInStockOfProducts[i];

                                break;
                            }
                        }


                        UI.updatePanelTag(flowLayoutPanel1);

                        panelUpdateProductQuantity.Visible = true;
                    }*/
                }
                isUpdateQuantityPanelOpen = true;
            }
            UI.HideBackground(panelUpdateProductQuantity, panelDisableBackground);
        }

        public async Task<bool> GetInStockOfProduct(string barcode)
        {
            string query = "SELECT (InStock) FROM PRODUCT WHERE Barcode = @barcode";
            using (MySqlCommand command = new MySqlCommand(query, mySqlConnection))
            {
                command.Parameters.AddWithValue("@barcode", barcode);
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        productInStock = (double)reader["InStock"];
                        return true;
                    } else
                    {
                        UI.HideBackground(panelMessageBoxOk, panelDisableBackground); 
                        panelMessageBoxOk.Visible = true;
                        panelMessageBoxOk.Location = new Point(100, 100);
                        lblMessageBoxOK.Text = "Product doesn't exist";
                        isAddedExtraProduct = true;
                        Form1.isOpenAnything = true;
                        return false;
                    }
                }
            }
            
        }
        public static void CalculateTotalPrice(Label lbl)
        {
            double totalPrice = 0;
            for (int i = 0; i < GlobalData.listPriceOfProducts.Count; i++)
            {
                totalPrice += Math.Round(GlobalData.listPriceOfProducts[i], 2);
                Math.Round(totalPrice, 2);
            }
            lbl.Text = totalPrice.ToString() + "KM";
        }
    }
}
