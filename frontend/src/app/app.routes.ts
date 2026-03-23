import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },

    { path: 'home', component: HomeComponent, title: 'Apex Shop | Home' },
    
    { 
        path: 'products', 
        title: 'All Products',
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent) 
    },

    { 
        path: 'product/:id', 
        title: 'Product Details',
        loadComponent: () => import('./products/product-details/product-details.component').then(m => m.ProductDetailsComponent)
    },

    { 
        path: 'products/:category', 
        title: 'Category View',
        loadComponent: () => import('./products/products.component').then(m => m.ProductsComponent) 
    },

    { 
        path: 'cart', 
        title: 'Your Cart',
        loadComponent: () => import('./cart/cart').then(m => m.Cart) 
    },
    { 
        path: 'wishlist', 
        title: 'Your Wishlist',
        loadComponent: () => import('./wishlist/wishlist').then(m => m.Wishlist) 
    },

    { 
        path: 'login', 
        title: 'Login',
        loadComponent: () => import('./login/login').then(m => m.Login) 
    },
    { 
        path: 'register', 
        title: 'Join the Peak',
        loadComponent: () => import('./register/register').then(m => m.Register) 
    },

    { 
        path: '404', 
        title: 'Not Found',
        loadComponent: () => import('./notfound/notfound').then(m => m.Notfound) 
    },
    { 
        path: '**', 
        redirectTo: '404'
    }
];