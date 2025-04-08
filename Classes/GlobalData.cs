using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MF_Shopping_Assistant.Classes
{
    public static class GlobalData
    {
        public static List<int> listIdsOfProducts = new List<int>();
        public static List<string> listNameOfProducts = new List<string>();
        public static List<string> listTypeOfProducts = new List<string>();
        public static List<string> listManufacturerOfProducts = new List<string>();
        public static List<double> listPriceOfProducts = new List<double>();
        public static List<double> listPricePerUnitOfProducts = new List<double>();
        public static List<double> listQuantityOfProducts = new List<double>();
        public static List<double> listInStockOfProducts = new List<double>();

        public static List<int> listDiscountProductId = new List<int>();
        public static List<int> listDiscountProductDiscountId = new List<int>();
        public static List<double> listDiscountProductPrice = new List<double>();
        public static List<string> listDiscountProductName = new List<string>();
        public static List<string> listDiscountProductType = new List<string>();
        public static List<string> listDiscountProductManufacturer = new List<string>();

        public static List<Panel> panelsOfProducts = new List<Panel>();
    }
}
