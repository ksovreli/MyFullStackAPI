export interface OrderRequest {
  userId: number
  items: OrderItem[]
  shippingAddress: string
  phoneNumber: string
  totalAmount: number
}

export interface OrderItem {
  backpackId: number
  quantity: number
  price: number
}
export interface Order {
  id: number;
  userId: number;
  user?: {
    userName: string;
    email: string;
  };
  orderDate: string;
  totalPrice: number;
  status: string;
  shippingAddress: string;
}

export interface OrderResponse {
  id: number
  message: string
}