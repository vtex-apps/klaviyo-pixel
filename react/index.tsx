import { canUseDOM } from 'vtex.render-runtime'
import { PixelMessage } from './typings/events'

declare var _learnq: any;

export function handleEvents(e: PixelMessage) {
  switch (e.data.eventName) {
    case 'vtex:productView': {
      var item = { "ProductName": "Winnie the Pooh", "ProductID": "1111", "Categories": ["Fiction", "Children"], "ImageURL": "http://www.example.com/path/to/product/image.png", "URL": "http://www.example.com/path/to/product", "Brand": "Kids Books", "Price": 9.99, "CompareAtPrice": 14.99 };
 
      _learnq.push(["track", "Viewed Product", item]);
    
      _learnq.push(["trackViewedItem", { "Title": item.ProductName, "ItemId": item.ProductID, "Categories": item.Categories, "ImageUrl": item.ImageURL, "Url": item.URL, "Metadata": { "Brand": item.Brand, "Price": item.Price, "CompareAtPrice": item.CompareAtPrice
     } }]);
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
