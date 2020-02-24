import { canUseDOM } from 'vtex.render-runtime'
import { PixelMessage } from './typings/events'

declare var _learnq: any;

export function handleEvents(e: PixelMessage) {
  switch (e.data.eventName) {
    case 'vtex:productView': {
      var item = {
        "ProductName": e.data.product.productName,
        "ProductID": e.data.product.productId,
        "Categories": e.data.product.categories,
        "ImageURL": "",
        "URL": "",
        "Brand": e.data.product.brand,
        "Price": 9.99,
        "CompareAtPrice": 14.99
      };
      
      _learnq.push(["track", "Viewed Product", item]);
    
      _learnq.push(["trackViewedItem", {
        "Title": item.ProductName,
        "ItemId": item.ProductID,
        "Categories": item.Categories,
        "ImageUrl": item.ImageURL,
        "Url": item.URL,
        "Metadata": {
          "Brand": item.Brand,
          "Price": item.Price,
          "CompareAtPrice": item.CompareAtPrice
        }
      }]);
      break
    }
    case 'vtex:addToCart':{
      let itemNames = [];
      for (let i = 0; i < e.data.items.length; i = i + 1){
        itemNames.push(e.data.items[i].name);
      }

      _learnq.push(["track", "Added to Cart", {
        "$value": e.data.items[0].price,
        "AddedItem_ProductName": e.data.items[0].name,
        "AddedItem_ProductID": e.data.items[0].productRefId,
        "AddedItem_SKU": e.data.items[0].skuId,
        "AddedItem_Categories": [e.data.items[0].category],
        "AddedItem_ImageURL": e.data.items[0].imageUrl,
        "AddedItem_URL": e.data.items[0].detailUrl,
        "AddedItem_Price": e.data.items[0].price,
        "AddedItem_Quantity": e.data.items[0].quantity,
        "ItemNames": itemNames,
        "CheckoutURL": "http://www.example.com/path/to/checkout",
        "Items": e.data.items
      }]);
      break
    }

    default: {
      return
    }
  }
}

if (canUseDOM && typeof _learnq == "object") {
  window.addEventListener('message', handleEvents)
}
