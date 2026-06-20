import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AdminService } from '../services/admin-service';
import { AlertService } from '../services/alert-service';
import { Product, ProductDto } from '../models/product';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, DecimalPipe],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboard implements OnInit {

  private adminService = inject(AdminService);
  private alertService = inject(AlertService);

  allOrders = this.adminService.allOrders;
  products = this.adminService.products;

  categories = signal<any[]>([]);
  searchTerm = signal('');
  showAddForm = signal(false);
  editingProductId = signal<number | null>(null);

  imagePreview = signal<string | null>(null);
  selectedFile: File | null = null;

  newProduct: ProductDto = {
    name: '',
    description: '',
    price: 0,
    imageUrl: '',
    categoryId: 1,
    quantity: 10,
    isNew: true
  };

  ngOnInit() {
    this.adminService.loadAllOrders();
    this.adminService.loadProducts();

    this.adminService.getCategories().subscribe(res => {
      this.categories.set(res);
    });
  }

  totalRevenue = computed(() =>
    (this.allOrders() || []).reduce((sum, o) => sum + (o?.totalPrice || 0), 0)
  );

  filteredOrders = computed(() => {
    const term = this.searchTerm().toLowerCase();
    return (this.allOrders() || []).filter(o =>
      o &&
      (
        !term ||
        o.id?.toString().includes(term) ||
        o.status?.toLowerCase().includes(term) ||
        o.user?.userName?.toLowerCase().includes(term)
      )
    );
  });

  onSearch(event: Event) {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }

  toggleAddForm() {
    this.showAddForm.update(v => !v);
    if (!this.showAddForm()) this.cancelForm();
  }

  editProduct(item: Product) {
    this.cancelForm();

    this.editingProductId.set(item.id);
    
    // აქ ვიყენებთ "Nullish Coalescing" (??) ერორების გამოსასწორებლად
    this.newProduct = {
      name: item.name ?? '',
      description: item.description ?? '',
      price: item.price ?? 0,
      imageUrl: item.image ?? '', 
      categoryId: item.categoryId ?? 1,
      quantity: item.quantity ?? 10,
      isNew: item.isNew ?? false
    };

    if (item.image) {
      this.imagePreview.set(item.image);
    }
    
    this.showAddForm.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (!file) return;

    this.selectedFile = file;

    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview.set(reader.result as string);
    };
    reader.readAsDataURL(file);
  }

  async uploadImage(): Promise<string> {
    if (!this.selectedFile) {
      return this.newProduct.imageUrl || '';
    }

    const formData = new FormData();
    formData.append('file', this.selectedFile);

    try {
      const res: any = await this.adminService.uploadImage(formData).toPromise();
      return res.url;
    } catch (error) {
      console.error('UPLOAD_FAILED:', error);
      return '';
    }
  }

  async submitNewProduct() {
    if (!this.newProduct.name || !this.newProduct.description || !this.newProduct.price) {
      this.alertService.error('please_fill_all_required_fields');
      return;
    }

    try {
      const uploadedUrl = await this.uploadImage();

      const payload = {
        name: this.newProduct.name,
        description: this.newProduct.description,
        price: Number(this.newProduct.price),
        imageUrl: uploadedUrl || this.newProduct.imageUrl || '',
        categoryId: Number(this.newProduct.categoryId),
        quantity: this.newProduct.quantity || 10,
        isNew: this.newProduct.isNew ?? true,
        rating: 0,
        salePrice: this.newProduct.salePrice ? Number(this.newProduct.salePrice) : null
      };

      const id = this.editingProductId();
      const request$ = id
        ? this.adminService.updateBackpack(id, payload)
        : this.adminService.addBackpack(payload);

      request$.subscribe({
        next: (res) => {
          this.products.update(list => 
            id ? list.map(p => p.id === id ? { ...p, ...res, image: payload.imageUrl } : p) : [res, ...list]
          );
          this.alertService.success(id ? 'product_updated' : 'product_added');
          this.cancelForm();
        },
        error: (err) => {
          console.error('server_error:', err.error);
          this.alertService.error('data_send_error');
        }
      });

    } catch (error) {
      this.alertService.error('image_upload_error');
    }
  }

  deleteProduct(id: number, name: string) {
    this.alertService.confirmDelete(name).then(r => {
      if (r.isConfirmed) {
        this.adminService.deleteBackpack(id).subscribe(() => {
          this.products.update(list => list.filter(p => p.id !== id));
        });
      }
    });
  }

  updateStatus(id: number, status: string) {
    this.alertService.selectStatus(id, status).then(r => {
      if (r.isConfirmed && r.value) {
        this.adminService.updateStatus(id, r.value).subscribe(() => {
          this.adminService.loadAllOrders();
        });
      }
    });
  }

  removeSelectedPhoto() {
  this.selectedFile = null;
  this.imagePreview.set(null);
  
  // ასევე მნიშვნელოვანია, რომ input-იც გავასუფთავოთ, 
  // თორემ იგივე ფაილს მეორედ თუ აირჩევთ, change event აღარ მოხდება.
  const fileInput = document.getElementById('backpack-file-input') as HTMLInputElement;
  if (fileInput) {
    fileInput.value = '';
  }
}

  cancelForm() {
    this.showAddForm.set(false);
    this.editingProductId.set(null);

    this.newProduct = {
      name: '',
      price: 0,
      description: '',
      imageUrl: '',
      categoryId: 1,
      quantity: 10,
      isNew: true
    };

    this.imagePreview.set(null);
    this.selectedFile = null;
  }
}