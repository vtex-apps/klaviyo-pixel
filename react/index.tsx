import { canUseDOM } from 'vtex.render-runtime'

import { CartItem, PixelMessage } from './typings/events'
import { sendExtendedEcommerceEvents } from './modules/extendedEcommerceEvents'
import push from './modules/push'
import { getCartProductId, getCartSkuId, getProductId } from "./modules/pixelHelper";

let newItems: CartItem[] = []

export function handleEvents(e: PixelMessage) {
  sendExtendedEcommerceEvents(e)
  switch (e.data.eventName) {
    case 'vtex:userData': {
      const { email, firstName, lastName } = e.data
      push([
        'identify',
        {
          $email: email,
          $first_name: firstName,
          $last_name: lastName,
        },
      ])
      break
    }
    case 'vtex:productView': {
      const { product } = e.data
      const item = {
        ProductName: product.productName,
        ProductID: getProductId(product),
        Categories: product.categories,
        ImageURL: product.selectedSku?.imageUrl,
        URL: `https://${window.location.hostname}${
          window.__RUNTIME__.rootPath ? `/${window.__RUNTIME__.rootPath}` : ''
        }${product.detailUrl}`,
        Brand: product.brand,
        Price: product.selectedSku?.sellers[0]?.commertialOffer?.Price ?? 0,
        CompareAtPrice:
          product.selectedSku?.sellers[0]?.commertialOffer?.ListPrice ?? 0,
      }

      if (!item.Price) break

      push(['track', 'Viewed Product', item])

      push([
        'trackViewedItem',
        {
          Title: item.ProductName,
          ItemId: item.ProductID,
          Categories: item.Categories,
          ImageUrl: item.ImageURL,
          Url: item.URL,
          Metadata: {
            Brand: item.Brand,
            Price: item.Price,
            CompareAtPrice: item.CompareAtPrice,
          },
        },
      ])
      break
    }
    case 'vtex:addToCart': {
      const { items } = e.data
      newItems = items
      break
    }
    case 'vtex:cartChanged': {
      if (!newItems.length) break
      const { items } = e.data
      const itemNames = items.map(item => {
        return item.name
      })
      const allItems = items.map(item => {
        return {
          ProductID: getCartProductId(item),
          SKU: getCartSkuId(item),
          ProductName: item.name,
          Quantity: item.quantity,
          ItemPrice: item.price,
          RowTotal: item.price * item.quantity,
          ProductURL: item.detailUrl,
          ImageURL: item.imageUrl,
          ProductCategories: item.category.split('/'),
        }
      })
      newItems.forEach(item => {
        push([
          'track',
          'Added to Cart',
          {
            $value: item.price,
            AddedItemProductName: item.name,
            AddedItemProductID: getCartProductId(item),
            AddedItemSKU: getCartSkuId(item),
            AddedItemCategories: [],
            AddedItemImageURL: item.imageUrl,
            AddedItemURL: item.detailUrl,
            AddedItemPrice: item.price,
            AddedItemQuantity: item.quantity,
            ItemNames: itemNames,
            CheckoutURL: `https://${window.location.hostname}/${
              window.__RUNTIME__.rootPath
                ? `${window.__RUNTIME__.rootPath}/`
                : ''
            }checkout/`,
            Items: allItems,
          },
        ])
      })
      break
    }
    default: {
      break
    }
  }
}

if (canUseDOM) {
  window.addEventListener('message', handleEvents)
}
