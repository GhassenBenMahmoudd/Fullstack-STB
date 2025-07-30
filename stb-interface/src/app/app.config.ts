import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
// On importe withFetch en plus
import { provideHttpClient, withFetch } from '@angular/common/http';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes ),
    // On modifie cette ligne
    provideHttpClient(withFetch()) 
  ]
};
