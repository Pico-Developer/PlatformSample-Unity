using Newtonsoft.Json;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class PurchaseItem : MonoBehaviour
    {
        public Button consumeButton;
        public Text descText;
        private IAPDemo iapDemo;
        private Purchase purchase;
        private int index;

        private void Start()
        {
        }

        public void SetPurchase(int index, Purchase purchase, IAPDemo iapDemo)
        {
            this.iapDemo = iapDemo;
            this.purchase = purchase;
            this.index = index;
            consumeButton.onClick.RemoveAllListeners();
            consumeButton.onClick.AddListener(ConsumePurchase);
            descText.text = $"index={index}\n{JsonConvert.SerializeObject(purchase, Formatting.Indented)}";
            descText.SetAllDirty();
            iapDemo.Log($"Purchase:{descText.text}");
        }

        void ConsumePurchase()
        {
            iapDemo.Log($"ConsumePurchase {purchase.SKU}");
            IAPService.ConsumePurchase(purchase.SKU).OnComplete(m =>
            {
                if (m.IsError)
                {
                    iapDemo.toast("ConsumePurchase failed");
                    iapDemo.Log($"ConsumePurchase failed:code={m.Error.Code} message={m.Error.Message}");
                    return;
                }

                iapDemo.toast("ConsumePurchase successfully");
                iapDemo.Log($"Consume Purchase Successfully.");
            });
        }
    }
}