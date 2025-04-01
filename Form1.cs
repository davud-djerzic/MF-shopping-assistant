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

namespace MF_Shopping_Assistant
{
    public partial class Form1 : Form
    {
        private string scannedBarcode;
        private bool isBarcodeScanned = false;
        private int passedSeconds = 0;
        private MySqlConnection mySqlConnection;

        private int locationLabelNameX = 17, locationLabelNameY = 21;
        private int locationLabelTotalPriceX = 298, locationLabelTotalPriceY = 48;
        private int locationLabelPricePerUnitX = 142, locationLabelPricePerUnitY = 48;
        private int locationLabelQuantityX = 17, locationLabelQuantityY = 48;

        private int numberOfProducts = 0;

        private List<int> listIdsOfProducts = new List<int>();
        private List<string> listNameOfProducts = new List<string>();
        private List<string> listTypeOfProducts = new List<string>();
        private List<string> listManufacturerOfProducts = new List<string>();
        private List<double> listPriceOfProducts = new List<double>();
        private List<double> listPricePerUnitOfProducts = new List<double>();
        private List<int> listQuantityOfProducts = new List<int>();
        private List<double> listInStockOfProducts = new List<double>();
        private int quantity = 1;
        private List<Panel> panelsOfProducts = new List<Panel>();

        private string selectedPanelTag;

        private int scrollStartY;
        private int scrollPosition;
        private bool isScrolling = false;

        private int updateQuantity = 1;
        private int quantityOfSelectedProduct;

        private bool isDoubleClicked = false;

        List<int> listDiscountProductId = new List<int>();
        List<int> listDiscountProductDiscountId = new List<int>();
        List<double> listDiscountProductPrice = new List<double>();
        List<string> listDiscountProductName = new List<string>();
        List<string> listDiscountProductType = new List<string>();
        List<string> listDiscountProductManufacturer = new List<string>();

        private bool isIntroFinish = false;

        List<double> listFruitPrice = new List<double>();
        List<double> listFruitDiscountPrice = new List<double>();
        List<double> listFruitInStock = new List<double>();
        List<string> listFruitName = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Env.Load();
            string connectionString = Env.GetString("Connection_String");
            //MessageBox.Show(connectionString);
            //string connectionString = Connections.Connection_String_Raspberry;

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
            txtScannedBarcode.Focus();

            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;
            homePagePanel.Click += HomePage_Click;
            lblHomePage.Click += HomePage_Click;

            panelDiscountPage.Click += DiscountPanelPage_Click;

            lblTotalPrice.Click += panelConfirmPayment_Click;
            lblClickToPay.Click += panelConfirmPayment_Click;

            addClickOnPanels(panelDiscountPage);

            await loadFruit();
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
        
