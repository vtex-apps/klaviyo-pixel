import { CartChangedItems, CartItem, ProductDetail } from '../typings/events'

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

export const sendAddToCartEvent = (
  learnq: any,
  items: CartItem[],
  itemNames?: string[],
  allItems?: CartChangedItems[],
) => {
  items.forEach(item => {
    const addedToCartItems = {
      $value: item.price / 100,
      AddedItemProductName: item.name,
      AddedItemProductID: getCartProductId(item),
      AddedItemSKU: getCartSkuId(item),
      AddedItemCategories: item.category.split('/'),
      AddedItemImageURL: item.imageUrl,
      AddedItemURL: item.detailUrl,
      AddedItemAbsoluteURL: window?.location?.origin + item.detailUrl,
      AddedItemPrice: item.price,
      AddedItemFormattedPrice: item.price / 100,
      AddedItemQuantity: item.quantity,
      ItemNames: itemNames,
      CheckoutURL: `https://${window.location.hostname}/${
        window.__RUNTIME__.rootPath ? `${window.__RUNTIME__.rootPath}/` : ''
      }checkout/`,
      Items: allItems?.length ? allItems : items,
    }
    learnq.push(['track', 'Added to Cart', addedToCartItems])
  })
}
