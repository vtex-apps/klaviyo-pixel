import { CartItem, ProductDetail } from "../typings/events";

export function getCategory(rawCategories: string[]) {
    if (!rawCategories || !rawCategories.length) {
        return
    }

    return removeStartAndEndSlash(rawCategories[0])
}

export const removeStartAndEndSlash = (category?: string) => category?.replace(/^\/|\/$/g, '')

export const replaceAll = (str: string, search: string, replacement: string) => str.split(search).join(replacement)

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