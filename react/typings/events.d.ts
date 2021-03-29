export interface PixelMessage extends MessageEvent {
  data:
    | ProductViewData
    | ProductClickData
    | OrderPlacedData
    | OrderPlacedTrackedData
    | PageViewData
    | ProductImpressionData
    | AddToCartData
    | RemoveToCartData
    | CartChangedData
    | HomePageInfo
    | ProductPageInfoData
    | SearchPageInfoData
    | UserData
    | CartIdData
    | CategoryViewData
    | AddToWishlistData
    | RemoveFromWishlistData
}

export interface EventData {
  event: string
  eventName: string
  currency: string
}

export interface PageInfoData extends EventData {
  event: 'pageInfo'
  eventName: 'vtex:pageInfo'
  accountName: string
  pageTitle: string
  pageUrl: string
}

export interface UserData extends PageInfoData {
  eventType: 'userData'
  eventName: 'vtex:userData'
  firstName?: string
  lastName?: string
  document?: string
  id?: string
  email?: string
  phone?: string
  isAuthenticated: boolean
}

export interface CartIdData extends PageInfoData {
  eventType: 'cartId'
  eventName: 'vtex:cartId'
  cartId: string
}

export interface HomePageInfo extends PageInfoData {
  eventType: 'homeView'
}

export interface ProductPageInfoData extends PageInfoData {
  eventType: 'productPageInfo'
}

export interface SearchPageInfoData extends PageInfoData {
  eventType:
    | 'internalSiteSearchView'
    | 'categoryView'
    | 'departmentView'
    | 'emptySearchView'
  category?: CategoryMetaData
  department?: DepartmentMetaData
  search?: SearchMetaData
}

interface CategoryMetaData {
  id: string
  name: string
}

interface DepartmentMetaData {
  id: string
  name: string
}

interface SearchMetaData {
  term: string
  category: CategoryMetaData
  results: number
}

export interface PageViewData extends EventData {
  event: 'pageView'
  eventName: 'vtex:pageView'
  pageTitle: string
  pageUrl: string
  referrer: string
}

export interface AddToCartData extends EventData {
  event: 'addToCart'
  eventName: 'vtex:addToCart'
  items: CartItem[]
}

export interface RemoveToCartData extends EventData {
  event: 'removeFromCart'
  eventName: 'vtex:removeFromCart'
  items: CartItem[]
}

export interface CartChangedData extends EventData {
  event: 'cartChanged'
  eventName: 'vtex:cartChanged'
  items: CartItem[]
}

export interface OrderPlacedData extends Order, EventData {
  event: 'orderPlaced'
  eventName: 'vtex:orderPlaced'
}

export interface OrderPlacedTrackedData extends Order, EventData {
  event: 'orderPlacedTracked'
  eventName: 'vtex:orderPlacedTracked'
}

export interface ProductViewData extends EventData {
  event: 'productView'
  eventName: 'vtex:productView'
  product: ProductDetail
}

export interface ProductClickData extends EventData {
  event: 'productClick'
  eventName: 'vtex:productClick'
  product: ProductSummary
}

export interface ProductImpressionData extends EventData {
  event: 'productImpression'
  eventName: 'vtex:productImpression'
  impressions: Impression[]
  product?: ProductSummary // deprecated, use impressions list!
  position?: number // deprecated, use impressions list!
  list: string
}

export interface CategoryViewData extends EventData {
  event: 'categoryView'
  eventName: 'vtex:categoryView'
  currency: string
  products: ProductMetaData[]
}

export interface AddToWishlistData extends EventData {
  event: 'removeFromWishlist'
  eventName: 'vtex:removeFromWishlist'
  wishlistEventObject: WishlistEventObject
}

export interface RemoveFromWishlistData extends EventData {
  event: 'addToWishlist'
  eventName: 'vtex:addToWishlist'
  wishlistEventObject: WishlistEventObject
}

interface CartItem {
  productId: string
  skuId: string
  variant: string
  price: number
  priceIsInt: boolean
  name: string
  quantity: number
  productRefId: string
  referenceId: string
  brand: string
  category: string
  detailUrl: string
  imageUrl: string
}

export interface Order {
  accountName: string
  corporateName: string
  coupon: string
  currency: string
  openTextField: string
  orderGroup: string
  salesChannel: string
  visitorAddressCity: string
  visitorAddressComplement: string
  visitorAddressCountry: string
  visitorAddressNeighborhood: string
  visitorAddressNumber: string
  visitorAddressPostalCode: string
  visitorAddressState: string
  visitorAddressStreet: string
  visitorContactInfo: string[]
  visitorContactPhone: string
  visitorType: string
  transactionId: string
  transactionDate: string
  transactionAffiliation: string
  transactionTotal: number
  transactionShipping: number
  transactionSubtotal: number
  transactionDiscounts: number
  transactionTax: number
  transactionCurrency: string
  transactionPaymentType: PaymentType[]
  transactionShippingMethod: ShippingMethod[]
  transactionLatestShippingEstimate: Date
  transactionProducts: ProductOrder[]
  transactionPayment: {
    id: string
  }
}

export interface Impression {
  product: ProductSummary
  position: number
}

export interface PaymentType {
  group: string
  paymentSystemName: string
  installments: number
  value: number
}

export interface ShippingMethod {
  itemId: string
  selectedSla: string
}

export interface ProductOrder {
  id: string
  name: string
  sku: string
  skuRefId: string
  skuName: string
  productRefId: string
  ean: string
  slug: string
  brand: string
  brandId: string
  seller: string
  sellerId: string
  category: string
  categoryId: string
  categoryTree: string[]
  categoryIdTree: string[]
  priceTags: PriceTag[]
  originalPrice: number
  price: number
  sellingPrice: number
  tax: number
  quantity: number
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  components: any[]
  measurementUnit: string
  unitMultiplier: number
}

export interface PriceTag {
  identifier: string
  isPercentual: boolean
  value: number
}

interface Product {
  brand: string
  brandId: string
  categories: string[]
  productId: string
  productName: string
  productReference: string
  detailUrl: string
  linkText: string
  items: Item[]
}

export interface ProductSummary extends Product {
  sku: Item
}

export interface ProductDetail extends Product {
  categoryId: string
  categoryTree: Array<{ id: string; name: string }>
  selectedSku: Item
}

export interface Item {
  itemId: string
  name: string
  imageUrl: string
  ean?: string // TODO: provide this info at productImpression
  referenceId: ReferenceId[]
  seller?: Seller
  sellers: Seller[]
}

interface ReferenceId {
  Key: string
  Value: string
}

export interface Seller {
  commertialOffer: CommertialOffer
  sellerId: string
}

export interface CommertialOffer {
  Price: number
  ListPrice: number
  AvailableQuantity: number
}

export interface WishlistEventObject {
  action?: string
  button_type?: string
  page_type?: string
  product_id?: string
  product_title?: string
  item_price?: number
  item_quantity?: number
  product_brand?: string
  categories_path?: string
  category_level_1?: string
  category_level_2?: string
  category_level_3?: string
}

export interface CartChangedItems {
  RowTotal: number
  ProductName: string
  ProductCategories: string[]
  ItemPrice: number
  Quantity: number
  ProductAbsoluteURL: string
  ImageURL: string
  ProductID: string
  SKU: string
  ProductURL: string
}