        private void btnQuantityIncrease_Click(object sender, EventArgs e)
        {
            quantity++;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        private void btnQuantityDecrease_Click(object sender, EventArgs e)
        {
            if (quantity > 1) quantity--;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        private async void btnFinishProduct_Click(object sender, EventArgs e)
        {
            txtScannedBarcode.Enabled = true;

            panelProductQuantity.Visible = false;
            string getProductByBarcode = "SELECT * FROM Product";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    bool productAlreadyExists = false;

                    while (productReader.Read())
                    {
                        if (productReader["Barcode"].ToString() == scannedBarcode)
                        {
                            double priceOfProduct = 0;
                            int columnIndex = productReader.GetOrdinal("DiscountId");

                            if (!productReader.IsDBNull(columnIndex))
                            {
                                priceOfProduct = Convert.ToDouble(productReader["DiscountPrice"]) * quantity;
                            } else
                            {
                                priceOfProduct = Convert.ToDouble(productReader["Price"]) * quantity;
                            }

                            int indexOfFoundProduct = 0;

                            for (int i = 0; i < listIdsOfProducts.Count; i++)
                            {
                                if (listIdsOfProducts[i] == Convert.ToInt32(productReader["ID"]))
                                {
                                    listQuantityOfProducts[i] += quantity;
                                    listPriceOfProducts[i] += priceOfProduct;
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
                                        labelTotalPrice.Text = listPriceOfProducts[indexOfFoundProduct].ToString();
                                        labelQuantity.Text = listQuantityOfProducts[indexOfFoundProduct].ToString();
                                    }
                                }
                            }

                            if (!productAlreadyExists)
                            {
                                int columnIndex1 = productReader.GetOrdinal("DiscountId");

                                if (!productReader.IsDBNull(columnIndex1))
                                {
                                    addNewProduct(productReader["Name"].ToString(), priceOfProduct.ToString(), productReader["DiscountPrice"].ToString(), quantity.ToString(), false);
                                } else
                                {
                                    addNewProduct(productReader["Name"].ToString(), priceOfProduct.ToString(), productReader["Price"].ToString(), quantity.ToString(), false);
                                }

                                listQuantityOfProducts.Add(quantity);
                                listPricePerUnitOfProducts.Add(Convert.ToDouble(productReader["Price"]));
                                listPriceOfProducts.Add(priceOfProduct);
                                listNameOfProducts.Add(productReader["Name"].ToString());
                                listIdsOfProducts.Add(Convert.ToInt32(productReader["ID"]));
                                listTypeOfProducts.Add(productReader["Type"].ToString());
                                listManufacturerOfProducts.Add(productReader["Manufacturer"].ToString());
                                listInStockOfProducts.Add(Convert.ToInt32(productReader["InStock"]));
                            }

                            break;
                        }
                    }
                    double totalPrice = 0;
                    for (int i = 0; i < listPriceOfProducts.Count; i++)
                    {
                        totalPrice += listPriceOfProducts[i];
                    }
                    lblTotalPrice.Text = totalPrice.ToString() + "KM#";

                    txtScannedBarcode.Clear();
                    txtScannedBarcode.Focus();
                    quantity = 1;
                    lblNumberOfProducts.Text = quantity.ToString();

                    isBarcodeScanned = false;
                }
            }
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
            scrollStartY = e.Y;
            scrollPosition = flowLayoutPanel1.AutoScrollPosition.Y;
            isScrolling = false;
        }

