using System;
using Pico.Platform;
using Pico.Platform.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Samples.IAP
{
    public class IAPDemo : MonoBehaviour
    {
        public GameObject productPrefab;
        public GameObject purchasePrefab;
        public Text logText;
        public GameObject purchaseListContainer;
        public GameObject productListContainer;

        private ProductList ProductList;
        private PurchaseList PurchaseList;

        void Start()
        {
            try
            {
                CoreService.Initialize();
                Log($"Init Successfully");
            }
            catch (Exception e)
            {
                Log($"Initialize failed {e}");
                return;
            }

            var buttonRefreshProduct = GameObject.Find("ButtonRefreshProduct").GetComponent<Button>();
            var buttonNextPageProduct = GameObject.Find("ButtonNextPageProduct").GetComponent<Button>();
            var buttonRefreshPurchase = GameObject.Find("ButtonRefreshPurchase").GetComponent<Button>();
            var buttonNextPagePurchase = GameObject.Find("ButtonNextPagePurchase").GetComponent<Button>();
            var buttonSwitchDlc = GameObject.Find("ButtonSwitchDlc").GetComponent<Button>();
            buttonRefreshProduct.onClick.AddListener(getProductList);
            buttonRefreshPurchase.onClick.AddListener(getPurchaseList);
            buttonNextPageProduct.onClick.AddListener(getNextPageProductList);
            buttonNextPagePurchase.onClick.AddListener(getNextPagePurchaseList);
            buttonSwitchDlc.onClick.AddListener(() => { SceneManager.LoadScene("Samples/IAP/DLC"); });
            // testList();
            getProductList();
            getPurchaseList();
        }

        void destroyChildren(GameObject x)
        {
            int childs = x.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(x.transform.GetChild(i).gameObject);
            }
        }

        void testList()
        {
            destroyChildren(productListContainer);
            for (var i = 0; i < 6; i++)
            {
                var product = Instantiate(productPrefab, productListContainer.transform, false);
                var desc = product.GetComponentInChildren<Text>();
                desc.text = $"name={i} sku={i} description={i} price{i}";
            }
        }

        void renderProductList()
        {
            destroyChildren(productListContainer);
            foreach (var i in ProductList)
            {
                var ii = i;
                var product = Instantiate(productPrefab, productListContainer.transform, false);
                var buy = product.GetComponentInChildren<Button>();
                buy.onClick.AddListener(() => { LaunchCheckoutFlow(ii.SKU, ii.Price, ii.Currency); });
                var desc = product.GetComponentInChildren<Text>();
                desc.text = $"name={i.Name} sku={i.SKU} description={i.Description} Price={i.Price} Currency={i.Currency}";
                Log($"product:{desc.text}");
            }
        }

        void LaunchCheckoutFlow(string sku, string price, string currency)
        {
            //点击购买
            Log($"LaunchCheckoutFlow {sku}");
            // IAPService.LaunchCheckoutFlow(null, null, null).OnComplete(m =>
            // {
            //     Log("impossible");
            // });
            IAPService.LaunchCheckoutFlow(sku, price, currency).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"LaunchCheckoutFlow error:Code={m.Error.Code},Message={m.Error.Message}");
                    return;
                }

                if (m.Data == null || string.IsNullOrWhiteSpace(m.Data.ID))
                {
                    Log($"LaunchCheckoutFlow failed:Data is empty");
                    return;
                }

                Log($"LaunchCheckoutFlow successfully:{sku}");
            });
        }

        void ConsumePurchase(string sku)
        {
            Log($"ConsumePurchase {sku}");
            IAPService.ConsumePurchase(sku).OnComplete(m =>
            {
                if (m.IsError)
                {
                    Log($"ConsumePurchase failed:code={m.Error.Code} message={m.Error.Message}");
                    return;
                }

                Log($"Consume Purchase Successfully.");
            });
        }

        void renderPurchaseList()
        {
            destroyChildren(purchaseListContainer);
            foreach (var i in PurchaseList)
            {
                var ii = i;
                var purchase = Instantiate(purchasePrefab, purchaseListContainer.transform, false);
                var consumeButton = purchase.GetComponentInChildren<Button>();
                consumeButton.onClick.AddListener(() => { ConsumePurchase(ii.SKU); });
                var desc = purchase.GetComponentInChildren<Text>();
                desc.text = $"sku={i.SKU} ID={i.ID} grantTime={i.GrantTime} expirationTime={i.ExpirationTime}";
                Log($"Purchase:{desc.text}");
            }
        }

        void getProductList()
        {
            Log($"GetProductsBySKU");
            IAPService.GetProductsBySKU(null).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get product list error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                this.ProductList = msg.Data;
                Log($"GetProductList successfully.Count={ProductList.Count}");
                renderProductList();
            });
        }

        void getPurchaseList()
        {
            Log("GetPurchaseList");
            IAPService.GetViewerPurchases().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"Get Purchase List error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                this.PurchaseList = msg.Data;
                Log($"GetPurchaseList Successfully.Count={PurchaseList.Count}");
                renderPurchaseList();
            });
        }

        void getNextPageProductList()
        {
            Log("get next page productList");
            if (this.ProductList == null)
            {
                Log("ProductList is empty");
                return;
            }

            if (!ProductList.HasNextPage)
            {
                Log("This is already the last page");
                return;
            }

            Log($"GetNextPageProductList {ProductList.NextPageParam}");
            IAPService.GetNextProductListPage(ProductList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"getNextProductListPage error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                this.ProductList = msg.Data;
                renderProductList();
            });
        }

        void getNextPagePurchaseList()
        {
            Log($"getNextPagePurchaseList");
            if (this.PurchaseList == null)
            {
                Log("PurchaseList is empty");
                return;
            }

            if (!PurchaseList.HasNextPage)
            {
                Log("This is already the last page");
                return;
            }

            Debug.Log($"GetNextPagePurchaseList {PurchaseList.NextPageParam}");
            IAPService.GetNextPurchaseListPage(PurchaseList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Log($"getNextProductListPage error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                this.PurchaseList = msg.Data;
                renderPurchaseList();
            });
        }

        void Log(string newLine)
        {
            print(newLine);
            logText.text = "> " + newLine + Environment.NewLine + logText.text;
            if (logText.text.Length > 1000)
            {
                logText.text = logText.text.Substring(0, 1000);
            }
        }
    }
}