using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Mkey
{
    public class ShopPackThingHelper : MonoBehaviour
    {
        public Image thingImage;
        public Image thingLabelImage;
        public Text thingLabelText;
        public Text thingTextCount;
        public Text thingTextCountOld;

        // bombs
        public Text bombCountText;
        public Text colbCountText;
        public Text hummCountText;
        public Text shuflCountText;
        // lifes
        public Text lifesCountText;

        public Text thingTextPrice;
        public Text thingName;
        public Button thingBuyButton;

        public void SetData(ShopPackThingData shopThing)
        {
            if (thingImage)
            {
                thingImage.sprite = shopThing.thingImage;
                thingImage.SetNativeSize();
            }
            if (thingTextCount) thingTextCount.text = shopThing.thingCountText;
            if (thingTextCountOld) thingTextCountOld.text = shopThing.thingCountOldText;
            if (thingTextPrice) thingTextPrice.text = shopThing.thingPriceText;

            if (bombCountText) bombCountText.text = shopThing.bombCountText;
            if (colbCountText) colbCountText.text = shopThing.bombCountText;
            if (hummCountText) hummCountText.text = shopThing.bombCountText;
            if (shuflCountText) shuflCountText.text = shopThing.bombCountText;

            if (lifesCountText) lifesCountText.text = shopThing.lifeCountText;

            if (thingLabelImage)
            {               
                if (shopThing.thingLabelImage)
                {            
                    thingLabelImage.sprite = shopThing.thingLabelImage;
                    thingLabelText.text = shopThing.thingLabelText;
                    thingLabelImage.SetNativeSize();
                }
                else
                {
                    thingLabelImage.gameObject.SetActive(false);
                }
            }
            if (thingBuyButton)
            {
                thingBuyButton.onClick.RemoveAllListeners();
                thingBuyButton.onClick = shopThing.clickEvent;
            }
            if (thingName)
            {
                thingName.text = shopThing.name;
            }
        }

        private void SetImages(Sprite thingSprite, Sprite thingLabelSprite)
        {
            if (thingImage)
            {
                thingImage.sprite = thingSprite;
                thingImage.SetNativeSize();
            }

            if (thingLabelImage)
            {                
                if (thingLabelSprite)
                {
                    thingLabelImage.gameObject.SetActive(true);
                    thingLabelImage.sprite = thingLabelSprite;
                    thingLabelImage.SetNativeSize();
                }
                else
                {
                    thingLabelImage.gameObject.SetActive(false);
                }
            }
        }

        public static ShopPackThingHelper CreateShopPackThingsHelper(GameObject prefab, RectTransform parent, ShopPackThingData shopThingData)
        {
            if (!prefab) return null;

            prefab.GetComponent<ShopPackThingHelper>().SetImages(shopThingData.thingImage, shopThingData.thingLabelImage); // fix 2019 unity

            GameObject shopThing = Instantiate(prefab);
            shopThing.transform.localScale = parent.transform.lossyScale;
            shopThing.transform.SetParent(parent.transform);
            ShopPackThingHelper sC = shopThing.GetComponent<ShopPackThingHelper>();
            sC.SetData(shopThingData);
            return sC;
        }
    }

    [System.Serializable]
    public class ShopPackThingData
    {
        public string name;
        public Sprite thingImage;
        public Sprite thingLabelImage;
        public string thingLabelText;
        public int thingCount;
        public string thingCountText;
        public string thingCountOldText;
        // bombs
        public int bombCount;
        public string bombCountText;
        // lifes
        public int lifeCount;
        public string lifeCountText;

        public float thingPrice;
        public string thingPriceText;
        public string kProductID;
        public GameObject prefab;
        [HideInInspector]
        public Button.ButtonClickedEvent clickEvent;

        public ShopPackThingData(ShopPackThingData prod)
        {
            if (prod == null) return;
            name = prod.name;
            thingImage = prod.thingImage;
            thingLabelImage = prod.thingLabelImage;
            thingLabelText = prod.thingLabelText;
            
            thingCount = prod.thingCount;
            thingPrice = prod.thingPrice;

            bombCount = prod.bombCount;
            bombCountText = prod.bombCountText;

            lifeCount = prod.lifeCount;
            lifeCountText = prod.lifeCountText;

            thingPriceText = prod.thingPriceText;
            thingCountText = prod.thingCountText;
            thingCountOldText = prod.thingCountOldText;
            kProductID = prod.kProductID;
            clickEvent = prod.clickEvent;
        }
    }

    [System.Serializable]
    public class ShopPackThingDataReal : ShopPackThingData
    {
        public RealShopType shopType = RealShopType.Coins;
        [Space(8, order = 0)]
        [Header("Purchase Event: ", order = 1)]
        public UnityEvent PurchaseEvent;

        public ShopPackThingDataReal(ShopPackThingDataReal prod) : base(prod)
        {
            shopType = prod.shopType;
            PurchaseEvent = prod.PurchaseEvent;
        }

    }
    public enum InGameShopPackType { None, Booster };
    [System.Serializable]
    public class ShopPackThingDataInGame: ShopPackThingData
    {
        public InGameShopType shopType = InGameShopType.Booster;

        public ShopPackThingDataInGame(ShopPackThingDataInGame prod) : base(prod)
        {
            shopType = prod.shopType;
        }
    }
}