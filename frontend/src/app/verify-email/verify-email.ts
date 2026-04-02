import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../services/auth-service';

@Component({
  selector: 'app-verify-email',
  imports: [],
  templateUrl: './verify-email.html',
  styleUrl: './verify-email.scss',
})
export class VerifyEmail {
private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private router = inject(Router);

  status = signal<'loading' | 'success' | 'error'>('loading');
  verificationId = Math.random().toString(36).substring(7).toUpperCase();

  ngOnInit() {
    // Get parameters from the URL link sent to the user's email
    const email = this.route.snapshot.queryParamMap.get('email');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (email && token) {
      this.authService.verifyEmail(email, token).subscribe({
        next: () => {
          this.status.set('success');
          // Delay redirect so user can see the "Success" state
          setTimeout(() => this.router.navigate(['/login']), 4000);
        },
        error: () => {
          this.status.set('error');
        }
      });
    } else {
      this.status.set('error');
    }
  }
}