        private void flowLayoutPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int deltaY = e.Y - scrollStartY;
                if (Math.Abs(deltaY) > 5)
                {
                    isScrolling = true;
                    flowLayoutPanel1.AutoScrollPosition = new Point(0, -(scrollPosition + deltaY));
                }
            }
        }

        private void flowLayoutPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            flowLayoutPanel1.Capture = false;
        }
        //get data from gpioport
        private void btnUpdateProduct_Click(object sender, EventArgs e)
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

                    double priceOfProduct = Convert.ToDouble(listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)] * updateQuantity);
                    double pricePerUnit = listPricePerUnitOfProducts[Convert.ToInt32(selectedPanelTag)];
                    string productName = listNameOfProducts[Convert.ToInt32(selectedPanelTag)];

                    labelTotalPrice.Text = priceOfProduct.ToString();
                    labelPricePerUnit.Text = pricePerUnit.ToString();
                    labelQuantity.Text = updateQuantity.ToString();

                    listPriceOfProducts[Convert.ToInt32(selectedPanelTag)] = priceOfProduct;
                    listQuantityOfProducts[Convert.ToInt32(selectedPanelTag)] = updateQuantity;
                }
            }

            double totalPrice = 0;
            for (int i = 0; i < listPriceOfProducts.Count; i++)
            {
                totalPrice += listPriceOfProducts[i];
                lblTotalPrice.Text = totalPrice.ToString();
            }

            txtScannedBarcode.Clear();
            txtScannedBarcode.Focus();
            updateQuantity = 1;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();

            isDoubleClicked = false;
        }

        private void btnUpdateQuantityDecrease_Click(object sender, EventArgs e)
        {
            if (updateQuantity > 1) updateQuantity--;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();
        }

        private void btnUpdateQuantityIncrease_Click(object sender, EventArgs e)
        {
            updateQuantity++;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();
        }

        private void panelConfirmPayment_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult rez = MessageBox.Show("Confirm purchase", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (rez == DialogResult.Yes)
                {
                    if (listPriceOfProducts.Count == 0) throw new Exception("You don't have product to buy");

                    flowLayoutPanel1.Visible = false;
                    flowLayoutPanel2.Visible = true;
                    btnPay.Visible = true;
                    panelConfirmPayment.Visible = false;

                    for (int i = 0; i < listPriceOfProducts.Count; i++)
                    {
                        Label label = new Label()
                        {
                            Size = new Size(477, 35),
                            Location = new Point(10, i * 40),
                        };
                        label.Text = listNameOfProducts[i] + "   " + listTypeOfProducts[i] + "   " + listManufacturerOfProducts[i] + "\n" + listQuantityOfProducts[i] + "x" + "   " + listPricePerUnitOfProducts[i] + "    " + listPriceOfProducts[i];
                        flowLayoutPanel2.Controls.Add(label);
                    }
                }
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
                flowLayoutPanel1.Visible = true;
                flowLayoutPanel2.Visible = false;
                btnPay.Visible = false;
                flowLayoutPanel1.Controls.Clear();
                flowLayoutPanel2.Controls.Clear();
                MessageBox.Show("Payment completed successfully");
                string queryUpdateQuantityOfProduct = "UPDATE Product SET InStock = @Item WHERE Id = @Id";
                MySqlCommand cmdUpdateProduct1 = new MySqlCommand(queryUpdateQuantityOfProduct, mySqlConnection);
                for (int i = 0; i < listIdsOfProducts.Count; i++)
                {
                    cmdUpdateProduct1.Parameters.Clear();

                    cmdUpdateProduct1.Parameters.AddWithValue("@Item", listInStockOfProducts[i] - listQuantityOfProducts[i]);
                    cmdUpdateProduct1.Parameters.AddWithValue("@Id", listIdsOfProducts[i]);
                    await cmdUpdateProduct1.ExecuteNonQueryAsync();
                }

                reset();

                await loadFruit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private void addNewProduct(string productName, string totalPrice, string pricePerUnit, string quantity, bool isFruit)
        {
            Panel panel;
            if (numberOfProducts == 0)
            {
                panel = new Panel
                {
                    Size = new Size(260, 68),
                    Location = new Point(6, 12),
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
            else
            {
                panel = new Panel
                {
                    Size = new Size(260, 68),
                    Location = new Point(6, 71 * numberOfProducts),
                    BackColor = Color.LightGray,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = numberOfProducts
                };
            }
            if (!isFruit) panel.DoubleClick += PanelDoubleClick_DoubleClick;

            panel.MouseDown += flowLayoutPanel1_MouseDown;
            panel.MouseUp += flowLayoutPanel1_MouseUp;
            panel.MouseMove += flowLayoutPanel1_MouseMove;

            Label labelName = new Label
            {
                Text = productName,
                Location = new Point(17, 21),
                AutoSize = true
            };
            panel.Controls.Add(labelName);

            Label labelTotalPrice = new Label
            {
                Name = "lblTotalPrice",
                Text = totalPrice,
                Location = new Point(230, 48),
                AutoSize = true
            };

            panel.Controls.Add(labelTotalPrice);

            Label labelPricePerUnit = new Label
            {
                Name = "lblPricePerUnit",
                Text = pricePerUnit,
                Location = new Point(142, 48),
                AutoSize = true
            };

            panel.Controls.Add(labelPricePerUnit);

            Label labelQuantity = new Label
            {
                Name = "lblQuantity",
                Text = quantity,
                Location = new Point(17, 48),
                AutoSize = true
            };

            panel.Controls.Add(labelQuantity);

            System.Windows.Forms.Button closeButton = new System.Windows.Forms.Button
            {
                Location = new Point(220, 1),
                AutoSize = true,
                Tag = numberOfProducts,

            };
            closeButton.Click += ButtonClick_Click;
            panel.Controls.Add(closeButton);

            numberOfProducts++;

            panelsOfProducts.Add(panel);
            flowLayoutPanel1.Controls.Add(panel);
        }

        private void ButtonClick_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Button clickedButton = sender as System.Windows.Forms.Button;
            if (clickedButton != null)
            {
                Panel parentPanel = clickedButton.Parent as Panel;
                if (parentPanel != null)
                {
                    flowLayoutPanel1.Controls.Remove(parentPanel);
                    parentPanel.Dispose();

                    numberOfProducts--;

                    listNameOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listPriceOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listPricePerUnitOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listQuantityOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listIdsOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listManufacturerOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listTypeOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));
                    listInStockOfProducts.RemoveAt(Convert.ToInt32(parentPanel.Tag));

                    double totalPrice = 0;
                    for (int i = 0; i < listPriceOfProducts.Count; i++)
                    {
                        totalPrice += listPriceOfProducts[i];
                    }
                    lblTotalPrice.Text = totalPrice.ToString();

                    updatePanelTag();
                }
            }
        }

        private void PanelDoubleClick_DoubleClick(object sender, EventArgs e)
        {
            if (!isScrolling && !isBarcodeScanned)
            {
                isDoubleClicked = true;

                btnUpdateProduct.Focus();
                Panel panel = sender as Panel;
                if (panel != null)
                {
                    for (int i = 0; i < listQuantityOfProducts.Count; i++)
                    {
                        if (Convert.ToInt32(panel.Tag) == i)
                        {
                            quantityOfSelectedProduct = listQuantityOfProducts[i];
                            lblUpdateNumberOfProducts.Text = quantityOfSelectedProduct.ToString();
                            updateQuantity = listQuantityOfProducts[i];

                            selectedPanelTag = panel.Tag.ToString();

                            break;
                        }
                    }


                    updatePanelTag();

                    panelUpdateProductQuantity.Visible = true;
                }
            }
        }

        private void updatePanelTag()
        {
            int tagCounter = 0;
            foreach (Panel panel in flowLayoutPanel1.Controls.OfType<Panel>())
            {
                panel.Tag = tagCounter++;
            }
        }

        private async void HomePage_Click(object sender, EventArgs e)
        {
            homePagePanel.Visible = false;
            panelDiscountPage.Location = new Point(homePagePanel.Location.X, homePagePanel.Location.Y);
            await getDiscountProducts();
        }

        private void btnShowFruit_Click(object sender, EventArgs e)
        {
            flpFruit.Visible = true;

            flpFruit.Location = new Point(homePagePanel.Location.X, homePagePanel.Location.Y);
            flpFruit.Size=new Size(homePagePanel.Width, homePagePanel.Height);
        }

        private void FruitClick_Click(object sender, EventArgs e)
        {
            if (sender is Panel panel && panel.Tag != null)
            {
                string clickedTag = panel.Tag.ToString();
                MessageBox.Show(clickedTag);
                for (int i = 0; i < listFruitName.Count; i++)
                {
                    if (listFruitName[i] == clickedTag)
                    {
                        if (listFruitDiscountPrice[i] == 0)
                        {
                            double priceOfProduct = listFruitPrice[i] * 1;
                            listQuantityOfProducts.Add(1);
                            listPricePerUnitOfProducts.Add(listFruitPrice[i]);
                            listPriceOfProducts.Add(priceOfProduct);
                            listNameOfProducts.Add(listFruitName[i]);
                            listIdsOfProducts.Add(1);
                            listTypeOfProducts.Add("1");
                            listManufacturerOfProducts.Add("1");
                            listInStockOfProducts.Add(listFruitInStock[i]);

                            double totalPriceOfProduct = listFruitPrice[i] * 1;
                            addNewProduct(listFruitName[i], totalPriceOfProduct.ToString(), listFruitPrice[i].ToString(), "1", true);
                        }
                        if (listFruitDiscountPrice[i] != 0)
                        {
                            double priceOfProduct = listFruitDiscountPrice[i] * 1;
                            listQuantityOfProducts.Add(1);
                            listPricePerUnitOfProducts.Add(listFruitPrice[i]);
                            listPriceOfProducts.Add(priceOfProduct);
                            listNameOfProducts.Add(listFruitName[i]);
                            listIdsOfProducts.Add(1);
                            listTypeOfProducts.Add("1");
                            listManufacturerOfProducts.Add("1");
                            listInStockOfProducts.Add(listFruitInStock[i]);

                            double totalPriceOfProduct = listFruitDiscountPrice[i] * 1;
                            addNewProduct(listFruitName[i], totalPriceOfProduct.ToString(), listFruitPrice[i].ToString(), "1", true);
                        }
                    }
                }

                double totalPrice = 0;
                for (int i = 0; i < listPriceOfProducts.Count; i++)
                {
                    totalPrice += listPriceOfProducts[i];
                }
                lblTotalPrice.Text = totalPrice.ToString();

                flpFruit.Visible = false;

                txtScannedBarcode.Focus();
            }
        }

        private async Task loadFruit()
        {
            flpFruit.Visible = false;

            listFruitInStock.Clear();
            listFruitName.Clear();
            listFruitDiscountPrice.Clear();
            listFruitPrice.Clear();
            flpFruit.Controls.Clear();

            string getCategoryForFruit = "SELECT * FROM Category";

            int fruitCategoryID = 0;
            int vegetablesCategoryID = 0;

            using (MySqlCommand cmdProduct = new MySqlCommand(getCategoryForFruit, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    while (productReader.Read())
                    {
                        if (productReader["name"].ToString() == "Fruit") fruitCategoryID = (int)productReader["ID"];
                        if (productReader["name"].ToString() == "Vegetables") vegetablesCategoryID = (int)productReader["ID"];
                    }
                }
            }

            //MessageBox.Show(FruitCategoryID.ToString() + " " + VegetablesCategoryID.ToString());

            string getProductByFruit = $"SELECT name,price,discountPrice,inStock FROM Product WHERE CategoryID={vegetablesCategoryID} OR CategoryID={fruitCategoryID}";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByFruit, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    while (productReader.Read())
                    {
                        int columnIndex = productReader.GetOrdinal("DiscountPrice");

                        if (!productReader.IsDBNull(columnIndex)) listFruitDiscountPrice.Add((double)productReader["DiscountPrice"]);
                        else listFruitDiscountPrice.Add(0);

                        listFruitInStock.Add((double)productReader["InStock"]);
                        listFruitName.Add(productReader["Name"].ToString().ToLower());
                        listFruitPrice.Add((double)productReader["Price"]);
                    }
                }
            }

            for (int i = 0; i < listFruitDiscountPrice.Count; i++)
            {
                string imagePath = Path.Combine(Application.StartupPath, "Pictures", $"{listFruitName[i]}.jpg");

                Panel panel = new Panel
                {
                    Size = new Size(100, 100),
                    Tag = listFruitName[i],
                    BackgroundImage = Image.FromFile(imagePath),
                    BackgroundImageLayout = ImageLayout.Stretch
                };
                panel.Click += FruitClick_Click;

                Label label = new Label {
                    Text = $"{i+1}"
                };
                panel.Controls.Add(label);
                flpFruit.Controls.Add(panel);
            }
        }

        private async Task getDiscountProducts()
        {
            string getProductByBarcode = "SELECT * FROM Product WHERE DiscountId IS NOT NULL";

            using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
            {
                using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                {
                    while (productReader.Read())
                    {
                        listDiscountProductId.Add(productReader.GetInt32(0));
                        listDiscountProductDiscountId.Add((int)productReader["DiscountId"]);
                        listDiscountProductPrice.Add((double)productReader["Price"]);
                        listDiscountProductName.Add((string)productReader["Name"]);
                        listDiscountProductType.Add((string)productReader["Type"]);
                        listDiscountProductManufacturer.Add((string)productReader["Manufacturer"]);
                    }
                }
            }


            List<int> listDiscountId = new List<int>();
            List<int> listDiscountPercentage = new List<int>();

            string getDiscount = "SELECT * FROM Discount";

            using (MySqlCommand cmdDiscount = new MySqlCommand(getDiscount, mySqlConnection))
            {
                using (DbDataReader discountReader = await cmdDiscount.ExecuteReaderAsync())
                {
                    while (discountReader.Read())
                    {
                        listDiscountId.Add(discountReader.GetInt32(0));
                        listDiscountPercentage.Add((int)discountReader["Percentage"]);
                    }
                }
            }

            int maxPercentage = 0;
            int maxPercentage2 = 0;

            int indexOfMaxPercentage1 = 0;
            int indexOfMaxPercentage2 = 0;


            for (int j = 0; j < listDiscountId.Count; j++)
            {
                for (int l = 0; l < listDiscountId.Count - j - 1; l++)
                {
                    if (listDiscountPercentage[l] < listDiscountPercentage[l + 1])
                    {
                        int temp = listDiscountPercentage[l];
                        listDiscountPercentage[l] = listDiscountPercentage[l + 1];
                        listDiscountPercentage[l + 1] = temp;

                        int temp1 = listDiscountId[l];
                        listDiscountId[l] = listDiscountId[l + 1];
                        listDiscountId[l + 1] = temp1;
                    }
                }
            }

            maxPercentage = listDiscountPercentage[0];
            indexOfMaxPercentage1 = listDiscountId[0];

            maxPercentage2 = listDiscountPercentage[1];
            indexOfMaxPercentage2 = listDiscountId[1];

            int numOfMaxDiscounts1 = 0;
            int numOfMaxDiscounts2 = 0;
            for (int i = 0; i < listDiscountProductDiscountId.Count; i++)
            {
                if (listDiscountProductDiscountId[i] == indexOfMaxPercentage1)
                {
                    numOfMaxDiscounts1++;
                }
                if (listDiscountProductDiscountId[i] == indexOfMaxPercentage2)
                {
                    numOfMaxDiscounts2++;
                }
            }

            List<double> listDiscountPrices = new List<double>();
            List<double> listPriceDifferences = new List<double>();
            int counter = 0;

            int indexOfMaxPriceDifference1 = 0;
            int indexOfMaxPriceDifference2 = 0;

            if (numOfMaxDiscounts1 >= 2)
            {
                List<int> listPriceDifferencesProductId = new List<int>();
                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    for (int j = 0; j < listDiscountId.Count; j++)
                    {
                        if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                        {
                            listDiscountPrices.Add(listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0));
                            listPriceDifferences.Add(listDiscountProductPrice[i] - listDiscountPrices[counter]);
                            listPriceDifferencesProductId.Add(listDiscountProductId[i]);
                            counter++;
                        }
                    }
                }

                double maxPriceDifference1 = 0;
                double maxPriceDifference2 = 0;


                for(int i = 0; i < listPriceDifferences.Count; i++)
                {
                    for (int j = 0; j < listPriceDifferences.Count - i - 1; j++)
                    {
                        if (listPriceDifferences[j] < listPriceDifferences[j + 1])
                        {
                            double temp = listPriceDifferences[j + 1];
                            listPriceDifferences[j + 1] = listPriceDifferences[j];
                            listPriceDifferences[j] = temp;

                            int temp1 = listPriceDifferencesProductId[j + 1];
                            listPriceDifferencesProductId[j + 1] = listPriceDifferencesProductId[j];
                            listPriceDifferencesProductId[j] = temp1;
                        }
                    }
                }

                maxPriceDifference1 = listPriceDifferences[0];
                indexOfMaxPriceDifference1 = listPriceDifferencesProductId[0];
                maxPriceDifference2 = listPriceDifferences[1];
                indexOfMaxPriceDifference2 = listPriceDifferencesProductId[1];


                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    if (listDiscountProductId[i] == indexOfMaxPriceDifference1)
                    {
                        lblProductNameP1.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                        lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                        lblOldPriceP1.Text = listDiscountProductPrice[i].ToString();
                        lblDiscountPriceP1.Text = (listDiscountProductPrice[i] - maxPriceDifference1).ToString();

                        //removeAlreadyUsedProduct(i);
                        //break;
                    }
                    if (listDiscountProductId[i] == indexOfMaxPriceDifference2)
                    {
                        lblProductNameP2.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                        lblDiscountPercentageP2.Text = maxPercentage.ToString() + "%";
                        lblOldPriceP2.Text = listDiscountProductPrice[i].ToString();
                        lblDiscountPriceP2.Text = (listDiscountProductPrice[i] - maxPriceDifference2).ToString();

                        //removeAlreadyUsedProduct(i);
                        //break;
                    }
                }
            }
            
            else if (numOfMaxDiscounts1 == 1 && numOfMaxDiscounts2 == 1)
            {
                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    for (int j = 0; j < listDiscountId.Count; j++)
                    {
                        if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                        {
                            indexOfMaxPriceDifference1 = listDiscountProductId[i];
                            lblProductNameP1.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                            lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                            lblOldPriceP1.Text = listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP1.Text = (listDiscountProductPrice[i] * ((100 - maxPercentage) / 100.0)).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }

                        if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage2)
                        {
                            indexOfMaxPriceDifference2 = listDiscountProductId[i];
                            lblProductNameP2.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                            lblDiscountPercentageP2.Text = maxPercentage2.ToString() + "%";
                            lblOldPriceP2.Text = listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP2.Text = (listDiscountProductPrice[i] * ((100 - maxPercentage2) / 100.0)).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }
                    }
                }
            }

            else if (numOfMaxDiscounts1 == 1 && numOfMaxDiscounts2 >= 2)
            {
                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    for (int j = 0; j < listDiscountId.Count; j++)
                    {
                        if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                        {
                            indexOfMaxPriceDifference1 = listDiscountProductId[i];
                            lblProductNameP1.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                            lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                            lblOldPriceP1.Text = listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP1.Text = (listDiscountProductPrice[i] * ((100 - maxPercentage) / 100.0)).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }
                    }
                }

                List<int> listPriceDifferencesProductId = new List<int>();
                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    for (int j = 0; j < listDiscountId.Count; j++)
                    {
                        if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage2)
                        {
                            listDiscountPrices.Add(listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0));
                            listPriceDifferences.Add(listDiscountProductPrice[i] - listDiscountPrices[counter]);
                            listPriceDifferencesProductId.Add(listDiscountProductId[i]);
                            counter++;
                        }
                    }
                }

                double maxPriceDifference2 = 0;
               // int indexOfMaxPriceDifference2 = 0;

                for (int i = 0; i < listPriceDifferences.Count; i++)
                {
                    for (int j = 0; j < listPriceDifferences.Count - i - 1; j++)
                    {
                        if (listPriceDifferences[j] < listPriceDifferences[j + 1])
                        {
                            double temp = listPriceDifferences[j + 1];
                            listPriceDifferences[j + 1] = listPriceDifferences[j];
                            listPriceDifferences[j] = temp;

                            int temp1 = listPriceDifferencesProductId[j + 1];
                            listPriceDifferencesProductId[j + 1] = listPriceDifferencesProductId[j];
                            listPriceDifferencesProductId[j] = temp1;
                        }
                    }
                }

                maxPriceDifference2 = listPriceDifferences[0];
                indexOfMaxPriceDifference2 = listPriceDifferencesProductId[0];

                for (int i = 0; i < listDiscountProductId.Count; i++)
                {
                    if (listDiscountProductId[i] == indexOfMaxPriceDifference2)
                    {
                        lblProductNameP2.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                        lblDiscountPercentageP2.Text = maxPercentage2.ToString() + "%";
                        lblOldPriceP2.Text = listDiscountProductPrice[i].ToString();
                        lblDiscountPriceP2.Text = (listDiscountProductPrice[i] - maxPriceDifference2).ToString();

                        //removeAlreadyUsedProduct(i);
                        //break;
                    }
                }
            }

            listPriceDifferences.Clear();
            List<int> listPriceDifferencesProductId1 = new List<int>();

            for (int i = 0; i < listDiscountProductId.Count; i++)
            {
                for (int j = 0; j < listDiscountId.Count; j++)
                {
                    if (listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountProductId[i] != indexOfMaxPriceDifference1 && listDiscountProductId[i] != indexOfMaxPriceDifference2)
                    {
                        double discountPrice = listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0);
                        listPriceDifferences.Add(listDiscountProductPrice[i] - discountPrice);
                        listPriceDifferencesProductId1.Add(listDiscountProductId[i]);
                    }
                }
            }

            for (int i = 0; i < listPriceDifferences.Count; i++)
            {
                for (int j = 0; j < listPriceDifferences.Count - i - 1; j++)
                {
                    if (listPriceDifferences[j] < listPriceDifferences[j + 1])
                    {
                        double temp = listPriceDifferences[j + 1];
                        listPriceDifferences[j + 1] = listPriceDifferences[j];
                        listPriceDifferences[j] = temp;

                        int temp1 = listPriceDifferencesProductId1[j + 1];
                        listPriceDifferencesProductId1[j + 1] = listPriceDifferencesProductId1[j];
                        listPriceDifferencesProductId1[j] = temp1;
                    }
                }
            }

           // MessageBox.Show(indexOfMaxPriceDifference1.ToString() + indexOfMaxPriceDifference2.ToString());

           // MessageBox.Show(listPriceDifferencesProductId1[0].ToString());
           /* for (int i = 0; i < listDiscountProductId.Count; i++)
            {
                MessageBox.Show(listDiscountProductId[i].ToString());
                    
            }*/

            for (int i = 0; i < listDiscountProductId.Count; i++)
            {
                if (listDiscountProductId[i] == listPriceDifferencesProductId1[0] && listDiscountProductId[i] != indexOfMaxPriceDifference1)
                {
                    lblProductNameP3.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                    lblOldPriceP3.Text = listDiscountProductPrice[i].ToString();
                    lblDiscountPriceP3.Text = (listDiscountProductPrice[i] - listPriceDifferences[0]).ToString();
                }

                if (listDiscountProductId[i] == listPriceDifferencesProductId1[1] && listDiscountProductId[i] != indexOfMaxPriceDifference2)
                {
                    lblProductNameP4.Text = listDiscountProductName[i] + " " + listDiscountProductType[i] + " " + listDiscountProductManufacturer[i];
                    lblOldPriceP4.Text = listDiscountProductPrice[i].ToString();
                    lblDiscountPriceP4.Text = (listDiscountProductPrice[i] - listPriceDifferences[1]).ToString();
                }
                
            }
        }

        private void removeAlreadyUsedProduct(int index)
        {
            listDiscountProductId.RemoveAt(index);
            listDiscountProductDiscountId.RemoveAt(index);
            listDiscountProductPrice.RemoveAt(index);
            listDiscountProductName.RemoveAt(index);
            listDiscountProductType.RemoveAt(index);
            listDiscountProductManufacturer.RemoveAt(index);
        }

        private void DiscountPanelPage_Click(object sender, EventArgs e)
        {
            panelDiscountPage.Visible = false;
            flowLayoutPanel1.Visible = true;
            txtScannedBarcode.Clear();
            isIntroFinish = true;
        }

        private void reset()
        {
            homePagePanel.Visible = true;
            listIdsOfProducts.Clear();
            listNameOfProducts.Clear();
            listTypeOfProducts.Clear();
            listManufacturerOfProducts.Clear();
            listPriceOfProducts.Clear();
            listPricePerUnitOfProducts.Clear();
            listQuantityOfProducts.Clear();
            listInStockOfProducts.Clear();
            numberOfProducts = 0;
            quantity = 1;
            updateQuantity = 1;
            isBarcodeScanned = false;
            passedSeconds = 0;

            panelsOfProducts.Clear();
            listDiscountProductId.Clear();
            listDiscountProductDiscountId.Clear();
            listDiscountProductPrice.Clear();
            listDiscountProductName.Clear();
            listDiscountProductType.Clear();
            listDiscountProductManufacturer.Clear();

            isDoubleClicked = false;
            isScrolling = false;

            panelDiscountPage.Visible = true;
            flowLayoutPanel1.Visible = true;
            flowLayoutPanel2.Visible = true;
            panelConfirmPayment.Visible = true;
            txtScannedBarcode.Focus();
            lblTotalPrice.Text = "0";

            isIntroFinish = false;
        }

    }
}

    

