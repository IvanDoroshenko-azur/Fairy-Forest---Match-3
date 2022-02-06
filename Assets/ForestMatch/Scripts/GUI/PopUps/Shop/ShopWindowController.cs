using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ShopWindowController : PopUpsController
    {
        [SerializeField]
        private RectTransform ThingsParent;
        [SerializeField]
        private RealShopType shopType = RealShopType.Coins;
        [SerializeField]
        private GameObject scrollFlag;

        private List<ShopPackThingHelper> shopPackThings;
        private List<ShopThingHelper> shopThings;

        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private LifeIncTimer LITimer { get { return LifeIncTimer.Instance; } }
        private VideoPurchaser VPurch { get { return VideoPurchaser.Instance; } }

        void Start()
        {
            CreateThingTab();
        }

        public override void RefreshWindow()
        {

            base.RefreshWindow();
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            ShopPackThingHelper[] sTt = ThingsParent.GetComponentsInChildren<ShopPackThingHelper>();
            foreach (var item in sTt)
            {
                DestroyImmediate(item.gameObject);
            }

            Purchaser p = Purchaser.Instance;
            if (p == null) return;

            List<ShopPackThingDataReal> productsPack = new List<ShopPackThingDataReal>();
            List<ShopThingDataReal> products = new List<ShopThingDataReal>();

            VideoPurchaser vP = VideoPurchaser.Instance;
            if (vP && vP.gameProducts!=null &&  vP.gameProducts.Length > 0)
            {
                products.AddRange(vP.gameProducts);
            }

            //
            if (p.consumable != null && p.consumable.Length > 0) productsPack.AddRange(p.consumablePack);
            //if (p.nonConsumable != null && p.nonConsumable.Length > 0) products.AddRange(p.nonConsumable);
            //if (p.subscriptions != null && p.subscriptions.Length > 0) products.AddRange(p.subscriptions);

            if((shopType == RealShopType.Life) && (MPlayer?.Life < LITimer.IncIflessThan) && VPurch.countVideoLifes > 0)
            {
                shopThings = new List<ShopThingHelper>();
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i] != null && products[i].prefab) shopThings.Add(ShopThingHelper.CreateShopThingsHelper(products[i].prefab, ThingsParent, products[i]));
                }
            }


            Debug.Log("products count: " + productsPack.Count);
            if (productsPack.Count > 0)
            {
                shopPackThings = new List<ShopPackThingHelper>();
                for (int i = 0; i < productsPack.Count; i++)
                {
                    if (productsPack[i] != null)// && (productsPack[i].shopType == shopType) && productsPack[i].prefab) 
                        shopPackThings.Add(ShopPackThingHelper.CreateShopPackThingsHelper(productsPack[i].prefab, ThingsParent, productsPack[i]));
                }
            }
            //

            if (p.consumable != null && p.consumable.Length > 0) products.AddRange(p.consumable);
            if (p.nonConsumable != null && p.nonConsumable.Length > 0) products.AddRange(p.nonConsumable);
            if (p.subscriptions != null && p.subscriptions.Length > 0) products.AddRange(p.subscriptions);

            Debug.Log("products count: " + products.Count);
            if (products.Count==0) return;

            if (shopType != RealShopType.Life) 
            {
                shopThings = new List<ShopThingHelper>();
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i] != null && (products[i].shopType == shopType) && products[i].prefab) shopThings.Add(ShopThingHelper.CreateShopThingsHelper(products[i].prefab, ThingsParent, products[i]));
                }
            }
            if (scrollFlag) scrollFlag.SetActive(products.Count > 4);
        }
    }
}