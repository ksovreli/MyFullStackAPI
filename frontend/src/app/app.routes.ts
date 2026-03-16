import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    
    { 
        path: 'products', 
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent) 
    },
    { 
        path: 'product/:id', 
        loadComponent: () => import('./products/product-details/product-details.component').then(m => m.ProductDetailsComponent)
    },
    { 
        path: 'products/:category', 
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent) 
    },
    { 
        path: 'cart', 
        loadComponent: () => import('./cart/cart').then(m => m.Cart) 
    },
    { 
        path: 'wishlist', 
        loadComponent: () => import('./wishlist/wishlist').then(m => m.Wishlist) 
    },
    { 
        path: 'login', 
        loadComponent: () => import('./login/login').then(m => m.Login) 
    },
    { 
        path: 'register', 
        loadComponent: () => import('./register/register').then(m => m.Register) 
    },
    { 
        path: '404', 
        loadComponent: () => import('./notfound/notfound').then(m => m.Notfound) 
    },
    { 
        path: '**', 
        loadComponent: () => import('./notfound/notfound').then(m => m.Notfound) 
    }
];