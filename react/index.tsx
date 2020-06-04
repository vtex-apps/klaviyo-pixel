import { canUseDOM } from 'vtex.render-runtime'

import {
  PixelMessage,
  ProductDetail,
  CartItem,
  // ProductOrder,
} from './typings/events'

declare const _learnq: any
let newItems: CartItem[] = []

function getProductId(product: ProductDetail) {
  if (window.__klaviyo_useRefIdSetting) {
    return product.productReference
  }
  return product.productId
}

function getCartProductId(product: CartItem) {
  if (window.__klaviyo_useRefIdSetting) {
    return product.productRefId
  }
  return product.productRefId
}

function getCartSkuId(product: CartItem) {
  if (window.__klaviyo_useRefIdSetting) {
    return product.referenceId
  }
  return product.skuId
}

// function getOrderProductId(product: ProductOrder) {
//   if (window.__klaviyo_useRefIdSetting) {
//     return product.skuRefId
//   }
//   return product.id
// }

export function handleEvents(e: PixelMessage) {
  switch (e.data.eventName) {
    case 'vtex:productView': {
      const { product } = e.data
      const learnq = _learnq || []
      const item = {
        ProductName: product.productName,
        ProductID: getProductId(product),
        Categories: product.categories,
        ImageURL: '',
        URL: product.linkText,
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
          ProductCategories: [],
        }
      })
      newItems.forEach(item => {
        _learnq.push([
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
    case 'vtex:orderPlaced': {
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
