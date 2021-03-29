📢 Use this project, [contribute](https://github.com/vtex-apps/klaviyo-pixel) to it or open issues to help evolve it using [Store Discussion](https://github.com/vtex-apps/store-discussion).

# Klaviyo Integration

This app enables basic Klaviyo tracking for your VTEX store, including these JavaScript Track API events:

- `Active on Site` - When someone visits your website
- `Viewed Page` - When someone views a page
- `Viewed Product` - When someone views a product
- `Viewed Category` - When someone views a category page
- `Added to Cart` - When someone adds an item to their cart
- `Removed from Cart` - When someone removes an item from their cart
- `Wishlist Update` - When someone updates their wishlist to add or remove a product

and these server-side Track API events:

- `Placed Order` - When an order successfully processes on your system
- `Ordered Product` - An event for each item in a processed order
- `Fulfilled Order` - When an order is sent to the customer

## Configuration

1. [Install](https://vtex.io/docs/recipes/store/installing-an-app) this app (`vtex.klaviyo-pixel`) in the desired account
2. In your admin dashboard, go to **Apps** > **Klaviyo Integration** and input the following settings:

- `Company ID`: Enter your Klaviyo Company ID, sometimes also referred to as a Public API Key.
- `Use Reference ID`: Check this box to use Reference IDs rather than Product IDs when tracking products.
- `Product Root URL`: Enter your store's root path for product links (i.e. `https://www.store.com` or `https://www.store.com/us`). In the server-side Track API requests this will be combined with the relative link for each product.

## Checkout Integration

If you wish to track the Klaviyo "checkout started" event, the following code must be added to your `checkout6-custom.js` file:

```js
let klaviyoNotified = false
var e = document.createElement('script')
;(e.type = 'text/javascript'),
  (e.async = true),
  (e.src = 'https://static.klaviyo.com/onsite/js/klaviyo.js?company_id=ABCDEF')
var t = document.getElementsByTagName('script')[0]
t.parentNode.insertBefore(e, t)
$(window).on('hashchange', function() {
  const { orderForm } = vtexjs.checkout
  if (orderForm.clientProfileData && !klaviyoNotified && _learnq) {
    _learnq.push([
      'identify',
      {
        $email: orderForm.clientProfileData.email,
        $first_name: orderForm.clientProfileData.firstName,
        $last_name: orderForm.clientProfileData.lastName,
      },
    ])
    let klaviyoItems = []
    let productNames = []
    let categories = []
    orderForm.items.forEach(item => {
      productNames.push(item.name)
      const key = Object.keys(item.productCategories)[0]
      categories.push(item.productCategories[key])
      klaviyoItems.push({
        ProductID: item.productRefId,
        SKU: item.id,
        ProductName: item.name,
        Quantity: item.quantity,
        ItemPrice: item.sellingPrice / 100,
        RowTotal: item.price / 100,
        ProductURL: window.location.hostname + item.detailUrl,
        ImageURL: item.imageUrl,
        ProductCategories: Object.values(item.productCategories),
      })
    })
    _learnq.push([
      'track',
      'Started Checkout',
      {
        $event_id: orderForm.orderFormId,
        $value: orderForm.value / 100,
        ItemNames: productNames,
        CheckoutURL: window.location.hostname + '/checkout#/cart',
        Categories: categories,
        Items: klaviyoItems,
      },
    ])
    klaviyoNotified = true
  }
})
```

⚠️ Make sure to change `?company_id=ABCDEF` on line 6 to use your specific Klaviyo Company ID.
