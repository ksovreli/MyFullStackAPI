import { Routes } from '@angular/router'
import { inject } from '@angular/core'
import { Router } from '@angular/router'
import { AuthService } from './services/auth-service'
import { HomeComponent } from './home/home.component'
import { AdminDashboard } from './admin-dashboard/admin-dashboard'

export const authGuard = () => {
  const authService = inject(AuthService)
  const router = inject(Router)

  if (authService.isLoggedIn()) {
    return true
  }

  router.navigate(['/login'])
  return false
}

export const adminGuard = () => {
  const authService = inject(AuthService)
  const router = inject(Router)

  const role = authService.getUserRole()

  if (authService.isLoggedIn() && role === 'Admin') {
    return true
  }

  console.warn("UNAUTHORIZED_ACCESS_ATTEMPT: Redirecting to 404")
  router.navigate(['/404'])
  return false
}

export const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  {
    path: 'home',
    component: HomeComponent,
    title: 'Apex Shop | Home'
  },

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
    path: 'checkout',
    title: 'Checkout',
    canActivate: [authGuard],
    loadComponent: () => import('./checkout/checkout').then(m => m.Checkout)
  },

  {
    path: 'order-history',
    title: 'Order History',
    canActivate: [authGuard],
    loadComponent: () => import('./order-history/order-history').then(m => m.OrderHistory)
  },

  {
    path: 'verify-email',
    title: 'Verify Email',
    loadComponent: () => import('./verify-email/verify-email').then(m => m.VerifyEmail)
  },

  {
    path: 'reset-password',
    title: 'Reset Password',
    loadComponent: () => import('./reset-password/reset-password').then(m => m.ResetPassword)
  },

  {
    path: 'admin',
    component: AdminDashboard,
    canActivate: [adminGuard]
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
]