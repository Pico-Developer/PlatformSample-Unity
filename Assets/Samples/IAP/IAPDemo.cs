using System;
using Pico.Platform.Models;
using PICO.Platform.Samples;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Pico.Platform.Samples.IAP
{
    public class IAPDemo : MonoBehaviour
    {
        public GameObject productPrefab;
        public GameObject purchasePrefab;
        public GameObject purchaseListContainer;
        public GameObject productListContainer;
        public Toast toastObject;
        public Button buttonRefreshProduct;
        public Button buttonNextPageProduct;
        public Button buttonRefreshPurchase;
        public Button buttonNextPagePurchase;
        public Button buttonSwitchDlc;
        public Button buttonSettings;
        public GameObject panelSettings;
        public Button buttonCloseSettings;
        public Text productListCount;
        public Text purchaseListCount;
        public Dropdown launchCheckoutFlowType;
        public InputField OrderComment;

        private ProductList ProductList;
        private PurchaseList PurchaseList;
        private ObjectPool productPool;
        private ObjectPool purchasePool;

        void Start()
        {
            productPool = new ObjectPool();
            purchasePool = new ObjectPool();
            try
            {
                InitUtil.Initialize();
                Log($"Init Successfully");
            }
            catch (Exception e)
            {
                toast("Initialize failed");
                Log($"Initialize failed {e}");
                return;
            }

            buttonRefreshProduct.onClick.AddListener(getProductList);
            buttonRefreshPurchase.onClick.AddListener(getPurchaseList);
            buttonNextPageProduct.onClick.AddListener(getNextPageProductList);
            buttonNextPagePurchase.onClick.AddListener(getNextPagePurchaseList);
            buttonSwitchDlc.onClick.AddListener(() => { SceneManager.LoadScene("DLC"); });
            buttonSettings.onClick.AddListener(() => { panelSettings.SetActive(true); });
            buttonCloseSettings.onClick.AddListener(() => { panelSettings.SetActive(false); });
            getProductList();
            getPurchaseList();
        }

        public void toast(string s)
        {
            toastObject.Show(s);
        }

        void destroyChildren(GameObject x, ObjectPool pool)
        {
            int childs = x.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                pool.Put(x.transform.GetChild(i).gameObject);
            }
        }

        void renderProductList()
        {
            Log($"GetProductList successfully.Count={ProductList.Count}");
            destroyChildren(productListContainer, productPool);
            productListCount.text = $"{ProductList.Count}";
            for (var index = 0; index < ProductList.Count; index++)
            {
                var i = ProductList[index];
                var product = productPool.Get(productPrefab);
                product.GetComponent<ProductItem>().SetProduct(index, i, this);
                product.transform.SetParent(productListContainer.transform, false);
            }
        }

        void renderPurchaseList()
        {
            Log($"GetPurchaseList Successfully.Count={PurchaseList.Count}");
            destroyChildren(purchaseListContainer, purchasePool);
            purchaseListCount.text = $"{PurchaseList.Count}";
            for (var index = 0; index < PurchaseList.Count; index++)
            {
                var i = PurchaseList[index];
                var purchase = purchasePool.Get(purchasePrefab);
                purchase.GetComponent<PurchaseItem>().SetPurchase(index, i, this);
                purchase.transform.SetParent(purchaseListContainer.transform, false);
            }
        }

        void getProductList()
        {
            Log($"GetProductsBySKU");
            IAPService.GetProductsBySKU(null).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    toast("GetProductsBySKU failed");
                    Log($"Get product list error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                toast("GetProductsBySKU successfully");
                this.ProductList = msg.Data;
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
                    toast("GetViewerPurchases failed");
                    Log($"Get Purchase List error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                toast("GetViewerPurchases successfully");
                this.PurchaseList = msg.Data;
                renderPurchaseList();
            });
        }

        void getNextPageProductList()
        {
            Log("get next page productList");
            if (this.ProductList == null)
            {
                toast("Please get the first page first");
                Log("ProductList is empty");
                return;
            }

            if (!ProductList.HasNextPage)
            {
                toast("This is already the last page");
                Log("This is already the last page");
                return;
            }

            Log($"GetNextPageProductList {ProductList.NextPageParam}");
            IAPService.GetNextProductListPage(ProductList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    toast("GetNextProductListPage failed");
                    Log($"getNextProductListPage error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                toast("GetNextProductListPage successfully");
                this.ProductList = msg.Data;
                renderProductList();
            });
        }

        void getNextPagePurchaseList()
        {
            Log($"getNextPagePurchaseList");
            if (this.PurchaseList == null)
            {
                toast("Please get the first page first");
                Log("PurchaseList is empty");
                return;
            }

            if (!PurchaseList.HasNextPage)
            {
                toast("This is already the last page");
                Log("This is already the last page");
                return;
            }

            Debug.Log($"GetNextPagePurchaseList {PurchaseList.NextPageParam}");
            IAPService.GetNextPurchaseListPage(PurchaseList).OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    toast("GetNextPurchaseListPage failed");
                    Log($"getNextProductListPage error:code={msg.Error.Code},message={msg.Error.Message}");
                    return;
                }

                toast("GetNextPurchaseListPage successfully");
                this.PurchaseList = msg.Data;
                renderPurchaseList();
            });
        }

        public void Log(string newLine)
        {
            Debug.Log(newLine);
        }
    }
}