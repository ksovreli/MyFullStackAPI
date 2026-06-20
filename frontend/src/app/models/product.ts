export class Product {
  id!: number
  name?: string
  image?: string
  price?: number
  description?: string
  quantity?: number
  salePrice?: number
  rating!: number
  categoryName?: string
  categoryId?: number
  isNew?: boolean = false
  images: { url: string }[] = []
}

export interface ProductDto {
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  categoryId: number;
  quantity: number; // აუცილებელი ველი
  isNew: boolean;   // აუცილებელი ველი
  salePrice?: number | null; // (Optional)
  rating?: number;           // (Optional)
}