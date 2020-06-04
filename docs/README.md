📢 Use this project, [contribute](https://github.com/vtex-apps/klaviyo-pixel) to it or open issues to help evolve it using [Store Discussion](https://github.com/vtex-apps/store-discussion).

# Klaviyo Integration

This app enables basic Klaviyo tracking for your VTEX store, including these JavaScript Track API events:

`Active on Site` - When someone visits your website
`Viewed Product` - When someone views a product
`Added to Cart` - When someone adds an item to their cart

and these server-side Track API events:

`Placed Order` - When an order successfully processes on your system
`Ordered Product` - An event for each item in a processed order
`Fulfilled Order` - When an order is sent to the customer

## Configuration

1. [Install](https://vtex.io/docs/recipes/store/installing-an-app) this app (`vtex.klaviyo-pixel`) in the desired account
2. In your admin dashboard, go to **Apps** > **Klaviyo Integration** and input the following settings:

- `Company ID`: Enter your Klaviyo Company ID.
- `Use Reference ID`: Check this box to use Reference IDs rather than Product IDs when tracking products.
- `Public API Key`: Enter your Klaviyo Public API Key (used for server-side API requests).
- `Product Root URL`: Enter your store's root path for product links (i.e. `https://www.store.com` or `https://www.store.com/us`). In the server-side Track API requests this will be combined with the relative link for each product.

---
