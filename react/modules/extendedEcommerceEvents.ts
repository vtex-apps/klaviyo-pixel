import push from './push'
import { PixelMessage, } from '../typings/events'
import { getCartProductId, getCartSkuId, getCategory, replaceAll } from "./pixelHelper"

export const sendExtendedEcommerceEvents = (e: PixelMessage) => {
    switch (e.data.eventName) {
        case 'vtex:categoryView': {
            const { products } = e.data
            const categoryNames = getCategory(products?.[0].categories)?.split('/')

            const categoryViewObject = {
                CategoryName: categoryNames?.[categoryNames?.length - 1],
                CategoryListedProducts: products.map(product => product.productId.toString()),
                CategoryLevel1: categoryNames?.[0],
                CategoryLevel2: categoryNames?.[1],
                CategoryLevel3: categoryNames?.[2],
                CategoriesPath: replaceAll(getCategory(products?.[0].categories)!!, '/', ',')
            }

            const categoryEventObject = ['track', 'Viewed Category', categoryViewObject]
            push(categoryEventObject)

            return
        }
        case 'vtex:removeFromCart': {
            const { items } = e.data

            items.forEach(item => {
                push([
                    'track',
                    'Removed from Cart',
                    {
                        $value: item.price,
                        RemovedItemProductName: item.name,
                        RemovedItemProductID: getCartProductId(item),
                        RemovedItemSKU: getCartSkuId(item),
                        RemovedItemCategories: [],
                        RemovedItemImageURL: item.imageUrl,
                        RemovedItemURL: item.detailUrl,
                        RemovedItemPrice: item.price,
                        RemovedItemQuantity: item.quantity,
                        CheckoutURL: `https://${window.location.hostname}/${
                            window.__RUNTIME__.rootPath
                                ? `${window.__RUNTIME__.rootPath}/`
                                : ''
                        }checkout/`,
                    },
                ])
            })
            return
        }
        case 'vtex:pageView': {
            const pageViewInfoBody = {
                Referrer: e.data.referrer,
                Location: e.data.pageUrl,
                Path: e.data.pageUrl.split('.com')?.[1] || '/'
            }

            push(['track', 'Page View', pageViewInfoBody])
            return
        }
        case 'vtex:removeFromWishlist':
        case 'vtex:addToWishlist': {
            const { wishlistEventObject } = e.data
            const klaviyoWishlistObject = ['track', 'Wishlist Update', wishlistEventObject]
            push(klaviyoWishlistObject)
            return
        }
    }
}