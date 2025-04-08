using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MF_Shopping_Assistant.Classes
{
    internal class ModifyQuantity
    {
        private readonly MySqlConnection mySqlConnection;
        private System.Windows.Forms.Label lblNumberOfProducts;
        public static int quantity = 1;

        private System.Windows.Forms.Label lblUpdateNumberOfProducts;
        public static double updateQuantity;

        public ModifyQuantity(MySqlConnection mySqlConnection, System.Windows.Forms.Label lblNumberOfProducts, System.Windows.Forms.Label lblUpdateNumberOfProducts)
        {
            this.mySqlConnection = mySqlConnection;
            this.lblNumberOfProducts = lblNumberOfProducts;
            this.lblUpdateNumberOfProducts = lblUpdateNumberOfProducts;
        }
        public void QuantityIncrease()
        {
            quantity++;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        public void QuantityDecrease()
        {
            if (quantity > 1) quantity--;
            lblNumberOfProducts.Text = quantity.ToString();
        }

        public void UpdateQuantityDecrease()
        {
            if (updateQuantity > 1) updateQuantity--;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();
        }

        public void UpdateQuantityIncrease()
        {
            updateQuantity++;
            lblUpdateNumberOfProducts.Text = updateQuantity.ToString();
        }

    }
}
