using System.Text;
using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class ProductItem : MonoBehaviour
    {
        public Button buyButton;
        public Button querySubscriptionStatusButton;
        public Text descText;
        private Product product;
        private IAPDemo iapDemo;
        private SubscriptionStatus SubscriptionStatus;
        private int index;

        public void SetProduct(int index, Product product, IAPDemo iapDemo)
        {
            this.index = index;
            this.product = product;
            this.iapDemo = iapDemo;
            this.updateText();
            iapDemo.Log($"product:{descText.text}");
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(LaunchCheckoutFlow);
            querySubscriptionStatusButton.onClick.RemoveAllListeners();
            querySubscriptionStatusButton.onClick.AddListener(QuerySubscriptionStatus);
        }

        public void updateText()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"index={index}");
            builder.AppendLine($"{JsonConvert.SerializeObject(product, Formatting.Indented)}");
            if (SubscriptionStatus != null)
            {
                builder.AppendLine("Subscription Status");
                builder.AppendLine(JsonConvert.SerializeObject(SubscriptionStatus, Formatting.Indented));
            }

            descText.text = builder.ToString();
            descText.SetAllDirty();
        }

        void QuerySubscriptionStatus()
        {
            iapDemo.Log($"GetSubscriptionStatus {product.SKU}");
            IAPService.GetSubscriptionStatus(product.SKU).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    iapDemo.toast("GetSubscriptionStatus failed");
                    iapDemo.Log($"GetSubscriptionStatus error:{msg.Error}");
                    return;
                }

                iapDemo.toast("GetSubscriptionStatus successfully");
                iapDemo.Log($"GetSubscriptionStatus successfully:{product.SKU}");
                SubscriptionStatus = msg.Data;
                this.updateText();
            });
        }

        void LaunchCheckoutFlow()
        {
            //点击购买
            iapDemo.Log($"LaunchCheckoutFlow {product.SKU}");
            Task<Purchase> task;
            if (iapDemo.useV2.isOn)
            {
                task = IAPService.LaunchCheckoutFlow2(product);
            }
            else
            {
                task = IAPService.LaunchCheckoutFlow(product.SKU, product.Price, product.Currency);
            }

            task.OnComplete(m =>
            {
                if (m.IsError)
                {
                    iapDemo.toast("LaunchCheckoutFlow failed");
                    iapDemo.Log($"LaunchCheckoutFlow error:Code={m.Error.Code},Message={m.Error.Message}");
                    return;
                }

                if (m.Data == null || string.IsNullOrWhiteSpace(m.Data.ID))
                {
                    iapDemo.toast("LaunchCheckoutFlow canceled");
                    iapDemo.Log($"LaunchCheckoutFlow failed:Data is empty");
                    return;
                }

                iapDemo.toast("LaunchCheckoutFlow successfully");
                iapDemo.Log($"LaunchCheckoutFlow successfully:{product.SKU}");
            });
        }
    }
}