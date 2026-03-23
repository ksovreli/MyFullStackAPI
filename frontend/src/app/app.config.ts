import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { PreloadAllModules, provideRouter, withComponentInputBinding, withInMemoryScrolling, withPreloading } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './auth-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(
      routes, 
      withInMemoryScrolling({
        scrollPositionRestoration: 'enabled',
        anchorScrolling: 'enabled'
      }),
      withPreloading(PreloadAllModules),
      withComponentInputBinding()
    ),
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};
