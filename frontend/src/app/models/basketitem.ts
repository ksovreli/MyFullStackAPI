export interface BasketItem {
  id: number
  backpackId: number
  name: string
  price: number
  salePrice?: number
  image: string
  quantity: number
  categoryName: string
}