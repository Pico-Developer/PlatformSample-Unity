using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class ProductItem : MonoBehaviour
    {
        public Button buyButton;
        public Text descText;
        private Product product;
        private IAPDemo iapDemo;
        private int index;

        public void SetProduct(int index, Product product, IAPDemo iapDemo)
        {
            this.index = index;
            this.product = product;
            this.iapDemo = iapDemo;
            descText.text = $"index={index}\n{JsonConvert.SerializeObject(product, Formatting.Indented)}";
            descText.SetAllDirty();
            iapDemo.Log($"product:{descText.text}");
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(LaunchCheckoutFlow);
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