import { CartChangedItems, CartItem, ProductDetail } from '../typings/events'
import push from './push'

export const removeStartAndEndSlash = (category?: string) =>
  category?.replace(/^\/|\/$/g, '')

export function getCategory(rawCategories: string[]) {
  if (!rawCategories || !rawCategories.length) {
    return
  }

  return removeStartAndEndSlash(rawCategories?.[0])
}

export const getProductId = (product: ProductDetail) => {
  if (window.__klaviyo_useRefIdSetting) {
    return product.productReference
  }
  return product.productId
}

export const getCartProductId = (product: CartItem) => {
  if (window.__klaviyo_useRefIdSetting) {
    return product.productRefId
  }
  return product.productId
}

export const getCartSkuId = (product: CartItem) => {
  if (window.__klaviyo_useRefIdSetting) {
    return product.referenceId
  }
  return product.skuId
}

export const replaceAll = (str: string, search: string, replacement: string) =>
  str.split(search).join(replacement)

export const sendAddToCartEvent = ({
  items,
  allItems,
  itemNames,
}: {
  items: CartItem[]
  allItems?: CartChangedItems[]
  itemNames?: string[]
}) => {
  items.forEach(item => {
    const addedToCartItems = {
      $value: item.price / 100,
      AddedItemProductName: item.name,
      AddedItemProductID: getCartProductId(item),
      AddedItemSKU: getCartSkuId(item),
      AddedItemCategories: item.category.split('/'),
      AddedItemImageURL: item.imageUrl,
      AddedItemURL: item.detailUrl,
      AddedItemAbsoluteURL: `${window?.location?.origin}${
        window.__RUNTIME__.rootPath ? `${window.__RUNTIME__.rootPath}` : ''
      }${item.detailUrl}`,
      AddedItemPrice: item.priceIsInt === true ? item.price / 100 : item.price,
      AddedItemQuantity: item.quantity,
      ItemNames: itemNames,
      CheckoutURL: `${window.location.origin}${
        window.__RUNTIME__.rootPath ? `${window.__RUNTIME__.rootPath}` : ''
      }/checkout/`,
      Items: allItems?.length ? allItems : items,
    }
    push(['track', 'Added to Cart', addedToCartItems])
  })
}
