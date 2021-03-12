import { canUseDOM } from 'vtex.render-runtime'

import { CartChangedItems, CartItem, PixelMessage } from './typings/events'
import {
  getCartProductId,
  getCartSkuId,
  getCategory,
  getProductId,
  sendAddToCartEvent,
} from './modules/pixelHelper'

declare const _learnq: any
let newItems: CartItem[] = []

export function handleEvents(e: PixelMessage) {
  const isDefaultAddToCartEventTrue = window.__defaultAddToCartEvent === 'true'
  switch (e.data.eventName) {
    case 'vtex:userData': {
      const { email, firstName, lastName } = e.data
      const learnq = _learnq || []
      learnq.push([
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
      const learnq = _learnq || []
      const item = {
        ProductName: product.productName,
        ProductID: getProductId(product),
        Categories: product.categories,
        ConcatenatedCategories: getCategory(product.categories)?.split('/'),
        SKU: product?.selectedSku?.itemId,
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

      learnq.push(['track', 'Viewed Product', item])

      learnq.push([
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
      const itemNames = items.map(item => {
        return item.name
      })
      newItems = items
      if (isDefaultAddToCartEventTrue) {
        sendAddToCartEvent(_learnq, items, itemNames)
      }
      break
    }
    case 'vtex:cartChanged': {
      if (!newItems.length) break
      const { items } = e.data
      const itemNames = items.map(item => {
        return item.name
      })
      const allItems: CartChangedItems[] = items.map(item => {
        return {
          ProductID: getCartProductId(item),
          SKU: getCartSkuId(item),
          ProductName: item.name,
          Quantity: item.quantity,
          ItemPrice: item.price,
          ItemFormattedPrice: item.price / 100,
          RowTotal: item.price * item.quantity,
          ProductURL: item.detailUrl,
          ProductAbsoluteURL: window?.location?.origin + item.detailUrl,
          ImageURL: item.imageUrl,
          ProductCategories: item.category.split('/'),
        }
      })
      if (!isDefaultAddToCartEventTrue) {
        sendAddToCartEvent(_learnq, newItems, itemNames, allItems)
      }
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
