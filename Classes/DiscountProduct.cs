using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    internal class DiscountProduct
    {
        private readonly MySqlConnection mySqlConnection;
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

        public DiscountProduct(MySqlConnection mySqlConnection, Label lblProductNameP1, Label lblDiscountPercentageP1, Label lblOldPriceP1, Label lblDiscountPriceP1, Label lblProductNameP2, Label lblDiscountPercentageP2, Label lblOldPriceP2, Label lblDiscountPriceP2, Label lblProductNameP3, Label lblOldPriceP3, Label lblDiscountPriceP3, Label lblProductNameP4, Label lblOldPriceP4, Label lblDiscountPriceP4)
        {
            this.mySqlConnection = mySqlConnection;

            this.lblProductNameP1 = lblProductNameP1;
            this.lblDiscountPriceP1 = lblDiscountPriceP1;
            this.lblOldPriceP1 = lblOldPriceP1;
            this.lblDiscountPercentageP1 = lblDiscountPercentageP1;

            this.lblProductNameP2 = lblProductNameP2;
            this.lblDiscountPercentageP2 = lblDiscountPercentageP2;
            this.lblOldPriceP2 = lblOldPriceP2;
            this.lblDiscountPriceP2 = lblDiscountPriceP2;

            this.lblProductNameP3 = lblProductNameP3;
            this.lblOldPriceP3 = lblOldPriceP3;
            this.lblDiscountPriceP3 = lblDiscountPriceP3;

            this.lblProductNameP4 = lblProductNameP4;
            this.lblOldPriceP4 = lblOldPriceP4;
            this.lblDiscountPriceP4 = lblDiscountPriceP4;
        }


        public async Task getDiscountProducts()
        {
            try
            {
                GlobalData.listDiscountProductId.Clear();
                GlobalData.listDiscountProductDiscountId.Clear();
                GlobalData.listDiscountProductPrice.Clear();
                GlobalData.listDiscountProductName.Clear();
                GlobalData.listDiscountProductType.Clear();
                GlobalData.listDiscountProductManufacturer.Clear();
                string getProductByBarcode = "SELECT * FROM Product WHERE DiscountId IS NOT NULL";

                using (MySqlCommand cmdProduct = new MySqlCommand(getProductByBarcode, mySqlConnection))
                {
                    using (DbDataReader productReader = await cmdProduct.ExecuteReaderAsync())
                    {
                        while (productReader.Read())
                        {
                            GlobalData.listDiscountProductId.Add(productReader.GetInt32(0));
                            GlobalData.listDiscountProductDiscountId.Add((int)productReader["DiscountId"]);
                            GlobalData.listDiscountProductPrice.Add((double)productReader["Price"]);
                            GlobalData.listDiscountProductName.Add((string)productReader["Name"]);
                            GlobalData.listDiscountProductType.Add((string)productReader["Type"]);
                            GlobalData.listDiscountProductManufacturer.Add((string)productReader["Manufacturer"]);
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
                for (int i = 0; i < GlobalData.listDiscountProductDiscountId.Count; i++)
                {
                    if (GlobalData.listDiscountProductDiscountId[i] == indexOfMaxPercentage1)
                    {
                        numOfMaxDiscounts1++;
                    }
                    if (GlobalData.listDiscountProductDiscountId[i] == indexOfMaxPercentage2)
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
                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        for (int j = 0; j < listDiscountId.Count; j++)
                        {
                            if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                            {
                                listDiscountPrices.Add(GlobalData.listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0));
                                listPriceDifferences.Add(GlobalData.listDiscountProductPrice[i] - listDiscountPrices[counter]);
                                listPriceDifferencesProductId.Add(GlobalData.listDiscountProductId[i]);
                                counter++;
                            }
                        }
                    }

                    double maxPriceDifference1 = 0;
                    double maxPriceDifference2 = 0;


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

                    maxPriceDifference1 = listPriceDifferences[0];
                    indexOfMaxPriceDifference1 = listPriceDifferencesProductId[0];
                    maxPriceDifference2 = listPriceDifferences[1];
                    indexOfMaxPriceDifference2 = listPriceDifferencesProductId[1];

                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        if (GlobalData.listDiscountProductId[i] == indexOfMaxPriceDifference1)
                        {
                            lblProductNameP1.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                            lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                            lblOldPriceP1.Text = GlobalData.listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP1.Text = (GlobalData.listDiscountProductPrice[i] - maxPriceDifference1).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }
                        if (GlobalData.listDiscountProductId[i] == indexOfMaxPriceDifference2)
                        {
                            lblProductNameP2.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                            lblDiscountPercentageP2.Text = maxPercentage.ToString() + "%";
                            lblOldPriceP2.Text = GlobalData.listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP2.Text = (GlobalData.listDiscountProductPrice[i] - maxPriceDifference2).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }
                    }
                }

                else if (numOfMaxDiscounts1 == 1 && numOfMaxDiscounts2 == 1)
                {
                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        for (int j = 0; j < listDiscountId.Count; j++)
                        {
                            if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                            {
                                indexOfMaxPriceDifference1 = GlobalData.listDiscountProductId[i];
                                lblProductNameP1.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                                lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                                lblOldPriceP1.Text = GlobalData.listDiscountProductPrice[i].ToString();
                                lblDiscountPriceP1.Text = (GlobalData.listDiscountProductPrice[i] * ((100 - maxPercentage) / 100.0)).ToString();

                                //removeAlreadyUsedProduct(i);
                                //break;
                            }

                            if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage2)
                            {
                                indexOfMaxPriceDifference2 = GlobalData.listDiscountProductId[i];
                                lblProductNameP2.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                                lblDiscountPercentageP2.Text = maxPercentage2.ToString() + "%";
                                lblOldPriceP2.Text = GlobalData.listDiscountProductPrice[i].ToString();
                                lblDiscountPriceP2.Text = (GlobalData.listDiscountProductPrice[i] * ((100 - maxPercentage2) / 100.0)).ToString();

                                //removeAlreadyUsedProduct(i);
                                //break;
                            }
                        }
                    }
                }

                else if (numOfMaxDiscounts1 == 1 && numOfMaxDiscounts2 >= 2)
                {
                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        for (int j = 0; j < listDiscountId.Count; j++)
                        {
                            if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage)
                            {
                                indexOfMaxPriceDifference1 = GlobalData.listDiscountProductId[i];
                                lblProductNameP1.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                                lblDiscountPercentageP1.Text = maxPercentage.ToString() + "%";
                                lblOldPriceP1.Text = GlobalData.listDiscountProductPrice[i].ToString();
                                lblDiscountPriceP1.Text = (GlobalData.listDiscountProductPrice[i] * ((100 - maxPercentage) / 100.0)).ToString();

                                //removeAlreadyUsedProduct(i);
                                //break;
                            }
                        }
                    }

                    List<int> listPriceDifferencesProductId = new List<int>();
                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        for (int j = 0; j < listDiscountId.Count; j++)
                        {
                            if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && listDiscountPercentage[j] == maxPercentage2)
                            {
                                listDiscountPrices.Add(GlobalData.listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0));
                                listPriceDifferences.Add(GlobalData.listDiscountProductPrice[i] - listDiscountPrices[counter]);
                                listPriceDifferencesProductId.Add(GlobalData.listDiscountProductId[i]);
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

                    for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                    {
                        if (GlobalData.listDiscountProductId[i] == indexOfMaxPriceDifference2)
                        {
                            lblProductNameP2.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                            lblDiscountPercentageP2.Text = maxPercentage2.ToString() + "%";
                            lblOldPriceP2.Text = GlobalData.listDiscountProductPrice[i].ToString();
                            lblDiscountPriceP2.Text = (GlobalData.listDiscountProductPrice[i] - maxPriceDifference2).ToString();

                            //removeAlreadyUsedProduct(i);
                            //break;
                        }
                    }
                }

                listPriceDifferences.Clear();
                List<int> listPriceDifferencesProductId1 = new List<int>();

                for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                {
                    for (int j = 0; j < listDiscountId.Count; j++)
                    {
                        if (GlobalData.listDiscountProductDiscountId[i] == listDiscountId[j] && GlobalData.listDiscountProductId[i] != indexOfMaxPriceDifference1 && GlobalData.listDiscountProductId[i] != indexOfMaxPriceDifference2)
                        {
                            double discountPrice = GlobalData.listDiscountProductPrice[i] * ((100 - listDiscountPercentage[j]) / 100.0);
                            listPriceDifferences.Add(GlobalData.listDiscountProductPrice[i] - discountPrice);
                            listPriceDifferencesProductId1.Add(GlobalData.listDiscountProductId[i]);
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

                for (int i = 0; i < GlobalData.listDiscountProductId.Count; i++)
                {
                    if (GlobalData.listDiscountProductId[i] == listPriceDifferencesProductId1[0] && GlobalData.listDiscountProductId[i] != indexOfMaxPriceDifference1)
                    {
                        lblProductNameP3.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                        lblOldPriceP3.Text = GlobalData.listDiscountProductPrice[i].ToString();
                        lblDiscountPriceP3.Text = (GlobalData.listDiscountProductPrice[i] - listPriceDifferences[0]).ToString();
                    }

                    if (GlobalData.listDiscountProductId[i] == listPriceDifferencesProductId1[1] && GlobalData.listDiscountProductId[i] != indexOfMaxPriceDifference2)
                    {
                        lblProductNameP4.Text = GlobalData.listDiscountProductName[i] + " " + GlobalData.listDiscountProductType[i] + " " + GlobalData.listDiscountProductManufacturer[i];
                        lblOldPriceP4.Text = GlobalData.listDiscountProductPrice[i].ToString();
                        lblDiscountPriceP4.Text = (GlobalData.listDiscountProductPrice[i] - listPriceDifferences[1]).ToString();
                    }

                }
                Form1.isOpenAnything = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
